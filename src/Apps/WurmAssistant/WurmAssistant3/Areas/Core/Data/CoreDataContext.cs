using System;
using System.ComponentModel;
using AldursLab.Persistence;
using AldursLab.WurmAssistant3.Areas.Persistence;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Data
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class CoreDataContext
    {
        public CoreDataContext([NotNull] IPersistentContextProvider persistentContextProvider)
        {
            if (persistentContextProvider == null) throw new ArgumentNullException(nameof(persistentContextProvider));
            
            var context = persistentContextProvider.GetPersistentContext("core");

            var defaultObjectSet = context.GetOrCreateObjectSet("default-object-set");
        }
    }
}
