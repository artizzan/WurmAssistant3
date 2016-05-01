using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;
using AldursLab.WurmApi.PersistentObjects;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    class SortedServerHistory
    {
        // note: at least one method in this class has to be thread safe, check methods for details

        readonly IPersistent<PersistentModel.ServerHistory> persistentData;

        List<ServerStamp> orderedStamps = new List<ServerStamp>();

        public SortedServerHistory(IPersistent<PersistentModel.ServerHistory> persistentData)
        {
            if (persistentData == null)
                throw new ArgumentNullException(nameof(persistentData));
            this.persistentData = persistentData;

            Rebuild(persistentData.Entity.ServerStamps);
        }

        public void Insert(params ServerStamp[] serverStamps)
        {
            persistentData.Entity.ServerStamps.AddRange(serverStamps);
            Rebuild(persistentData.Entity.ServerStamps);
        }

        /// <summary>
        /// Null if nothing found
        /// </summary>
        public ServerName TryGetServerAtStamp(DateTime timestamp)
        {
            // note: this method must be thread safe!
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