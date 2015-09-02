using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Infrastructure.Modules
{
    public class ModuleManager
    {
        readonly IEnumerable<IModule> modules;

        public ModuleManager([NotNull] IEnumerable<IModule> modules)
        {
            if (modules == null) throw new ArgumentNullException("modules");
            this.modules = modules;
        }

        public IEnumerable<IModule> Modules
        {
            get { return modules; }
        }
    }
}
