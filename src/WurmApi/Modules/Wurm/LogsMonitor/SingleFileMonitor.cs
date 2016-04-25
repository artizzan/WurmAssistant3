using System;
using System.Collections.Generic;
using System.IO;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    class SingleFileMonitor
    {
        private readonly LogFileInfo logFileInfo;
        private readonly LogFileStreamReaderFactory streamReaderFactory;
        private readonly LogFileParser logFileParser;

        private long lastFileSize = 0;
        private readonly FileInfo fileInfo;

        public SingleFileMonitor(
            LogFileInfo logFileInfo,
            LogFileStreamReaderFactory streamReaderFactory,
            LogFileParser logFileParser)
        {
            if (logFileInfo == null) throw new ArgumentNullException(nameof(logFileInfo));
            if (streamReaderFactory == null) throw new ArgumentNullException(nameof(streamReaderFactory));
            if (logFileParser == null) throw new ArgumentNullException(nameof(logFileParser));
            this.logFileInfo = logFileInfo;
            this.streamReaderFactory = streamReaderFactory;
            this.logFileParser = logFileParser;

            fileInfo = new FileInfo(logFileInfo.FullPath);
            lastFileSize = fileInfo.Length;
        }

        public LogFileInfo LogFileInfo => logFileInfo;

        public ICollection<LogEntry> GetNewEvents()
        {
            List<string> logLines = ReadNewLogLines();
            var parsedEntries = logFileParser.ParseLinesFromLogsScan(logLines, Time.Get.LocalNow);
            return parsedEntries;
        }

        private List<string> ReadNewLogLines()
        {
            List<string> rawLogLines = new List<string>();

            fileInfo.Refresh();
            if (fileInfo.Length > lastFileSize)
            {
                using (var reader = streamReaderFactory.Create(logFileInfo.FullPath))
                {
                    reader.Seek(lastFileSize);
                    string line;
                    while ((line = reader.TryReadNextLine()) != null)
                    {
                        rawLogLines.Add(line);
                    }
                    lastFileSize = reader.StreamPosition;
                }
            }
            return rawLogLines;
        }

        public void OverrideCurrentPosition(long beginReaderOffset)
        {
            lastFileSize = beginReaderOffset;
        }
    }
}
