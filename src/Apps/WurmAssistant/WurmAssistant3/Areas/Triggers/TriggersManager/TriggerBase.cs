using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Persistence;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Insights;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using AldursLab.WurmAssistant3.Areas.Triggers.Notifiers;
using JetBrains.Annotations;
using PopupNotifier = AldursLab.WurmAssistant3.Areas.Triggers.Notifiers.PopupNotifier;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    public abstract class TriggerBase : TriggerAbstract, ITrigger
    {
        protected readonly TriggerEntity TriggerEntity;

        protected readonly ISoundManager SoundManager;
        protected readonly ITrayPopups TrayPopups;
        protected readonly IWurmApi WurmApi;
        protected readonly ILogger Logger;
        readonly ITelemetry telemetry;

        public TriggerBase([NotNull] TriggerEntity triggerEntity, [NotNull] ISoundManager soundManager, [NotNull] ITrayPopups trayPopups,
            [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger,
            [NotNull] ITelemetry telemetry)
        {
            if (triggerEntity == null) throw new ArgumentNullException("triggerEntity");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.TriggerEntity = triggerEntity;
            this.SoundManager = soundManager;
            this.TrayPopups = trayPopups;
            this.WurmApi = wurmApi;
            this.Logger = logger;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));

            MuteChecker = () => false;
            Active = true;

            if (triggerEntity.HasSound)
            {
                Sound = new SoundNotifier(this, soundManager);
            }
            if (triggerEntity.HasPopup)
            {
                Popup = new PopupNotifier(this, trayPopups);
            }

            telemetry.TrackEvent($"Triggers: initialized trigger {this.GetType().Name}");
        }

        public Guid TriggerId { get { return TriggerEntity.TriggerId; } }

        public TriggerKind TriggerKind { get { return TriggerEntity.TriggerKind; } }

        public bool DelayEnabled
        {
            get { return TriggerEntity.DelayEnabled; }
            set
            {
                TriggerEntity.DelayEnabled = value;
            }
        }

        public TimeSpan Delay
        {
            get { return TriggerEntity.Delay; }
            set { TriggerEntity.Delay = value; }
        }

        public Func<bool> MuteChecker { private get; set; }

        private bool Muted
        {
            get
            {
                return MuteChecker();
            }
        }

        public void AddLogType(LogType type)
        {
            if (LogTypesLocked) throw new TriggerException("child has blocked adding log types to this trigger");
            TriggerEntity.AddLogType(type);
        }

        public void RemoveLogType(LogType type)
        {
            TriggerEntity.RemoveLogType(type);
        }

        public virtual bool CheckLogType(LogType type)
        {
            return TriggerEntity.HasLogType(type);
        }

        public SoundNotifier Sound { get; private set; }

        public PopupNotifier Popup { get; private set; }

        public string Name
        {
            get { return TriggerEntity.Name; }
            set { TriggerEntity.Name = value; }
        }

        protected DateTime CooldownUntil
        {
            get { return TriggerEntity.CooldownUntil; }
            set { TriggerEntity.CooldownUntil = value; }
        }

        public bool CooldownEnabled
        {
            get { return TriggerEntity.CooldownEnabled; }
            set
            {
                TriggerEntity.CooldownEnabled = value;
                if (!value) CooldownUntil = DateTime.MinValue;
            }
        }

        public TimeSpan Cooldown
        {
            get { return TriggerEntity.Cooldown; }
            set { TriggerEntity.Cooldown = value; }
        }

        public bool Active
        {
            get { return TriggerEntity.Active; }
            set { TriggerEntity.Active = value; }
        }

        public virtual void Update(LogEntry logEntry, DateTime dateTimeNow)
        {
            if (Active)
            {
                if (!CooldownEnabled)
                {
                    if (CheckCondition(logEntry))
                    {
                        DoNotifies(dateTimeNow);
                    }
                }
                else
                {
                    if (ResetOnConditonHit)
                    {
                        if (CheckCondition(logEntry))
                        {
                            if (dateTimeNow > CooldownUntil) DoNotifies(dateTimeNow);
                            CooldownUntil = dateTimeNow + Cooldown;
                        }
                    }
                    else if (dateTimeNow > CooldownUntil)
                    {
                        if (CheckCondition(logEntry))
                        {
                            DoNotifies(dateTimeNow);
                            CooldownUntil = dateTimeNow + Cooldown;
                        }
                    }
                }
            }
        }

        private DateTime? ScheduledNotify { get; set; }

        public virtual bool DefaultDelayFunctionalityDisabled { get { return false; } }

        public virtual void FixedUpdate(DateTime dateTimeNow)
        {
            if (ScheduledNotify != null)
            {
                if (dateTimeNow > ScheduledNotify)
                {
                    ScheduledNotify = null;
                    FireAllNotification();
                }
            }
        }

        protected virtual void DoNotifies(DateTime dateTimeNow)
        {
            var fireNotifyOn = dateTimeNow;
            if (DelayEnabled)
            {
                fireNotifyOn = fireNotifyOn + Delay;
            }
            ScheduledNotify = fireNotifyOn;
        }

        protected void FireAllNotification()
        {
            if (Sound != null && !Muted) Sound.Notify();
            if (Popup != null) Popup.Notify();

            telemetry.TrackEvent($"Triggers: fired trigger {this.GetType().Name}");
        }

        protected abstract bool CheckCondition(LogEntry logMessage); 

        public virtual IEnumerable<INotifier> GetNotifiers()
        {
            var notifiers = new List<INotifier>();
            if (Sound != null) notifiers.Add(Sound);
            if (Popup != null) notifiers.Add(Popup);
            return notifiers;
        }

        public virtual void AddNotifier(INotifier notifier)
        {
            if (Sound == null)
            {
                if (notifier is ISoundNotifier)
                {
                    TriggerEntity.HasSound = true;
                    Sound = (SoundNotifier)notifier;
                    return;
                }
            }
            if (Popup == null)
            {
                if (notifier is IPopupNotifier)
                {
                    TriggerEntity.HasPopup = true;
                    Popup = (PopupNotifier)notifier;
                    return;
                }
            }
            throw new TriggerException(
                string.Format(
                    "could not add notifier of type {0}, either this type exists in trigger or is not supported",
                    notifier.GetType()));
        }

        public virtual void RemoveNotifier(INotifier notifier)
        {
            if (notifier is ISoundNotifier)
            {
                TriggerEntity.HasSound = false;
                Sound = null;
                return;
            }
            if (notifier is IPopupNotifier)
            {
                TriggerEntity.HasPopup = false;
                Popup = null;
                return;
            }
            throw new TriggerException("Removal of notifier failed, argument exact type: " + notifier.GetType());
        }

        public virtual string LogTypesAspect
        {
            get { return string.Join(", ", TriggerEntity.LogTypes); }
        }

        public virtual string ConditionAspect { get { return "Unknown"; } }

        public virtual string TypeAspect { get { return "Unknown"; } }

        public ThreeStateBool HasSoundAspect
        {
            get
            {
                if (Sound != null)
                {
                    if (Sound.HasEmptySound)
                    {
                        return ThreeStateBool.Error;
                    }
                    else
                    {
                        return ThreeStateBool.True;
                    }
                }
                return ThreeStateBool.False;
            }
        }

        public ThreeStateBool HasPopupAspect
        {
            get
            {
                if (Popup != null)
                {
                    return ThreeStateBool.True;
                }
                return ThreeStateBool.False;
            }
        }

        public virtual string CooldownRemainingAspect
        {
            get
            {
                if (!CooldownEnabled) return string.Empty;
                var cdRem = CooldownUntil - DateTime.Now;
                if (cdRem.Ticks > 0)
                    return cdRem.ToStringCompact();
                return string.Empty;
            }
        }

        protected LogType[] LockedLogTypes
        {
            set
            {
                foreach (var gameLogType in value)
                {
                    AddLogType(gameLogType);
                }
                LogTypesLocked = true;
            }
        }

        public bool LogTypesLocked { get; private set; }

        public virtual IEnumerable<ITriggerConfig> Configs
        {
            get
            {
                return new[] { new TriggerBaseConfig(this, WurmApi, Logger) };
            }
        }

        public Guid SoundId
        {
            get { return TriggerEntity.SoundId; }
            set { TriggerEntity.SoundId = value; }
        }

        public string PopupTitle
        {
            get { return TriggerEntity.PopupTitle; }
            set { TriggerEntity.PopupTitle = value; }
        }
        public string PopupContent
        {
            get { return TriggerEntity.PopupContent; }
            set { TriggerEntity.PopupContent = value; }
        }
        public bool StayUntilClicked
        {
            get { return TriggerEntity.StayUntilClicked; }
            set { TriggerEntity.StayUntilClicked = value; }
        }
        public int PopupDurationMillis
        {
            get { return TriggerEntity.PopupDurationMillis; }
            set { TriggerEntity.PopupDurationMillis = value; }
        }

        private EditTrigger _currentEditUi = null;
        public EditTrigger ShowAndGetEditUi(Form parent) 
        {
            try
            {
                if (_currentEditUi == null)
                {
                    _currentEditUi = new EditTrigger(this, SoundManager, TrayPopups);
                    _currentEditUi.ShowCenteredOnForm(parent);
                }
                else
                {
                    _currentEditUi.ShowAndBringToFront();
                }
                return _currentEditUi;
            }
            catch (ObjectDisposedException)
            {
                _currentEditUi = null;
                return ShowAndGetEditUi(parent);
            }
        }

        public TriggerEntity GetTriggerEntityCopy(ISerializer serializer)
        {
            return serializer.Deserialize<TriggerEntity>(serializer.Serialize(this.TriggerEntity));
        }

        public string GetDescription()
        {
            return TriggerEntity.GetDescription();
        }

        public bool ResetOnConditonHit
        {
            get { return TriggerEntity.ResetOnConditonHit; }
            set { TriggerEntity.ResetOnConditonHit = value; }
        }

        internal void SetLogType(LogType logType, System.Windows.Forms.CheckState checkState)
        {
            if (checkState == CheckState.Checked)
            {
                AddLogType(logType);
            }
            else
            {
                RemoveLogType(logType);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
