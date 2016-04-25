using System;

namespace AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel
{
    public class ServerUptimeStamped
    {
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// default value should mean, that there is no data for uptime.
        /// </summary>
        public DateTimeOffset Stamp { get; set; }
    }
}