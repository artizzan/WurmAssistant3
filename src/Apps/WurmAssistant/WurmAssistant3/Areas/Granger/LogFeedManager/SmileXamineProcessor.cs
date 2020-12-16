using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Insights;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager
{
    class SmileXamineProcessor
    {
        class ValidationList
        {
            public bool Name;
            public bool Parents;
            public bool Traits;
            public bool Gender;
            public bool CaredBy;
            public bool Pregnant;
            public bool Foalization;
            public bool Branding;

            public bool IsValid => (Name && (Gender || Parents || Traits || CaredBy));
        }

        class CreatureBuffer
        {
            public string Name;
            public CreatureAge Age;
            public string CaredBy;
            public string BrandedBy;
            public string FatherName;
            public string MotherName;
            public readonly List<CreatureTrait> Traits = new List<CreatureTrait>();
            public float InspectSkill;
            public bool IsMale;
            public DateTime PregnantUntil = DateTime.MinValue;
            public IWurmServer Server;
            public CreatureEntity.SecondaryInfoTag SecondaryInfo = CreatureEntity.SecondaryInfoTag.None;
            public string ColorWurmLogText;
            public bool newBorn;

            public bool HasFatherName => !string.IsNullOrEmpty(FatherName);
            public bool HasMotherName => !string.IsNullOrEmpty(MotherName);
            public bool HasColorWurmLogText => !string.IsNullOrEmpty(ColorWurmLogText);
        }

        static readonly TimeSpan ProcessorTimeout = new TimeSpan(0, 0, 5);

        readonly GrangerDebugLogger debugLogger;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;
        readonly IWurmAssistantConfig wurmAssistantConfig;
        readonly CreatureColorDefinitions creatureColorDefinitions;

        bool isProcessing = false;
        DateTime startedProcessingOn;
        CreatureBuffer creatureBuffer;
        ValidationList verifyList;

        private readonly GrangerFeature parentModule;
        private readonly GrangerContext context;
        private readonly PlayerManager playerMan;
        private readonly GrangerSettings grangerSettings;
        readonly ITelemetry telemetry;

        public SmileXamineProcessor(
            [NotNull] GrangerFeature parentModule,
            [NotNull] GrangerContext context,
            [NotNull] PlayerManager playerMan,
            [NotNull] GrangerDebugLogger debugLogger,
            [NotNull] ITrayPopups trayPopups, [NotNull] ILogger logger,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig,
            [NotNull] CreatureColorDefinitions creatureColorDefinitions,
            [NotNull] GrangerSettings grangerSettings,
            [NotNull] ITelemetry telemetry)
        {
            this.debugLogger = debugLogger ?? throw new ArgumentNullException(nameof(debugLogger));
            this.trayPopups = trayPopups ?? throw new ArgumentNullException(nameof(trayPopups));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.wurmAssistantConfig = wurmAssistantConfig ?? throw new ArgumentNullException(nameof(wurmAssistantConfig));
            this.creatureColorDefinitions = creatureColorDefinitions ?? throw new ArgumentNullException(nameof(creatureColorDefinitions));
            this.parentModule = parentModule ?? throw new ArgumentNullException(nameof(parentModule));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.playerMan = playerMan ?? throw new ArgumentNullException(nameof(playerMan));
            this.grangerSettings = grangerSettings ?? throw new ArgumentNullException(nameof(grangerSettings));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public void HandleLogEvent(string line)
        {
            // Smile emote triggers processing of new creature. 
            // If previous processing is still active, it should be finalized.
            if (line.StartsWith("You smile at", StringComparison.Ordinal))
            {
                debugLogger.Log("smile cond: " + line);
                AttemptToStartProcessing(line);
            }
            // Hug emote triggers processing of new born creature. 
            // If previous processing is still active, it should be finalized.
            else if (line.StartsWith("You hug", StringComparison.Ordinal))
            {
                debugLogger.Log("hug cond: " + line);
                AttemptToStartProcessing(line, true);
            }

            // While processing creature, log events are parsed and valid data buffered into the current buffer.
            if (isProcessing)
            {
                //[20:23:18] It has fleeter movement than normal. It has a strong body. It has lightning movement. It can carry more than average. It seems overly aggressive.                           
                if (!verifyList.Traits && CreatureTrait.CanThisBeTraitLogMessage(line))
                {
                    debugLogger.Log("found maybe trait line: " + line);
                    var extractedTraits = GrangerHelpers.ParseTraitsFromLine(line);
                    foreach (var trait in extractedTraits)
                    {
                        debugLogger.Log("found trait: " + trait);
                        creatureBuffer.Traits.Add(trait);
                        verifyList.Traits = true;
                    }
                    debugLogger.Log("trait parsing finished");
                    if (creatureBuffer.InspectSkill == 0 && creatureBuffer.Traits.Count > 0)
                    {
                        var message =
                            $"{playerMan.PlayerName} ({creatureBuffer.Server}) can see traits, but Granger found no Animal Husbandry skill for him. Is this a bug? Creature will be added anyway.";
                        logger.Error(message);
                        trayPopups.Schedule(message, "POSSIBLE PROBLEM", 5000);
                    }
                }
                //[20:23:18] She is very strong and has a good reserve of fat.
                if (line.StartsWith("He", StringComparison.Ordinal) && !verifyList.Gender)
                {
                    creatureBuffer.IsMale = true;
                    verifyList.Gender = true;
                    debugLogger.Log("creature set to male");
                }
                if (line.StartsWith("She", StringComparison.Ordinal) && !verifyList.Gender)
                {
                    creatureBuffer.IsMale = false;
                    verifyList.Gender = true;
                    debugLogger.Log("creature set to female");
                }
                //[22:34:28] His mother is the old fat Painthop. His father is the venerable fat Starkclip. 
                //[22:34:28] Her mother is the old fat Painthop. Her father is the venerable fat Starkclip. 
                if (IsParentIdentifyingLine(line) && !verifyList.Parents)
                {
                    debugLogger.Log("found maybe parents line");
                    
                    Match motherMatch = ParseMother(line);
                    if (motherMatch.Success)
                    {
                        string mother = motherMatch.Groups["g"].Value;
                        mother = GrangerHelpers.ExtractCreatureName(mother);
                        creatureBuffer.MotherName = mother;
                        debugLogger.Log("mother set to: " + mother);
                    }
                    Match fatherMatch = ParseFather(line);
                    if (fatherMatch.Success)
                    {
                        string father = fatherMatch.Groups["g"].Value;
                        father = GrangerHelpers.ExtractCreatureName(father);
                        creatureBuffer.FatherName = father;
                        debugLogger.Log("father set to: " + father);
                    }
                    verifyList.Parents = true;
                    debugLogger.Log("finished parsing parents line");
                }
                //[20:23:18] It is being taken care of by Darkprincevale.
                if (line.Contains("It is being taken care") && !verifyList.CaredBy)
                {
                    debugLogger.Log("found maybe take care of line");
                    Match caredby = Regex.Match(line, @"care of by (\w+)");
                    if (caredby.Success)
                    {
                        creatureBuffer.CaredBy = caredby.Groups[1].Value;
                        debugLogger.Log("cared set to: " + creatureBuffer.CaredBy);
                    }
                    verifyList.CaredBy = true;
                    debugLogger.Log("finished parsing care line");
                }
                //[17:11:42] She will deliver in about 4 days.
                //[17:11:42] She will deliver in about 1 day.
                if (line.Contains("She will deliver in") && !verifyList.Pregnant)
                {
                    debugLogger.Log("found maybe pregnant line");
                    Match match = Regex.Match(line, @"She will deliver in about (\d+)");
                    if (match.Success)
                    {
                        double length = Double.Parse(match.Groups[1].Value) + 1D;
                        creatureBuffer.PregnantUntil = DateTime.Now + TimeSpan.FromDays(length);
                        debugLogger.Log("found creature to be pregnant, estimated delivery: " + creatureBuffer.PregnantUntil);
                    }
                    verifyList.Pregnant = true;
                    debugLogger.Log("finished parsing pregnant line");
                }
                //[20:58:26] A foal skips around here merrily
                //[01:59:09] This calf looks happy and free.
                //This fiery creature is rumoured to grow up to be a mount of the demons of Sol
                if ((line.Contains("A foal skips around here merrily") 
                    || line.Contains("This calf looks happy and free")
                    || line.Contains("A small cuddly ball of fluff")
                    || line.Contains("This fiery creature is rumoured to grow up to be a mount of the demons of Sol"))
                    && !verifyList.Foalization)
                {
                    debugLogger.Log("applying foalization to the creature");
                    try
                    {
                        creatureBuffer.Age = CreatureAge.Foalize(creatureBuffer.Age);
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
                    debugLogger.Log("found maybe branding line");
                    Match match = Regex.Match(line, @"belongs to the settlement of (.+)\.");
                    if (match.Success)
                    {
                        string settlementName = match.Groups[1].Value;
                        creatureBuffer.BrandedBy = settlementName;
                        debugLogger.Log("found creature to be branded for: " + creatureBuffer.BrandedBy);
                        verifyList.Branding = true;
                    }
                }
                //[11:43:35] Its colour is ash.
                if (line.Contains("Its colour is"))
                {
                    debugLogger.Log("found maybe color line");
                    Match match = Regex.Match(line, @"Its colour is (.+)\.");
                    if (match.Success)
                    {
                        string colorName = match.Groups[1].Value;
                        creatureBuffer.ColorWurmLogText = colorName;
                        debugLogger.Log("found creature to have color: " + creatureBuffer.ColorWurmLogText);
                        verifyList.Branding = true;
                    }
                }
            }
        }

        bool IsParentIdentifyingLine(string line)
        {
            return 
                // Proper parsing after Rift update for WO:
                line.Contains("mother is")
                || line.Contains("father is")
                // WU server was not updated together with WO Rift update, old conditions are still needed:
                || line.Contains("Mother is")
                || line.Contains("Father is");
        }

        Match ParseMother(string line)
        {
            var result = Regex.Match(line,
                @"mother is \w+ (?<g>\w+ \w+ .+?)\.|mother is \w+ (?<g>\w+ .+?)\.",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // WU server was not updated together with WO Rift update, need old check for WU:
            if (!result.Success && wurmAssistantConfig.WurmUnlimitedMode)
            {
                result = Regex.Match(line,
                    @"Mother is (?<g>\w+ \w+ .+?)\.|Mother is (?<g>\w+ .+?)\.",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            return result;
        }

        Match ParseFather(string line)
        {
            var result = Regex.Match(line,
                @"father is \w+ (?<g>\w+ \w+ .+?)\.|father is \w+ (?<g>\w+ .+?)\.",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // WU server was not updated together with WO Rift update, need old check for WU:
            if (!result.Success && wurmAssistantConfig.WurmUnlimitedMode)
            {
                result = Regex.Match(line,
                        @"Father is (?<g>\w+ \w+ .+?)\.|Father is (?<g>\w+ .+?)\.",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            return result;
        }

        void VerifyAndApplyProcessing()
        {
            if (creatureBuffer != null)
            {
                try
                {
                    telemetry.TrackEvent("Granger: Smilexamine");

                    debugLogger.Log("finishing processing creature: " + creatureBuffer.Name);

                    if (verifyList.IsValid)
                    {
                        debugLogger.Log("Creature data is valid");

                        var selectedHerds = GetSelectedHerds();

                        var herdsFinds = GetHerdsFinds(selectedHerds, creatureBuffer, checkInnerName: false);
                        if (herdsFinds.Length == 0 && !parentModule.Settings.DoNotMatchCreaturesByBrandName)
                        {
                            herdsFinds = GetHerdsFinds(selectedHerds, creatureBuffer, checkInnerName: true);
                        }
                        var selectedHerdsFinds = herdsFinds;

                        bool allHerdSearch = false;
                        if (herdsFinds.Length == 0 &&
                            parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb)
                        {
                            allHerdSearch = true;
                            string[] allHerds = GetAllHerds();

                            herdsFinds = GetHerdsFinds(allHerds, creatureBuffer, checkInnerName: false);
                            if (herdsFinds.Length == 0 && !parentModule.Settings.DoNotMatchCreaturesByBrandName)
                            {
                                herdsFinds = GetHerdsFinds(allHerds, creatureBuffer, checkInnerName: true);
                            }
                        }

                        if (!TryUpdateExistingCreature(herdsFinds))
                        {
                            if (!TryAddNewCreature(selectedHerds, selectedHerdsFinds))
                            {
                                AnalyzeWhyNothingHappened(herdsFinds, allHerdSearch, selectedHerds);
                            }
                        }
                    }
                    else
                    {
                        debugLogger.Log("creature data was invalid, data: " + GetVerifyListData(verifyList));
                    }
                }
                finally
                {
                    //clear the buffer
                    creatureBuffer = null;
                    debugLogger.Log("processor buffer cleared");
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

                string sanityFailReason = null;

                // Verifying if creature parents match.

                // Wurm trivia:
                // If a creature has a mother name or a father name, these names cannot change.
                // However when parent dies, Wurm loses reference and the name is no longer in the log event! 

                // father checks
                if (String.IsNullOrEmpty(oldCreature.FatherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.FatherName))
                {
                    sanityFail = true;
                    if (sanityFailReason == null)
                        sanityFailReason = "Old father was blank but new data has a father name";
                }

                if (!String.IsNullOrEmpty(oldCreature.FatherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.FatherName) &&
                    oldCreature.FatherName != creatureBuffer.FatherName)
                {
                    sanityFail = true;
                    if (sanityFailReason == null)
                        sanityFailReason = "Old data father name was different than new father name";
                }

                // mother checks
                if (String.IsNullOrEmpty(oldCreature.MotherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.MotherName))
                {
                    sanityFail = true;
                    if (sanityFailReason == null)
                        sanityFailReason = "Old mother was blank but new data has a mother name";
                }

                if (!String.IsNullOrEmpty(oldCreature.MotherName) &&
                    !String.IsNullOrEmpty(creatureBuffer.MotherName) &&
                    oldCreature.MotherName != creatureBuffer.MotherName)
                {
                    sanityFail = true;
                    if (sanityFailReason == null)
                        sanityFailReason = "Old data mother name was different than new mother name";
                }

                // Verifying if creature traits match.

                // Have to take into account current AH level of the player,
                // as well as the level this creature has been previously inspected at.

                if (oldCreature.TraitsInspectedAtSkill.HasValue)
                {
                    // Skip this check if creature had genesis cast within last 1 hour.
                    // Genesis clears some negative traits.
                    debugLogger.Log($"Checking creature for Genesis cast (creature name: {creatureBuffer.Name}");
                    if (!parentModule.Settings.HasGenesisCast(creatureBuffer.Name))
                    {
                        debugLogger.Log("No genesis cast found");
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
                                    sanityFailReason =
                                        $"Trait mismatch below inspect skill treshhold ({lowskill}): {trait.ToCompactString()}";
                                break;
                            }
                        }
                    }
                    else
                    {
                        debugLogger.Log("Genesis cast found, skipping trait sanity check");
                        parentModule.Settings.RemoveGenesisCast(creatureBuffer.Name);
                        debugLogger.Log($"Removed cached genesis cast data for {creatureBuffer.Name}");
                    }
                }

                #endregion

                if (sanityFail)
                {
                    debugLogger.Log("sanity check failed for creature update: " + oldCreature + ". Reason: " +
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
                    if (creatureBuffer.HasFatherName)
                    {
                        oldCreature.FatherName = creatureBuffer.FatherName;
                    }
                    if (creatureBuffer.HasMotherName)
                    {
                        oldCreature.MotherName = creatureBuffer.MotherName;
                    }
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
                        debugLogger.Log("old creature data had more accurate trait info, skipping");
                    }
                    oldCreature.SetTag("dead", false);
                    oldCreature.SetSecondaryInfoTag(creatureBuffer.SecondaryInfo);
                    oldCreature.IsMale = creatureBuffer.IsMale;
                    oldCreature.PregnantUntil = creatureBuffer.PregnantUntil;
                    if (oldCreature.Name != creatureBuffer.Name)
                    {
                        if (NameIsUniqueInHerd(creatureBuffer.Name, oldCreature.Herd))
                        {
                            trayPopups.Schedule(
                                $"Updating name of creature {oldCreature.Name} to {creatureBuffer.Name}", "CREATURE NAME UPDATE");
                            oldCreature.Name = creatureBuffer.Name;
                        }
                        else
                        {
                            trayPopups.Schedule(
                                $"Could not update name of creature {oldCreature.Name} to {creatureBuffer.Name}, because herd already has a creature with such name.", "WARNING");
                        }
                    }
                    oldCreature.SmilexamineLastDate = DateTime.Now;
                    if (creatureBuffer.HasColorWurmLogText && grangerSettings.UpdateCreatureColorOnSmilexamines)
                    {
                        oldCreature.CreatureColorId =
                            creatureColorDefinitions.GetColorIdByWurmLogText(creatureBuffer.ColorWurmLogText);
                    }
                    context.SubmitChanges();
                    debugLogger.Log("successfully updated creature in db");
                    trayPopups.Schedule($"Updated creature: {oldCreature}", "CREATURE UPDATED");
                }

                debugLogger.Log("processor buffer cleared");
                return true;
            }
            return false;
        }

        bool NameIsUniqueInHerd(string creatureName, string herdName)
        {
            return !context.Creatures.Any(x => x.Name == creatureName && x.Herd == herdName);
        }

        bool TryAddNewCreature(string[] selectedHerds, CreatureEntity[] selectedHerdsFinds)
        {
            if (selectedHerds.Length == 1 ||
                (parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb && selectedHerds.Length > 0))
            {
                // Creature can be added only if it's identity does not collide with any other creature within a pool.
                // Pool can be either creatures from selected herds, or everything from the database, depending on user settings.
                if (selectedHerdsFinds.Length == 0)
                {
                    string herd = selectedHerds[0];
                    var existing =
                        context.Creatures.Where(x => creatureBuffer.Name == x.Name && x.Herd == herd).ToArray();

                    if (!existing.Any())
                    {
                        AddNewCreature(herd, creatureBuffer);
                    }
                    else
                    {
                        var message = $"Creature with name: {creatureBuffer.Name} already exists in herd: {herd}";
                        if (existing.Any(entity =>
                                entity.Name == creatureBuffer.Name
                                && !string.IsNullOrEmpty(entity.ServerName) 
                                && !creatureBuffer.Server.ServerName.Matches(entity.ServerName)))
                        {
                            message += " (creature server name mismatch)";
                        }
                        trayPopups.Schedule(message, "CAN'T ADD CREATURE", 4000);
                        debugLogger.Log(message);
                    }

                    debugLogger.Log("processor buffer cleared");
                    return true;
                }
                else if (!parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb)
                {
                    string message = $"Creature with name: {creatureBuffer.Name} already exists in active herd";

                    trayPopups.Schedule(message, "CAN'T ADD CREATURE", 4000);
                    debugLogger.Log(message);
                }
            }
            return false;
        }

        void AnalyzeWhyNothingHappened(CreatureEntity[] herdsFinds, bool allHerdSearch, string[] selectedHerds)
        {
            if (herdsFinds.Length > 1)
            {
                var partialMessage = allHerdSearch ? "database" : "selected herds";
                debugLogger.Log(
                    $"many creatures named {creatureBuffer.Name} found in {partialMessage}, add/update aborted");
                trayPopups.Schedule(
                    $"{partialMessage} contain many creatures named {creatureBuffer.Name}, narrow herd selection",
                    "CAN'T ADD OR UPDATE CREATURE",
                    6000);
            }
            else if (!parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb
                     && (selectedHerds.Length == 0 || selectedHerds.Length > 1))
            {
                const string message = "exactly one herd has to be active to add new creature";
                debugLogger.Log(message);
                trayPopups.Schedule(message, "CAN'T ADD OR UPDATE CREATURE", 4000);
            }
            else if (parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb
                     && selectedHerds.Length == 0)
            {
                const string message = "at least one herd must be select to add new creature";
                debugLogger.Log(message);
                trayPopups.Schedule(message, "CAN'T ADD OR UPDATE CREATURE", 4000);
            }
            else
            {
                const string message = "add/update creature failed for unknown reasons";
                debugLogger.Log(message);
                logger.Error(message);
                trayPopups.Schedule(message, "CAN'T ADD OR UPDATE CREATURE", 4000);
            }
        }

        private string GetVerifyListData(ValidationList validationList)
        {
            return
                $"(name: {validationList.Name}, Gender: {validationList.Gender}, Parents: {validationList.Parents}, Traits: {validationList.Traits}, CaredBy: {validationList.CaredBy})";
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
                FatherName = newCreature.FatherName,
                MotherName = newCreature.MotherName,
                Traits = newCreature.Traits,
                TraitsInspectedAtSkill = newCreature.InspectSkill,
                IsMale = newCreature.IsMale,
                PregnantUntil = newCreature.PregnantUntil,
                SecondaryInfoTagSetter = newCreature.SecondaryInfo,
                ServerName = newCreature.Server != null ? newCreature.Server.ServerName.Original : string.Empty,
                SmilexamineLastDate = DateTime.Now,
                CreatureColorId = newCreature.HasColorWurmLogText
                    ? creatureColorDefinitions.GetColorIdByWurmLogText(newCreature.ColorWurmLogText)
                    : CreatureColor.GetDefaultColor().CreatureColorId,
            };

            if (newCreature.newBorn)
            {
                newEntity.BirthDate = DateTime.Now;
            }

            newEntity.EpicCurve = newCreature.Server != null
                                  && newCreature.Server.ServerGroup.ServerGroupId == ServerGroup.EpicId;

            context.InsertCreature(newEntity);
            debugLogger.Log(
                $"successfully inserted {(newCreature.newBorn ? "newborn " : string.Empty)}creature to db");
            trayPopups.Schedule($"Added new creature to herd {selectedHerd}: {newEntity}{(newCreature.newBorn ? " (with Now as birth date)" : string.Empty)}", "CREATURE ADDED");
        }

        private string[] GetAllHerds()
        {
            return context.Herds.ToArray()
                .Select(x => x.HerdId.ToString()).ToArray();
        }

        private CreatureEntity[] GetHerdsFinds(IEnumerable<string> viableHerds, CreatureBuffer creatureBuffer, bool checkInnerName)
        {
            IEnumerable<CreatureEntity> query;
            if (checkInnerName)
            {
                var bufferInnerNameInfo = Creature.GetInnerNameInfo(creatureBuffer.Name);
                if (!bufferInnerNameInfo.HasInnerName)
                {
                    return new CreatureEntity[0];
                }
                query = context.Creatures
                               .Where(x =>
                               {
                                   var iteratedInnerNameInfo = Creature.GetInnerNameInfo(x.Name);
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
                .Select(x => x.HerdId.ToString()).ToArray();
        }

        void AttemptToStartProcessing(string line, bool isNewborn = false)
        {
            debugLogger.Log("attempting to start processing creature due to line: " + line);
            // Apply previous processing, if still active.
            VerifyAndApplyProcessing();

            try
            {
                debugLogger.Log("extracting object name");
                string objectNameWithPrefixes = string.Empty;
                string objectNameMatchingPattern;

                if (isNewborn)
                {
                    // [16:48:25] You hug the aged fat Pinkieflea.
                    objectNameMatchingPattern = @"You hug (a|an|the) (?<g>.+)\.|You hug (?<g>.+)\.";
                }
                else
                {
                    // [20:48:42] You smile at the Adolescent diseased Mountainheart.
                    // This regex preserves condition from before WO Rift update, where determiner was not present.
                    // This is kept, because WU servers cannot be guaranteed to have been updated by their administrators.
                    objectNameMatchingPattern = @"You smile at (a|an|the) (?<g>.+)\.|You smile at (?<g>.+)\.";
                }

                Match match = Regex.Match(line,
                    objectNameMatchingPattern,
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
                if (match.Success)
                {
                    objectNameWithPrefixes = match.Groups["g"].Value;
                }

                if (GrangerHelpers.HasAgeInName(objectNameWithPrefixes, ignoreCase:true))
                {
                    debugLogger.Log("object assumed to be a creature");
                    var server = playerMan.CurrentServer;
                    var skill = playerMan.CurrentServerAhSkill;

                    if (grangerSettings.RequireServerAndSkillToBeKnownForSmilexamine 
                        && (server == null || skill == null))
                    {
                        trayPopups.Schedule(
                            $"Server or AH skill level unknown for {playerMan.PlayerName}. If WA was just started, give it a few seconds. (This check can be disabled in Granger options)", "CAN'T PROCESS CREATURE", 5000);
                        debugLogger.Log(
                            $"processing creature cancelled, AH skill or server group unknown for player {playerMan.PlayerName} (skill: {skill} ; server: {server}");
                    }
                    else
                    { 
                        debugLogger.Log("building new creature object and moving to processor");

                        isProcessing = true;
                        startedProcessingOn = DateTime.Now;
                        verifyList = new ValidationList();
                        creatureBuffer = new CreatureBuffer
                        {
                            Name = GrangerHelpers.ExtractCreatureName(objectNameWithPrefixes),
                            Age = GrangerHelpers.ExtractCreatureAge(objectNameWithPrefixes),
                            Server = server,
                            InspectSkill = skill ?? 0,
                            newBorn = isNewborn
                        };

                        var fat = GrangerHelpers.TryParseCreatureNameIfLineContainsFat(objectNameWithPrefixes);
                        if (fat != null) creatureBuffer.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Fat;

                        var starving = GrangerHelpers.TryParseCreatureNameIfLineContainsStarving(objectNameWithPrefixes);
                        if (starving != null) creatureBuffer.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Starving;

                        var diseased = GrangerHelpers.TryParseCreatureNameIfLineContainsDiseased(objectNameWithPrefixes);
                        if (diseased != null) creatureBuffer.SecondaryInfo = CreatureEntity.SecondaryInfoTag.Diseased;

                        verifyList.Name = true;
                        debugLogger.Log("finished building");
                    }
                }
                else debugLogger.Log(objectNameWithPrefixes + " was not recognized as a named creature.");
            }
            catch (Exception exception)
            {
                debugLogger.Log("! Granger: error while BeginProcessing, event: " + line, true, exception);
            }
        }

        public void Update()
        {
            if (isProcessing)
            {
                if (DateTime.Now > startedProcessingOn + ProcessorTimeout)
                {
                    debugLogger.Log("processing timed out, attempting to verify and apply last inspected creature");
                    isProcessing = false;
                    VerifyAndApplyProcessing();
                }
            }
        }
    }
}
