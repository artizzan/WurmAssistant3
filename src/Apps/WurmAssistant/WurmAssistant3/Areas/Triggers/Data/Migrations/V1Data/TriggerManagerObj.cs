using AldursLab.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations.V1Data
{
    [KernelBind, PersistentObject("TriggersFeature_TriggerManager")]
    public class TriggerManagerObj : PersistentObjectBase
    {
        [JsonProperty] public bool muted = false;

        [JsonProperty] public byte[] TriggerListState = new byte[0];

        public TriggerManagerObj(string persistentObjectId) : base(persistentObjectId)
        {
        }
    }
}