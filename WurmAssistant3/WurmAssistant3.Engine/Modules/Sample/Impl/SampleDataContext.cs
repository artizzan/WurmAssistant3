using System;
using AldursLab.Deprec.Core;

namespace AldursLab.WurmAssistant3.Engine.Modules.Sample.Impl
{
    class SampleDataContext
    {
        private readonly IPersistentManager persistentManager;

        public SampleDataContext([NotNull] IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;
        }
    }
}
