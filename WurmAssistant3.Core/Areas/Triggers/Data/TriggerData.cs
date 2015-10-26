using System;
using System.Collections.Generic;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TriggerData
    {
        [JsonProperty("triggerId")]
        public Guid TriggerId { get; private set; }

        [JsonProperty("triggerKind")]
        public TriggerKind TriggerKind { get; private set; }

        public event EventHandler<EventArgs> DataChanged;

        public TriggerData(Guid triggerId, TriggerKind triggerKind)
        {
            TriggerId = triggerId;
            TriggerKind = triggerKind;

            condition = string.Empty;
            notificationDelay = 1.0D;
            Condition = string.Empty;
            logTypes = new HashSet<LogType>();
        }

        private void OnDataChanged()
        {
            var handler = DataChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        [JsonProperty]
        Guid soundId;
        public Guid SoundId
        {
            get { return soundId; }
            set
            {
                soundId = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        string popupTitle;
        public string PopupTitle
        {
            get { return popupTitle; }
            set
            {
                popupTitle = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        string popupContent;
        public string PopupContent
        {
            get { return popupContent; }
            set
            {
                popupContent = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        int popupDurationMillis;
        public int PopupDurationMillis
        {
            get { return popupDurationMillis; }
            set
            {
                popupDurationMillis = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        TimeSpan cooldown;
        public TimeSpan Cooldown
        {
            get { return cooldown; }
            set
            {
                cooldown = value;
                OnDataChanged();
            }
        }
        [JsonProperty]
        DateTime cooldownUntil;
        public DateTime CooldownUntil
        {
            get { return cooldownUntil; }
            set
            {
                cooldownUntil = value;
                OnDataChanged();
            }
        }
        [JsonProperty]
        bool cooldownEnabled;
        public bool CooldownEnabled
        {
            get { return cooldownEnabled; }
            set
            {
                cooldownEnabled = value;
                OnDataChanged();
            }
        }
        [JsonProperty]
        bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                OnDataChanged();
            }
        }
        [JsonProperty]
        HashSet<LogType> logTypes;

        public IEnumerable<LogType> LogTypes
        {
            get { return logTypes; }
            set
            {
                logTypes = new HashSet<LogType>(value);
                OnDataChanged();
            }
        }
        
        public void AddLogType(LogType logType)
        {
            logTypes.Add(logType); OnDataChanged();
        }

        public void RemoveLogType(LogType logType)
        {
            logTypes.Remove(logType);
            OnDataChanged();
        }

        public bool HasLogType(LogType logType)
        {
            return logTypes.Contains(logType);
        }

        [JsonProperty]
        bool resetOnConditonHit;
        public bool ResetOnConditonHit
        {
            get { return resetOnConditonHit; }
            set
            {
                resetOnConditonHit = value;
                OnDataChanged();
            }
        }
        [JsonProperty]
        bool delayEnabled;
        public bool DelayEnabled
        {
            get { return delayEnabled; }
            set
            {
                delayEnabled = value;
                OnDataChanged();
            }
        }
        [JsonProperty]
        TimeSpan delay;
        public TimeSpan Delay
        {
            get { return delay; }
            set
            {
                delay = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        string condition;
        public string Condition
        {
            get { return condition; }
            set
            {
                condition = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        double notificationDelay;
        public double NotificationDelay
        {
            get { return notificationDelay; }
            set
            {
                notificationDelay = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        bool stayUntilClicked;
        public bool StayUntilClicked
        {
            get { return stayUntilClicked; }
            set
            {
                stayUntilClicked = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        bool hasSound;
        public bool HasSound
        {
            get { return hasSound; }
            set
            {
                hasSound = value;
                OnDataChanged();
            }
        }

        [JsonProperty]
        bool hasPopup;
        public bool HasPopup
        {
            get { return hasPopup; }
            set
            {
                hasPopup = value;
                OnDataChanged();
            }
        }
    }
}