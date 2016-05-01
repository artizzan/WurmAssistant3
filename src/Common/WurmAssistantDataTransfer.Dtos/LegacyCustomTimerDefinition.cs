using System;
using System.Collections.Generic;

namespace WurmAssistantDataTransfer.Dtos
{
    public class LegacyCustomTimerDefinition
    {
        public class Condition
        {
            public string Pattern { get; set; }
            public string LogType { get; set; }
        }

        public LegacyCustomTimerDefinition()
        {
            TriggerConditions = new List<Condition>();
            ResetConditions = new List<Condition>();
        }

        public Guid? Id { get; set; }

        public string Name { get; set; }
        public List<Condition> TriggerConditions { get; set; }
        public List<Condition> ResetConditions { get; set; }
        public bool ResetOnUptime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsRegex { get; set; }

        /// <summary>
        /// For legacy (WA2) support only, in WA2 definition was identified by Name and ServerGroup.
        /// WA3 definions are always identified by GUIDs.
        /// </summary>
        public string LegacyServerGroupId { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}", Id, Name);
        }
    }
}