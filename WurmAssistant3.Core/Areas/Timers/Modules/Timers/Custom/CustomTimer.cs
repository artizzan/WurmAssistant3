using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Custom
{
    [PersistentObject("TimersFeature_CustomTimer")]
    public class CustomTimer : WurmTimer
    {
        readonly ILogger logger;

        CustomTimerConfig config;
        DateTime cooldownTo = DateTime.MinValue;
        DateTime uptimeResetSince = DateTime.MinValue;

        public CustomTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            TimerDisplayView.SetCooldown(config.Duration);

            PerformAsyncInits();
        }

        async void PerformAsyncInits()
        {
            try
            {
                await UpdateDateOfLastCooldownReset();

                HashSet<LogType> condLogTypes = new HashSet<LogType>(
                    config.TriggerConditions.Select(x => x.LogType));

                foreach (var type in condLogTypes)
                {
                    LogType captType = type;
                    List<LogEntry> lines = await GetLogLinesFromLogHistoryAsync(captType,
                        DateTime.Now - config.Duration - TimeSpan.FromDays(2));
                    foreach (var cond in config.TriggerConditions)
                    {
                        if (cond.LogType == captType) ProcessLinesForCooldownTriggers(lines, cond);
                    }
                }

                InitCompleted = true;
            }
            catch (Exception _e)
            {
                logger.Error(_e, "init error");
            }
        }

        DateTime CooldownTo
        {
            get { return cooldownTo; }
            set
            {
                cooldownTo = value;
                CDNotify.CooldownTo = value;
            }
        }

        public void ApplyCustomTimerOptions(CustomTimerConfig customTimerConfig)
        {
            this.config = customTimerConfig;
        }

        public override void Update()
        {
            base.Update();
            if (TimerDisplayView.Visible) TimerDisplayView.UpdateCooldown(CooldownTo);
        }

        public override void HandleAnyLogLine(LogsMonitorEventArgs container)
        {
            foreach (var cond in config.TriggerConditions)
            {
                if (cond.LogType == container.LogType)
                {
                    ProcessLinesForCooldownTriggers(container.WurmLogEntries.ToList(), cond);
                }
            }
        }

        protected override void HandleServerChange()
        {
            BeginUpdateDateOfLastCooldownReset();
            try
            {
                TriggerCooldown(CooldownTo - config.Duration);
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

        void ProcessLinesForCooldownTriggers(List<LogEntry> lines, CustomTimerConfig.Condition condition)
        {
            foreach (LogEntry line in lines)
            {
                RegexOptions opt = new RegexOptions();
                if (!config.IsRegex) opt = RegexOptions.IgnoreCase;
                if (Regex.IsMatch(line.Content, condition.RegexPattern, opt))
                {
                    TriggerCooldown(line.Timestamp);
                }
            }
        }

        void TriggerCooldown(DateTime startDate)
        {
            if (config.ResetOnUptime && startDate > uptimeResetSince)
            {
                DateTime cdTo = startDate + config.Duration;
                DateTime nextUptimeReset = uptimeResetSince + TimeSpan.FromDays(1);
                if (cdTo > nextUptimeReset)
                    cdTo = nextUptimeReset;
                CooldownTo = cdTo;
            }
            else
            {
                CooldownTo = startDate + config.Duration;
            }
        }

        async void BeginUpdateDateOfLastCooldownReset()
        {
            try
            {
                await UpdateDateOfLastCooldownReset();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error during UpdateDateOfLastCooldownReset at timer " + this.ToString());
            }

        }

        async Task UpdateDateOfLastCooldownReset()
        {
            var result = await GetLatestUptimeCooldownResetDate();
            if (result > DateTime.MinValue) uptimeResetSince = result;
        }
    }
}
