using System;
using System.IO;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Wurm.LogFiles
{
    class LogFileInfoFactory
    {
        private readonly IWurmLogDefinitions wurmLogDefinitions;
        private readonly IWurmApiLogger logger;

        public LogFileInfoFactory(IWurmLogDefinitions wurmLogDefinitions, IWurmApiLogger logger)
        {
            if (wurmLogDefinitions == null) throw new ArgumentNullException(nameof(wurmLogDefinitions));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.wurmLogDefinitions = wurmLogDefinitions;
            this.logger = logger;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public LogFileInfo Create(FileInfo fileInfo)
        {
            // should never throw

            var dateFromLogFileName = ParsingHelper.TryGetDateFromLogFileName(fileInfo.Name);
            var typeFromLogFileName = wurmLogDefinitions.TryGetTypeFromLogFileName(fileInfo.Name);
            string pmRecipient = null;
            if (typeFromLogFileName == LogType.Pm)
            {
                pmRecipient = ParsingHelper.TryParsePmRecipientFromFileName(fileInfo.Name);
            }

            bool parsingError = false;

            if (dateFromLogFileName.LogSavingType == LogSavingType.Unknown
                || dateFromLogFileName.DateTime == DateTime.MinValue)
            {
                parsingError = true;
                logger.Log(LogLevel.Info, "Detected issues with parsing log date for file " + fileInfo.FullName, this, null);
            }

            if (typeFromLogFileName == LogType.Unspecified)
            {
                parsingError = true;
                logger.Log(LogLevel.Info, "Detected issues with parsing log type for file " + fileInfo.FullName, this, null);
            }

            return new LogFileInfo(
                fileInfo.FullName,
                fileInfo.Name,
                dateFromLogFileName,
                typeFromLogFileName,
                parsingError,
                pmRecipient);
        }
    }
}