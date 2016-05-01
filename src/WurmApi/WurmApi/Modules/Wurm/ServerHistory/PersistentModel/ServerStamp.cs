using System;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel
{
    /// <summary>
    /// Contains a server name, along with date of it's parseout.
    /// </summary>
    public class ServerStamp
    {
        /// <summary>
        /// Timestamp of the source log entry, that contained this information.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Parsed server name.
        /// </summary>
        public ServerName ServerName { get; set; }
    }
}