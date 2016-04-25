using System;

namespace AldursLab.WurmApi
{
    public class WurmServerInfo
    {
        public WurmServerInfo(string name, string webStatsUrl, ServerGroup serverGroup)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (webStatsUrl == null) throw new ArgumentNullException(nameof(webStatsUrl));
            ServerName = new ServerName(name);
            WebStatsUrl = webStatsUrl;
            ServerGroup = serverGroup;
        }

        public ServerName ServerName { get; }
        public string WebStatsUrl { get; }
        public ServerGroup ServerGroup { get; }

        public override string ToString()
        {
            return $"Name: {ServerName}, WebStatsUrl: {WebStatsUrl}, ServerGroup: {ServerGroup}";
        }
    }
}