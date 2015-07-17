using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Engine.Repositories;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Timers.Impl
{
    class TimersDataContext
    {
        private readonly IPersistentManager persistentManager;

        public TimersDataContext([NotNull] IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;
        }
    }
}
