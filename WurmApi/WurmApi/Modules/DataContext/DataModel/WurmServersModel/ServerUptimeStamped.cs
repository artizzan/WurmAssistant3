using System;

namespace AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel
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