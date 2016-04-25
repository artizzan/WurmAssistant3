using System;

namespace AldursLab.WurmApi
{
    public class LogFileInfo : IComparable<LogFileInfo>, IComparable
    {
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
            LogType = logType;
            // normalize name
            pmRecipient = pmRecipient?.ToUpperInvariant();
            PmRecipientNormalized = pmRecipient; 
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            LogFileDate = logFileDate;
            ParsingError = parsingError;
            FullPath = fullPath;
            FileName = fileName;
            FileNameNormalized = FileName.ToUpperInvariant();
        }

        public LogType LogType { get; }

        public string PmRecipientNormalized { get; }

        public int CompareTo(LogFileInfo other)
        {
            return LogFileDate.DateTime.CompareTo(other.LogFileDate.DateTime);
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