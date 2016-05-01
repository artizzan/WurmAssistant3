using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.PersistentObjects.Tests.PersistentObjectSamples
{
    [PersistentObject("FakeSubsystem")]
    [JsonObject(MemberSerialization.OptIn)]
    public class FakeSubsystem : IPersistentObject
    {
        [JsonProperty]
        OuterDataContainer outerDataContainer;
        [JsonProperty]
        InnerDataContainer innerDataContainer;
        [JsonProperty]
        string someLooseString;

        readonly FakeSubsystemComponent fakeSubsystemComponent;

        public FakeSubsystem([NotNull] FakeSubsystemComponent fakeSubsystemComponent)
        {
            if (fakeSubsystemComponent == null) throw new ArgumentNullException("fakeSubsystemComponent");
            this.fakeSubsystemComponent = fakeSubsystemComponent;

            // set this instance persistent Id
            PersistentObjectId = "PersistentObjectId";

            // creates default values, should be replaced by load
            outerDataContainer = new OuterDataContainer();
            innerDataContainer = new InnerDataContainer();
            someLooseString = "someLooseString";
        }

        [JsonObject(MemberSerialization.OptIn)]
        class InnerDataContainer
        {
            public InnerDataContainer()
            {
                privateDataField = "privateDataField";
                publicDataField = "publicDataField";
                PublicDataProperty = "PublicDataProperty";
            }

            [JsonProperty]
            string privateDataField;

            [JsonProperty]
            public string publicDataField;

            [JsonProperty]
            public string PublicDataProperty { get; set; }

            public string PrivateDataFieldAcsr
            {
                get { return privateDataField; }
                set { privateDataField = value; }
            }
        }

        public string PersistentObjectId { get; private set; }

        public bool PersistibleChangesPending { get; set; }

        public OuterDataContainer OuterDataContainer
        {
            get { return outerDataContainer; }
        }

        public string InnerDataContainer_PublicDataPropertyAcsr
        {
            get { return innerDataContainer.PublicDataProperty; }
            set { innerDataContainer.PublicDataProperty = value; }
        }

        public string InnerDataContainer_publicDataFieldAcsr
        {
            get { return innerDataContainer.publicDataField; }
            set { innerDataContainer.publicDataField = value; }
        }

        public string InnerDataContainer_PrivateDataFieldAcsr
        {
            get { return innerDataContainer.PrivateDataFieldAcsr; }
            set { innerDataContainer.PrivateDataFieldAcsr = value; }
        }

        public string SomeLooseStringAcsr
        {
            get { return someLooseString; }
            set { someLooseString = value; }
        }

        public FakeSubsystemComponent SubsystemComponent
        {
            get { return fakeSubsystemComponent; }
        }

        public event EventHandler<SaveEventArgs> PersistentStateSaveRequested;
        void IPersistentObject.PersistentDataLoaded()
        {
        }

        protected virtual void OnPersistentStateSaveRequested(SaveEventArgs e)
        {
            var handler = PersistentStateSaveRequested;
            if (handler != null) handler(this, e);
        }
    }

    public class FakeSubsystemComponent
    {
        public FakeSubsystemComponent()
        {
            somePrivateData = "somePrivateData";
            SomePublicData = "SomePublicData";
        }

        string somePrivateData;
        public string SomePublicData { get; set; }

        public string SomePrivateDataAcsr
        {
            get { return somePrivateData; }
            set { somePrivateData = value; }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class OuterDataContainer
    {
        public OuterDataContainer()
        {
            privateDataField = "privateDataField";
            publicDataField = "publicDataField";
            PublicDataProperty = "PublicDataProperty";
        }

        [JsonProperty]
        string privateDataField;

        [JsonProperty]
        public string publicDataField;

        [JsonProperty]
        public string PublicDataProperty { get; set; }

        public string PrivateDataFieldAcsr
        {
            get { return privateDataField; }
            set { privateDataField = value; }
        }
    }
}