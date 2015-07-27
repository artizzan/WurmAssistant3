using System;
using System.IO;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Wurm.LogFiles
{
    public class LogFileInfoFactory
    {
        private readonly ParsingHelper parsingHelper = new ParsingHelper();
        private readonly IWurmLogDefinitions wurmLogDefinitions;
        private readonly ILogger logger;

        public LogFileInfoFactory(IWurmLogDefinitions wurmLogDefinitions, ILogger logger)
        {
            if (wurmLogDefinitions == null) throw new ArgumentNullException("wurmLogDefinitions");
            if (logger == null) throw new ArgumentNullException("logger");
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

            var dateFromLogFileName = parsingHelper.TryGetDateFromLogFileName(fileInfo.Name);
            var typeFromLogFileName = wurmLogDefinitions.TryGetTypeFromLogFileName(fileInfo.Name);
            string pmRecipient = null;
            if (typeFromLogFileName == LogType.Pm)
            {
                pmRecipient = parsingHelper.TryParsePmRecipientFromFileName(fileInfo.Name);
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