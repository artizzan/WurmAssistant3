using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Sermon
{
    [PersistentObject("TimersFeature_SermonTimer")]
    public class SermonTimer : WurmTimer
    {
        private static readonly TimeSpan SermonPreacherCooldown = new TimeSpan(3, 0, 0);

        DateTime _dateOfNextSermon = DateTime.MinValue;
        DateTime DateOfNextSermon
        {
            get { return _dateOfNextSermon; }
            set {
                _dateOfNextSermon = value; 
                CDNotify.CooldownTo = value; 
            }
        }

        //DateTime CooldownResetSince = DateTime.MinValue;

        public SermonTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            TimerDisplayView.SetCooldown(SermonPreacherCooldown);

            PerformAsyncInits();
        }

        async Task PerformAsyncInits()
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
                    UpdateDateOfNextSermon(lastSermonLine, false);
                }

                InitCompleted = true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "init error");
            }
        }

        public override void Update()
        {
            base.Update();
            if (TimerDisplayView.Visible) TimerDisplayView.UpdateCooldown(DateOfNextSermon);
        }

        protected override void HandleServerChange()
        {
            //UpdateDateOfLastCooldownReset();
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("You finish this sermon", StringComparison.Ordinal))
            {
                UpdateDateOfNextSermon(line, true);
            }
        }

        void UpdateDateOfNextSermon(LogEntry line, bool liveLogs)
        {
            //UpdateDateOfLastCooldownReset();
            DateOfNextSermon = line.Timestamp + SermonPreacherCooldown;

            //if (DateOfNextSermon > CooldownResetSince + TimeSpan.FromDays(1))
            //{
            //    DateOfNextSermon = CooldownResetSince + TimeSpan.FromDays(1);
            //}
        }

        //void UpdateDateOfLastCooldownReset()
        //{
        //    var result = GetLatestUptimeCooldownResetDate();
        //    if (result > DateTime.MinValue) CooldownResetSince = result;
        //}
    }
}
