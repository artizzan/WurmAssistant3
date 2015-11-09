using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Views;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.TriggersManager;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager
{
    public abstract class TriggerBase : TriggerAbstract, ITrigger
    {
        protected readonly TriggerData TriggerData;

        protected readonly ISoundEngine SoundEngine;
        protected readonly ITrayPopups TrayPopups;
        protected readonly IWurmApi WurmApi;
        protected readonly ILogger Logger;

        public TriggerBase([NotNull] TriggerData triggerData, [NotNull] ISoundEngine soundEngine, [NotNull] ITrayPopups trayPopups,
            [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger)
        {
            if (triggerData == null) throw new ArgumentNullException("triggerData");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.TriggerData = triggerData;
            this.SoundEngine = soundEngine;
            this.TrayPopups = trayPopups;
            this.WurmApi = wurmApi;
            this.Logger = logger;

            MuteChecker = () => false;
            Active = true;

            if (triggerData.HasSound)
            {
                Sound = new SoundNotifier(this, soundEngine);
            }
            if (triggerData.HasPopup)
            {
                Popup = new PopupNotifier(this, trayPopups);
            }
        }

        public Guid TriggerId { get { return TriggerData.TriggerId; } }

        public TriggerKind TriggerKind { get { return TriggerData.TriggerKind; } }

        public bool DelayEnabled
        {
            get { return TriggerData.DelayEnabled; }
            set
            {
                TriggerData.DelayEnabled = value;
            }
        }

        public TimeSpan Delay
        {
            get { return TriggerData.Delay; }
            set { TriggerData.Delay = value; }
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
            TriggerData.AddLogType(type);
        }

        public void RemoveLogType(LogType type)
        {
            TriggerData.RemoveLogType(type);
        }

        public virtual bool CheckLogType(LogType type)
        {
            return TriggerData.HasLogType(type);
        }

        public SoundNotifier Sound { get; private set; }

        public PopupNotifier Popup { get; private set; }

        public string Name
        {
            get { return TriggerData.Name; }
            set { TriggerData.Name = value; }
        }

        protected DateTime CooldownUntil
        {
            get { return TriggerData.CooldownUntil; }
            set { TriggerData.CooldownUntil = value; }
        }

        public bool CooldownEnabled
        {
            get { return TriggerData.CooldownEnabled; }
            set
            {
                TriggerData.CooldownEnabled = value;
                if (!value) CooldownUntil = DateTime.MinValue;
            }
        }

        public TimeSpan Cooldown
        {
            get { return TriggerData.Cooldown; }
            set { TriggerData.Cooldown = value; }
        }

        public bool Active
        {
            get { return TriggerData.Active; }
            set { TriggerData.Active = value; }
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
                    TriggerData.HasSound = true;
                    Sound = (SoundNotifier)notifier;
                    return;
                }
            }
            if (Popup == null)
            {
                if (notifier is IPopupNotifier)
                {
                    TriggerData.HasPopup = true;
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
                TriggerData.HasSound = false;
                Sound = null;
                return;
            }
            if (notifier is IPopupNotifier)
            {
                TriggerData.HasPopup = false;
                Popup = null;
                return;
            }
            throw new TriggerException("Removal of notifier failed, argument exact type: " + notifier.GetType());
        }

        public virtual string LogTypesAspect
        {
            get { return string.Join(", ", TriggerData.LogTypes); }
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
            get { return TriggerData.SoundId; }
            set { TriggerData.SoundId = value; }
        }

        public string PopupTitle
        {
            get { return TriggerData.PopupTitle; }
            set { TriggerData.PopupTitle = value; }
        }
        public string PopupContent
        {
            get { return TriggerData.PopupContent; }
            set { TriggerData.PopupContent = value; }
        }
        public bool StayUntilClicked
        {
            get { return TriggerData.StayUntilClicked; }
            set { TriggerData.StayUntilClicked = value; }
        }
        public int PopupDurationMillis
        {
            get { return TriggerData.PopupDurationMillis; }
            set { TriggerData.PopupDurationMillis = value; }
        }

        private EditTrigger _currentEditUi = null;
        public EditTrigger ShowAndGetEditUi(Form parent) 
        {
            try
            {
                if (_currentEditUi == null)
                {
                    _currentEditUi = new EditTrigger(this, SoundEngine, TrayPopups);
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

        public bool ResetOnConditonHit
        {
            get { return TriggerData.ResetOnConditonHit; }
            set { TriggerData.ResetOnConditonHit = value; }
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
    }
}
