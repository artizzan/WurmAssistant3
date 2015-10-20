using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Alignment;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Alignment
{
    [PersistentObject("TimersFeature_AlignmentTimer")]
    public class AlignmentTimer : WurmTimer
    {
        public enum WurmReligions { Vynora, Magranon, Fo, Libila };

        private static class AlignmentVerifier
        {
            public static bool CheckConditions(LogEntry line, bool isWhiteLight, WurmReligions religion)
            {
                //Holding a sermon (+/- 1)
                if (line.Content.Contains("You finish this sermon"))
                {
                    return true;
                }
                //Listening to a sermon (up to +/- 4 (depends on preaching success))
                if (line.Content.Contains("finishes the sermon by asking you"))
                {
                    return true;
                }
                //Converting someone to your religion (+/- 1)
                //Sacrificing items in an altar (at least worth 50c get price) (+/- 1)

                if (isWhiteLight)
                {
                    //Burying a human corpse (+ 2)
                    if (line.Content.Contains("You bury"))
                    {
                        if (Regex.IsMatch(line.Content, @"You bury the corpse of \w+", RegexOptions.Compiled))
                        {
                            if (!Regex.IsMatch(line.Content, @"You bury the corpse of \w+ \w+", RegexOptions.Compiled))
                            {
                                return true;
                            }
                        }
                        if (line.Content.Contains("tower guar"))
                            return true;
                    }
                    //Healing someone else (+ 1)
                    if (line.Content.Contains("You treat the wound") || line.Content.Contains("You bandage the wound"))
                    {
                        return true;
                    }
                    //Casting Bless on players (seems random). (+ 1)
                    //Praying at the White Light on the Wild server. (+ 3)
                    //Killing of a blacklighter (+ 5) no way to tell light = NOT POSSIBLE
                }
                else if (!isWhiteLight)
                {
                    //Butchering a human corpse (- 1)
                    if (line.Content.Contains("You butcher"))
                    {
                        if (Regex.IsMatch(line.Content, @"You butcher the corpse of \w+", RegexOptions.Compiled))
                        {
                            if (!Regex.IsMatch(line.Content, @"You butcher the corpse of \w+ \w+", RegexOptions.Compiled))
                            {
                                return true;
                            }
                        }
                        if (line.Content.Contains("tower guar"))
                            return true;
                    }
                    //Successful Lockpicking (-5)
                    //Praying at the Black Light on the Wild server. (- 3)
                    //Desecrating an altar (- 2)
                    //Killing of a whitelighter (- 5) no way to tell light = NOT POSSIBLE
                }

                if (religion == WurmReligions.Fo)
                {
                    //Listening to a confession (+/- 1)
                    if (line.Content.Contains("You decide that a good penance is for"))
                    {
                        return true;
                    }
                    //Confessing to a priest (+/- 5)
                    if (line.Content.Contains("thinks for a while and asks you"))
                    {
                        return true;
                    }
                    //Fo special: plant a sprout or a flower (+ 1)
                    if (line.Content.Contains("You plant the sprout"))
                    {
                        return true;
                    }

                    if (line.Content.Contains("You plant the"))
                    {
                        if (Regex.IsMatch(line.Content, @"You plant the.+flowers\.", RegexOptions.Compiled))
                            return true;
                    }
                }
                else if (religion == WurmReligions.Vynora)
                {
                    //Listening to a confession (+/- 1)
                    if (line.Content.Contains("You decide that a good penance is for"))
                    {
                        return true;
                    }
                    //Confessing to a priest (+/- 5)
                    if (line.Content.Contains("thinks for a while and asks you"))
                    {
                        return true;
                    }
                    //Vynora special: cutting down an old, very old or overaged tree (+ 1) NOTE: doesn't care about the age
                    if (line.Content.Contains("You cut down the"))
                    {
                        return true;
                    }
                    //Vynora special: working on walls (+ 0.5) (what walls? fences? house walls? roofs floors?)
                }
                else if (religion == WurmReligions.Magranon)
                {
                    //Listening to a confession (+/- 1)
                    if (line.Content.Contains("You decide that a good penance is for"))
                    {
                        return true;
                    }
                    //Confessing to a priest (+/- 5)
                    if (line.Content.Contains("thinks for a while and asks you"))
                    {
                        return true;
                    }
                    //Magranon special: mine (+ 0.5)
                    if (line.Content.Contains("You mine some"))
                    {
                        return true;
                    }
                    //Magranon special: kill a creature (+ 0.5)
                    if (line.Content.Contains("is dead. R.I.P."))
                    {
                        if (!line.Content.Contains("tower guard"))
                        {
                            return true;
                        }
                    }
                }
                else if (religion == WurmReligions.Libila)
                {
                    //Listening to a confession (+/- 1)
                    if (line.Content.Contains("You decide that you can probably fool"))
                    {
                        return true;
                    }
                    //Confessing to a priest (+/- 5)
                    if (line.Content.Contains("scorns you and tells you to give"))
                    {
                        return true;
                    }
                    //Libila special: kill a creature (- 0.5)
                    if (line.Content.Contains("is dead. R.I.P."))
                    {
                        if (!line.Content.Contains("tower guard"))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        class AlignmentHistoryEntry : IComparable<AlignmentHistoryEntry>
        {
            public DateTime EntryDateTime;
            public bool AlwaysValid = false;
            public bool Valid = false;
            public string Reason;
            public bool ComesFromLiveLogs = false;

            public AlignmentHistoryEntry(DateTime date, bool alwaysValid = false, string reason = null, bool comesfromlivelogs = false)
            {
                this.EntryDateTime = date;
                this.AlwaysValid = alwaysValid;
                this.Reason = reason;
                this.ComesFromLiveLogs = comesfromlivelogs;
            }

            public int CompareTo(AlignmentHistoryEntry dtlm)
            {
                return this.EntryDateTime.CompareTo(dtlm.EntryDateTime);
            }
        }

        public AlignmentTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
        }

        [JsonProperty]
        public bool isWhiteLighter = true;
        [JsonProperty]
        public WurmReligions playerReligion;

        public static TimeSpan AlignmentCooldown = new TimeSpan(0, 30, 0);

        DateTime _dateOfNextAlignment = DateTime.MinValue;
        DateTime DateOfNextAlignment
        {
            get { return _dateOfNextAlignment; }
            set { _dateOfNextAlignment = value; CDNotify.CooldownTo = value; }
        }

        List<AlignmentHistoryEntry> AlignmentHistory = new List<AlignmentHistoryEntry>();

        //DateTime _cooldownResetSince = DateTime.MinValue;

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            TimerDisplayView.SetCooldown(AlignmentCooldown);
            MoreOptionsAvailable = true;

            PerformAsyncInits();
        }

        public bool IsWhiteLighter
        {
            get { return isWhiteLighter; }
            set
            {
                isWhiteLighter = value;
                FlagAsChanged();
            }
        }
        public WurmReligions PlayerReligion
        {
            get { return playerReligion; }
            set
            {
                playerReligion = value;
                FlagAsChanged();
            }
        }

        public void SetLightAndReligion(bool whiteLighter, WurmReligions religion)
        {
            if (whiteLighter != IsWhiteLighter || religion != PlayerReligion)
            {
                IsWhiteLighter = whiteLighter;
                PlayerReligion = religion;
                PerformAsyncInits();
            }
        }

        async Task PerformAsyncInits()
        {
            try
            {
                InitCompleted = false;
                AlignmentHistory.Clear();
                
                List<LogEntry> lines = await GetLogLinesFromLogHistoryAsync(LogType.Event, TimeSpan.FromDays(3));

                foreach (LogEntry line in lines)
                {
                    if (AlignmentVerifier.CheckConditions(line, IsWhiteLighter, PlayerReligion))
                    {
                        AlignmentHistory.Add(new AlignmentHistoryEntry(line.Timestamp, reason: line.Content));
                    }
                }

                UpdateAlignmentCooldown();

                InitCompleted = true;
            }
            catch (Exception _e)
            {
                Logger.Error(_e, "init error");
            }
        }

        public override void Update()
        {
            base.Update();
            if (TimerDisplayView.Visible) TimerDisplayView.UpdateCooldown(DateOfNextAlignment);
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            AlignmentTimerOptionsForm ui = new AlignmentTimerOptionsForm(this, form);
            if (ui.ShowDialogCenteredOnForm(form) == System.Windows.Forms.DialogResult.OK)
            {
                SetLightAndReligion(ui.WhiteLighter, ui.Religion);
            }
        }

        public void ShowVerifyList()
        {
            var allalignments = new List<string>();
            foreach (AlignmentHistoryEntry entry in AlignmentHistory)
            {
                if (entry.EntryDateTime > DateTime.Now - TimeSpan.FromMinutes(31))
                {
                    if (entry.AlwaysValid) allalignments.Add(entry.EntryDateTime + ", accurate (comes from skill log)");
                    else
                    {
                        string output;
                        if (entry.Reason != null)
                        {
                            output = (entry.ComesFromLiveLogs == true ? entry.EntryDateTime + ", " + entry.Reason : entry.Reason);
                        }
                        else
                        {
                            output = entry.EntryDateTime + ", reason missing";
                        }
                        allalignments.Add(output);
                    }
                }
            }
            var ui = new VerifyAlignmentForm(allalignments.ToArray());
            ui.Show();
        }

        public override void HandleNewSkillLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("Alignment increased", StringComparison.Ordinal))
            {
                AlignmentHistory.Add(new AlignmentHistoryEntry(DateTime.Now, true));
                UpdateAlignmentCooldown();
            }
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (AlignmentVerifier.CheckConditions(line, IsWhiteLighter, PlayerReligion))
            {
                AlignmentHistory.Add(new AlignmentHistoryEntry(DateTime.Now, reason: line.Content, comesfromlivelogs: true));
                UpdateAlignmentCooldown();
            }
        }

        void UpdateAlignmentCooldown()
        {
            //UpdateDateOfLastCooldownReset();
            RevalidateAlignmentHistory();
            UpdateNextAlignmentDate();
        }

        //void UpdateDateOfLastCooldownReset()
        //{
        //    var result = GetLatestUptimeCooldownResetDate();
        //    if (result > DateTime.MinValue) _cooldownResetSince = result;
        //}

        void RevalidateAlignmentHistory()
        {
            AlignmentHistory.Sort();

            var lastValidEntry = new DateTime(0);
            // validate entries
            foreach (AlignmentHistoryEntry entry in AlignmentHistory)
            {
                entry.Valid = false;
                // all entries are default invalid
                // discard any entry prior to cooldown reset => this is no longer true
                //if (entry.EntryDateTime > _cooldownResetSince)
                //{
                    if (entry.AlwaysValid)
                    {
                        entry.Valid = true;
                        lastValidEntry = entry.EntryDateTime;
                    }
                    //if entry date is later, than last valid + cooldown period
                    else if (entry.EntryDateTime > lastValidEntry + AlignmentCooldown)
                    {
                        entry.Valid = true;
                        //lastValidEntry = entry.EntryDateTime;  //this will never be accurate enough to qualify for validity
                    }
                //}
            }
        }

        void UpdateNextAlignmentDate()
        {
            DateOfNextAlignment = FindLastValidAlignmentInHistory() + AlignmentCooldown;

            //if (DateOfNextAlignment > _cooldownResetSince + TimeSpan.FromDays(1))
            //{
            //    DateOfNextAlignment = _cooldownResetSince + TimeSpan.FromDays(1);
            //}
        }

        DateTime FindLastValidAlignmentInHistory()
        {
            if (AlignmentHistory.Count > 0)
            {
                for (int i = AlignmentHistory.Count - 1; i >= 0; i--)
                {
                    if (AlignmentHistory[i].Valid) return AlignmentHistory[i].EntryDateTime;
                }
            }
            return new DateTime(0);
        }
    }
}
