namespace AldursLab.WurmAssistant3.Core.Areas.Config.Contracts
{
    public class ServerInformation
    {
        public string ServerName { get; set; }
        public string ServerGroup { get; set; }
        public string ServerStatsUrl { get; set; }

        public ServerInformation CreateCopy()
        {
            return (ServerInformation)this.MemberwiseClone();
        }
    }
}