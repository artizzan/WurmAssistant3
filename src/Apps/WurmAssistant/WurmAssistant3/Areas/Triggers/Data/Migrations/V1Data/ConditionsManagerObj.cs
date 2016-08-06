using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Triggers.ActionQueueParsing;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations.V1Data
{
    [KernelBind(BindingHint.Singleton), PersistentObject("ActionQueueParsing_ConditionsManager")]
    public class ConditionsManagerObj : PersistentObjectBase
    {
        [JsonProperty] public Dictionary<Guid, Condition> allConditions;

        public ConditionsManagerObj()
        {
            allConditions = new Dictionary<Guid, Condition>();
        }
    }
}