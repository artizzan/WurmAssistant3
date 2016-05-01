namespace AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel
{
    public class WurmServerLogsData
    {
        public WurmServerLogsData()
        {
            TimeDetails = new TimeDetails();
        }

        public TimeDetails TimeDetails { get; set; }
    }
}