using System;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    public class WurmDateAndUptime
    {
        public WurmDateTime WurmDateTime { get; set; }
        public DateTimeOffset WurmDateTimeStamp { get; set; }
        public TimeSpan ServerUptime { get; set; }
        public DateTimeOffset ServerUptimeStamp { get; set; }
    }
}