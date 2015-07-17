using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Engine.Repositories;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Triggers.Impl
{
    class TriggersDataContext
    {
        private readonly IPersistentManager persistentManager;

        public TriggersDataContext([NotNull] IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;
        }
    }
}
