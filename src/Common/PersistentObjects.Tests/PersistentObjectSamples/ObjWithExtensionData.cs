using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AldursLab.PersistentObjects.Tests.PersistentObjectSamples
{
    [PersistentObject("SimpleObj")]
    public class ObjWithExtensionData : IPersistentObject
    {
        public ObjWithExtensionData(string id)
        {
            PersistentObjectId = id;
        }

        public string PersistentObjectId { get; private set; }
        public bool PersistibleChangesPending { get; set; }
        public event EventHandler<SaveEventArgs> PersistentStateSaveRequested;

        protected virtual void OnPersistentStateSaveRequested(SaveEventArgs e)
        {
            var handler = PersistentStateSaveRequested;
            if (handler != null) handler(this, e);
        }

        [JsonExtensionData] 
        IDictionary<string, JToken> extensionData;

        void IPersistentObject.PersistentDataLoaded()
        {
        }
    }
}
