using System;
using AldursLab.Deprec.Core;

namespace AldursLab.WurmAssistant3.Engine.Modules.Granger.Impl
{
    class GrangerDataContext
    {
        private readonly IPersistentManager persistentManager;

        public GrangerDataContext([NotNull] IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;
        }
    }
}
