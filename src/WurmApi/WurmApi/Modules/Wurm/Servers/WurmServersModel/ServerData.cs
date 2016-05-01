using System;
using System.Collections.Generic;
using AldursLab.WurmApi.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel
{
    [JsonObject(MemberSerialization.Fields)]
    class ServersData : Entity
    {
        readonly Dictionary<string, ServerData> serverDatas;

        /// <summary>
        /// Server Name Normalized to Server Data
        /// </summary>
        public Dictionary<string, ServerData> ServerDatas
        {
            get { return serverDatas; }
        }

        public ServersData()
        {
            serverDatas = new Dictionary<string, ServerData>();
            LastScanDate = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero); //something that wont on break on DateTimeOffset +/- methods.
        }

        public DateTimeOffset LastScanDate { get; set; }
    }

    public class ServerData
    {
        public ServerData()
        {
            LogHistory = new TimeDetails();
        }

        public TimeDetails LogHistory { get; set; }
    }
}
