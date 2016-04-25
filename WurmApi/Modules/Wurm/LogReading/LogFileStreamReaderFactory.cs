using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogReading
{
    class LogFileStreamReaderFactory
    {
        readonly IWurmApiConfig wurmApiConfig;

        public LogFileStreamReaderFactory([NotNull] IWurmApiConfig wurmApiConfig)
        {
            if (wurmApiConfig == null) throw new ArgumentNullException(nameof(wurmApiConfig));
            this.wurmApiConfig = wurmApiConfig;
        }

        public LogFileStreamReader Create(
            string fileFullPath,
            long startPosition = 0,
            bool trackFileBytePositions = false)
        {
            // why are there 2 stream readers?
            // wurm has a nasty habbit of adding /n in log lines on windows, 
            // this happens if a newline-separated text is pasted into chat input

            // update: well well, it seems wurms habbits have improved, replacing SR's with universal one!

            if (wurmApiConfig.Platform == Platform.Windows)
            {
                return new LogFileAnyLineEndingStreamReader(fileFullPath, startPosition, trackFileBytePositions);
            }
            if (trackFileBytePositions)
            {
                throw new NotSupportedException("trackFileBytePositions is not supported outside Windows platform");
            }
            return new LogFileAnyLineEndingStreamReader(fileFullPath, startPosition, false);
        }

        public LogFileStreamReader CreateWithLineCountFastForward(
            string fileFullPath,
            int lineCountToSkip)
        {
            LogFileStreamReader reader = null;
            try
            {
                reader = Create(fileFullPath, 0, false);
                reader.FastForwardLinesCount(lineCountToSkip);
                return reader;
            }
            catch (Exception)
            {
                reader?.Dispose();
                throw;
            }
        }
    }
}
