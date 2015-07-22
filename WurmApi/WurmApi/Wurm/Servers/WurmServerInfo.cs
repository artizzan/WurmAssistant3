using System;

namespace AldurSoft.WurmApi.Wurm.Servers
{
    public class WurmServerInfo
    {
        public WurmServerInfo(string name, string webStatsUrl, ServerGroup serverGroup)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (webStatsUrl == null) throw new ArgumentNullException("webStatsUrl");
            this.Name = new ServerName(name);
            this.WebStatsUrl = webStatsUrl;
            this.ServerGroup = serverGroup;
        }

        public ServerName Name { get; private set; }
        public string WebStatsUrl { get; private set; }
        public ServerGroup ServerGroup { get; private set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, WebStatsUrl: {1}, ServerGroup: {2}", Name, WebStatsUrl, ServerGroup);
        }
    }
}