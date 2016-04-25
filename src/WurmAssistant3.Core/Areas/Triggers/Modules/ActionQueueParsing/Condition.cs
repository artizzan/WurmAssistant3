using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Contracts.ActionQueueParsing;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.ActionQueueParsing
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Condition : IActionQueueParsingCondition
    {
        public Condition()
        {
            ConditionId = Guid.NewGuid();
            Default = false;
        }

        public Condition(Guid conditionId, bool @default)
        {
            ConditionId = conditionId;
            Default = @default;
            Pattern = string.Empty;
        }

        [JsonProperty("conditionId")]
        public Guid ConditionId { get; private set; }
        [JsonProperty("default")]
        public bool Default { get; private set; }
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }
        [JsonProperty("contents")]
        public string Pattern { get; set; }
        [JsonProperty("conditionKind")]
        public ConditionKind ConditionKind { get; set; }
        [JsonProperty("matchingKind")]
        public MatchingKind MatchingKind { get; set; }

        public Condition CreateCopy()
        {
            return (Condition)this.MemberwiseClone();
        }
    }
}
