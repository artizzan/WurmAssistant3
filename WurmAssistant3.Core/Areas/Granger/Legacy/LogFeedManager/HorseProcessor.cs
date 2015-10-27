using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.LogFeedManager
{
    class HorseProcessor
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

        class HorseBuilder
        {
            public string Name;
            public HorseAge Age;
            public string CaredBy;
            public string BrandedBy;
            public string Father;
            public string Mother;
            public List<HorseTrait> Traits = new List<HorseTrait>();
            public bool IsDiseased;
            public float InspectSkill;
            public bool IsMale;
            public DateTime PregnantUntil = DateTime.MinValue;
            public ServerGroupId ServerGroup;
            public HorseEntity.SecondaryInfoTag SecondaryInfo = HorseEntity.SecondaryInfoTag.None;
        }

        //public ModuleGranger Parent;

        readonly GrangerDebugLogger _grangerDebug;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;

        readonly TimeSpan _processorTimeout = new TimeSpan(0, 0, 5);
        bool _isProcessing = false;
        DateTime _startedProcessingOn;
        HorseBuilder _newHorse;
        ProcessorVerifyList _verifyList;

        private readonly GrangerFeature _parentModule;
        private readonly GrangerContext _context;
        private readonly PlayerManager _playerMan;

        //float? AHSkill = null;

        public HorseProcessor(GrangerFeature parentModule, GrangerContext context, PlayerManager playerMan,
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

            //attempt to start building new horse data
            if (line.StartsWith("You smile at", StringComparison.Ordinal))
            {
                _grangerDebug.Log("smile cond: " + line);
                AttemptToStartProcessing(line);
            }
            // append/update incoming data to current horse in buffer
            if (_isProcessing)
            {
                //[20:23:18] It has fleeter movement than normal. It has a strong body. It has lightning movement. It can carry more than average. It seems overly aggressive.                           
                if (!_verifyList.Traits && HorseTrait.CanThisBeTraitLogMessage(line))
                {
                    _grangerDebug.Log("found maybe trait line: " + line);
                    var extractedTraits = GrangerHelpers.GetTraitsFromLine(line);
                    foreach (var trait in extractedTraits)
                    {
                        _grangerDebug.Log("found trait: " + trait);
                        _newHorse.Traits.Add(trait);
                        _verifyList.Traits = true;
                    }
                    _grangerDebug.Log("trait parsing finished");
                    if (_newHorse.InspectSkill == 0 && _newHorse.Traits.Count > 0)
                    {
                        var message =
                            String.Format(
                                "{0} ({1}) can see traits, but Granger found no Animal Husbandry skill for him. Is this a bug? Creature will be added anyway.",
                                _playerMan.PlayerName, _newHorse.ServerGroup);
                        logger.Error(message);
                        trayPopups.Schedule("POSSIBLE PROBLEM", message, 5000);
                    }
                }
                //[20:23:18] She is very strong and has a good reserve of fat.
                if (line.StartsWith("He", StringComparison.Ordinal) && !_verifyList.Gender)
                {
                    _newHorse.IsMale = true;
                    _verifyList.Gender = true;
                    _grangerDebug.Log("creature set to male");
                }
                if (line.StartsWith("She", StringComparison.Ordinal) && !_verifyList.Gender)
                {
                    _newHorse.IsMale = false;
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
                        mother = GrangerHelpers.ExtractHorseName(mother);
                        _newHorse.Mother = mother;
                        _grangerDebug.Log("mother set to: " + mother);
                    }
                    Match match2 = Regex.Match(line, @"Father is (?<g>\w+ \w+ .+?)\.|Father is (?<g>\w+ .+?)\.");
                    if (match2.Success)
                    {
                        string father = match2.Groups["g"].Value;
                        father = GrangerHelpers.ExtractHorseName(father);
                        _newHorse.Father = father;
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
                        _newHorse.CaredBy = caredby.Groups[1].Value;
                        _grangerDebug.Log("cared set to: " + _newHorse.CaredBy);
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
                        _newHorse.PregnantUntil = DateTime.Now + TimeSpan.FromHours(length * 21D);
                        _grangerDebug.Log("found creature to be pregnant, estimated delivery: " + _newHorse.PregnantUntil);
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
                        _newHorse.Age.Foalize();
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
                        _newHorse.BrandedBy = settlementName;
                        _grangerDebug.Log("found creature to be branded for: " + _newHorse.BrandedBy);
                        _verifyList.Branding = true;
                    }
                }
            }
        }

        void VerifyAndApplyProcessing()
        {
            if (_newHorse != null)
            {
                _grangerDebug.Log("finishing processing creature: " + _newHorse.Name);
                //verify if enough fields are filled to warrant updating
                if (_verifyList.IsValid)
                {
                    _grangerDebug.Log("Creature data is valid");

                    var selectedHerds = GetSelectedHerds();

                    //string[] herdsToCheck = selectedHerds;

                    var herdsFinds = GetHerdsFinds(selectedHerds, _newHorse.Name);
                    var selectedHerdsFinds = herdsFinds;
                    // if there isn't any horse found in selected herds,
                    // try all herds if setting is set
                    bool allHerdSearch = false;
                    if (herdsFinds.Length == 0 &&
                        _parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb)
                    {
                        allHerdSearch = true;
                        string[] allHerds = GetAllHerds();

                        herdsFinds = GetHerdsFinds(allHerds, _newHorse.Name);
                    }

                    // first try to update
                    // update only if found exactly one horse
                    if (herdsFinds.Length == 1)
                    {
                        HorseEntity oldHorse = herdsFinds[0];

                        bool sanityFail = false;

                        #region SANITY_CHECKS

                        //perform sanity checks
                        string sanityFailReason = null;
                        //horses cant suddenly get younger
                        if (oldHorse.Age > _newHorse.Age)
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "New creature data would make the creature younger than it was";
                        }
                        //basically if both horses HAVE a mother name or father name, they cant have different names
                        //but its entirely possible a mother or father dies and reference is lost, with it the name
                        //no longer is shown, horse appears then as if it had no father or mother

                        //2 cases to check
                        //if current father name is blank, new name can not suddenly hold a name!
                        if (String.IsNullOrEmpty(oldHorse.FatherName) &&
                            !String.IsNullOrEmpty(_newHorse.Father))
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old father was blank but new data has a father name";
                        }

                        //if both names are not blank, then they can't be different!
                        if (!String.IsNullOrEmpty(oldHorse.FatherName) &&
                            !String.IsNullOrEmpty(_newHorse.Father) &&
                            oldHorse.FatherName != _newHorse.Father)
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old data father name was different than new father name";
                        }

                        //same for mother
                        if (String.IsNullOrEmpty(oldHorse.MotherName) &&
                            !String.IsNullOrEmpty(_newHorse.Mother))
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old mother was blank but new data has a mother name";
                        }

                        if (!String.IsNullOrEmpty(oldHorse.MotherName) &&
                            !String.IsNullOrEmpty(_newHorse.Mother) &&
                            oldHorse.MotherName != _newHorse.Mother)
                        {
                            sanityFail = true;
                            if (sanityFailReason == null)
                                sanityFailReason = "Old data mother name was different than new mother name";
                        }

                        //need to compare traits up to lower AH inspect level,
                        //if they mismatch, thats also sanity fail
                        //we should treat null ah inspect value as 0

                        if (oldHorse.TraitsInspectedAtSkill.HasValue)
                        {
                            //exclude this check if horse had genesis cast within last 1 hour
                            _grangerDebug.Log(string.Format("Checking creature for Genesis cast (creature name: {0}",
                                _newHorse.Name));
                            if (!_parentModule.Settings.HasGenesisCast(_newHorse.Name))
                            {
                                _grangerDebug.Log("No genesis cast found");
                                var lowskill = Math.Min(oldHorse.TraitsInspectedAtSkill.Value, _newHorse.InspectSkill);
                                HorseTrait[] certainTraits = HorseTrait.GetTraitsUpToSkillLevel(lowskill,
                                    oldHorse.EpicCurve ?? false);
                                var oldHorseTraits = oldHorse.Traits.ToArray();
                                var newHorseTraits = _newHorse.Traits.ToArray();
                                foreach (var trait in certainTraits)
                                {
                                    if (oldHorseTraits.Contains(trait) != newHorseTraits.Contains(trait))
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
                                _parentModule.Settings.RemoveGenesisCast(_newHorse.Name);
                                _grangerDebug.Log(string.Format("Removed cached genesis cast data for {0}",
                                    _newHorse.Name));
                            }
                        }
                        //is new horse server group not within allowed ones?
                        if (_newHorse.ServerGroup == ServerGroupId.Unknown)
                        {
                            sanityFail = true;
                            sanityFailReason = "New creature data had unsupported server group: " + _newHorse.ServerGroup;
                        }
                        //if old horse isEpic != new horse isEpic
                        bool oldIsEpic = oldHorse.EpicCurve ?? false;
                        bool newIsEpic = _newHorse.ServerGroup == ServerGroupId.Epic;
                        if (oldIsEpic != newIsEpic)
                        {
                            sanityFail = true;
                            sanityFailReason = "Old creature is of different server group than current player server group";
                        }

                        #endregion

                        if (sanityFail)
                        {
                            _grangerDebug.Log("sanity check failed for creature update: " + oldHorse + ". Reason: " +
                                              sanityFailReason);
                            trayPopups.Schedule("COULD NOT UPDATE CREATURE",
                                "There was data mismatch when trying to update creature, reason: " + sanityFailReason, 8000);
                        }
                        else
                        {
                            oldHorse.Age = _newHorse.Age;
                            oldHorse.TakenCareOfBy = _newHorse.CaredBy;
                            oldHorse.BrandedFor = _newHorse.BrandedBy;
                            oldHorse.FatherName = _newHorse.Father;
                            oldHorse.MotherName = _newHorse.Mother;
                            if (oldHorse.TraitsInspectedAtSkill <= _newHorse.InspectSkill ||
                                _newHorse.InspectSkill >
                                HorseTrait.GetFullTraitVisibilityCap(oldHorse.EpicCurve ?? false))
                            {
                                oldHorse.Traits = _newHorse.Traits;
                                oldHorse.TraitsInspectedAtSkill = _newHorse.InspectSkill;
                            }
                            else
                                _grangerDebug.Log("old creature data had more accurate trait info, skipping");
                            oldHorse.SetTag("dead", false);
                            //oldHorse.SetTag("diseased", _newHorse.IsDiseased);
                            oldHorse.SetSecondaryInfoTag(_newHorse.SecondaryInfo);
                            oldHorse.IsMale = _newHorse.IsMale;
                            oldHorse.PregnantUntil = _newHorse.PregnantUntil;

                            _context.SubmitChangesToHorses();
                            _grangerDebug.Log("successfully updated creature in db");
                            trayPopups.Schedule("CREATURE UPDATED", String.Format("Updated creature: {0}", oldHorse));
                        }

                        _newHorse = null;
                        _grangerDebug.Log("processor buffer cleared");
                        return;
                        // we are done here
                    }

                    // no update performed, try to add
                    bool entireDB = _parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb;
                    if (selectedHerds.Length == 1 ||
                        (entireDB && selectedHerds.Length > 0))
                    {
                        // can't add a horse if it's already in selected herds
                        // also with entireDB, this will trigger if for some reason 2 or more horses are already in db (update is skipped)
                        if (selectedHerdsFinds.Length == 0)
                        {
                            //do a sanity check to verify this horse name is not in current herd already
                            string herd = selectedHerds[0];
                            bool exists =
                                _context.Horses.AsEnumerable().Any(x => _newHorse.Name == x.Name && x.Herd == herd);

                            if (!exists)
                            {
                                //add horse
                                AddNewHorse(herd, _newHorse);
                            }
                            else
                            {
                                var message = "Creature with name: " + _newHorse.Name +
                                              " already exists in herd: " + herd;
                                trayPopups.Schedule("CAN'T ADD CREATURE", message, 4000);
                                _grangerDebug.Log(message);
                            }

                            _newHorse = null;
                            _grangerDebug.Log("processor buffer cleared");
                            return;
                            // we are done here
                        }
                        else if (!entireDB)
                        {
                            string message = "Creature with name: " + _newHorse.Name +
                                             " already exists in active herd";

                            trayPopups.Schedule("CAN'T ADD CREATURE", message, 4000);
                            _grangerDebug.Log(message);
                        }
                    }

                    // no update or add performed, figure what went wrong

                    if (herdsFinds.Length > 1)
                    {
                        var partialMessage = allHerdSearch ? "database" : "selected herds";
                        _grangerDebug.Log("many creatures named " + _newHorse.Name + " found in " + partialMessage +
                                          ", add/update aborted");
                        trayPopups.Schedule("CAN'T ADD OR UPDATE CREATE",
                            partialMessage + " contain many creatures named " + _newHorse.Name + ", narrow herd selection",
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
                _newHorse = null;
                _grangerDebug.Log("processor buffer cleared");
            }
        }

        private string GetVerifyListData(ProcessorVerifyList verifyList)
        {
                            //get { return (Name && (Gender || Parents || Traits || CaredBy)); }
            return string.Format("(name: {0}, Gender: {1}, Parents: {2}, Traits: {3}, CaredBy: {4})",
                verifyList.Name, verifyList.Gender, verifyList.Parents, verifyList.Traits, verifyList.CaredBy);
        }

        private void AddNewHorse(string selectedHerd, HorseBuilder newHorse)
        {
            var newEntity = new HorseEntity
            {
                ID = HorseEntity.GenerateNewHorseID(_context),
                Name = newHorse.Name,
                Herd = selectedHerd,
                Age = _newHorse.Age,
                TakenCareOfBy = newHorse.CaredBy,
                BrandedFor = newHorse.BrandedBy,
                FatherName = newHorse.Father,
                MotherName = newHorse.Mother,
                Traits = newHorse.Traits,
                TraitsInspectedAtSkill = newHorse.InspectSkill,
                IsMale = newHorse.IsMale,
                PregnantUntil = newHorse.PregnantUntil,
                SecondaryInfoTagSetter = newHorse.SecondaryInfo
            };

            newEntity.EpicCurve = newHorse.ServerGroup == ServerGroupId.Epic;
            if (newHorse.ServerGroup == ServerGroupId.Unknown)
            {
                logger.Error("Adding creature with unknown server group, name: "+newHorse.Name);
            }

            _context.InsertHorse(newEntity);
            _grangerDebug.Log("successfully inserted creature to db");
            trayPopups.Schedule("CREATURE ADDED", String.Format("Added new creature to herd {0}: {1}", selectedHerd, newEntity));
        }

        private string[] GetAllHerds()
        {
            return _context.Herds.ToArray()
                .Select(x => x.HerdID.ToString()).ToArray();
        }

        private HorseEntity[] GetHerdsFinds(IEnumerable<string> selectedHerds, string newHorseName)
        {
            return _context.Horses
                .Where(x =>
                    x.Name == newHorseName &&
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

            //it is unknown if smiled at horse or something else
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
                    if (ahSkill != null && currentGroup != ServerGroupId.Unknown)
                    {
                        _grangerDebug.Log("building new creature object and moving to processor");

                        _isProcessing = true;
                        _startedProcessingOn = DateTime.Now;
                        _verifyList = new ProcessorVerifyList();
                        _newHorse = new HorseBuilder
                        {
                            Name = GrangerHelpers.ExtractHorseName(objectNameWithPrefixes),
                            Age = GrangerHelpers.ExtractHorseAge(objectNameWithPrefixes),
                            ServerGroup = currentGroup,
                            InspectSkill = ahSkill.Value,
                            //IsDiseased =
                            //    (GrangerHelpers.LineContainsDiseased(objectNameWithPrefixes) != null)
                        };

                        var fat = GrangerHelpers.LineContainsFat(objectNameWithPrefixes);
                        if (fat != null) _newHorse.SecondaryInfo = HorseEntity.SecondaryInfoTag.Fat;

                        var starving = GrangerHelpers.LineContainsStarving(objectNameWithPrefixes);
                        if (starving != null) _newHorse.SecondaryInfo = HorseEntity.SecondaryInfoTag.Starving;

                        var diseased = GrangerHelpers.LineContainsDiseased(objectNameWithPrefixes);
                        if (diseased != null) _newHorse.SecondaryInfo = HorseEntity.SecondaryInfoTag.Diseased;

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
