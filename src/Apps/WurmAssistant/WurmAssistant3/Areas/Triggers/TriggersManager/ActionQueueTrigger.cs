using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.ActionQueueParsing;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    public class ActionQueueTrigger : TriggerBase
    {
        readonly ILogger logger;
        readonly IActionQueueConditions conditionsManager;

        private bool levelingMode;
        private string lastEventLine;
        bool notificationScheduled;
        // last queue ending action
        DateTime lastActionFinished;
        // last queue starting action
        DateTime lastActionStarted;
        // wurm log line that triggered queue sound
        private string logEntryThatTriggeredLastQueueSound;

        public ActionQueueTrigger(TriggerEntity triggerEntity, ISoundManager soundManager, ITrayPopups trayPopups,
            IWurmApi wurmApi, ILogger logger, [NotNull] IActionQueueConditions conditionsManager)
            : base(triggerEntity, soundManager, trayPopups, wurmApi, logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (conditionsManager == null) throw new ArgumentNullException("conditionsManager");
            this.logger = logger;
            this.conditionsManager = conditionsManager;
            LockedLogTypes = new[] { LogType.Event };
            lastActionFinished = DateTime.Now;
            lastActionStarted = DateTime.Now;
            lastEventLine = string.Empty;
        }

        public double NotificationDelay
        {
            get { return TriggerEntity.NotificationDelay; }
            set { TriggerEntity.NotificationDelay = value; }
        }

        public override bool CheckLogType(LogType type)
        {
            return type == LogType.Event;
        }

        bool CheckMatch(LogEntry entry, IActionQueueParsingCondition condition)
        {
            Debug.Assert(entry != null);
            Debug.Assert(condition != null);
            Debug.Assert(condition.Pattern != null);

            if (condition.MatchingKind == MatchingKind.StartsWithCaseSensitiveOrdinal)
            {
                return entry.Content.StartsWith(condition.Pattern);
            }
            else if (condition.MatchingKind == MatchingKind.ContainsCaseSensitiveOrdinal)
            {
                return entry.Content.Contains(condition.Pattern);
            }
            else if (condition.MatchingKind == MatchingKind.RegexDefaultOptions)
            {
                return Regex.IsMatch(entry.Content, condition.Pattern);
            }
            else
            {
                throw new ApplicationException(string.Format("Unsupported {0} of: {1}",
                    typeof (IActionQueueParsingCondition).Name,
                    condition.MatchingKind));
            }
        }

        public override void Update(LogEntry logEntry, DateTime dateTimeNow)
        {
            bool playerActionStarted = false;

            foreach (var c in conditionsManager.ActionStart)
            {
                if (CheckMatch(logEntry, c))
                {
                    playerActionStarted = true;
                    levelingMode = false;
                }
            }

            if (levelingMode)
            {
                foreach (var c in conditionsManager.LevelingEnd)
                {
                    if (CheckMatch(logEntry, c))
                    {
                        levelingMode = false;
                    }
                }
            }

            foreach (var c in conditionsManager.LevelingStart)
            {
                if (CheckMatch(logEntry, c))
                {
                    levelingMode = true;
                }
            }

            foreach (var c in conditionsManager.ActionFalstart)
            {
                if (CheckMatch(logEntry, c))
                {
                    playerActionStarted = false;
                }
            }

            if (playerActionStarted)
            {
                lastActionStarted = DateTime.Now;
            }

            bool playerActionFinished = false;

            foreach (var c in conditionsManager.ActionEnd)
            {
                if (CheckMatch(logEntry, c))
                    playerActionFinished = true;
            }

            foreach (var c in conditionsManager.ActionFalsEnd)
            {
                if (CheckMatch(logEntry, c))
                    playerActionFinished = false;
            }
            foreach (var c in conditionsManager.ActionFalsEndPreviousEvent)
            {
                if (CheckMatch(logEntry, c))
                    playerActionFinished = false;
            }

            if (levelingMode) playerActionFinished = false;
            if (playerActionFinished == true)
            {
                logEntryThatTriggeredLastQueueSound = logEntry.Content;
                lastActionFinished = DateTime.Now;
                // if action finished, older action started is no longer valid
                // and should not disable queuesound in next conditional
                lastActionStarted = lastActionStarted.AddSeconds(-TriggerEntity.NotificationDelay);
                notificationScheduled = true;
            }

            // cancel scheduled queue sound if new action started before its played
            if (lastActionStarted.AddSeconds(TriggerEntity.NotificationDelay) >= DateTime.Now)
            {
                notificationScheduled = false;
            }

            if (!Active) notificationScheduled = false;

            lastEventLine = logEntry.Content;
        }

        public override void FixedUpdate(DateTime dateTimeNow)
        {
            if (Active && notificationScheduled && dateTimeNow >= lastActionFinished.AddSeconds(TriggerEntity.NotificationDelay))
            {
                if (!CooldownEnabled)
                {
                    DoNotifies(dateTimeNow);
                }
                else
                {
                    if (ResetOnConditonHit)
                    {
                        if (dateTimeNow > CooldownUntil)
                        {
                            DoNotifies(dateTimeNow);
                        }
                        else
                        {
                            CooldownUntil = dateTimeNow + Cooldown;
                            notificationScheduled = false;
                        }
                    }
                    else
                    {
                        if (dateTimeNow > CooldownUntil)
                        {
                            DoNotifies(dateTimeNow);
                            CooldownUntil = dateTimeNow + Cooldown;
                        }
                    }
                }
            }
        }

        protected override void DoNotifies(DateTime dateTimeNow)
        {
            base.FireAllNotification();
            logger.Info("Queue notification triggered due to event: " + logEntryThatTriggeredLastQueueSound);
            notificationScheduled = false;
        }

        protected override bool CheckCondition(LogEntry logMessage)
        {
            throw new TriggerException("ActionQueueTrigger does not implement this method");
        }

        public override string ConditionAspect
        {
            get { return "# when action queue is finished #"; }
        }

        public override string TypeAspect
        {
            get { return "Action Queue"; }
        }

        public override IEnumerable<ITriggerConfig> Configs
        {
            get
            {
                return new List<ITriggerConfig>(base.Configs) { new ActionQueueTriggerConfig(this, conditionsManager) };
            }
        }

        public override bool DefaultDelayFunctionalityDisabled
        {
            get { return true; }
        }
    }
}
