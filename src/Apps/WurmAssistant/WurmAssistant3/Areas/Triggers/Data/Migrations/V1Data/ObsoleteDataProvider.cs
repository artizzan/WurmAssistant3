using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Persistence;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations.V1Data
{
    [KernelBind(BindingHint.Singleton)]
    class ObsoleteDataProvider
    {
        public ObsoleteDataProvider(IPersistentObjectResolver persistentObjectResolver)
        {
            Tfo = persistentObjectResolver.GetDefault<TriggersFeatureObj>();
            Tmos = Tfo.activeCharacterNames.ToDictionary(s => s, s => persistentObjectResolver.Get<TriggerManagerObj>(s));
            Atos = Tfo.activeCharacterNames.ToDictionary(s => s, s => persistentObjectResolver.Get<ActiveTriggersObj>(s));
            Cmo = persistentObjectResolver.GetDefault<ConditionsManagerObj>();
        }

        internal Dictionary<string, ActiveTriggersObj> Atos { get; }
        internal Dictionary<string, TriggerManagerObj> Tmos { get; }
        internal ConditionsManagerObj Cmo { get; }
        internal TriggersFeatureObj Tfo { get; }
    }
}
