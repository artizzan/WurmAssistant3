using AldursLab.WurmApi.PersistentObjects;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.PersistentCollections.Data
{
    class Dto : Entity
    {
        public string Data { get; set; }

        public int OldField { get; set; }
    }

    class DtoClone : Entity
    {
        public string Data { get; set; }

        public int NewField { get; set; }
    }

    class DtoWithDefaults : Entity
    {
        public DtoWithDefaults()
        {
            Data = TestValues.Default;
        }
        
        public string Data { get; set; }

        public int NumericValue { get; set; }
    }
}