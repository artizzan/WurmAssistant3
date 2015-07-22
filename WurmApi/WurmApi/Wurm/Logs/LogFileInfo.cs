using System;

namespace AldurSoft.WurmApi.Wurm.Logs
{
    public class LogFileInfo : IComparable<LogFileInfo>, IComparable
    {
        private readonly LogType logType;
        private readonly string pmRecipientNormalized;
        /// <summary>
        /// Full path to the file with extension.
        /// </summary>
        public string FullPath { get; private set; }
        /// <summary>
        /// Log file name with extension.
        /// </summary>
        public string FileName { get; private set; }
        public string FileNameNormalized { get; private set; }
        public LogFileDate LogFileDate { get; private set; }

        /// <summary>
        /// There was an error when building this LogFileInfo.
        /// </summary>
        public bool ParsingError { get; private set; }

        public LogFileInfo(string fullPath, string fileName, LogFileDate logFileDate, LogType logType, bool parsingError, string pmRecipient)
        {
            this.logType = logType;
            if (pmRecipient != null) // can be null
            {
                // normalize name
                pmRecipient = pmRecipient.ToUpperInvariant();
            }
            this.pmRecipientNormalized = pmRecipient; 
            if (fullPath == null) throw new ArgumentNullException("fullPath");
            if (fileName == null) throw new ArgumentNullException("fileName");
            LogFileDate = logFileDate;
            ParsingError = parsingError;
            FullPath = fullPath;
            FileName = fileName;
            FileNameNormalized = FileName.ToUpperInvariant();
        }

        public LogType LogType
        {
            get { return logType; }
        }

        public string PmRecipientNormalized { get { return pmRecipientNormalized; } }

        public int CompareTo(LogFileInfo other)
        {
            return this.LogFileDate.DateTime.CompareTo(other.LogFileDate.DateTime);
        }

        public int CompareTo(object obj)
        {
            LogFileInfo info = obj as LogFileInfo;
            if (info == null) throw new InvalidOperationException("Other must be LogFileInfo");
            return info.CompareTo(this);
        }

        public override string ToString()
        {
            return string.Format(
                "[LogFileInfo; LogFileDate: {1}, ParsingError: {2}, LogType: {3}, FullPath: {0}]",
                FullPath,
                LogFileDate,
                ParsingError,
                LogType);
        }
    }
}