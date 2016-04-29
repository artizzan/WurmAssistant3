using AldursLab.WurmApi.Tests.Integration.Scenarios.PersistentCollections.Data;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.PersistentCollections.Tests
{
    [TestFixture]
    class PersistenceAndLoadingTests : TestsBase
    {
        [Test]
        public void PersistsAndLoads_DefaultCollection()
        {
            {
                var lib = CreateLibrary();
                var obj = new PersistentData(lib.DefaultCollection.GetObject<Dto>("id"));
                obj.Data = TestValues.Value;
                lib.SaveChanged();
            }
            {
                var lib = CreateLibrary();
                var obj = new PersistentData(lib.DefaultCollection.GetObject<Dto>("id"));
                Expect(obj.Data, EqualTo(TestValues.Value));
            }
        }

        [Test]
        public void PersistsAndLoads_NamedCollection()
        {
            {
                var lib = CreateLibrary();
                var collection = lib.GetCollection("cid");
                var obj = new PersistentData(collection.GetObject<Dto>("id"));
                obj.Data = TestValues.Value;
                lib.SaveChanged();
            }
            {
                var lib = CreateLibrary();
                var collection = lib.GetCollection("cid");
                var obj = new PersistentData(collection.GetObject<Dto>("id"));
                Expect(obj.Data, EqualTo(TestValues.Value));
            }
        }

        [Test]
        public void PersistsAndLoads_MultipleObjects_MultipleCollections()
        {
            {
                var lib = CreateLibrary();
                {
                    var collection1 = lib.GetCollection("cid1");
                    {
                        var obj1C1 = collection1.GetObject<Dto>("obj1C1");
                        obj1C1.Entity.Data = "obj1C1";
                        obj1C1.FlagAsChanged();
                    }
                    {
                        var obj2C1 = collection1.GetObject<Dto>("obj2C1");
                        obj2C1.Entity.Data = "obj2C1";
                        obj2C1.FlagAsChanged();
                    }
                }
                {
                    var collection2 = lib.GetCollection("cid2");
                    var obj1C2 = collection2.GetObject<Dto>("obj2C1");
                    obj1C2.Entity.Data = "obj2C1";
                    obj1C2.FlagAsChanged();
                }
                lib.SaveChanged();
            }
            {
                var lib = CreateLibrary();
                {
                    var collection1 = lib.GetCollection("cid1");
                    {
                        var obj1C1 = collection1.GetObject<Dto>("obj1C1");
                        Expect(obj1C1.Entity.Data, EqualTo("obj1C1"));
                    }
                    {
                        var obj2C1 = collection1.GetObject<Dto>("obj2C1");
                        Expect(obj2C1.Entity.Data, EqualTo("obj2C1"));
                    }
                }
                {
                    var collection2 = lib.GetCollection("cid2");
                    var obj1C2 = collection2.GetObject<Dto>("obj2C1");
                    Expect(obj1C2.Entity.Data, EqualTo("obj2C1"));
                }
            }
        }

        [Test]
        public void DeserializesIntoAnyCompatibleType_WhenNotYetResolved()
        {
            {
                var lib = CreateLibrary();
                var obj = lib.DefaultCollection.GetObject<Dto>("id");
                obj.Entity.Data = TestValues.Value;
                obj.FlagAsChanged();
                lib.SaveChanged();
            }
            {
                var lib = CreateLibrary();
                var obj = lib.DefaultCollection.GetObject<DtoClone>("id");
                Expect(obj.Entity.Data, EqualTo(TestValues.Value));
            }
        }

        [Test]
        public void UnflaggedDataChangesNotSaved_WhenSaveOnlyChanged() 
        {
            {
                var lib = CreateLibrary();
                var obj = lib.DefaultCollection.GetObject<Dto>("id");
                obj.Entity.Data = TestValues.Value;
                lib.SaveChanged();
            }
            {
                var lib = CreateLibrary();
                var obj = new PersistentData(lib.DefaultCollection.GetObject<Dto>("id"));
                Expect(obj.Data, Not.EqualTo(TestValues.Value));
            }
        }

        [Test]
        public void AllDataSaved_WhenSaveIsForced()
        {
            {
                var lib = CreateLibrary();
                var obj = lib.DefaultCollection.GetObject<Dto>("id");
                obj.Entity.Data = TestValues.Value;
                lib.SaveAll();
            }
            {
                var lib = CreateLibrary();
                var obj = new PersistentData(lib.DefaultCollection.GetObject<Dto>("id"));
                Expect(obj.Data, EqualTo(TestValues.Value));
            }
        }

        [Test]
        public void ResolvesSameCollection_WhenAskedForSameCollectionId()
        {
            {
                var lib = CreateLibrary();
                var c1 = lib.GetCollection("cid");
                var c2 = lib.GetCollection("cid");
                Expect(ReferenceEquals(c1, c2));
            }
        }

        [Test]
        public void ResolvesSameObject_WhenAskedForSameEntityIdFromSameCollectionId()
        {
            var lib = CreateLibrary();
            var c1 = lib.GetCollection("cid");
            var o1 = c1.GetObject<Dto>("id");
            var o2 = c1.GetObject<Dto>("id");
            Expect(ReferenceEquals(o1, o2));
        }

        [Test]
        public void ResolvedObjectsMaintainDefaultValues()
        {
            var lib = CreateLibrary();
            var obj = lib.DefaultCollection.GetObject<DtoWithDefaults>("id");
            Expect(obj.Entity.Data, EqualTo(TestValues.Default));
        }
    }
}
