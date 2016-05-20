using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Modules.LogFeedManager
{
    class SmileXamineProcessor
    {
        class ProcessorVerifyList
        {
            public bool Name;
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

        class CreatureBuffer
        {
            public string Name;
            public CreatureAge Age;
            public string CaredBy;
            public string BrandedBy;
            public string Father;
            public string Mother;
            public readonly List<CreatureTrait> Traits = new List<CreatureTrait>();
            public float InspectSkill;
            public bool IsMale;
            public DateTime PregnantUntil = DateTime.MinValue;
            public IWurmServer Server;
            public CreatureEntity.SecondaryInfoTag SecondaryInfo = CreatureEntity.SecondaryInfoTag.None;
        }

        static readonly TimeSpan ProcessorTimeout = new TimeSpan(0, 0, 5);

        readonly GrangerDebugLogger grangerDebug;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;

        bool isProcessing = false;
        DateTime startedProcessingOn;
        CreatureBuffer creatureBuffer;
        ProcessorVerifyList verifyList;

        private readonly GrangerFeature parentModule;
        private readonly GrangerContext context;
        private readonly PlayerManager playerMan;

        public SmileXamineProcessor(GrangerFeature parentModule, GrangerContext context, PlayerManager playerMan,
            GrangerDebugLogger debugLogger,
            [NotNull] ITrayPopups trayPopups, [NotNull] ILogger logger)
        {
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (logger == null) throw new ArgumentNullException("logger");
            grangerDebug = debugLogger;
            this.trayPopups = trayPopups;
            this.logger = logger;
            this.parentModule = parentModule;
            this.context = context;
            this.playerMan = playerMan;
        }

        public void HandleLogEvent(string line)
        {
            //attempt to start building new creature data
            if (line.StartsWith("You smile at", StringComparison.Ordinal))
            {
                grangerDebug.Log("smile cond: " + line);
                AttemptToStartProcessing(line);
            }
            // append/update incoming data to current creature in buffer
            if (isProcessing)
            {
                //[20:23:18] It has fleeter movement than normal. It has a strong body. It has lightning movement. It can carry more than average. It seems overly aggressive.                           
                if (!verifyList.Traits && CreatureTrait.CanThisBeTraitLogMessage(line))
                {
                    grangerDebug.Log("found maybe trait line: " + line);
                    var extractedTraits = GrangerHelpers.GetTraitsFromLine(line);
                    foreach (var trait in extractedTraits)
                    {
                        grangerDebug.Log("found trait: " + trait);
                        creatureBuffer.Traits.Add(trait);
                        verifyList.Traits = true;
                    }
                    grangerDebug.Log("trait parsing finished");
                    if (creatureBuffer.InspectSkill == 0 && creatureBuffer.Traits.Count > 0)
                    {
                        var message =
                            String.Format(
                                "{0} ({1}) can see traits, but Granger found no Animal Husbandry skill for him. Is this a bug? Creature will be added anyway.",
                                playerMan.PlayerName, creatureBuffer.Server);
                        logger.Error(message);
                        trayPopups.Schedule(message, "POSSIBLE PROBLEM", 5000);
                    }
                }
                //[20:23:18] She is very strong and has a good reserve of fat.
                if (line.StartsWith("He", StringComparison.Ordinal) && !verifyList.Gender)
                {
                    creatureBuffer.IsMale = true;
                    verifyList.Gender = true;
                    grangerDebug.Log("creature set to male");
                }
                if (line.StartsWith("She", StringComparison.Ordinal) && !verifyList.Gender)
                {
                    creatureBuffer.IsMale = false;
                    verifyList.Gender = true;
                    grangerDebug.Log("creature set to female");
                }
                //[01:05:57] Mother is Venerable fat Starkdance. Father is Venerable fat Jollypie. 
                if ((line.Contains("Mother is") || line.Contains("Father is")) && !verifyList.Parents)
                {
                    grangerDebug.Log("found maybe parents line");
                    Match match = Regex.Match(line, @"Mother is (?<g>\w+ \w+ .+?)\.|Mother is (?<g>\w+ .+?)\.");
                    if (match.Success)
                    {
                        string mother = match.Groups["g"].Value;
                        mother = GrangerHelpers.ExtractCreatureName(mother);
                        creatureBuffer.Mother = mother;
                        grangerDebug.Log("mother set to: " + mother);
                    }
                    Match match2 = Regex.Match(line, @"Father is (?<g>\w+ \w+ .+?)\.|Father is (?<g>\w+ .+?)\.");
                    if (match2.Success)
                    {
                        string father = match2.Groups["g"].Value;
                        father = GrangerHelpers.ExtractCreatureName(father);
                        creatureBuffer.Father = father;
                        grangerDebug.Log("father set to: " + father);
                    }
                    verifyList.Parents = true;
                    grangerDebug.Log("finished parsing parents line");
                }
                //[20:23:18] It is being taken care of by Darkprincevale.
                if (line.Contains("It is being taken care") && !verifyList.CaredBy)
                {
                    grangerDebug.Log("found maybe take care of line");
                    Match caredby = Regex.Match(line, @"care of by (\w+)");
                    if (caredby.Success)
                    {
                        creatureBuffer.CaredBy = caredby.Groups[1].Value;
                        grangerDebug.Log("cared set to: " + creatureBuffer.CaredBy);
                    }
                    verifyList.CaredBy = true;
                    grangerDebug.Log("finished parsing care line");
                }
                //[17:11:42] She will deliver in about 4 days.
                //[17:11:42] She will deliver in about 1 day.
                if (line.Contains("She will deliver in") && !verifyList.Pregnant)
                {
                    grangerDebug.Log("found maybe pregnant line");
                    Match match = Regex.Match(line, @"She will deliver in about (\d+)");
                    if (match.Success)
                    {
                        double length = Double.Parse(match.Groups[1].Value) + 1D;
                        creatureBuffer.PregnantUntil = DateTime.Now + TimeSpan.FromDays(length);
                        grangerDebug.Log("found creature to be pregnant, estimated delivery: " + creatureBuffer.PregnantUntil);
                    }
                    verifyList.Pregnant = true;
                    grangerDebug.Log("finished parsing pregnant line");
                }
                //[20:58:26] A foal skips around here merrily
                //[01:59:09] This calf looks happy and free.
                if ((line.Contains("A foal skips around here merrily") 
                    || line.Contains("This calf looks happy and free")
                    || line.Contains("A small cuddly ball of fluff"))
                    && !verifyList.Foalization)
                {
                    grangerDebug.Log("applying foalization to the creature");
                    try
                    {
                        creatureBuffer.Age.Foalize();
                        verifyList.Foalization = true;
                    }
                    catch (InvalidOperationException exception)
                    {
                        logger.Error(exception, "The creature appears to be a foal, but has invalid age for a foal!");
                    }
                }
                //[20:57:27] It has been branded by and belongs to the settlement of Silver Hill Estate.
                if (line.Contains("It has been branded") && !verifyList.Branding)
                {
                    grangerDebug.Log("found maybe branding line");
                    Match match = Regex.Match(line, @"belongs to the settlement of (.+)\.");
                    if (match.Success)
                    {
                        string settlementName = match.Groups[1].Value;
                        creatureBuffer.BrandedBy = settlementName;
                        grangerDebug.Log("found creature to be branded for: " + creatureBuffer.BrandedBy);
                        verifyList.Branding = true;
                    }
                }
            }
        }

        void VerifyAndApplyProcessing()
        {
            if (creatureBuffer != null)
            {
                try
                {
                    grangerDebug.Log("finishing processing creature: " + creatureBuffer.Name);
                    //verify if enough fields are filled to warrant updating
                    if (verifyList.IsValid)
                    {
                        grangerDebug.Log("Creature data is valid");

                        var selectedHerds = GetSelectedHerds();

                        var herdsFinds = GetHerdsFinds(selectedHerds, creatureBuffer, checkInnerName: false);
                        if (herdsFinds.Length == 0)
                        {
                            herdsFinds = GetHerdsFinds(selectedHerds, creatureBuffer, checkInnerName: true);
                        }
                        var selectedHerdsFinds = herdsFinds;
                        // if there isn't any creature found in selected herds,
                        // try all herds if setting is enabled
                        bool allHerdSearch = false;
                        if (herdsFinds.Length == 0 &&
                            parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb)
                        {
                            allHerdSearch = true;
                            string[] allHerds = GetAllHerds();

                            herdsFinds = GetHerdsFinds(allHerds, creatureBuffer, checkInnerName: false);
                            if (herdsFinds.Length == 0)
                            {
                                herdsFinds = GetHerdsFinds(allHerds, creatureBuffer, checkInnerName: true);
                            }
                        }

                        // first try to update
                        // update only if found exactly one creature
                        if (!TryUpdateExistingCreature(herdsFinds))
                        {
                            // no update performed, try to add
                            if (!TryAddNewCreature(selectedHerds, selectedHerdsFinds))
                            {
                                // no update or add performed, figure what went wrong
                                AnalyzeWhyNothingHappened(herdsFinds, allHerdSearch, selectedHerds);
                            }
                        }
                    }
                    else
                    {
                        grangerDebug.Log("creature data was invalid, data: " + GetVerifyListData(verifyList));
                    }
                }
                finally
                {
                    //clear the buffer
                    creatureBuffer = null;
                    grangerDebug.Log("processor buffer cleared");
                }
            }
        }

        bool TryUpdateExistingCreature(CreatureEntity[] herdsFinds)
        {
            if (herdsFinds.Length == 1)
            {
                CreatureEntity oldCreature = herdsFinds[0];

                bool sanityFail = false;

                #region SANITY_CHECKS

                // comment: are these sanity checks really needed?

                string sanityFailReason = null;

                // note: dropped age check, due to foal recognition issues with non-horses

                // if both creatures have a mother name or father name, they cant change
                // however its possible a mother or father dies and reference is lost (thats how it works in Wurm), 
                // creature appears then as if it had no father or mother

                // father checks
                if (String.IsNullOrEmpty(oldCreature.FatherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.Father))
                {
                    sanityFail = true;
                    if (sanityFailReason == null)
                        sanityFailReason = "Old father was blank but new data has a father name";
                }

                if (!String.IsNullOrEmpty(oldCreature.FatherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.Father) &&
                    oldCreature.FatherName != creatureBuffer.Father)
                {
                    sanityFail = true;
                    if (sanityFailReason == null)
                        sanityFailReason = "Old data father name was different than new father name";
                }

                // mother checks
                if (String.IsNullOrEmpty(oldCreature.MotherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.Mother))
                {
                    sanityFail = true;
                    if (sanityFailReason == null)
                        sanityFailReason = "Old mother was blank but new data has a mother name";
                }

                if (!String.IsNullOrEmpty(oldCreature.MotherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.Mother) &&
                    oldCreature.MotherName != creatureBuffer.Mother)
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
                    grangerDebug.Log(string.Format("Checking creature for Genesis cast (creature name: {0}",
                        creatureBuffer.Name));
                    if (!parentModule.Settings.HasGenesisCast(creatureBuffer.Name))
                    {
                        grangerDebug.Log("No genesis cast found");
                        var lowskill = Math.Min(oldCreature.TraitsInspectedAtSkill.Value, creatureBuffer.InspectSkill);
                        CreatureTrait[] certainTraits = CreatureTrait.GetTraitsUpToSkillLevel(lowskill,
                            oldCreature.EpicCurve ?? false);
                        var oldCreatureTraits = oldCreature.Traits.ToArray();
                        var newCreatureTraits = creatureBuffer.Traits.ToArray();
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
                        grangerDebug.Log("Genesis cast found, skipping trait sanity check");
                        parentModule.Settings.RemoveGenesisCast(creatureBuffer.Name);
                        grangerDebug.Log(string.Format("Removed cached genesis cast data for {0}",
                            creatureBuffer.Name));
                    }
                }

                #endregion

                if (sanityFail)
                {
                    grangerDebug.Log("sanity check failed for creature update: " + oldCreature + ". Reason: " +
                                     sanityFailReason);
                    trayPopups.Schedule("There was data mismatch when trying to update creature, reason: " + sanityFailReason,
                        "ERROR AT UPDATE CREATURE",
                        8000);
                }
                else
                {
                    oldCreature.Age = creatureBuffer.Age;
                    oldCreature.TakenCareOfBy = creatureBuffer.CaredBy;
                    oldCreature.BrandedFor = creatureBuffer.BrandedBy;
                    oldCreature.FatherName = creatureBuffer.Father;
                    oldCreature.MotherName = creatureBuffer.Mother;
                    oldCreature.ServerName = creatureBuffer.Server.ServerName.Original;
                    if (oldCreature.TraitsInspectedAtSkill <= creatureBuffer.InspectSkill ||
                        creatureBuffer.InspectSkill >
                        CreatureTrait.GetFullTraitVisibilityCap(oldCreature.EpicCurve ?? false))
                    {
                        oldCreature.Traits = creatureBuffer.Traits;
                        oldCreature.TraitsInspectedAtSkill = creatureBuffer.InspectSkill;
                    }
                    else
                    {
                        grangerDebug.Log("old creature data had more accurate trait info, skipping");
                    }
                    oldCreature.SetTag("dead", false);
                    oldCreature.SetSecondaryInfoTag(creatureBuffer.SecondaryInfo);
                    oldCreature.IsMale = creatureBuffer.IsMale;
                    oldCreature.PregnantUntil = creatureBuffer.PregnantUntil;
                    if (oldCreature.Name != creatureBuffer.Name)
                    {
                        if (NameUniqueInHerd(creatureBuffer.Name, oldCreature.Herd))
                        {
                            trayPopups.Schedule(String.Format("Updating name of creature {0} to {1}",
                                oldCreature.Name,
                                creatureBuffer.Name), "CREATURE NAME UPDATE");
                            oldCreature.Name = creatureBuffer.Name;
                        }
                        else
                        {
                            trayPopups.Schedule(String.Format("Could not update name of creature {0} to {1}, " +
                                                              "because herd already has a creature with such name.",
                                oldCreature.Name,
                                creatureBuffer.Name), "WARNING");
                        }
                    }

                    context.SubmitChanges();
                    grangerDebug.Log("successfully updated creature in db");
                    trayPopups.Schedule(String.Format("Updated creature: {0}", oldCreature), "CREATURE UPDATED");
                }

                grangerDebug.Log("processor buffer cleared");
                return true;
            }
            return false;
        }

        bool NameUniqueInHerd(string creatureName, string herdName)
        {
            return !context.Creatures.Any(x => x.Name == creatureName && x.Herd == herdName);
        }

        bool TryAddNewCreature(string[] selectedHerds, CreatureEntity[] selectedHerdsFinds)
        {
            if (selectedHerds.Length == 1 ||
                (parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb && selectedHerds.Length > 0))
            {
                // can't add a creature if it's already in selected herds
                // also with entireDB, this will trigger if for some reason 2 or more creatures are already in db (update is skipped)
                if (selectedHerdsFinds.Length == 0)
                {
                    //do a sanity check to verify this creature name is not in current herd already
                    string herd = selectedHerds[0];
                    var existing =
                        context.Creatures.Where(x => creatureBuffer.Name == x.Name && x.Herd == herd).ToArray();

                    if (!existing.Any())
                    {
                        //add creature
                        AddNewCreature(herd, creatureBuffer);
                    }
                    else
                    {
                        var message = "Creature with name: " + creatureBuffer.Name +
                                      " already exists in herd: " + herd;
                        if (existing.Any(entity =>
                                entity.Name == creatureBuffer.Name
                                && !string.IsNullOrEmpty(entity.ServerName) 
                                && !creatureBuffer.Server.ServerName.Matches(entity.ServerName)))
                        {
                            message += " (creature server name mismatch)";
                        }
                        trayPopups.Schedule(message, "CAN'T ADD CREATURE", 4000);
                        grangerDebug.Log(message);
                    }

                    grangerDebug.Log("processor buffer cleared");
                    return true;
                }
                else if (!parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb)
                {
                    string message = "Creature with name: " + creatureBuffer.Name +
                                     " already exists in active herd";

                    trayPopups.Schedule(message, "CAN'T ADD CREATURE", 4000);
                    grangerDebug.Log(message);
                }
            }
            return false;
        }

        void AnalyzeWhyNothingHappened(CreatureEntity[] herdsFinds, bool allHerdSearch, string[] selectedHerds)
        {
            if (herdsFinds.Length > 1)
            {
                var partialMessage = allHerdSearch ? "database" : "selected herds";
                grangerDebug.Log("many creatures named " + creatureBuffer.Name + " found in "
                                 + partialMessage
                                 +
                                 ", add/update aborted");
                trayPopups.Schedule(
                    partialMessage + " contain many creatures named " + creatureBuffer.Name
                    + ", narrow herd selection",
                    "CAN'T ADD OR UPDATE CREATURE",
                    6000);
                //notify user to narrow the herd selection
            }
            else if (!parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb
                     && (selectedHerds.Length == 0 || selectedHerds.Length > 1))
            {
                const string message = "exactly one herd has to be active to add new creature";
                grangerDebug.Log(message);
                trayPopups.Schedule(message, "CAN'T ADD OR UPDATE CREATURE", 4000);
            }
            else if (parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb
                     && selectedHerds.Length == 0)
            {
                const string message = "at least one herd must be select to add new creature";
                grangerDebug.Log(message);
                trayPopups.Schedule(message, "CAN'T ADD OR UPDATE CREATURE", 4000);
            }
            else
            {
                //shield against any possibly missed situations
                const string message = "add/update creature failed for unknown reasons";
                grangerDebug.Log(message);
                logger.Error(message);
                trayPopups.Schedule(message, "CAN'T ADD OR UPDATE CREATURE", 4000);
            }
        }

        private string GetVerifyListData(ProcessorVerifyList verifyList)
        {
            return string.Format("(name: {0}, Gender: {1}, Parents: {2}, Traits: {3}, CaredBy: {4})",
                verifyList.Name, verifyList.Gender, verifyList.Parents, verifyList.Traits, verifyList.CaredBy);
        }

        private void AddNewCreature(string selectedHerd, CreatureBuffer newCreature)
        {
            var newEntity = new CreatureEntity
            {
                Id = CreatureEntity.GenerateNewCreatureId(context),
                Name = newCreature.Name,
                Herd = selectedHerd,
                Age = newCreature.Age,
                TakenCareOfBy = newCreature.CaredBy,
                BrandedFor = newCreature.BrandedBy,
                FatherName = newCreature.Father,
                MotherName = newCreature.Mother,
                Traits = newCreature.Traits,
                TraitsInspectedAtSkill = newCreature.InspectSkill,
                IsMale = newCreature.IsMale,
                PregnantUntil = newCreature.PregnantUntil,
                SecondaryInfoTagSetter = newCreature.SecondaryInfo,
                ServerName = newCreature.Server != null ? newCreature.Server.ServerName.Original : string.Empty
            };

            newEntity.EpicCurve = newCreature.Server != null
                                  && newCreature.Server.ServerGroup.ServerGroupId == ServerGroup.EpicId;

            context.InsertCreature(newEntity);
            grangerDebug.Log("successfully inserted creature to db");
            trayPopups.Schedule(String.Format("Added new creature to herd {0}: {1}", selectedHerd, newEntity), "CREATURE ADDED");
        }

        private string[] GetAllHerds()
        {
            return context.Herds.ToArray()
                .Select(x => x.HerdID.ToString()).ToArray();
        }

        private CreatureEntity[] GetHerdsFinds(IEnumerable<string> viableHerds, CreatureBuffer creatureBuffer, bool checkInnerName)
        {
            IEnumerable<CreatureEntity> query;
            if (checkInnerName)
            {
                var bufferInnerNameInfo = Creature.GetInnerNameInfo(creatureBuffer.Name);
                if (!bufferInnerNameInfo.HasInnerName)
                {
                    // cannot match by inner name, this creature doesn't have one
                    return new CreatureEntity[0];
                }
                query = context.Creatures
                               .Where(x =>
                               {
                                   var iteratedInnerNameInfo = Creature.GetInnerNameInfo(x.Name);
                                   // consider only those creatures, which have inner names
                                   if (!iteratedInnerNameInfo.HasInnerName)
                                   {
                                       return false;
                                   }
                                   return iteratedInnerNameInfo.InnerName == bufferInnerNameInfo.InnerName &&
                                          viableHerds.Contains(x.Herd);
                               });
            }
            else
            {
                query = context.Creatures
                               .Where(x =>
                                   x.Name == creatureBuffer.Name &&
                                   viableHerds.Contains(x.Herd));
            }

            if (parentModule.Settings.UseServerNameAsCreatureIdComponent)
            {
                query =
                    query.Where(
                        entity =>
                            string.IsNullOrEmpty(entity.ServerName)
                            || creatureBuffer.Server.ServerName.Matches(entity.ServerName));
            }
            return query.ToArray();
        }

        private string[] GetSelectedHerds()
        {
            return context.Herds.ToArray()
                .Where(x => x.Selected)
                .Select(x => x.HerdID.ToString()).ToArray();
        }

        void AttemptToStartProcessing(string line)
        {
            grangerDebug.Log("attempting to start processing creature due to line: " + line);
            //clean up if there is still non-timed out process
            VerifyAndApplyProcessing();

            //it is unknown if smiled at creature or something else
            //attempt to extract the name of game object
            try
            {
                //[20:48:42] You smile at Adolescent diseased Mountainheart.
                grangerDebug.Log("extracting object name");
                string objectNameWithPrefixes = line.Remove(0, 13).Replace(".", "");
                if (GrangerHelpers.HasAgeInName(objectNameWithPrefixes, ignoreCase:true))
                {
                    grangerDebug.Log("object asumed to be a creature");
                    var server = playerMan.CurrentServer;
                    var skill = playerMan.CurrentServerAhSkill;
                    if (server != null && skill != null)
                    {
                        grangerDebug.Log("building new creature object and moving to processor");

                        isProcessing = true;
                        startedProcessingOn = DateTime.Now;
                        verifyList = new ProcessorVerifyList();
                        creatureBuffer = new CreatureBuffer
                        {
                            Name = GrangerHelpers.ExtractCreatureName(objectNameWithPrefixes),
                            Age = GrangerHelpers.ExtractCreatureAge(objectNameWithPrefixes),
                            Server = server,
                            InspectSkill = skill.Value,
                        };

                        var fat = GrangerHelpers.LineContainsFat(objectNameWithPrefixes);
                        if (fat != null) creatureBuffer.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Fat;

                        var starving = GrangerHelpers.LineContainsStarving(objectNameWithPrefixes);
                        if (starving != null) creatureBuffer.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Starving;

                        var diseased = GrangerHelpers.LineContainsDiseased(objectNameWithPrefixes);
                        if (diseased != null) creatureBuffer.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Diseased;

                        verifyList.Name = true;
                        grangerDebug.Log("finished building");
                    }
                    else
                    {
                        trayPopups.Schedule(
                            "Server or AH skill level unknown for " + playerMan.PlayerName +
                            ". If WA was just started, give it a few seconds.", "CAN'T PROCESS CREATURE", 5000);
                        grangerDebug.Log(string.Format("processing creature cancelled, AH skill or server group unknown for player {0} (skill: {1} ; server: {2}", playerMan.PlayerName, skill, server));
                    }
                }
                else grangerDebug.Log(objectNameWithPrefixes + " cannot be added. Only named creatures can be added to Granger.");
            }
            catch (Exception exception)
            {
                //this shouldn't happen, there is always something player is smiling at, unless error happened elsewhere
                grangerDebug.Log("! Granger: error while BeginProcessing, event: " + line, true, exception);
            }
        }

        public void Update()
        {
            if (isProcessing)
            {
                if (DateTime.Now > startedProcessingOn + ProcessorTimeout)
                {
                    grangerDebug.Log("processing timed out, attempting to verify and apply last inspected creature");
                    isProcessing = false;
                    VerifyAndApplyProcessing();
                }
            }
        }
    }
}
