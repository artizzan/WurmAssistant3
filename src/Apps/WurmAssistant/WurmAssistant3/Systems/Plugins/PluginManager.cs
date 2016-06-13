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
        readonly DirectoryInfo pluginsDirectory;
        List<PluginInfo> plugins;
        readonly List<Assembly> pluginAssemblies = new List<Assembly>();
        readonly List<DllLoadError> dllLoadErrors = new List<DllLoadError>();

        public PluginManager([NotNull] DirectoryInfo pluginsRootDirectory)
        {
            if (pluginsRootDirectory == null) throw new ArgumentNullException(nameof(pluginsRootDirectory));
            pluginsDirectory = pluginsRootDirectory;

            if (!pluginsDirectory.Exists)
            {
                pluginsDirectory.Create();
            }
        }

        public IEnumerable<Assembly> PluginAssemblies => pluginAssemblies;
        public IEnumerable<DllLoadError> DllLoadErrors => dllLoadErrors;

        public void EnablePlugins()
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
                    try
                    {
                        pluginAssemblies.Add(Assembly.LoadFile(info.FullName));
                    }
                    catch (Exception exception)
                    {
                        dllLoadErrors.Add(new DllLoadError(exception, info.FullName));
                    }
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

    public class DllLoadError
    {
        public Exception Exception { get; }
        public string DllFileName { get; }

        public DllLoadError([NotNull] Exception exception, [NotNull] string dllFileName)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (dllFileName == null) throw new ArgumentNullException(nameof(dllFileName));
            Exception = exception;
            DllFileName = dllFileName;
        }
    }
}
