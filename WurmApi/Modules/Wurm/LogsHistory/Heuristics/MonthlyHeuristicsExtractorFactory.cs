using System;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    class MonthlyHeuristicsExtractorFactory
    {
        readonly LogFileStreamReaderFactory logFileStreamReaderFactory;
        readonly IWurmApiLogger logger;
        readonly IWurmApiConfig wurmApiConfig;

        public MonthlyHeuristicsExtractorFactory(
            LogFileStreamReaderFactory logFileStreamReaderFactory,
            IWurmApiLogger logger, [NotNull] IWurmApiConfig wurmApiConfig)
        {
            if (logFileStreamReaderFactory == null) throw new ArgumentNullException(nameof(logFileStreamReaderFactory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmApiConfig == null) throw new ArgumentNullException(nameof(wurmApiConfig));
            this.logFileStreamReaderFactory = logFileStreamReaderFactory;
            this.logger = logger;
            this.wurmApiConfig = wurmApiConfig;
        }

        public MonthlyHeuristicsExtractor Create(LogFileInfo logFileInfo)
        {
            return new MonthlyHeuristicsExtractor(logFileInfo, logFileStreamReaderFactory, logger, wurmApiConfig);
        }
    }
}
