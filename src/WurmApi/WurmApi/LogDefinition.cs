using System;

namespace AldursLab.WurmApi
{
    public class LogDefinition
    {
        public LogDefinition(LogType logType, string prefix, string friendlyName)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }
            if (friendlyName == null)
            {
                throw new ArgumentNullException(nameof(friendlyName));
            }
            LogType = logType;
            LogPrefix = prefix;
            FriendlyName = friendlyName;
        }
        public LogType LogType { get; private set; }
        public string LogPrefix { get; private set; }
        public string FriendlyName { get; private set; }
    }
}