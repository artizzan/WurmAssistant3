using System;
using System.IO;
using AldursLab.WurmAssistant.Shared;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    [KernelHint(BindingHint.Singleton)]
    class WaVersion : IWaVersion
    {
        readonly IBinDirectory binDirectory;
        readonly ILogger logger;

        public WaVersion([NotNull] IBinDirectory binDirectory, [NotNull] ILogger logger)
        {
            if (binDirectory == null) throw new ArgumentNullException("binDirectory");
            if (logger == null) throw new ArgumentNullException("logger");
            this.binDirectory = binDirectory;
            this.logger = logger;

            try
            {
                var filePath = Path.Combine(binDirectory.FullPath, "version.dat");
                if (!File.Exists(filePath))
                {
                    logger.Warn($"version.dat does not exist at {filePath}. Is this development build?");
                }
                else
                {
                    var fileContent = File.ReadAllText(filePath);
                    var version = Wa3VersionInfo.CreateFromVersionDat(fileContent);
                    logger.Info("Parsed WA version: " + version);
                    VersionInfo = version;
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error at parsing WA version information");
            }
        }

        [CanBeNull]
        public Wa3VersionInfo VersionInfo { get; private set; }

        public string AsString()
        {
            if (VersionInfo == null) return string.Empty;
            else
            {
                return string.Format("{0} {1} R{2}",
                    VersionInfo.MinorVersion,
                    VersionInfo.BuildCode,
                    VersionInfo.BuildNumber);
            }
        }

        public bool Known { get { return VersionInfo != null; } }
    }
}
