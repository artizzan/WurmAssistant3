using AldursLab.WurmApi.Tests.Integration.Scenarios.PersistentCollections.Data;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.PersistentCollections.Tests
{
    [TestFixture]
    class MigrationsTests : TestsBase
    {
        [Test]
        public void MigratesData_IfMatchingFilter()
        {
            {
                // create persistent entity without any migration
                var lib = CreateLibrary();
                var obj = new PersistentData(lib.DefaultCollection.GetObject<Dto>("id"));
                obj.Data = TestValues.Value;
                lib.SaveChanged();
            }
            {
                // use new type, that has a migration defined,
                // verify that value has been migrated
                var lib = CreateLibrary();
                var persistent = lib.DefaultCollection.GetObject<Dto>("id");
                var obj = new PersistentDataWithMigration(persistent);
                Expect(obj.Data, EqualTo(TestValues.ValueAfterMigration));
                Expect(persistent.Entity.Version, EqualTo(1));
            }
        }

        [Test]
        public void MigratesData_WhenNoFilter()
        {
            {
                // create persistent entity without any migration
                var lib = CreateLibrary();
                var obj = new PersistentData(lib.DefaultCollection.GetObject<Dto>("id"));
                obj.Data = TestValues.Value;
                lib.SaveChanged();
            }
            {
                // use new type, that has a migration defined,
                // verify that value has been migrated
                var lib = CreateLibrary();
                var persistent = lib.DefaultCollection.GetObject<Dto>("id");
                var obj = new PersistentDataWithMigrationNoFilter(persistent);
                Expect(obj.Data, EqualTo(TestValues.ValueAfterMigration));
                Expect(persistent.Entity.Version, EqualTo(1));
            }
        }

        [Test]
        public void DoesNotMigrate_WhenVersionDoesNotMatch()
        {
            const int currentObjVersion = 3;
            {
                // create persistent entity without any migration
                var lib = CreateLibrary();
                var persistent = lib.DefaultCollection.GetObject<Dto>("id");
                // we are modifying version manually for test purposes, 
                // this is normally not possible, as the field is internal.
                persistent.Entity.Version = currentObjVersion;
                var obj = new PersistentData(lib.DefaultCollection.GetObject<Dto>("id"));
                obj.Data = TestValues.Value;
                lib.SaveChanged();
            }
            {
                // use new type, that has a migration defined,
                // verify that value has NOT been migrated
                var lib = CreateLibrary();
                var persistent = lib.DefaultCollection.GetObject<Dto>("id");
                var obj = new PersistentDataWithMigration(persistent);
                Expect(obj.Data, EqualTo(TestValues.Value));
                Expect(persistent.Entity.Version, EqualTo(currentObjVersion));
            }
        }
    }
}