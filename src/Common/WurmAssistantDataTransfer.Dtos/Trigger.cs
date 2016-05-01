using System;
using System.Collections.Generic;

namespace WurmAssistantDataTransfer.Dtos
{
    public class Trigger
    {
        public Trigger()
        {
            LogTypes = new List<string>();
        }

        public Guid? TriggerId { get; set; }

        public string CharacterName { get; set; }

        public string Name { get; set; }
        public string TriggerKind { get; set; }
        public string PopupTitle { get; set; }
        public string PopupContent { get; set; }
        public int? PopupDurationMillis { get; set; }
        public TimeSpan Cooldown { get; set; }
        public DateTime? CooldownUntil { get; set; }
        public bool CooldownEnabled { get; set; }
        public bool Active { get; set; }
        public List<string> LogTypes { get; set; }
        public bool ResetOnConditonHit { get; set; }
        public bool? DelayEnabled { get; set; }
        public TimeSpan Delay { get; set; }
        public string Condition { get; set; }
        public double? NotificationDelay { get; set; }
        public bool? StayUntilClicked { get; set; }
        public bool HasSound { get; set; }
        public bool HasPopup { get; set; }

        public Sound Sound { get; set; }
    }
}