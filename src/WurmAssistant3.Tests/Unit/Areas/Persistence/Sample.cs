using System;
using AldursLab.PersistentObjects;

namespace AldursLab.WurmAssistant3.Tests.Unit.Areas.Persistence
{
    public class Sample : IPersistentObject
    {
        public Sample(string persistentObjectId)
        {
            PersistentObjectId = persistentObjectId;
        }

        public string PersistentObjectId { get; private set; }
        public bool PersistibleChangesPending { get; set; }
        public event EventHandler<SaveEventArgs> PersistentStateSaveRequested;
        public void PersistentDataLoaded()
        {
        }
    }
}