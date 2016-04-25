using System;

namespace AldursLab.WurmApi.Modules.Wurm.Servers.Jobs
{
    class JobResult
    {
        public WurmDateTime? WurmDateTime { get; private set; }
        public TimeSpan? Uptime { get; private set; }

        public JobResult(WurmDateTime? wurmDateTime, TimeSpan? uptime)
        {
            WurmDateTime = wurmDateTime;
            Uptime = uptime;
        }
    }
}