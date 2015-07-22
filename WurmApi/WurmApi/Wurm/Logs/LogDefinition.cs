using System;

namespace AldurSoft.WurmApi.Wurm.Logs
{
    public class LogDefinition
    {
        public LogDefinition(LogType logType, string prefix, string friendlyName)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            if (friendlyName == null)
            {
                throw new ArgumentNullException("friendlyName");
            }
            this.LogType = logType;
            this.LogPrefix = prefix;
            this.FriendlyName = friendlyName;
        }
        public LogType LogType { get; private set; }
        public string LogPrefix { get; private set; }
        public string FriendlyName { get; private set; }
    }
}