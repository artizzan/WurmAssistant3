using System;
using AldurSoft.WurmApi.Wurm.DateAndTime;

namespace AldurSoft.WurmApi.Wurm.Servers.WurmServersModule
{
    public class WurmDateAndUptime
    {
        public WurmDateTime WurmDateTime { get; set; }
        public DateTimeOffset WurmDateTimeStamp { get; set; }
        public TimeSpan ServerUptime { get; set; }
        public DateTimeOffset ServerUptimeStamp { get; set; }
    }
}