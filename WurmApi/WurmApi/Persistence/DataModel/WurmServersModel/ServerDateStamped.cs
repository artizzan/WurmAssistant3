using System;
using AldurSoft.WurmApi.Wurm.DateAndTime;

namespace AldurSoft.WurmApi.Persistence.DataModel.WurmServersModel
{
    public class ServerDateStamped
    {
        public WurmDateTime WurmDateTime { get; set; }

        /// <summary>
        /// default value should mean, that there is no data for wurm date time.
        /// </summary>
        public DateTimeOffset Stamp { get; set; }
    }
}