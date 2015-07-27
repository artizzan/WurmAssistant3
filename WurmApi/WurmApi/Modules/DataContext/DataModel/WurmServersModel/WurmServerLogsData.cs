namespace AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel
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