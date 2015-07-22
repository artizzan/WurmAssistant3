using System;

namespace AldurSoft.WurmApi.Wurm.Logs
{
    public struct LogFileDate : IEquatable<LogFileDate>
    {
        private readonly DateTime dateTime;

        private readonly LogSavingType logSavingType;

        public LogFileDate(DateTime dateTime, LogSavingType logSavingType)
        {
            this.dateTime = dateTime;
            this.logSavingType = logSavingType;
        }

        public DateTime DateTime
        {
            get { return dateTime; }
        }

        public LogSavingType LogSavingType
        {
            get { return logSavingType; }
        }

        public bool Equals(LogFileDate other)
        {
            return dateTime.Equals(other.dateTime) && logSavingType == other.logSavingType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is LogFileDate && Equals((LogFileDate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (dateTime.GetHashCode() * 397) ^ (int)logSavingType;
            }
        }

        public static bool operator ==(LogFileDate left, LogFileDate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LogFileDate left, LogFileDate right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[LogFileDate; DateTime: {0}, LogSavingType: {1}]", DateTime, LogSavingType);
        }
    }
}