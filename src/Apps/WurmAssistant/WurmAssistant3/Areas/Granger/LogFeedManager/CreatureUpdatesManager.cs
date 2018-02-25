using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager
{
    class CreatureUpdatesManager
    {
        class CreatureData
        {
            public CreatureEntity Creature = null;
            public CreatureAge Age = new CreatureAge(CreatureAgeId.Unknown);
            public string SecondaryInfo = null;
            public bool TooManyCreaturesFound;
        }

        readonly GrangerFeature parentModule;
        readonly GrangerContext context;
        readonly PlayerManager playerManager;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;

        readonly SmileXamineProcessor smileXamineProcessor;

        DateTime lastBreedingOn;
        CreatureEntity lastBreedingFemale;
        CreatureEntity lastBreedingMale;

        readonly GrangerDebugLogger grangerDebug;

        public CreatureUpdatesManager(
            [NotNull] GrangerFeature parentModule,
            [NotNull] GrangerContext context,
            [NotNull] PlayerManager playerManager,
            [NotNull] ITrayPopups trayPopups, 
            [NotNull] ILogger logger,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig,
            [NotNull] CreatureColorDefinitions creatureColorDefinitions)
        {
            if (parentModule == null) throw new ArgumentNullException(nameof(parentModule));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (playerManager == null) throw new ArgumentNullException(nameof(playerManager));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            if (creatureColorDefinitions == null) throw new ArgumentNullException(nameof(creatureColorDefinitions));

            this.parentModule = parentModule;
            this.context = context;
            this.playerManager = playerManager;
            this.trayPopups = trayPopups;
            this.logger = logger;

            grangerDebug = new GrangerDebugLogger(logger);

            smileXamineProcessor = new SmileXamineProcessor(this.parentModule,
                this.context,
                this.playerManager,
                grangerDebug,
                trayPopups,
                logger,
                wurmAssistantConfig,
                creatureColorDefinitions);
        }

        internal void ProcessEventForCreatureUpdates(LogEntry line)
        {
            HandleAgeAndTagsUpdates(line);

            smileXamineProcessor.HandleLogEvent(line.Content);

            HandleDead(line);
            HandleGrooming(line);
            HandleBreeding(line);
            HandleDiseased(line);
            HandleGenesis(line);
        }

        void HandleAgeAndTagsUpdates(LogEntry line)
        {
            if (CreatureUpdateAnyEvent && MaybeCreatureAgeLine(line.Content))
            {
                CreatureData data = GetCreatureDataFromAnyLogEvent(line.Content, searchEntireDb:false);

                bool cont = true;

                if (data != null && data.TooManyCreaturesFound)
                {
                    var partialMessage = "selected herds";
                    ScheduleTrayPopup(
                        String.Format(
                            "There are multiple creatures named {0} in {1}, can't update health/age!",
                            data.Creature.Name,
                            partialMessage),
                        "CREATURE UPDATE PROBLEM",
                        5000);
                    cont = false;
                }
                else if (data == null && SearchEntireDb)
                {
                    data = GetCreatureDataFromAnyLogEvent(line.Content, searchEntireDb:true);
                    if (data != null && data.TooManyCreaturesFound)
                    {
                        var partialMessage = "database";
                        ScheduleTrayPopup(
                            String.Format(
                                "There are multiple creatures named {0} in {1}, can't update health/age!",
                                data.Creature.Name,
                                partialMessage),
                            "CREATURE UPDATE PROBLEM",
                            5000);
                        cont = false;
                    }
                }

                if (data != null && cont)
                {
                    if (data.SecondaryInfo != null)
                    {
                        string notify = null;
                        if (data.SecondaryInfo == String.Empty)
                        {
                            if (data.Creature.CheckTag("diseased")
                                || data.Creature.CheckTag("starving")
                                || data.Creature.CheckTag("fat"))
                            {
                                notify = "cleared";
                            }
                            data.Creature.SetSecondaryInfoTag(CreatureEntity.SecondaryInfoTag.None);
                        }
                        else if (data.SecondaryInfo == "diseased")
                        {
                            if (!data.Creature.CheckTag("diseased"))
                            {
                                notify = "diseased";
                            }
                            data.Creature.SetSecondaryInfoTag(CreatureEntity.SecondaryInfoTag.Diseased);
                        }
                        else if (data.SecondaryInfo == "starving")
                        {
                            if (!data.Creature.CheckTag("starving"))
                            {
                                notify = "starving";
                            }
                            data.Creature.SetSecondaryInfoTag(CreatureEntity.SecondaryInfoTag.Starving);
                        }
                        else if (data.SecondaryInfo == "fat")
                        {
                            if (!data.Creature.CheckTag("fat"))
                            {
                                notify = "fat";
                            }
                            data.Creature.SetSecondaryInfoTag(CreatureEntity.SecondaryInfoTag.Fat);
                        }


                        bool ageChanged = data.Age != data.Creature.Age;

                        if (notify != null || ageChanged)
                        {
                            string health = null;
                            if (notify != null)
                            {
                                if (notify == "cleared")
                                {
                                    health = "Cleared health tags.";
                                }
                                else
                                {
                                    health = "Set health tag to " + notify + ".";
                                }
                            }

                            string age = null;
                            if (ageChanged) age = "updated age to " + data.Age;

                            ScheduleTrayPopup(data.Creature + ": " + health + " " + age, "HEALTH/AGE UPDATE");
                        }

                        data.Creature.Age = data.Age;
                    }
                    context.SubmitChanges();
                }
            }
        }

        void HandleDead(LogEntry line)
        {
            //[13:47:19] Venerable fat Kisspick is dead. R.I.P.
            if (line.Content.Contains("is dead."))
            {
                Match match = Regex.Match(line.Content, @".+ (\w+) is dead\. R\.I\.P\.", RegexOptions.Compiled);
                if (!match.Success)
                {
                    // maybe branded creature:
                    match = Regex.Match(line.Content, @".+ (\w+ '\w+') is dead\. R\.I\.P\.", RegexOptions.Compiled);
                }
                TryApplyDeadFlag(line.Content, match);
            }
            //[03:10:29] You bury the corpse of venerable tammyrain.
            if (line.Content.StartsWith("You bury the corpse", StringComparison.Ordinal))
            {
                Match match = Regex.Match(line.Content, @"You bury the corpse of .+ (\w+)", RegexOptions.Compiled);
                // can't try branded creature name pattern - dead creatures don't show a branding name any more
                TryApplyDeadFlag(line.Content, match);
            }
        }

        void HandleGrooming(LogEntry line)
        {
            //[11:34:44] You have now tended to Aged fat Lightningzoe and she seems pleased.
            if (line.Content.StartsWith("You have now tended", StringComparison.Ordinal))
            {
                Match match = Regex.Match(line.Content, @"You have now tended to (.+) and \w+ seems pleased");
                if (match.Success)
                {
                    grangerDebug.Log("LIVETRACKER: applying groomed flag due to: " + line);
                    string prefixedName = match.Groups[1].Value;
                    string fixedName = GrangerHelpers.RemoveAllPrefixes(prefixedName);
                    CreatureEntity[] creatureEntities = GetCreatureToUpdate(fixedName, playerManager.CurrentServer);
                    if (SearchEntireDb && creatureEntities.Length > 1)
                    {
                        ScheduleTrayPopup(
                            String.Format(
                                "There are multiple creatures named {0} in database, marking them all as groomed!",
                                fixedName),
                            "GROOMING ISSUE DETECTED",
                            6000);
                    }
                    foreach (var creature in creatureEntities)
                    {
                        creature.GroomedOn = DateTime.Now;
                        context.SubmitChanges();
                    }
                }
            }
        }

        void HandleBreeding(LogEntry line)
        {
            //[04:23:27] The Aged fat Dancedog and the Aged fat Cliffdog get intimate.
            if (line.Content.Contains("get intimate"))
            {
                Match match = Regex.Match(line.Content, @"The (.+) and the (.+) get intimate.");
                if (match.Success)
                {
                    grangerDebug.Log("LIVETRACKER: attempting to cache last bred pair data due to: " + line);
                    lastBreedingFemale = null;
                    lastBreedingMale = null;
                    lastBreedingOn = DateTime.Now;

                    string name1 = match.Groups[1].Value;
                    string name2 = match.Groups[2].Value;
                    string fixedName1 = GrangerHelpers.RemoveAllPrefixes(name1);
                    string fixedName2 = GrangerHelpers.RemoveAllPrefixes(name2);
                    CreatureEntity[] creatures1 = GetCreatureToUpdate(fixedName1, playerManager.CurrentServer);
                    CreatureEntity[] creatures2 = GetCreatureToUpdate(fixedName2, playerManager.CurrentServer);

                    ExtractBreedingPairCreature(fixedName1, creatures1);
                    ExtractBreedingPairCreature(fixedName2, creatures2);
                }
            }

            //The Old fat Ebonycloud will probably give birth in a while!
            //[04:23:47] The Aged fat Dancedog will probably give birth in a while!
            if (line.Content.Contains("will probably give birth"))
            {
                if (lastBreedingOn > DateTime.Now - TimeSpan.FromMinutes(3))
                {
                    grangerDebug.Log("LIVETRACKER: applying breeding update due to: " + line);

                    Match match = Regex.Match(line.Content, @"The (.+) will probably give birth in a while");
                    if (match.Success)
                    {
                        string prefixedName = match.Groups[1].Value;
                        string fixedName = GrangerHelpers.RemoveAllPrefixes(prefixedName);

                        if (lastBreedingFemale != null)
                        {
                            if (lastBreedingFemale.Name == fixedName) //sanity check? maybe pointless
                            {
                                lastBreedingFemale.PregnantUntil = DateTime.Now + GrangerHelpers.LongestPregnancyPossible;
                                ScheduleTrayPopup(
                                    String.Format(
                                        "({0}) is now marked as pregnant. Be sure to smilexamine to get more accurate pregnancy duration!",
                                        lastBreedingFemale.Name),
                                    "BREED UPDATE",
                                    6000,
                                    forceShow:true);
                                context.SubmitChanges();
                            }
                            else
                            {
                                ScheduleTrayPopup(
                                    String.Format("Female name ({0}) does not match the cached name ({1})!",
                                        lastBreedingFemale.Name,
                                        fixedName),
                                    "BREED UPDATE PROBLEM",
                                    6000,
                                    forceShow: true);
                            }
                        }
                        if (lastBreedingMale != null)
                        {
                            lastBreedingMale.NotInMood = DateTime.Now + GrangerHelpers.BreedingNotInMoodDuration;
                            ScheduleTrayPopup(
                                String.Format(
                                    "({0}) is now marked as Not In Mood. You can't breed this creature for next 45 minutes.",
                                    lastBreedingMale.Name),
                                "BREED UPDATE",
                                6000,
                                forceShow: true);
                            context.SubmitChanges();
                        }
                    }
                }
            }
            //[06:18:19] The Aged fat Umasad shys away and interrupts the action.
            if (line.Content.Contains("shys away and interrupts"))
            {
                if (lastBreedingOn > DateTime.Now - TimeSpan.FromMinutes(1))
                {
                    grangerDebug.Log("LIVETRACKER: processing failed breeding update due to: " + line);
                    Match match = Regex.Match(line.Content, @"The (.+) shys away and interrupts the action");
                    if (match.Success)
                    {
                        string prefixedName = match.Groups[1].Value;
                        string fixedName = GrangerHelpers.RemoveAllPrefixes(prefixedName);
                        if (lastBreedingMale != null)
                            lastBreedingMale.NotInMood = DateTime.Now + GrangerHelpers.BreedingNotInMoodDuration;
                        if (lastBreedingFemale != null)
                            lastBreedingFemale.NotInMood = DateTime.Now + GrangerHelpers.BreedingNotInMoodDuration;
                        ScheduleTrayPopup(
                            String.Format(
                                "Breeding appears to have failed, {0} and {1} will be Not In Mood for next 45 minutes.",
                                lastBreedingMale == null ? "Some creature" : lastBreedingMale.Name,
                                lastBreedingFemale == null ? "Some creature" : lastBreedingFemale.Name),
                            "BREED UPDATE",
                            6000,
                            forceShow: true);
                    }
                }
            }
        }

        void HandleDiseased(LogEntry line)
        {
            //[20:48:42] You smile at Adolescent diseased Mountainheart.
            string diseaseCheck = GrangerHelpers.TryParseCreatureNameIfLineContainsDiseased(line.Content);
            if ((diseaseCheck) != null)
            {
                string possibleCreatureName = diseaseCheck;
                var creaturesToUpdate = context.Creatures.Where(x => x.Name == possibleCreatureName).ToArray();
                if (creaturesToUpdate.Length > 0)
                {
                    foreach (var creature in creaturesToUpdate)
                    {
                        grangerDebug.Log("Marking creature diseased: " + creature);
                        creature.SetTag("diseased", true);
                    }
                    context.SubmitChanges();
                }
            }
        }

        void HandleGenesis(LogEntry line)
        {
            // genesis handling, purpose:
            // remembering genesis casts so smilexamine can be allowed to update traits, 
            // even though it would normally be blocked by sanity check

            //[2013-08-02] [23:08:54] You cast 'Genesis' on Old fat Jollyhalim.
            if (Regex.IsMatch(line.Content, @".+ cast", RegexOptions.Compiled))// optimization
            {
                grangerDebug.Log("Found maybe genesis log event: " + line);
                // New matcher after changes in wurm caves update.
                Match match = Regex.Match(line.Content,
                    @"(?:You cast|.+ casts) 'Genesis' on(?: a| an| the) (.+)\.",
                    RegexOptions.Compiled);
                if (!match.Success)
                {
                    // Old matcher for older WU servers.
                    match = Regex.Match(line.Content, @"(?:You cast|.+ casts) 'Genesis' on (.+)\.", RegexOptions.Compiled);
                }
                if (match.Success)
                {
                    string prefixedCreatureName = match.Groups[1].Value;
                    string creatureName = GrangerHelpers.ExtractCreatureName(prefixedCreatureName);
                    grangerDebug.Log(string.Format("Recognized Genesis cast on: {0} (raw name: {1})",
                        creatureName,
                        prefixedCreatureName));
                    parentModule.Settings.AddGenesisCast(DateTime.Now, creatureName);
                }
            }
        }

        void TryApplyDeadFlag(string line, Match match)
        {
            if (match.Success)
            {
                grangerDebug.Log("LIVETRACKER: R.I.P. log line detected, checking if it's a creature from herds, line: " + line);
                string lowercasename = match.Groups[1].Value;
                string fixedName = GrangerHelpers.CapitalizeCreatureName(lowercasename);
                CreatureEntity[] creatures = GetCreatureToUpdate(fixedName, playerManager.CurrentServer);
                foreach (var creature in creatures)
                {
                    grangerDebug.Log("LIVETRACKER: applying maybedead flag to "+creature+" due to: " + line);
                    creature.SetTagDead();
                    context.SubmitChanges();
                }
            }
        }

        bool MaybeCreatureAgeLine(string line)
        {
            if (GrangerHelpers.HasAgeInName(line, true)) return true;
            else return false;
        }

        /// <returns></returns>
        CreatureData GetCreatureDataFromAnyLogEvent(string line, bool searchEntireDb)
        {
            var creaturesQuery = context.Creatures;

            if (!searchEntireDb)
            {
                var selectedHerds = context.Herds.Where(x => x.Selected).Select(x => x.HerdId).ToArray();
                creaturesQuery = creaturesQuery.Where(x => selectedHerds.Contains(x.Herd)).ToArray();
            }

            if (UseServerAsCreatureId)
            {
                var server = playerManager.CurrentServer;
                if (server == null)
                {
                    logger.Info(
                        "GetCreatureDataFromAnyLogEvent skipped processing log line, " +
                        "due to unknown current server and UseServerAsCreatureId is enabled.");
                    creaturesQuery = new CreatureEntity[0];
                }
                else
                {
                    creaturesQuery =
                        creaturesQuery.Where(
                            entity =>
                                string.IsNullOrEmpty(entity.ServerName) || server.ServerName.Matches(entity.ServerName));
                }
            }

            var filteredCreatures = creaturesQuery.Where(x => line.Contains(x.Name, StringComparison.OrdinalIgnoreCase)).ToArray();

            var result = new CreatureData();
            foreach (CreatureEntity creatureEntity in filteredCreatures)
            {
                result.SecondaryInfo = null;
                Match match = Regex.Match(line, @"(\w+) (\w+) " + creatureEntity.Name, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    result.Age = CreatureAge.CreateAgeFromRawCreatureName(match.Groups[1].Value);
                    if (result.Age.CreatureAgeId != CreatureAgeId.Unknown)
                    {
                        result.SecondaryInfo = match.Groups[2].Value;
                    }
                    else
                    {
                        match = Regex.Match(line, @"(\w+) " + creatureEntity.Name, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            result.Age = CreatureAge.CreateAgeFromRawCreatureName(match.Groups[1].Value);
                            if (result.Age.CreatureAgeId != CreatureAgeId.Unknown)
                            {
                                result.SecondaryInfo = String.Empty;
                            }
                        }
                    }
                    if (result.Age.CreatureAgeId != CreatureAgeId.Unknown)
                    {
                        result.Creature = creatureEntity;

                        // Attempting to advance age, unless it's ambiguous.
                        var prevAge = result.Creature.Age.CreatureAgeId;
                        var newAge = result.Age.CreatureAgeId;

                        if (prevAge == CreatureAgeId.YoungFoal)
                        {
                            // Ambiguous: young foal or young mature, keep old value
                            if (newAge == CreatureAgeId.Young) 
                                result.Age = new CreatureAge(prevAge);
                            // Ambiguous: adolescent foal or adolescent mature, keep old value
                            if (newAge == CreatureAgeId.Adolescent)
                                result.Age = new CreatureAge(prevAge);
                        }
                        if (prevAge == CreatureAgeId.AdolescentFoal)
                        {
                            // Acceptable: young in this context can only mean young mature
                            if (newAge == CreatureAgeId.Young)
                                result.Age = new CreatureAge(CreatureAgeId.Young);
                            // Ambiguous: adolescent foal or adolescent mature, keep old value
                            if (newAge == CreatureAgeId.Adolescent)
                                result.Age = new CreatureAge(prevAge);
                        }

                        break;
                    }
                }
            }

            if (result.Creature == null)
            {
                return null;
            }
            var existingCreaturesInQuery = filteredCreatures.Where(x => x.Name == result.Creature.Name).ToArray();
            if (existingCreaturesInQuery.Length == 1)
            {
                return result;
            }
            else if (existingCreaturesInQuery.Length > 1)
            {
                result.TooManyCreaturesFound = true;
                return result;
            }
            return null;
        }

        bool SearchEntireDb => parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb;

        bool CreatureUpdateAnyEvent => parentModule.Settings.UpdateCreatureDataFromAnyEventLine;

        bool UseServerAsCreatureId => parentModule.Settings.UseServerNameAsCreatureIdComponent;

        bool HideLiveTrackerPopups => parentModule.Settings.HideLiveTrackerPopups;

        private void ExtractBreedingPairCreature(string fixedName, CreatureEntity[] creatures)
        {
            if (creatures.Length == 1)
            {
                CreatureEntity creature = creatures[0];
                if (creature.IsMale ?? false)
                {
                    lastBreedingMale = creature;
                }
                else
                {
                    lastBreedingFemale = creature;
                }
            }
            else
            {
                string title = null;
                string message = null;
                if (creatures.Length > 1)
                {
                    message = (SearchEntireDb ? "Database has" : "Selected herds have")
                              + " more than one creature named: " + fixedName;
                }
                else if (creatures.Length == 0)
                {
                    message = "Not a creature or " + (SearchEntireDb ? "" : "selected")
                                + " herds don't have a creature named: " + fixedName;
                }
                if (message != null)
                {
                    ScheduleTrayPopup(message,
                        title ?? "BREED UPDATE PROBLEM",
                        6000,
                        forceShow: true);
                }
            }
        }

        private CreatureEntity[] GetCreatureToUpdate(string creatureName, IWurmServer server)
        {
            if (server == null)
            {
                logger.Info("Skipped creature lookup at GetCreatureToUpdate due to unknown current server");
                return new CreatureEntity[0];
            }

            IEnumerable<HerdEntity> herdsQuery = context.Herds;
            if (!parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb)
            {
                herdsQuery = herdsQuery.Where(x => x.Selected);
            }
            var herdsToCheck = herdsQuery.Select(x => x.HerdId).ToArray();

            var creaturesQuery =
                context.Creatures.Where(
                    x => x.Name.Equals(creatureName, StringComparison.CurrentCultureIgnoreCase)
                         && herdsToCheck.Contains(x.Herd));

            if (UseServerAsCreatureId)
            {
                creaturesQuery =
                    creaturesQuery.Where(
                        entity =>
                            string.IsNullOrEmpty(entity.ServerName) || server.ServerName.Matches(entity.ServerName));
            }
            return creaturesQuery.ToArray();
        }

        internal void Update()
        {
            smileXamineProcessor.Update();
        }

        void ScheduleTrayPopup(string message, string title, int durationMillis = 3000, bool forceShow = false)
        {
            if (forceShow || !HideLiveTrackerPopups)
            {
                trayPopups.Schedule(message ?? string.Empty, title ?? string.Empty, durationMillis);
            }
        }
    }
}
