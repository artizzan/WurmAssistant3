using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations.V1Data
{
    [KernelBind, PersistentObject("TriggersFeature_ActiveTriggers")]
    public class ActiveTriggersObj : PersistentObjectBase
    {
        [JsonProperty] public Dictionary<Guid, TriggerEntity> triggerDatas = new Dictionary<Guid, TriggerEntity>();

        public ActiveTriggersObj(string persistentObjectId) : base(persistentObjectId)
        {
        }
    }
}