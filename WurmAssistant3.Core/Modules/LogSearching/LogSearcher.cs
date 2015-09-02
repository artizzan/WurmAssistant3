using System;
using AldursLab.WurmAssistant3.Core.Infrastructure.Modules;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Modules.LogSearching
{
    public class LogSearcher : IModule
    {
        readonly IModuleGui logSearcherGui;

        public LogSearcher([NotNull] ILogSearcherModuleGui logSearcherGui)
        {
            if (logSearcherGui == null) throw new ArgumentNullException("logSearcherGui");
            this.logSearcherGui = logSearcherGui;
        }

        public void ShowGui()
        {
            logSearcherGui.Display();
        }

        public string Name { get { return "Log Searcher"; } }
    }

    public interface ILogSearcherModuleGui : IModuleGui
    {
    }
}
