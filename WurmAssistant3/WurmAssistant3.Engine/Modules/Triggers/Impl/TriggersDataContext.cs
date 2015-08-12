using System;
using AldursLab.Deprec.Core;

namespace AldursLab.WurmAssistant3.Engine.Modules.Triggers.Impl
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
