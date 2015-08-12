using System;
using AldursLab.Deprec.Core;

namespace AldursLab.WurmAssistant3.Engine.Modules.Timers.Impl
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
