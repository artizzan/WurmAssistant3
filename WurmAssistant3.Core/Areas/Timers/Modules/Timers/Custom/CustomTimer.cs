using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Custom
{
    [PersistentObject("TimersFeature_CustomTimer")]
    public class CustomTimer : WurmTimer
    {
        readonly ILogger logger;
        CustomTimerOptionsTemplate Options;

        DateTime _cooldownTo = DateTime.MinValue;
        DateTime CooldownTo
        {
            get { return _cooldownTo; }
            set
            {
                _cooldownTo = value;
                CDNotify.CooldownTo = value;
            }
        }
        DateTime UptimeResetSince = DateTime.MinValue;

        public CustomTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        //happens before Initialize
        public void ApplyCustomTimerOptions(CustomTimerOptionsTemplate options)
        {
            Options = options;
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            //MoreOptionsAvailable = true;
            TimerDisplayView.SetCooldown(Options.Duration);

            PerformAsyncInits();
        }

        async void PerformAsyncInits()
        {
            try
            {
                await UpdateDateOfLastCooldownReset();

                HashSet<LogType> condLogTypes = new HashSet<LogType>(
                    Options.TriggerConditions.Select(x => x.LogType));

                foreach (var type in condLogTypes)
                {
                    LogType captType = type;
                    List<LogEntry> lines = await GetLogLinesFromLogHistoryAsync(captType,
                        DateTime.Now - Options.Duration - TimeSpan.FromDays(2));
                    foreach (var cond in Options.TriggerConditions)
                    {
                        if (cond.LogType == captType) ProcessLinesForCooldownTriggers(lines, cond, false);
                    }
                }

                InitCompleted = true;
            }
            catch (Exception _e)
            {
                logger.Error(_e, "init error");
            }
        }

        public override void Update(bool engineSleeping)
        {
            base.Update(engineSleeping);
            if (TimerDisplayView.Visible) TimerDisplayView.UpdateCooldown(CooldownTo);
        }

        public override void Stop()
        {

            base.Stop();
        }

        public override void HandleAnyLogLine(LogsMonitorEventArgs container)
        {
            foreach (var cond in Options.TriggerConditions)
            {
                if (cond.LogType == container.LogType)
                {
                    ProcessLinesForCooldownTriggers(container.WurmLogEntries.ToList(), cond, true);
                }
            }
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            base.OpenMoreOptions(form);
        }

        protected override void HandleServerChange()
        {
            UpdateDateOfLastCooldownReset();
            try
            {
                TriggerCooldown(CooldownTo - Options.Duration);
            }
            catch (Exception _e)
            {
                if (CooldownTo == DateTime.MinValue)
                {
                    TriggerCooldown(CooldownTo);
                }
                else
                {
                    logger.Info(_e, "unknown problem with HandleServerChange");
                }
            }
        }

        void ProcessLinesForCooldownTriggers(List<LogEntry> lines, CustomTimerOptionsTemplate.Condition condition, bool liveLogs)
        {
            foreach (LogEntry line in lines)
            {
                RegexOptions opt = new RegexOptions();
                if (!Options.IsRegex) opt = RegexOptions.IgnoreCase;
                if (Regex.IsMatch(line.Content, condition.RegexPattern, opt))
                {
                    TriggerCooldown(line.Timestamp);
                }
            }
        }

        void TriggerCooldown(DateTime startDate)
        {
            if (Options.ResetOnUptime && startDate > UptimeResetSince)
            {
                DateTime cd_to = startDate + Options.Duration;
                DateTime NextUptimeReset = UptimeResetSince + TimeSpan.FromDays(1);
                if (cd_to > NextUptimeReset)
                    cd_to = NextUptimeReset;
                CooldownTo = cd_to;
            }
            else
            {
                CooldownTo = startDate + Options.Duration;
            }
        }

        async Task UpdateDateOfLastCooldownReset()
        {
            var result = await GetLatestUptimeCooldownResetDate();
            if (result > DateTime.MinValue) UptimeResetSince = result;
        }
    }
}
