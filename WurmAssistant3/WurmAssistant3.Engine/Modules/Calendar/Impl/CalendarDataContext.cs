using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Engine.Repositories;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Calendar.Impl
{
    class CalendarDataContext
    {
        private readonly IPersistentManager persistentManager;

        public CalendarDataContext([NotNull] IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;
        }
    }
}
