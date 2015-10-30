using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.LogFeedManager
{
    class CreatureProcessor
    {
        struct ProcessorVerifyList
        {
            public bool Name;
            //public bool Age; //not needed
            public bool Parents;
            public bool Traits;
            public bool Gender;
            public bool CaredBy;
            public bool Pregnant;
            public bool Foalization;
            public bool Branding;

            public bool IsValid
            {
                get { return (Name && (Gender || Parents || Traits || CaredBy)); }
            }
        }

        class CreatureBuilder
        {
            public string Name;
            public CreatureAge Age;
            public string CaredBy;
            public string BrandedBy;
            public string Father;
            public string Mother;
            public List<CreatureTrait> Traits = new List<CreatureTrait>();
            public bool IsDiseased;
            public float InspectSkill;
            public bool IsMale;
            public DateTime PregnantUntil = DateTime.MinValue;
            public ServerGroup ServerGroup;
            public CreatureEntity.SecondaryInfoTag SecondaryInfo = CreatureEntity.SecondaryInfoTag.None;
        }

        //public ModuleGranger Parent;

        readonly GrangerDebugLogger _grangerDebug;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;

        readonly TimeSpan _processorTimeout = new TimeSpan(0, 0, 5);
        bool _isProcessing = false;
        DateTime _startedProcessingOn;
        CreatureBuilder _newCreature;
        ProcessorVerifyList _verifyList;

        private readonly GrangerFeature _parentModule;
        private readonly GrangerContext _context;
        private readonly PlayerManager _playerMan;

        //float? AHSkill = null;

        public CreatureProcessor(GrangerFeature parentModule, GrangerContext context, PlayerManager playerMan,
            GrangerDebugLogger debugLogger,
            [NotNull] ITrayPopups trayPopups, [NotNull] ILogger logger)
        {
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (logger == null) throw new ArgumentNullException("logger");
            _grangerDebug = debugLogger;
            this.trayPopups = trayPopups;
            this.logger = logger;
            _parentModule = parentModule;
            _context = context;
            _playerMan = playerMan;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">event log line</param>
        /// <param name="moreData">extra data parsed out of the line, if applicable</param>
        public void HandleLogEvent(string line)
        {
            //TODO all line parsing should be moved to WurmEventParser, when it is needed more than just here!
            //keep this DRY

            //attempt to start building new creature data
            if (line.StartsWith("You smile at", StringComparison.Ordinal))
            {
                _grangerDebug.Log("smile cond: " + line);
                AttemptToStartProcessing(line);
            }
            // append/update incoming data to current creature in buffer
            if (_isProcessing)
            {
                //[20:23:18] It has fleeter movement than normal. It has a strong body. It has lightning movement. It can carry more than average. It seems overly aggressive.                           
                if (!_verifyList.Traits && CreatureTrait.CanThisBeTraitLogMessage(line))
                {
                    _grangerDebug.Log("found maybe trait line: " + line);
                    var extractedTraits = GrangerHelpers.GetTraitsFromLine(line);
                    foreach (var trait in extractedTraits)
                    {
                        _grangerDebug.Log("found trait: " + trait);
                        _newCreature.Traits.Add(trait);
                        _verifyList.Traits = true;
                    }
                    _grangerDebug.Log("trait parsing finished");
                    if (_newCreature.InspectSkill == 0 && _newCreature.Traits.Count > 0)
                    {
                        var message =
                            String.Format(
                                "{0} ({1}) can see traits, but Granger found no Animal Husbandry skill for him. Is this a bug? Creature will be added anyway.",
                                _playerMan.PlayerName, _newCreature.ServerGroup);
                        logger.Error(message);
                        trayPopups.Schedule("POSSIBLE PROBLEM", message, 5000);
                    }
                }
                //[20:23:18] She is very strong and has a good reserve of fat.
                if (line.StartsWith("He", StringComparison.Ordinal) && !_verifyList.Gender)
                {
                    _newCreature.IsMale = true;
                    _verifyList.Gender = true;
                    _grangerDebug.Log("creature set to male");
                }
                if (line.StartsWith("She", StringComparison.Ordinal) && !_verifyList.Gender)
                {
                    _newCreature.IsMale = false;
                    _verifyList.Gender = true;
                    _grangerDebug.Log("creature set to female");
                }
                //[01:05:57] Mother is Venerable fat Starkdance. Father is Venerable fat Jollypie. 
                if ((line.Contains("Mother is") || line.Contains("Father is")) && !_verifyList.Parents)
                {
                    _grangerDebug.Log("found maybe parents line");
                    Match match = Regex.Match(line, @"Mother is (?<g>\w+ \w+ .+?)\.|Mother is (?<g>\w+ .+?)\.");
                    if (match.Success)
                    {
                        string mother = match.Groups["g"].Value;
                        mother = GrangerHelpers.ExtractCreatureName(mother);
                        _newCreature.Mother = mother;
                        _grangerDebug.Log("mother set to: " + mother);
                    }
                    Match match2 = Regex.Match(line, @"Father is (?<g>\w+ \w+ .+?)\.|Father is (?<g>\w+ .+?)\.");
                    if (match2.Success)
                    {
                        string father = match2.Groups["g"].Value;
                        father = GrangerHelpers.ExtractCreatureName(father);
                        _newCreature.Father = father;
                        _grangerDebug.Log("father set to: " + father);
                    }
                    _verifyList.Parents = true;
                    _grangerDebug.Log("finished parsing parents line");
                }
                //[20:23:18] It is being taken care of by Darkprincevale.
                if (line.Contains("It is being taken care") && !_verifyList.CaredBy)
                {
                    _grangerDebug.Log("found maybe take care of line");
                    Match caredby = Regex.Match(line, @"care of by (\w+)");
                    if (caredby.Success)
                    {
                        _newCreature.CaredBy = caredby.Groups[1].Value;
                        _grangerDebug.Log("cared set to: " + _newCreature.CaredBy);
                    }
                    _verifyList.CaredBy = true;
                    _grangerDebug.Log("finished parsing care line");
                }
                //[17:11:42] She will deliver in about 4.
                if (line.Contains("She will deliver in") && !_verifyList.Pregnant)
                {
                    _grangerDebug.Log("found maybe prengant line");
                    Match match = Regex.Match(line, @"She will deliver in about (\d+)");
                    if (match.Success)
                    {
                        double length = Double.Parse(match.Groups[1].Value) + 1D;
                        _newCreature.PregnantUntil = DateTime.Now + TimeSpan.FromHours(length * 21D);
                        _grangerDebug.Log("found creature to be pregnant, estimated delivery: " + _newCreature.PregnantUntil);
                    }
                    _verifyList.Pregnant = true;
                    _grangerDebug.Log("finished parsing pregnant line");
                }
                //[20:58:26] A foal skips around here merrily
                if (line.Contains("A foal skips around here merrily") && !_verifyList.Foalization)
                {
                    _grangerDebug.Log("applying foalization to the creature");
                    try
                    {
                        _newCreature.Age.Foalize();
                        _verifyList.Foalization = true;
                    }
                    catch (Exception _e)
                    {
                        // we can swallow because age is of little significance to granger and can be adjusted easily
                        logger.Error(_e, "The creature appears to be a foal, but has invalid age for a foal!");
                    }
                }
                //[20:57:27] It has been branded by and belongs to the settlement of Silver Hill Estate.
                if (line.Contains("It has been branded") && !_verifyList.Branding)
                {
                    _grangerDebug.Log("found maybe branding line");
                    Match match = Regex.Match(line, @"belongs to the settlement of (.+)\.");
                    if (match.Success)
                    {
                        string settlementName = match.Groups[1].Value;
                        _newCreature.BrandedBy = settlementName;
                        _grangerDebug.Log("found creature to be branded for: " + _newCreature.BrandedBy);
                        _verifyList.Branding = true;
                    }
                }
            }
        }

        void VerifyAndApplyProcessing()
        {
            if (_newCreature != null)
            {
                _grangerDebug.Log("finishing processing creature: " + _newCreature.Name);
                //verify if enough fields are filled to warrant updating
                if (_verifyList.IsValid)
                {
                    _grangerDebug.Log("Creature data is valid");

                    var selectedHerds = GetSelectedHerds();

                    //string[] herdsToCheck = selectedHerds;

                    var herdsFinds = GetHerdsFinds(selectedHerds, _newCreature.Name);
                    var selectedHerdsFinds = herdsFinds;
                    // if there isn't any creature found in selected herds,
                    // try all herds if setting is set
                    bool allHerdSearch = false;
                    if (herdsFinds.Length == 0 &&
                        _parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb)
                    {
                        allHerdSearch = true;
                        string[] allHerds = GetAllHerds();

                        herdsFinds = GetHerdsFinds(allHerds, _newCreature.Name);
                    }

                    // first try to update
                    // update only if found exactly one creature
                    if (herdsFinds.Length == 1)
                    {
                        CreatureEntity oldCreature = herdsFinds[0];

                        bool sanityFail = false;

                        #region SANITY_CHECKS

                        //perform sanity checks
                        string sanityFailReason = null;
                        //creatures cant suddenly get younger
                        if (oldCreature.Age > _newCreature.Age)
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "New creature data would make the creature younger than it was";
                        }
                        //basically if both creatures HAVE a mother name or father name, they cant have different names
                        //but its entirely possible a mother or father dies and reference is lost, with it the name
                        //no longer is shown, creature appears then as if it had no father or mother

                        //2 cases to check
                        //if current father name is blank, new name can not suddenly hold a name!
                        if (String.IsNullOrEmpty(oldCreature.FatherName) &&
                            !String.IsNullOrEmpty(_newCreature.Father))
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old father was blank but new data has a father name";
                        }

                        //if both names are not blank, then they can't be different!
                        if (!String.IsNullOrEmpty(oldCreature.FatherName) &&
                            !String.IsNullOrEmpty(_newCreature.Father) &&
                            oldCreature.FatherName != _newCreature.Father)
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old data father name was different than new father name";
                        }

                        //same for mother
                        if (String.IsNullOrEmpty(oldCreature.MotherName) &&
                            !String.IsNullOrEmpty(_newCreature.Mother))
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old mother was blank but new data has a mother name";
                        }

                        if (!String.IsNullOrEmpty(oldCreature.MotherName) &&
                            !String.IsNullOrEmpty(_newCreature.Mother) &&
                            oldCreature.MotherName != _newCreature.Mother)
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old data mother name was different than new mother name";
                        }

                        //need to compare traits up to lower AH inspect level,
                        //if they mismatch, thats also sanity fail
                        //we should treat null ah inspect value as 0

                        if (oldCreature.TraitsInspectedAtSkill.HasValue)
                        {
                            //exclude this check if creature had genesis cast within last 1 hour
                            _grangerDebug.Log(string.Format("Checking creature for Genesis cast (creature name: {0}",
                                _newCreature.Name));
                            if (!_parentModule.Settings.HasGenesisCast(_newCreature.Name))
                            {
                                _grangerDebug.Log("No genesis cast found");
                                var lowskill = Math.Min(oldCreature.TraitsInspectedAtSkill.Value, _newCreature.InspectSkill);
                                CreatureTrait[] certainTraits = CreatureTrait.GetTraitsUpToSkillLevel(lowskill,
                                    oldCreature.EpicCurve ?? false);
                                var oldCreatureTraits = oldCreature.Traits.ToArray();
                                var newCreatureTraits = _newCreature.Traits.ToArray();
                                foreach (var trait in certainTraits)
                                {
                                    if (oldCreatureTraits.Contains(trait) != newCreatureTraits.Contains(trait))
                                    {
                                        sanityFail = true;
                                        if (sanityFailReason == null)
                                            sanityFailReason = "Trait mismatch below inspect skill treshhold (" +
                                                               lowskill + "): " + trait.ToCompactString();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                _grangerDebug.Log("Genesis cast found, skipping trait sanity check");
                                _parentModule.Settings.RemoveGenesisCast(_newCreature.Name);
                                _grangerDebug.Log(string.Format("Removed cached genesis cast data for {0}",
                                    _newCreature.Name));
                            }
                        }
                        //is new creature server group not within allowed ones?
                        if (_newCreature.ServerGroup.ServerGroupId == ServerGroup.UnknownId)
                        {
                            sanityFail = true;
                            sanityFailReason = "New creature data had unsupported server group: " + _newCreature.ServerGroup;
                        }
                        //if old creature isEpic != new creature isEpic
                        bool oldIsEpic = oldCreature.EpicCurve ?? false;
                        bool newIsEpic = _newCreature.ServerGroup.ServerGroupId == ServerGroup.EpicId;
                        if (oldIsEpic != newIsEpic)
                        {
                            sanityFail = true;
                            sanityFailReason = "Old creature is of different server group than current player server group";
                        }

                        #endregion

                        if (sanityFail)
                        {
                            _grangerDebug.Log("sanity check failed for creature update: " + oldCreature + ". Reason: " +
                                              sanityFailReason);
                            trayPopups.Schedule("COULD NOT UPDATE CREATURE",
                                "There was data mismatch when trying to update creature, reason: " + sanityFailReason, 8000);
                        }
                        else
                        {
                            oldCreature.Age = _newCreature.Age;
                            oldCreature.TakenCareOfBy = _newCreature.CaredBy;
                            oldCreature.BrandedFor = _newCreature.BrandedBy;
                            oldCreature.FatherName = _newCreature.Father;
                            oldCreature.MotherName = _newCreature.Mother;
                            if (oldCreature.TraitsInspectedAtSkill <= _newCreature.InspectSkill ||
                                _newCreature.InspectSkill >
                                CreatureTrait.GetFullTraitVisibilityCap(oldCreature.EpicCurve ?? false))
                            {
                                oldCreature.Traits = _newCreature.Traits;
                                oldCreature.TraitsInspectedAtSkill = _newCreature.InspectSkill;
                            }
                            else
                                _grangerDebug.Log("old creature data had more accurate trait info, skipping");
                            oldCreature.SetTag("dead", false);
                            //oldCreature.SetTag("diseased", _newCreature.IsDiseased);
                            oldCreature.SetSecondaryInfoTag(_newCreature.SecondaryInfo);
                            oldCreature.IsMale = _newCreature.IsMale;
                            oldCreature.PregnantUntil = _newCreature.PregnantUntil;

                            _context.SubmitChanges();
                            _grangerDebug.Log("successfully updated creature in db");
                            trayPopups.Schedule("CREATURE UPDATED", String.Format("Updated creature: {0}", oldCreature));
                        }

                        _newCreature = null;
                        _grangerDebug.Log("processor buffer cleared");
                        return;
                        // we are done here
                    }

                    // no update performed, try to add
                    bool entireDB = _parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb;
                    if (selectedHerds.Length == 1 ||
                        (entireDB && selectedHerds.Length > 0))
                    {
                        // can't add a creature if it's already in selected herds
                        // also with entireDB, this will trigger if for some reason 2 or more creatures are already in db (update is skipped)
                        if (selectedHerdsFinds.Length == 0)
                        {
                            //do a sanity check to verify this creature name is not in current herd already
                            string herd = selectedHerds[0];
                            bool exists =
                                _context.Creatures.AsEnumerable().Any(x => _newCreature.Name == x.Name && x.Herd == herd);

                            if (!exists)
                            {
                                //add creature
                                AddNewCreature(herd, _newCreature);
                            }
                            else
                            {
                                var message = "Creature with name: " + _newCreature.Name +
                                              " already exists in herd: " + herd;
                                trayPopups.Schedule("CAN'T ADD CREATURE", message, 4000);
                                _grangerDebug.Log(message);
                            }

                            _newCreature = null;
                            _grangerDebug.Log("processor buffer cleared");
                            return;
                            // we are done here
                        }
                        else if (!entireDB)
                        {
                            string message = "Creature with name: " + _newCreature.Name +
                                             " already exists in active herd";

                            trayPopups.Schedule("CAN'T ADD CREATURE", message, 4000);
                            _grangerDebug.Log(message);
                        }
                    }

                    // no update or add performed, figure what went wrong

                    if (herdsFinds.Length > 1)
                    {
                        var partialMessage = allHerdSearch ? "database" : "selected herds";
                        _grangerDebug.Log("many creatures named " + _newCreature.Name + " found in " + partialMessage +
                                          ", add/update aborted");
                        trayPopups.Schedule("CAN'T ADD OR UPDATE CREATE",
                            partialMessage + " contain many creatures named " + _newCreature.Name + ", narrow herd selection",
                            6000);
                        //notify user to narrow the herd selection
                    }
                    else if (!entireDB && (selectedHerds.Length == 0 || selectedHerds.Length > 1))
                    {
                        const string message = "exactly one herd has to be active to add new creature";
                        _grangerDebug.Log(message);
                        trayPopups.Schedule("CAN'T ADD OR UPDATE CREATE", message, 4000);
                    }
                    else if (entireDB && selectedHerds.Length == 0)
                    {
                        const string message = "at least one herd must be select to add new creature";
                        _grangerDebug.Log(message);
                        trayPopups.Schedule("CAN'T ADD OR UPDATE CREATE", message, 4000);
                    }
                    else
                    {
                        //shield against any possibly missed situations
                        const string message = "add/update creature failed for unknown reasons";
                        _grangerDebug.Log(message);
                        logger.Error(message);
                        trayPopups.Schedule("CAN'T ADD OR UPDATE CREATE", message, 4000);
                    }
                }
                else
                {
                    _grangerDebug.Log("creature data was invalid, data: " + GetVerifyListData(_verifyList));
                }
                //clear the buffer
                _newCreature = null;
                _grangerDebug.Log("processor buffer cleared");
            }
        }

        private string GetVerifyListData(ProcessorVerifyList verifyList)
        {
                            //get { return (Name && (Gender || Parents || Traits || CaredBy)); }
            return string.Format("(name: {0}, Gender: {1}, Parents: {2}, Traits: {3}, CaredBy: {4})",
                verifyList.Name, verifyList.Gender, verifyList.Parents, verifyList.Traits, verifyList.CaredBy);
        }

        private void AddNewCreature(string selectedHerd, CreatureBuilder newCreature)
        {
            var newEntity = new CreatureEntity
            {
                Id = CreatureEntity.GenerateNewCreatureId(_context),
                Name = newCreature.Name,
                Herd = selectedHerd,
                Age = _newCreature.Age,
                TakenCareOfBy = newCreature.CaredBy,
                BrandedFor = newCreature.BrandedBy,
                FatherName = newCreature.Father,
                MotherName = newCreature.Mother,
                Traits = newCreature.Traits,
                TraitsInspectedAtSkill = newCreature.InspectSkill,
                IsMale = newCreature.IsMale,
                PregnantUntil = newCreature.PregnantUntil,
                SecondaryInfoTagSetter = newCreature.SecondaryInfo
            };

            newEntity.EpicCurve = newCreature.ServerGroup.ServerGroupId == ServerGroup.EpicId;

            _context.InsertCreature(newEntity);
            _grangerDebug.Log("successfully inserted creature to db");
            trayPopups.Schedule("CREATURE ADDED", String.Format("Added new creature to herd {0}: {1}", selectedHerd, newEntity));
        }

        private string[] GetAllHerds()
        {
            return _context.Herds.ToArray()
                .Select(x => x.HerdID.ToString()).ToArray();
        }

        private CreatureEntity[] GetHerdsFinds(IEnumerable<string> selectedHerds, string newCreatureName)
        {
            return _context.Creatures
                .Where(x =>
                    x.Name == newCreatureName &&
                    selectedHerds.Contains(x.Herd))
                .ToArray();
        }

        private string[] GetSelectedHerds()
        {
            return _context.Herds.ToArray()
                .Where(x => x.Selected == true)
                .Select(x => x.HerdID.ToString()).ToArray();
        }

        void AttemptToStartProcessing(string line)
        {
            _grangerDebug.Log("attempting to start processing creature due to line: " + line);
            //clean up if there is still non-timed out process
            VerifyAndApplyProcessing();

            //it is unknown if smiled at creature or something else
            //attempt to extract the name of game object
            try
            {
                //[20:48:42] You smile at Adolescent diseased Mountainheart.
                _grangerDebug.Log("extracting object name");
                string objectNameWithPrefixes = line.Remove(0, 13).Replace(".", "");
                if (!GrangerHelpers.IsBlacklistedCreatureName(objectNameWithPrefixes) && GrangerHelpers.HasAgeInName(objectNameWithPrefixes))
                {
                    _grangerDebug.Log("object asumed to be a creature");
                    var ahSkill = _playerMan.GetAhSkill();
                    var currentGroup = _playerMan.GetCurrentServerGroup();
                    if (ahSkill != null)
                    {
                        _grangerDebug.Log("building new creature object and moving to processor");

                        _isProcessing = true;
                        _startedProcessingOn = DateTime.Now;
                        _verifyList = new ProcessorVerifyList();
                        _newCreature = new CreatureBuilder
                        {
                            Name = GrangerHelpers.ExtractCreatureName(objectNameWithPrefixes),
                            Age = GrangerHelpers.ExtractCreatureAge(objectNameWithPrefixes),
                            ServerGroup = currentGroup,
                            InspectSkill = ahSkill.Value,
                            //IsDiseased =
                            //    (GrangerHelpers.LineContainsDiseased(objectNameWithPrefixes) != null)
                        };

                        var fat = GrangerHelpers.LineContainsFat(objectNameWithPrefixes);
                        if (fat != null) _newCreature.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Fat;

                        var starving = GrangerHelpers.LineContainsStarving(objectNameWithPrefixes);
                        if (starving != null) _newCreature.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Starving;

                        var diseased = GrangerHelpers.LineContainsDiseased(objectNameWithPrefixes);
                        if (diseased != null) _newCreature.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Diseased;

                        _verifyList.Name = true;
                        _grangerDebug.Log("finished building");
                    }
                    else
                    {
                        trayPopups.Schedule("CAN'T PROCESS CREATURE", "Cannot gather data for " + _playerMan.PlayerName + " yet, please try again once Granger fully loads.", 5000);
                        _grangerDebug.Log("processing creature cancelled, still waiting for AH skill or server group searches to finish (skill: " + ahSkill + " ; server group: " + currentGroup);
                    }
                }
                else _grangerDebug.Log(objectNameWithPrefixes + " cannot be added. Only named creatures can be added to Granger.");
            }
            catch (Exception _e)
            {
                //this shouldn't happen, there is always something player is smiling at, unless error happened elsewhere
                _grangerDebug.Log("! Granger: error while BeginProcessing, event: " + line, true, _e);
            }
        }

        public void Update()
        {
            if (_isProcessing)
            {
                if (DateTime.Now > _startedProcessingOn + _processorTimeout)
                {
                    _grangerDebug.Log("processing timed out, attempting to verify and apply last inspected creature");
                    _isProcessing = false;
                    VerifyAndApplyProcessing();
                }
            }
        }
    }
}
