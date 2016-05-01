using AldursLab.WurmApi.PersistentObjects;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.PersistentCollections.Data
{
    class PersistentData : PersistentEntityBase<Dto>
    {
        public PersistentData(IPersistent<Dto> persistent) : base(persistent)
        {
        }

        public string Data
        {
            get { return Entity.Data; }
            set
            {
                Entity.Data = value;
                FlagAsChanged();
            }
        }
    }

    class PersistentDataWithMigration : PersistentEntityBase<Dto>
    {
        public PersistentDataWithMigration(IPersistent<Dto> persistent)
            : base(persistent)
        {
            RunMigration(0, 1, dto =>
            {
                dto.Data = TestValues.ValueAfterMigration;
            },
                dto => dto.Data == TestValues.Value
                );
        }

        public string Data
        {
            get { return Entity.Data; }
            set
            {
                Entity.Data = value;
                FlagAsChanged();
            }
        }
    }

    class PersistentDataWithMigrationNoFilter : PersistentEntityBase<Dto>
    {
        public PersistentDataWithMigrationNoFilter(IPersistent<Dto> persistent)
            : base(persistent)
        {
            RunMigration(0, 1, dto =>
            {
                dto.Data = TestValues.ValueAfterMigration;
            });
        }

        public string Data
        {
            get { return Entity.Data; }
            set
            {
                Entity.Data = value;
                FlagAsChanged();
            }
        }
    }
}