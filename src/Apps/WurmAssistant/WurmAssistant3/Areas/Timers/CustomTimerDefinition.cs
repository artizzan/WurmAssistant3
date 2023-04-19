using System;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Timers
{
    [JsonObject(MemberSerialization.OptIn)] //set of options used to init custom timers behavior
    public class CustomTimerDefinition
    {
        [JsonObject(MemberSerialization.OptIn)]
        public struct Condition
        {
            [JsonProperty]
            public string RegexPattern;
            [JsonProperty]
            public LogType LogType;
        }

        [JsonProperty(IsReference = false)]
        public Condition[] TriggerConditions;
        [JsonProperty(IsReference = false)]
        public Condition[] ResetConditions;
        [JsonProperty]
        public bool ResetOnUptime;
        [JsonProperty]
        public bool ShowElapsedTime;
        [JsonProperty]
        public TimeSpan Duration;
        [JsonProperty]
        public bool IsRegex { get; private set; }

        Condition ConditionFactory(string pattern, LogType logtype, bool isRegex)
        {
            if (pattern == null)
                pattern = "";
            pattern = pattern.Trim();
            if (!isRegex)
                pattern = Regex.Escape(pattern);
            else
                IsRegex = true;

            Condition cond = new Condition();
            cond.RegexPattern = pattern;
            cond.LogType = logtype;
            return cond;
        }

        public void AddTrigger(string pattern, LogType logtype, bool isRegex)
        {
            var cond = ConditionFactory(pattern, logtype, isRegex);
            if (TriggerConditions != null)
            {
                var x = TriggerConditions.ToList();
                x.Add(cond);
                TriggerConditions = x.ToArray();
            }
            else
                TriggerConditions = new Condition[] { cond };
        }

        public void AddReset(string pattern, LogType logtype, bool isRegex)
        {
            var cond = ConditionFactory(pattern, logtype, isRegex);
            if (ResetConditions != null)
            {
                var x = ResetConditions.ToList();
                x.Add(ConditionFactory(pattern, logtype, isRegex));
                ResetConditions = x.ToArray();
            }
            else
                ResetConditions = new Condition[] { cond };
        }

        public void ClearTriggers()
        {
            TriggerConditions = new Condition[0];
        }
    }
}