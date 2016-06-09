using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.Plugins
{
    class PluginInfo
    {
        readonly DirectoryInfo pluginDirectory;

        public PluginInfo([NotNull] DirectoryInfo pluginDirectory)
        {
            if (pluginDirectory == null) throw new ArgumentNullException(nameof(pluginDirectory));
            this.pluginDirectory = pluginDirectory;
        }

        public FileInfo TryGetDepdendency(string assemblyName)
        {
            return pluginDirectory.GetFiles(assemblyName).FirstOrDefault();
        }

        public IEnumerable<FileInfo> GetAllAssemblies()
        {
            return pluginDirectory.GetFiles("*.dll");
        }
    }
}