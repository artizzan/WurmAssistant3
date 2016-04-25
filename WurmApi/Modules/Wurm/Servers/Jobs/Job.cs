namespace AldursLab.WurmApi.Modules.Wurm.Servers.Jobs
{
    abstract class Job
    {
        protected Job(ServerName serverName)
        {
            ServerName = serverName;
        }

        public ServerName ServerName { get; private set; }
    }
}
