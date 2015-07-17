using System;

namespace AldurSoft.WurmApi.Impl.WurmServersImpl
{
    public class WurmDateAndUptime
    {
        public WurmDateTime WurmDateTime { get; set; }
        public DateTimeOffset WurmDateTimeStamp { get; set; }
        public TimeSpan ServerUptime { get; set; }
        public DateTimeOffset ServerUptimeStamp { get; set; }
    }
}