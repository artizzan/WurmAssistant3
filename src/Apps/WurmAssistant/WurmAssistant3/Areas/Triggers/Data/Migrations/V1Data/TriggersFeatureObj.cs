using System.Collections.Generic;
using AldursLab.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations.V1Data
{
    [KernelBind(BindingHint.Singleton), PersistentObject("TriggersFeature")]
    public class TriggersFeatureObj : PersistentObjectBase
    {
        [JsonProperty] public HashSet<string> activeCharacterNames;

        public TriggersFeatureObj()
        {
            activeCharacterNames = new HashSet<string>();
        }
    }
}
