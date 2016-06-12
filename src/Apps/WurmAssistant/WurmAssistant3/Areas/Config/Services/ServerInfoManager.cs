using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Config.Services
{
    [KernelHint(BindingHint.Singleton), PersistentObject("Config_ServerInfoManager")]
    public class ServerInfoManager : PersistentObjectBase
    {
        [JsonProperty]
        List<ServerInformation> infos = new List<ServerInformation>();

        public IEnumerable<ServerInformation> GetAllMappings()
        {
            return infos.Select(information => information.CreateCopy()).ToArray();
        }

        public void UpdateAllMappings(IEnumerable<ServerInformation> mappings)
        {
            var mappingsArray = mappings.Where(information => !string.IsNullOrEmpty(information.ServerName)).ToArray();

            if (mappingsArray.Length
                != mappingsArray.Select(information => information.ServerName.ToUpperInvariant()).Distinct().Count())
            {
                throw new ApplicationException(
                    "Server names are duplicated in the defined mappings list. Choose only one mapping for each server");
            }

            if (mappingsArray.Any(information => information.ServerGroup.Contains(':')))
            {
                throw new ApplicationException(
                    "At least one server group contains reserved colon character ':', please use another character instead.");
            }

            infos = new List<ServerInformation>(mappingsArray);
            FlagAsChanged();
        }

        public void UpdateWurmApiConfigDictionary(IDictionary<ServerName, WurmServerInfo> dictionary)
        {
            foreach (var serverInformation in infos)
            {
                if (string.IsNullOrWhiteSpace(serverInformation.ServerName))
                {
                    continue;
                }

                var serverName = new ServerName(serverInformation.ServerName);
                dictionary[serverName] = new WurmServerInfo(serverInformation.ServerName,
                    serverInformation.ServerStatsUrl ?? string.Empty,
                    new ServerGroup(serverInformation.ServerGroup ?? string.Empty));
            }
        }
    }
}
