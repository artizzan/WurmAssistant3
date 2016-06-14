using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Parts;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Timers.Services.Timers.Sermon
{
    [KernelBind, PersistentObject("TimersFeature_SermonTimer")]
    public class SermonTimer : WurmTimer
    {
        private static readonly TimeSpan SermonPreacherCooldown = new TimeSpan(3, 0, 0);

        DateTime dateOfNextSermon = DateTime.MinValue;

        public SermonTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundManager soundManager,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundManager)
        {
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            View.SetCooldown(SermonPreacherCooldown);

            PerformAsyncInits();
        }

        async void PerformAsyncInits()
        {
            try
            {
                List<LogEntry> lines = await GetLogLinesFromLogHistoryAsync(LogType.Event, TimeSpan.FromDays(2));

                LogEntry lastSermonLine = null;
                foreach (LogEntry line in lines)
                {
                    if (line.Content.Contains("You finish this sermon"))
                    {
                        lastSermonLine = line;
                    }
                }
                if (lastSermonLine != null)
                {
                    UpdateDateOfNextSermon(lastSermonLine);
                }

                InitCompleted = true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "init error");
            }
        }

        DateTime DateOfNextSermon
        {
            get { return dateOfNextSermon; }
            set {
                dateOfNextSermon = value; 
                CDNotify.CooldownTo = value; 
            }
        }

        public override void Update()
        {
            base.Update();
            if (View.Visible)
            {
                View.SetCooldown(SermonPreacherCooldown);
                View.UpdateCooldown(DateOfNextSermon);
            }
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("You finish this sermon", StringComparison.Ordinal))
            {
                UpdateDateOfNextSermon(line);
            }
        }

        void UpdateDateOfNextSermon(LogEntry line)
        {
            DateOfNextSermon = line.Timestamp + SermonPreacherCooldown;
        }
    }
}
