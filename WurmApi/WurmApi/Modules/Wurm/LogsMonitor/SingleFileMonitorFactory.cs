using System;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    public class SingleFileMonitorFactory
    {
        private readonly LogFileStreamReaderFactory streamReaderFactory;
        private readonly LogFileParser logFileParser;

        public SingleFileMonitorFactory(
            LogFileStreamReaderFactory streamReaderFactory,
            LogFileParser logFileParser)
        {
            if (streamReaderFactory == null) throw new ArgumentNullException("streamReaderFactory");
            if (logFileParser == null) throw new ArgumentNullException("logFileParser");
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