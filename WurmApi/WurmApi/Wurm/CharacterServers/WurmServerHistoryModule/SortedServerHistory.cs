using System;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Persistence.DataModel.ServerHistoryModel;
using AldurSoft.WurmApi.Wurm.Servers;

namespace AldurSoft.WurmApi.Wurm.CharacterServers.WurmServerHistoryModule
{
    public class SortedServerHistory
    {
        private readonly IPersistent<ServerHistory> serverHistoryRepository;

        public SortedServerHistory(IPersistent<ServerHistory> serverHistoryRepository)
        {
            if (serverHistoryRepository == null)
                throw new ArgumentNullException("serverHistoryRepository");
            this.serverHistoryRepository = serverHistoryRepository;

            Rebuild(serverHistoryRepository.Entity.ServerStamps);
        }

        private List<ServerStamp> orderedStamps = new List<ServerStamp>();

        public void Insert(params ServerStamp[] serverStamps)
        {
            serverHistoryRepository.Entity.ServerStamps.AddRange(serverStamps);
            Rebuild(serverHistoryRepository.Entity.ServerStamps);
        }

        /// <summary>
        /// Null if nothing found
        /// </summary>
        public ServerName TryGetServerAtStamp(DateTime timestamp)
        {
            foreach (var orderedStamp in orderedStamps)
            {
                if (orderedStamp.Timestamp <= timestamp)
                {
                    return orderedStamp.ServerName;
                }
            }
            return null;
        }

        private void Rebuild(IEnumerable<ServerStamp> serverStamps)
        {
            var ordered = serverStamps.OrderBy(stamp => stamp.Timestamp).ToList();
            var culled = new List<ServerStamp>();
            for (int i = 0; i < ordered.Count; i++)
            {
                var currentStamp = ordered[i];
                if (i == 0)
                {
                    culled.Add(currentStamp);
                }
                else
                {
                    var previousStamp = ordered[i - 1];
                    if (previousStamp.ServerName != currentStamp.ServerName)
                    {
                        culled.Add(currentStamp);
                    }
                }
            }
            orderedStamps = culled.OrderByDescending(stamp => stamp.Timestamp).ToList();
        }
    }
}