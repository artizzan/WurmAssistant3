using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects;

namespace AldurSoft.WurmApi.Modules.Wurm.ServerHistory.PersistentModel
{
    public class ServerHistory : Entity
    {
        public ServerHistory()
        {
            Reset();
        }

        public DateTime SearchedFrom { get; set; }
        public DateTime SearchedTo { get; set; }
        public bool AnySearchCompleted { get; set; }

        public List<ServerStamp> ServerStamps { get; set; }

        public void Reset()
        {
            AnySearchCompleted = false;
            // set values, that will not cause overflows on +/- ops
            SearchedFrom = new DateTime(1900, 1, 1);
            SearchedTo = new DateTime(1900, 1, 1);
            ServerStamps = new List<ServerStamp>();
        }
    }
}
