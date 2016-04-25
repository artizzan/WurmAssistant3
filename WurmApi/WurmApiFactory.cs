using System.IO;
using AldursLab.WurmApi.Extensions.DotNet.Reflection;
using AldursLab.WurmApi.Modules.Wurm.InstallDirectory;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Creates new instances of WurmApi systems.
    /// </summary>
    public static class WurmApiFactory
    {
        /// <summary>
        /// Creates new instance of WurmApiManager.
        /// All WurmApi services are thread safe and independent of execution context (i.e. synchronization context, async handling).
        /// Always call Dispose on the Manager, before closing app or dropping the instance. This will ensure proper cleanup and internal cache consistency.
        /// Not calling Dispose without terminating hosting process, may result in resource leaks.
        /// </summary>
        /// <param name="creationOptions">
        /// Optional configurable parameters of the WurmApi.
        /// </param>
        public static IWurmApi Create(WurmApiCreationOptions creationOptions = null)
        {
            if (creationOptions == null)
            {
                creationOptions = new WurmApiCreationOptions();
            }

            if (creationOptions.DataDirPath == null)
            {
                creationOptions.DataDirPath = "WurmApi";
            }
            if (!Path.IsPathRooted(creationOptions.DataDirPath))
            {
                var codebase = typeof(WurmApiFactory).Assembly.GetAssemblyDllDirectoryAbsolutePath();
                creationOptions.DataDirPath = Path.Combine(codebase, creationOptions.DataDirPath);
            }
            if (creationOptions.WurmApiLogger == null)
            {
                creationOptions.WurmApiLogger = new WurmApiLoggerStub();
            }
            if (creationOptions.WurmClientInstallDirectory == null)
            {
                creationOptions.WurmClientInstallDirectory = WurmClientInstallDirectory.AutoDetect();
            }
            if (creationOptions.WurmApiConfig == null)
            {
                creationOptions.WurmApiConfig = new WurmApiConfig();
            }
            return new WurmApiManager(new WurmApiDataDirectory(creationOptions.DataDirPath, true),
                creationOptions.WurmClientInstallDirectory,
                creationOptions.WurmApiLogger,
                creationOptions.WurmApiEventMarshaller,
                creationOptions.WurmApiConfig);
        }
    }

    public class WurmApiCreationOptions
    {
        /// <summary>
        /// Directory path, where this instance of WurmApiManager can store data caches. Defaults to \WurmApi in the library DLL location. 
        /// If relative path is provided, it will also be in relation to DLL location.
        /// </summary>
        public string DataDirPath { get; set; }
        /// <summary>
        /// An optional implementation of ILogger, where all WurmApi errors and warnings can be forwarded. 
        /// Defaults to no logging. 
        /// Providing this parameter is highly is recommended.
        /// </summary>
        public IWurmApiLogger WurmApiLogger { get; set; }
        /// <summary>
        /// An optional event marshaller, used to marshal all events in a specific way (for example to GUI thread).
        /// Defaults to running events on ThreadPool threads.
        /// Providing this parameter will greatly simplify usage in any application, that has synchronization context.
        /// </summary>
        public IWurmApiEventMarshaller WurmApiEventMarshaller { get; set; }
        /// <summary>
        /// An optional Wurm Game Client directory path provider.
        /// Defaults to autodetection.
        /// </summary>
        public IWurmClientInstallDirectory WurmClientInstallDirectory { get; set; }
        /// <summary>
        /// Optional extra configuration options.
        /// </summary>
        public WurmApiConfig WurmApiConfig { get; set; }
    }

    public enum WurmApiPersistenceMethod
    {
        FlatFiles = 0,
        SqLite
    }
}