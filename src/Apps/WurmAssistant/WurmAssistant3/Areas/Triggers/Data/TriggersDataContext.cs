using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Persistence;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class TriggersDataContext
    {
        public TriggersDataContext([NotNull] IPersistentContextProvider persistentContextProvider)
        {
            if (persistentContextProvider == null) throw new ArgumentNullException(nameof(persistentContextProvider));

            var context = persistentContextProvider.GetPersistentContext("triggers");
            var defaultObjectSet = context.GetOrCreateObjectSet("default-object-set");
            
            ActionQueueTriggerConfig = defaultObjectSet.GetOrCreate<ActionQueueTriggerConfig>("action-queue-trigger-config");
            TriggersConfig = defaultObjectSet.GetOrCreate<TriggersConfig>("triggers-config");
            CharacterTriggersConfigs = context.GetOrCreateObjectSet<CharacterTriggersConfig>("character-triggers-config");
        }

        public ActionQueueTriggerConfig ActionQueueTriggerConfig { get; private set; }
        public TriggersConfig TriggersConfig { get; private set; }
        public IObjectSet<CharacterTriggersConfig> CharacterTriggersConfigs { get; private set; }
    }
}
