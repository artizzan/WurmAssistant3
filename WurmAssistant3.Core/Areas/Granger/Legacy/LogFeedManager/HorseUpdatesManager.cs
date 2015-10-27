using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.LogFeedManager
{

    class HorseUpdatesManager
    {
        private readonly GrangerFeature _parentModule;
        private readonly GrangerContext _context;
        private readonly PlayerManager _playerMan;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;

        readonly HorseProcessor _processor;

        DateTime _lastBreedingOn;
        HorseEntity _lastBreedingFemale;
        HorseEntity _lastBreedingMale;

        readonly GrangerDebugLogger _grangerDebug;

        public HorseUpdatesManager(GrangerFeature parentModule, GrangerContext context, PlayerManager playerManager,
            [NotNull] ITrayPopups trayPopups, [NotNull] ILogger logger)
        {
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (logger == null) throw new ArgumentNullException("logger");
            _parentModule = parentModule;
            _context = context;
            _playerMan = playerManager;
            this.trayPopups = trayPopups;
            this.logger = logger;

            _grangerDebug = new GrangerDebugLogger(logger);

            _processor = new HorseProcessor(_parentModule, _context, _playerMan, _grangerDebug, trayPopups, logger);
        }

        //this should not be called if user disabled capturing data from logs
        //this should not contain any update logic that has tu run regardless of above setting
        internal void ProcessEventForHorseUpdates(LogEntry line)
        {
            //this runs if updating on any event is enabled
            if (HorseUpdateAnyEvent && MaybeHorseAgeLine(line.Content))
            {
                // first try selected herds
                HorseData data = GetHorseDataFromAnyLogEvent(line.Content, false);

                bool cont = true;

                if (data != null && data.TooManyHorsesFound)
                {
                    // if we found too many same named in selected, can't continue
                    var partialMessage = "selected herds";
                    trayPopups.Schedule(
                        "HORSE UPDATE PROBLEM",
                        String.Format(
                            "There are multiple horses named {0} in {1}, can't update health/age!",
                            data.Horse.Name, partialMessage),
                        5000);
                    cont = false;
                }
                else if (data == null && EntireDbSetting)
                {
                    // if we found nothing but EDS is set, try searching entire db
                    data = GetHorseDataFromAnyLogEvent(line.Content, true);
                    if (data != null && data.TooManyHorsesFound)
                    {
                        // again if multiple found in db, we can't continue
                        var partialMessage = "database";
                        trayPopups.Schedule(
                            "HORSE UPDATE PROBLEM",
                            String.Format(
                                "There are multiple horses named {0} in {1}, can't update health/age!",
                                data.Horse.Name, partialMessage),
                            5000);
                        cont = false;
                    }
                }

                // at this point, either we got single, nothing or many
                // but cont == true means single or nothing

                if (data != null && cont)
                {
                    // if we got single and something, update
                    if (data.SecondaryInfo != null)
                    {
                        string notify = null;
                        if (data.SecondaryInfo == String.Empty)
                        {
                            if (data.Horse.CheckTag("diseased")
                                || data.Horse.CheckTag("starving")
                                || data.Horse.CheckTag("fat"))
                            {
                                notify = "cleared";
                            }
                            data.Horse.SetSecondaryInfoTag(HorseEntity.SecondaryInfoTag.None);
                        }
                        else if (data.SecondaryInfo == "diseased")
                        {
                            if (!data.Horse.CheckTag("diseased"))
                            {
                                notify = "diseased";
                            }
                            data.Horse.SetSecondaryInfoTag(HorseEntity.SecondaryInfoTag.Diseased);
                        }
                        else if (data.SecondaryInfo == "starving")
                        {
                            if (!data.Horse.CheckTag("starving"))
                            {
                                notify = "starving";
                            }
                            data.Horse.SetSecondaryInfoTag(HorseEntity.SecondaryInfoTag.Starving);
                        }
                        else if (data.SecondaryInfo == "fat")
                        {
                            if (!data.Horse.CheckTag("fat"))
                            {
                                notify = "fat";
                            }
                            data.Horse.SetSecondaryInfoTag(HorseEntity.SecondaryInfoTag.Fat);
                        }


                        bool ageChanged = data.Age > data.Horse.Age;

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

                            trayPopups.Schedule("HEALTH/AGE UPDATE", data.Horse + ": " + health + " " + age);
                        }
                        
                        data.Horse.Age = data.Age;
                    }
                    _context.SubmitChangesToHorses();
                }
            }

            _processor.HandleLogEvent(line.Content);

            //apply updates
            //[13:47:19] Venerable fat Kisspick is dead. R.I.P.
            if (line.Content.Contains("is dead."))
            {
                Match match = Regex.Match(line.Content, @".+ (\w+) is dead\. R\.I\.P\.");
                TryApplyDeadFlag(line.Content, match);
            }
            //[03:10:29] You bury the corpse of venerable tammyrain.
            if (line.Content.StartsWith("You bury the corpse", StringComparison.Ordinal))
            {
                Match match = Regex.Match(line.Content, @"You bury the corpse of .+ (\w+)");
                TryApplyDeadFlag(line.Content, match);
            }

            //[11:34:44] You have now tended to Aged fat Lightningzoe and she seems pleased.
            if (line.Content.StartsWith("You have now tended", StringComparison.Ordinal))
            {
                Match match = Regex.Match(line.Content, @"You have now tended to (.+) and \w+ seems pleased");
                if (match.Success)
                {
                    _grangerDebug.Log("LIVETRACKER: applying groomed flag due to: " + line);
                    string prefixedName = match.Groups[1].Value;
                    HorseAge newAge = GrangerHelpers.ExtractHorseAge(prefixedName);
                    string fixedName = GrangerHelpers.RemoveAllPrefixes(prefixedName);
                    HorseEntity[] horses = GetHorseToUpdate(fixedName, _playerMan.GetCurrentServerGroup());
                    if (EntireDbSetting && horses.Length > 1)
                    {
                        trayPopups.Schedule(
                            "GROOMING ISSUE DETECTED",
                            String.Format(
                                "There are multiple horses named {0} in database, going to mark them all!",
                                fixedName),
                            6000);
                    }
                    foreach (var horse in horses)
                    {
                        horse.GroomedOn = DateTime.Now;
                        _context.SubmitChangesToHorses();
                    }
                }
            }
            //[04:23:27] The Aged fat Dancedog and the Aged fat Cliffdog get intimate.
            if (line.Content.Contains("get intimate"))
            {
                Match match = Regex.Match(line.Content, @"The (.+) and the (.+) get intimate.");
                if (match.Success)
                {
                    _grangerDebug.Log("LIVETRACKER: attempting to cache last bred pair data due to: " + line);
                    _lastBreedingFemale = null;
                    _lastBreedingMale = null;
                    _lastBreedingOn = DateTime.Now;

                    string name1 = match.Groups[1].Value;
                    string name2 = match.Groups[2].Value;
                    string fixedName1 = GrangerHelpers.RemoveAllPrefixes(name1);
                    string fixedName2 = GrangerHelpers.RemoveAllPrefixes(name2);
                    HorseEntity[] horses1 = GetHorseToUpdate(fixedName1, _playerMan.GetCurrentServerGroup());
                    HorseEntity[] horses2 = GetHorseToUpdate(fixedName2, _playerMan.GetCurrentServerGroup());

                    ExtractBreedingPairHorse(fixedName1, horses1);
                    ExtractBreedingPairHorse(fixedName2, horses2);
                }
            }
            //The Old fat Ebonycloud will probably give birth in a while!
            //[04:23:47] The Aged fat Dancedog will probably give birth in a while!
            if (line.Content.Contains("will probably give birth"))
            {
                if (_lastBreedingOn > DateTime.Now - TimeSpan.FromMinutes(3))
                {
                    _grangerDebug.Log("LIVETRACKER: applying breeding update due to: " + line);

                    // we don't need to requery female horse entity, because any other changes done to this horse
                    // will be reflected on current entity. 
                    // This is true only as long, as entire module uses only a single GrangerContext !!!

                    Match match = Regex.Match(line.Content, @"The (.+) will probably give birth in a while");
                    if (match.Success)
                    {
                        string prefixedName = match.Groups[1].Value;
                        string fixedName = GrangerHelpers.RemoveAllPrefixes(prefixedName);

                        if (_lastBreedingFemale != null)
                        {
                            if (_lastBreedingFemale.Name == fixedName) //sanity check
                            {
                                _lastBreedingFemale.PregnantUntil = DateTime.Now + GrangerHelpers.LongestPregnancyPossible;
                                trayPopups.Schedule(
                                    "BREED UPDATE",
                                    String.Format("({0}) is now marked as pregnant. Be sure to smilexamine to get more accurate pregnancy duration!", _lastBreedingFemale.Name),
                                    6000);
                                _context.SubmitChangesToHorses();
                            }
                            else
                            {
                                trayPopups.Schedule(
                                    "BREED UPDATE PROBLEM",
                                    String.Format("Female name ({0}) does not match the cached name ({1})!", _lastBreedingFemale.Name, fixedName),
                                    6000);
                            }
                        }
                        if (_lastBreedingMale != null)
                        {
                            _lastBreedingMale.NotInMood = DateTime.Now + GrangerHelpers.Breeding_NotInMood_Duration;
                            trayPopups.Schedule(
                                "BREED UPDATE",
                                String.Format("({0}) is now marked as Not In Mood. You can't breed this horse for next 45 minutes.", _lastBreedingMale.Name),
                                6000);
                            _context.SubmitChangesToHorses();
                        }
                    }

                }
            }
            //[06:18:19] The Aged fat Umasad shys away and interrupts the action.
            if (line.Content.Contains("shys away and interrupts"))
            {
                if (_lastBreedingOn > DateTime.Now - TimeSpan.FromMinutes(1))
                {
                    _grangerDebug.Log("LIVETRACKER: processing failed breeding update due to: " + line);
                    Match match = Regex.Match(line.Content, @"The (.+) shys away and interrupts the action");
                    if (match.Success)
                    {
                        string prefixedName = match.Groups[1].Value;
                        string fixedName = GrangerHelpers.RemoveAllPrefixes(prefixedName);
                        if (_lastBreedingMale != null) _lastBreedingMale.NotInMood = DateTime.Now + GrangerHelpers.Breeding_NotInMood_Duration;
                        if (_lastBreedingFemale != null) _lastBreedingFemale.NotInMood = DateTime.Now + GrangerHelpers.Breeding_NotInMood_Duration;
                        trayPopups.Schedule(
                            "BREED UPDATE",
                            String.Format("Breeding appears to have failed, {0} and {1} will be Not In Mood for next 45 minutes.",
                                _lastBreedingMale == null ? "Some horse" : _lastBreedingMale.Name,
                                _lastBreedingFemale == null ? "Some horse" : _lastBreedingFemale.Name),
                            6000);
                    }
                }
            }
            // disease tracking
            //[20:48:42] You smile at Adolescent diseased Mountainheart.
            string diseaseCheck = GrangerHelpers.LineContainsDiseased(line.Content);
            if ((diseaseCheck) != null)
            {
                string possibleHorseName = diseaseCheck;
                var horsesToUpdate = _context.Horses.Where(x => x.Name == possibleHorseName).ToArray();
                if (horsesToUpdate.Length > 0)
                {
                    foreach (var horse in horsesToUpdate)
                    {
                        _grangerDebug.Log("Marking horse diseased: " + horse);
                        horse.SetTag("diseased", true);
                    }
                    _context.SubmitChangesToHorses();
                }
            }
            // genesis handling (add a horse to dict after genesis cast, 
            // ignore horse for smile-examine trait sanity check next time
            //[2013-08-02] [23:08:54] You cast 'Genesis' on Old fat Jollyhalim.
            if (line.Content.Contains("You cast"))
            {
                _grangerDebug.Log("Found maybe genesis log event: " + line);
                Match match = Regex.Match(line.Content, @"You cast 'Genesis' on (.+)\.");
                if (match.Success)
                {

                    string prefixedHorseName = match.Groups[1].Value;
                    string horseName = GrangerHelpers.ExtractHorseName(prefixedHorseName);
                    _grangerDebug.Log(string.Format("Recognized Genesis cast on: {0} (raw name: {1})", horseName, prefixedHorseName));
                    _parentModule.Settings.AddGenesisCast(DateTime.Now, horseName);
                }
            }
        }

        private void TryApplyDeadFlag(string line, Match match)
        {
            if (match.Success)
            {
                _grangerDebug.Log("LIVETRACKER: R.I.P. log line detected, checking if it's a horse from herds, line: " + line);
                string lowercasename = match.Groups[1].Value;
                string fixedName = GrangerHelpers.FixCase(lowercasename);
                HorseEntity[] horses = GetHorseToUpdate(fixedName, _playerMan.GetCurrentServerGroup());
                //its perfectly possible for this to set few horses as dead,
                //but this tag is just informative, so it's ok
                foreach (var horse in horses)
                {
                    _grangerDebug.Log("LIVETRACKER: applying maybedead flag to "+horse+" due to: " + line);
                    horse.SetTagDead();
                    _context.SubmitChangesToHorses();
                }
            }
        }

        private bool MaybeHorseAgeLine(string line)
        {
            if (GrangerHelpers.HasAgeInName(line, true)) return true;
            else return false;
        }

        private class HorseData
        {
            public HorseEntity Horse = null;
            public HorseAge Age = new HorseAge(HorseAgeId.Unknown);
            public string SecondaryInfo = null;

            public string ErrorMessage = null;
            public bool TooManyHorsesFound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="entireDbSetting">false to try selected herds, true to try entire DB</param>
        /// <returns></returns>
        private HorseData GetHorseDataFromAnyLogEvent(string line, bool entireDbSetting)
        {
            HorseEntity[] horses = _context.Horses.ToArray();
            if (!entireDbSetting)
            {
                var selectedHerds = _context.Herds.AsEnumerable().Where(x => x.Selected).Select(x => x.HerdID).ToArray();
                horses = horses.Where(x => selectedHerds.Contains(x.Herd)).ToArray();
            }

            horses = horses.Where(x => line.Contains(x.Name, StringComparison.OrdinalIgnoreCase)).ToArray();

            var result = new HorseData();
            foreach (HorseEntity horseEntity in horses)
            {
                // it is possible we have found event lines that simply contain horse name
                // in a totally unrelated context
                // attempting to get horse age should fail for those and loop continues
                result.SecondaryInfo = null;
                Match match = Regex.Match(line, @"(\w+) (\w+) " + horseEntity.Name, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    result.Age = HorseAge.CreateAgeFromRawHorseName(match.Groups[1].Value);
                    if (result.Age.HorseAgeId != HorseAgeId.Unknown)
                    {
                        result.SecondaryInfo = match.Groups[2].Value;
                    }
                    else
                    {
                        match = Regex.Match(line, @"(\w+) " + horseEntity.Name, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            result.Age = HorseAge.CreateAgeFromRawHorseName(match.Groups[1].Value);
                            if (result.Age.HorseAgeId != HorseAgeId.Unknown)
                            {
                                result.SecondaryInfo = String.Empty;
                            }
                        }
                    }
                    if (result.Age.HorseAgeId != HorseAgeId.Unknown)
                    {
                        // we have found the correct horse data
                        result.Horse = horseEntity;
                        // possibly existing horse is a foal
                        var prevAge = result.Horse.Age.HorseAgeId;
                        var newAge = result.Age.HorseAgeId;
                        // since adding horse for first time gets correct age
                        // we can asume here that horse can't be younger than it is
                        // and go for foal status where applicable
                        if (prevAge == HorseAgeId.YoungFoal)
                        {
                            if (newAge == HorseAgeId.Young) 
                                result.Age = new HorseAge(HorseAgeId.YoungFoal);
                            if (newAge == HorseAgeId.Adolescent)
                                result.Age = new HorseAge(HorseAgeId.AdolescentFoal);
                        }
                        if (prevAge == HorseAgeId.AdolescentFoal)
                        {
                            // here we allow updating to regular Young
                            if (newAge == HorseAgeId.Adolescent) 
                                result.Age = new HorseAge(HorseAgeId.AdolescentFoal);
                        }

                        break;
                    }
                }
            }

            if (result.Horse == null)
            {
                return null;
            }
            var existingHorsesInQuery = horses.Where(x => x.Name == result.Horse.Name).ToArray();
            if (existingHorsesInQuery.Length == 1)
            {
                return result;
            }
            else if (existingHorsesInQuery.Length > 1)
            {
                result.TooManyHorsesFound = true;
                return result;
            }
            return null;
        }

        bool EntireDbSetting
        {
            get
            {
                return _parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb;
            }
        }

        bool HorseUpdateAnyEvent
        {
            get
            {
                return _parentModule.Settings.UpdateHorseDataFromAnyEventLine;
            }
        }

        private void ExtractBreedingPairHorse(string fixedName, HorseEntity[] horses)
        {
            if (horses.Length == 1)
            {
                HorseEntity horse = horses[0];
                if (horse.IsMale ?? false)
                {
                    _lastBreedingMale = horse;
                }
                else
                {
                    _lastBreedingFemale = horse;
                }
            }
            else
            {
                string title = null;
                string message = null;
                if (horses.Length > 1)
                {
                    message = (EntireDbSetting ? "Database has" : "Selected herds have")
                              + " more than one horse named: " + fixedName;
                }
                else if (horses.Length == 0)
                {
                    if (GrangerHelpers.IsBlacklistedCreatureName_EqualCheck(fixedName))
                    {
                        _grangerDebug.Log("Discared breeding candidate due not a horse: " + fixedName);
                    }
                    else
                    {
                        message = "Not a horse or " + (EntireDbSetting ? "" : "selected")
                                  + " herds don't have a horse named: " + fixedName;
                    }
                }
                if (message != null) trayPopups.Schedule(title ?? "BREED UPDATE PROBLEM", message, 6000);
            }
        }

        private HorseEntity[] GetHorseToUpdate(string horseName, ServerGroup serverGroup)
        {
            bool isEpic;
            if (serverGroup.ServerGroupId == ServerGroup.EpicId) isEpic = true;
            else if (serverGroup.ServerGroupId == ServerGroup.FreedomId) isEpic = false;
            else return new HorseEntity[0];

            IEnumerable<HerdEntity> query = _context.Herds;
            if (_parentModule.Settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb == false)
            {
                query = query.Where(x => x.Selected);
            }
            var herdsToCheck = query.Select(x => x.HerdID).ToArray();
            //var step0 = _context.Horses.ToArray();
            //var step1 = step0.Where(x => x.Name == horseName).ToArray();
            //var step2 = step1.Where(x => selectedHerds.Contains(x.Herd)).ToArray();
            //var step3 = step2.Where(x => x.EpicCurve == isEpic).ToArray();
            var foundHorses = _context.Horses.Where(x => x.Name == horseName && herdsToCheck.Contains(x.Herd) && (x.EpicCurve ?? false) == isEpic).ToArray();
            return foundHorses;
        }

        internal void Update()
        {
            _processor.Update();
        }
    }
}
