using System;
using AldursLab.WurmAssistant3.Areas.Main.Data.Model;
using AldursLab.WurmAssistant3.Areas.Persistence.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main.Data
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class MainDataContext
    {
        public MainDataContext([NotNull] IPersistentContextProvider persistentContextProvider)
        {
            if (persistentContextProvider == null) throw new ArgumentNullException(nameof(persistentContextProvider));
            
            var context = persistentContextProvider.GetPersistentContext("main");
            var defaultObjectSet = context.GetOrCreateObjectSet("DefaultObjectSet");

            MainWindowSettings = defaultObjectSet.GetOrCreate<MainWindowSettings>("MainWindowSettings");
        }

        public MainWindowSettings MainWindowSettings { get; }
    }
}
