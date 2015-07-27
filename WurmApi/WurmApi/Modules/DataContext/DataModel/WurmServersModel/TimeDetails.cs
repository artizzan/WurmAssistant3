namespace AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel
{
    public class TimeDetails
    {
        public TimeDetails()
        {
            ServerDate = new ServerDateStamped();
            ServerUptime = new ServerUptimeStamped();
        }

        public ServerDateStamped ServerDate { get; set; }
        public ServerUptimeStamped ServerUptime { get; set; }
    }
}