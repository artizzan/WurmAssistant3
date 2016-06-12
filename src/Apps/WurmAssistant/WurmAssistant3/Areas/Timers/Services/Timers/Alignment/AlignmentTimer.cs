using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Parts;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers.Alignment;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Timers.Services.Timers.Alignment
{
    [PersistentObject("TimersFeature_AlignmentTimer")]
    public class AlignmentTimer : WurmTimer
    {
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
            public DateTime EntryDateTime { get; private set; }
            public bool AlwaysValid { get; private set; }
            public string Reason { get; private set; }
            public bool ComesFromLiveLogs { get; private set; }
            public bool Valid { get; set; }

            public AlignmentHistoryEntry(DateTime date, bool alwaysValid = false, string reason = null, bool comesfromlivelogs = false)
            {
                this.EntryDateTime = date;
                this.AlwaysValid = alwaysValid;
                this.Reason = reason;
                this.ComesFromLiveLogs = comesfromlivelogs;
            }

            public int CompareTo(AlignmentHistoryEntry dtlm)
            {
                return this.EntryDateTime.CompareTo((DateTime) dtlm.EntryDateTime);
            }
        }

        public enum WurmReligions { Vynora, Magranon, Fo, Libila };

        static readonly TimeSpan AlignmentCooldown = new TimeSpan(0, 30, 0);

        [JsonProperty]
        bool isWhiteLighter = true;

        [JsonProperty]
        WurmReligions playerReligion;

        DateTime dateOfNextAlignment = DateTime.MinValue;
        readonly List<AlignmentHistoryEntry> alignmentHistory = new List<AlignmentHistoryEntry>();

        public AlignmentTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundManager soundManager,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundManager)
        {
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            View.SetCooldown(AlignmentCooldown);
            MoreOptionsAvailable = true;

            PerformAsyncInits();
        }

        async void PerformAsyncInits()
        {
            try
            {
                InitCompleted = false;
                alignmentHistory.Clear();

                List<LogEntry> lines = await GetLogLinesFromLogHistoryAsync(LogType.Event, TimeSpan.FromDays(3));

                foreach (LogEntry line in lines)
                {
                    if (AlignmentVerifier.CheckConditions(line, IsWhiteLighter, PlayerReligion))
                    {
                        alignmentHistory.Add(new AlignmentHistoryEntry(line.Timestamp, reason: line.Content));
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

        DateTime DateOfNextAlignment
        {
            get { return dateOfNextAlignment; }
            set { dateOfNextAlignment = value; CDNotify.CooldownTo = value; }
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

        void SetLightAndReligion(bool whiteLighter, WurmReligions religion)
        {
            if (whiteLighter != IsWhiteLighter || religion != PlayerReligion)
            {
                IsWhiteLighter = whiteLighter;
                PlayerReligion = religion;
                PerformAsyncInits();
            }
        }

        public override void Update()
        {
            base.Update();
            if (View.Visible)
            {
                View.SetCooldown(AlignmentCooldown);
                View.UpdateCooldown(DateOfNextAlignment);
            }
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            AlignmentTimerOptionsForm ui = new AlignmentTimerOptionsForm(this);
            if (ui.ShowDialogCenteredOnForm(form) == System.Windows.Forms.DialogResult.OK)
            {
                SetLightAndReligion(ui.WhiteLighter, ui.Religion);
            }
        }

        public void ShowVerifyList()
        {
            var allalignments = new List<string>();
            foreach (AlignmentHistoryEntry entry in alignmentHistory)
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
                alignmentHistory.Add(new AlignmentHistoryEntry(DateTime.Now, true));
                UpdateAlignmentCooldown();
            }
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (AlignmentVerifier.CheckConditions(line, IsWhiteLighter, PlayerReligion))
            {
                alignmentHistory.Add(new AlignmentHistoryEntry(DateTime.Now, reason: line.Content, comesfromlivelogs: true));
                UpdateAlignmentCooldown();
            }
        }

        void UpdateAlignmentCooldown()
        {
            RevalidateAlignmentHistory();
            UpdateNextAlignmentDate();
        }

        void RevalidateAlignmentHistory()
        {
            alignmentHistory.Sort();

            var lastValidEntry = new DateTime(0);
            // validate entries
            foreach (AlignmentHistoryEntry entry in alignmentHistory)
            {
                entry.Valid = false;

                // all entries are default invalid
                if (entry.AlwaysValid)
                {
                    entry.Valid = true;
                    lastValidEntry = entry.EntryDateTime;
                }
                else if (entry.EntryDateTime > lastValidEntry + AlignmentCooldown)
                {
                    entry.Valid = true;
                }
            }
        }

        void UpdateNextAlignmentDate()
        {
            DateOfNextAlignment = FindLastValidAlignmentInHistory() + AlignmentCooldown;
        }

        DateTime FindLastValidAlignmentInHistory()
        {
            if (alignmentHistory.Count > 0)
            {
                for (int i = alignmentHistory.Count - 1; i >= 0; i--)
                {
                    if (alignmentHistory[i].Valid) return alignmentHistory[i].EntryDateTime;
                }
            }
            return new DateTime(0);
        }
    }
}
