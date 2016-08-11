using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AldursLab.WurmApi;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TriggerEntity : PropertyChangedBase
    {
        public TriggerEntity(Guid triggerId, TriggerKind triggerKind)
        {
            TriggerId = triggerId;
            TriggerKind = triggerKind;

            condition = string.Empty;
            source = string.Empty;
            notificationDelay = 1.0D;
            Condition = string.Empty;
            logTypes = new HashSet<LogType>();
        }

        [JsonProperty("triggerId")]
        public Guid TriggerId
        {
            get { return triggerId; }
            set
            {
                if (value.Equals(triggerId)) return;
                triggerId = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty("triggerKind")]
        public TriggerKind TriggerKind
        {
            get { return triggerKind; }
            set
            {
                if (value == triggerKind) return;
                triggerKind = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty]
        Guid soundId;
        public Guid SoundId
        {
            get { return soundId; }
            set
            {
                soundId = value;
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty]
        int popupDurationMillis = 3000;
        public int PopupDurationMillis
        {
            get { return popupDurationMillis; }
            set
            {
                popupDurationMillis = value;
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
            }
        }
        
        public void AddLogType(LogType logType)
        {
            logTypes.Add(logType);
            NotifyOfPropertyChange();
        }

        public void RemoveLogType(LogType logType)
        {
            logTypes.Remove(logType);
            NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty]
        bool matchEveryLine;
        public bool MatchEveryLine
        {
            get { return matchEveryLine; }
            set
            {
                matchEveryLine = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty] 
        string source;
        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty]
        double skillTriggerTreshhold;
        public double SkillTriggerTreshhold
        {
            get { return skillTriggerTreshhold; }
            set
            {
                skillTriggerTreshhold = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty]
        string skillTriggerSkillName = string.Empty;

        Guid triggerId;
        TriggerKind triggerKind;

        public string SkillTriggerSkillName
        {
            get { return skillTriggerSkillName; }
            set
            {
                skillTriggerSkillName = value;
                NotifyOfPropertyChange();
            }
        }

        public string GetDescription()
        {
            return $"{TriggerKind} trigger named {Name}";
        }
    }
}