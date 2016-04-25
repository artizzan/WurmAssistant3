using System;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    class SingleFileMonitorFactory
    {
        private readonly LogFileStreamReaderFactory streamReaderFactory;
        private readonly LogFileParser logFileParser;

        public SingleFileMonitorFactory(
            LogFileStreamReaderFactory streamReaderFactory,
            LogFileParser logFileParser)
        {
            if (streamReaderFactory == null) throw new ArgumentNullException(nameof(streamReaderFactory));
            if (logFileParser == null) throw new ArgumentNullException(nameof(logFileParser));
            this.streamReaderFactory = streamReaderFactory;
            this.logFileParser = logFileParser;
        }

        public virtual SingleFileMonitor Create(LogFileInfo logFileInfo)
        {
            return new SingleFileMonitor(logFileInfo, streamReaderFactory, logFileParser);
        }

        public virtual SingleFileMonitor Create(LogFileInfo logFileInfo, long beginReaderOffsetBytes)
        {
            var monitor = new SingleFileMonitor(logFileInfo, streamReaderFactory, logFileParser);
            monitor.OverrideCurrentPosition(beginReaderOffsetBytes);
            return monitor;
        }
    }
}