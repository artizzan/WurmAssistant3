using System;

namespace AldursLab.PersistentObjects.Tests.PersistentObjectSamples
{
    [PersistentObject("SimpleObj")]
    public class SimpleObj : IPersistentObject
    {
        public SimpleObj(string id)
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

        public string Data { get; set; }

        void IPersistentObject.PersistentDataLoaded()
        {
        }
    }

    public class SimpleObjWithNoAttribute : IPersistentObject
    {
        public SimpleObjWithNoAttribute(string id)
        {
            PersistentObjectId = id;
        }

        public string PersistentObjectId { get; private set; }
        public bool PersistibleChangesPending { get; set; }
        public event EventHandler<SaveEventArgs> PersistentStateSaveRequested;

        protected virtual void OnPersistentStateSaveRequested(SaveEventArgs e)
        {
            var handler = PersistentStateSaveRequested;
            if (handler != null)
                handler(this, e);
        }

        public string Data { get; set; }

        void IPersistentObject.PersistentDataLoaded()
        {
        }
    }

    [PersistentObject("SimpleObj")]
    public class SimpleObjWithNoInterface
    {
        public SimpleObjWithNoInterface(string id)
        {
            PersistentObjectId = id;
        }

        public string PersistentObjectId { get; private set; }
        public bool PersistibleChangesPending { get; set; }
        public event EventHandler<SaveEventArgs> PersistentStateSaveRequested;

        protected virtual void OnPersistentStateSaveRequested(SaveEventArgs e)
        {
            var handler = PersistentStateSaveRequested;
            if (handler != null)
                handler(this, e);
        }

        public string Data { get; set; }
    }

    [PersistentObject("SimpleObj")]
    public class SimpleObjWithIntData : IPersistentObject
    {
        public const int DataDefaultValue = 1;

        public SimpleObjWithIntData(string id)
        {
            PersistentObjectId = id;
            Data = DataDefaultValue;
        }

        public string PersistentObjectId { get; private set; }
        public bool PersistibleChangesPending { get; set; }
        public event EventHandler<SaveEventArgs> PersistentStateSaveRequested;

        protected virtual void OnPersistentStateSaveRequested(SaveEventArgs e)
        {
            var handler = PersistentStateSaveRequested;
            if (handler != null) handler(this, e);
        }

        public int Data { get; set; }

        void IPersistentObject.PersistentDataLoaded()
        {
        }
    }
}