using System;
using AldurSoft.WurmApi.Modules.Wurm.InstallDirectory;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi
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
        /// <param name="dataDirAbsolutePath">
        /// Absolute directory path, where this instance of WurmApiManager can store data caches.
        /// </param>
        /// <param name="logger">
        /// An optional implementation of ILogger, where all WurmApi errors and warnings can be forwarded. 
        /// Defaults to no logging. 
        /// Providing this parameter is highly is recommended.
        /// </param>
        /// <param name="eventMarshaller">
        /// An optional event marshaller, used to marshal all events in a specific way (for example to GUI thread).
        /// Defaults to running events on ThreadPool threads.
        /// Providing this parameter will greatly simplify usage in any application, that has synchronization context.
        /// </param>
        /// <param name="installDirectory">
        /// An optional Wurm Game Client directory path provider.
        /// Defaults to autodetection.
        /// </param>
        /// <returns></returns>
        public static WurmApiManager Create([NotNull] string dataDirAbsolutePath, ILogger logger = null,
            IEventMarshaller eventMarshaller = null, IWurmInstallDirectory installDirectory = null)
        {
            if (dataDirAbsolutePath == null) throw new ArgumentNullException("dataDirAbsolutePath");
            if (logger == null)
            {
                logger = new LoggerStub();
            }
            if (installDirectory == null)
            {
                installDirectory = new WurmInstallDirectory();
            }
            return new WurmApiManager(new WurmApiDataDirectory(dataDirAbsolutePath, true),
                installDirectory,
                logger,
                eventMarshaller);
        }
    }
}