using System.Collections.Generic;
using System.Linq;
using AldurSoft.WurmApi.Modules.Wurm.ServerGroups;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    public class WurmServerList : IWurmServerList
    {
        private readonly IReadOnlyCollection<WurmServerInfo> defaultDescriptions;

        public WurmServerList()
        {
            defaultDescriptions = new[]
            {
                new WurmServerInfo(
                    "Golden Valley",
                    "http://jenn001.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Independence",
                    "http://freedom001.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Deliverance",
                    "http://freedom002.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Exodus",
                    "http://freedom003.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Celebration",
                    "http://freedom004.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Chaos",
                    "http://wild001.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Elevation",
                    "http://elevation.wurmonline.com/battles/stats.html",
                    new EpicServerGroup()),
                new WurmServerInfo(
                    "Serenity",
                    "http://serenity.wurmonline.com/battles/stats.html",
                    new EpicServerGroup()),
                new WurmServerInfo(
                    "Desertion",
                    "http://desertion.wurmonline.com/battles/stats.html",
                    new EpicServerGroup()),
                new WurmServerInfo(
                    "Affliction",
                    "http://affliction.wurmonline.com/battles/stats.html",
                    new EpicServerGroup()),
                new WurmServerInfo(
                    "Pristine",
                    "http://freedom005.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Release",
                    "http://freedom006.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
                new WurmServerInfo(
                    "Xanadu",
                    "http://freedom007.game.wurmonline.com/battles/stats.html",
                    new FreedomServerGroup()),
            };
        }

        public virtual IEnumerable<WurmServerInfo> All
        {
            get { return defaultDescriptions; }
        }

        public bool Exists(ServerName serverName)
        {
            return defaultDescriptions.Any(info => info.Name == serverName);
        }
    }
}