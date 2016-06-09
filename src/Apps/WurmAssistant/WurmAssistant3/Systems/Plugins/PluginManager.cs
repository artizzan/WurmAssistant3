using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet.IO;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.Plugins
{
    class PluginManager : IDisposable
    {
        DirectoryInfo pluginsDirectory;
        List<PluginInfo> plugins;
        readonly List<Assembly> pluginAssemblies = new List<Assembly>();

        public PluginManager([NotNull] DirectoryInfo pluginsRootDirectory)
        {
            if (pluginsRootDirectory == null) throw new ArgumentNullException(nameof(pluginsRootDirectory));
            pluginsDirectory = pluginsRootDirectory;

            if (!pluginsDirectory.Exists)
            {
                pluginsDirectory.Create();
            }

            EnablePlugins();
        }

        public List<Assembly> PluginAssemblies => pluginAssemblies;

        void EnablePlugins()
        {
            LookupAllPlugins();
            EnableAlternativeAssemblyResolvers();
            LoadAllPluginAssemblies();
        }

        void LookupAllPlugins()
        {
            plugins = pluginsDirectory.GetDirectories()
                                      .Select(info => new PluginInfo(info))
                                      .ToList();
        }

        void EnableAlternativeAssemblyResolvers()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        void LoadAllPluginAssemblies()
        {
            foreach (var plugin in plugins)
            {
                plugin.GetAllAssemblies().ToList().ForEach(info =>
                {
                    pluginAssemblies.Add(Assembly.LoadFile(info.FullName));
                });
            }

        }

        public void Dispose()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
        }

        Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";
            // try plugin directories
            foreach (var plugin in plugins)
            {
                var file = plugin.TryGetDepdendency(assemblyName);
                if (file != null)
                {
                    return Assembly.LoadFrom(file.FullName);
                }
            }

            return null;
        }
    }
}
