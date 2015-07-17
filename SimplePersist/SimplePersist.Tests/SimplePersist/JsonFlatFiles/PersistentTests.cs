using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using AldurSoft.SimplePersist.Persistence.FlatFiles;
using AldurSoft.SimplePersist.Serializers.JsonNet;

using SimplePersist.Tests;

using Telerik.JustMock;
using Telerik.JustMock.Helpers;

using Xunit;

namespace AldurSoft.SimplePersist.Tests.SimplePersist.JsonFlatFiles
{
    public class PersistentTests
    {
        private readonly ISimplePersistLogger logger = Mock.Create<ISimplePersistLogger>();

        private readonly EntityKey entityKey1 = new EntityKey("1");
        private readonly EntityKey entityKey2 = new EntityKey("2");
        public const string EntityNameString1 = "ENT1";
        private readonly EntityName entityName1 = new EntityName(EntityNameString1);
        private readonly string dirPath;

        public PersistentTests()
        {
            dirPath = Path.Combine(TestingEnvironment.BinDirectory, "SimplePersistTestDataStore" + "759E5F72-9C90-4CB0-967F-EA11707D1781");

            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, recursive:true);
            }
        }

        [Fact]
        public void SetHasCorrectEntityName()
        {
            var fixture = new TestFixture(this);
            Assert.Equal(EntityNameString1, fixture.DataContext.SampleEntities.EntityName.Value);
        }

        [Fact]
        public void EntityHasCorrectKey()
        {
            var fixture1 = new TestFixture(this);
            var persistent1 = fixture1.DataContext.SampleEntities.Get(entityKey1);
            Assert.Equal(entityKey1, persistent1.EntityKey);
        }

        [Fact]
        public void SavesAndLoads()
        {
            var fixture1 = new TestFixture(this);
            var persistent1 = fixture1.DataContext.SampleEntities.Get(entityKey1);
            Assert.Equal(false, persistent1.Loaded);

            persistent1.Entity.String = "test123";
            persistent1.Entity.Strings.AddRange(new [] { "1", "2" });
            persistent1.Save();

            var fixture2 = new TestFixture(this);
            var persistent2 = fixture2.DataContext.SampleEntities.Get(entityKey1);
            Assert.Equal(false, persistent2.Loaded);
            Assert.Equal("test123", persistent2.Entity.String);
            Assert.True(persistent2.Entity.Strings.SingleOrDefault(s => s == "1") != null);
            Assert.True(persistent2.Entity.Strings.SingleOrDefault(s => s == "2") != null);
        }

        [Fact]
        public void SavesIfChanged()
        {
            const string Content1 = "123123";
            const string Content2 = "321321";

            var fixture1 = new TestFixture(this);
            var persistent1 = fixture1.DataContext.SampleEntities.Get(entityKey1);
            persistent1.Entity.String = Content1;
            persistent1.SaveIfSetChanged();

            var fixture2 = new TestFixture(this);
            var persistent2 = fixture2.DataContext.SampleEntities.Get(entityKey1);
            Assert.NotEqual(Content1, persistent2.Entity.String);
            persistent2.Entity.String = Content2;
            persistent2.SetChanged();
            persistent2.SaveIfSetChanged();

            var fixture3 = new TestFixture(this);
            var persistent3 = fixture3.DataContext.SampleEntities.Get(entityKey1);
            Assert.Equal(Content2, persistent3.Entity.String);
        }

        [Fact]
        public void ForceSaves()
        {
            const string Content1 = "123123";
            var fixture1 = new TestFixture(this);
            var persistent1 = fixture1.DataContext.SampleEntities.Get(entityKey1);
            persistent1.Entity.String = Content1;
            persistent1.Save();

            var fixture2 = new TestFixture(this);
            var persistent2 = fixture2.DataContext.SampleEntities.Get(entityKey1);
            Assert.Equal(Content1, persistent2.Entity.String);
        }

        [Fact]
        public void Loads()
        {
            var fixture1 = new TestFixture(this);
            var persistent1 = fixture1.DataContext.SampleEntities.Get(entityKey1);
            persistent1.Load();
            Assert.True(persistent1.Loaded);
        }

        [Fact]
        public void ChecksIfPersisted()
        {
            var fixture1 = new TestFixture(this);
            var persistent1 = fixture1.DataContext.SampleEntities.Get(entityKey1);
            Assert.False(persistent1.Persisted);
            persistent1.Save();
            Assert.True(persistent1.Persisted);
        }

        [Fact]
        public void DoesMotThrowWhenDeserializeErrorsFound()
        {
            var fixture1 = new TestFixture(this);
            var persistent1 = fixture1.DataContext.SampleEntities.Get(entityKey1);
            persistent1.Save();

            var fixture2 = new BadTestFixture(this);
            var persistent2 = fixture2.BadDataContext.BadSampleEntities.Get(entityKey1);
            Assert.Equal(default(DateTime), persistent2.Entity.String);
            Assert.Equal(0, persistent2.Entity.Strings.Count);

            logger.Assert(persistLogger => persistLogger.Log(Arg.AnyString, Arg.IsAny<Severity>()), Occurs.AtLeastOnce());
        }

        [Fact]
        public void PersistentSetGetsAllKeys()
        {
            var fixture1 = new TestFixture(this);
            fixture1.DataContext.SampleEntities.Get(entityKey1).Save();
            fixture1.DataContext.SampleEntities.Get(entityKey2).Save();

            var keys = fixture1.DataContext.SampleEntities.GetAllKeys().ToArray();
            Assert.True(keys.SingleOrDefault(key => key == entityKey1) != null);
            Assert.True(keys.SingleOrDefault(key => key == entityKey2) != null);
        }

        [Fact]
        public void DeletesAllPersisted()
        {
            const string Content = "213123123";

            var fixture1 = new TestFixture(this);
            var persistent1A = fixture1.DataContext.SampleEntities.Get(entityKey1);
            persistent1A.Entity.String = Content;
            persistent1A.Save();
            var persistent1B = fixture1.DataContext.SampleEntities.Get(entityKey2);
            persistent1B.Entity.String = Content;
            persistent1B.Save();

            var fixture2 = new TestFixture(this);
            fixture2.DataContext.SampleEntities.DeleteAllPersisted();

            var fixture3 = new TestFixture(this);
            var persistent3A = fixture3.DataContext.SampleEntities.Get(entityKey1);
            Assert.NotEqual(Content, persistent3A.Entity.String);
            var persistent3B = fixture3.DataContext.SampleEntities.Get(entityKey2);
            Assert.NotEqual(Content, persistent3B.Entity.String);
        }

        [Fact]
        public void SavesAllDirty()
        {
            var fixture = new TestFixture(this);
            var entity = fixture.DataContext.SampleEntities.Get(new EntityKey("Test"));
            entity.Entity.String = "test";
            entity.SetChanged();

            var verifyFixture = new TestFixture(this);
            var verifyEntity = verifyFixture.DataContext.SampleEntities.Get(new EntityKey("Test"));
            Assert.NotEqual(verifyEntity.Entity.String, "test");

            fixture.PersistentManager.SaveAllChanged();

            verifyFixture = new TestFixture(this);
            verifyEntity = verifyFixture.DataContext.SampleEntities.Get(new EntityKey("Test"));
            Assert.Equal(verifyEntity.Entity.String, "test");
        }

        private class TestFixture
        {
            private readonly PersistentTests tests;

            public TestFixture(PersistentTests tests)
            {
                this.tests = tests;

                PersistentManager = new PersistentManager(
                    new JsonSerializationStrategy(),
                    new FlatFilesPersistenceStrategy(tests.dirPath),
                    tests.logger);

                DataContext = new DataContext(PersistentManager);
            }

            public PersistentManager PersistentManager { get; private set; }
            public DataContext DataContext { get; private set; }
        }

        private class BadTestFixture
        {
            private readonly PersistentTests tests;

            public BadTestFixture(PersistentTests tests)
            {
                this.tests = tests;

                PersistentManager = new PersistentManager(
                    new JsonSerializationStrategy(),
                    new FlatFilesPersistenceStrategy(tests.dirPath),
                    tests.logger);

                BadDataContext = new BadDataContext(PersistentManager);
            }

            public PersistentManager PersistentManager { get; private set; }
            public BadDataContext BadDataContext { get; private set; }
        }
    }

    public class DataContext
    {
        private readonly IPersistentManager persistentManager;

        public DataContext(IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;

            SampleEntities =
                persistentManager.GetPersistentCollection<SampleEntity>(
                    new EntityName(PersistentTests.EntityNameString1));
        }

        public IPersistentSet<SampleEntity> SampleEntities { get; private set; }
    }

    public class BadDataContext
    {
        private readonly IPersistentManager persistentManager;

        public BadDataContext(IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;

            BadSampleEntities =
                persistentManager.GetPersistentCollection<BadSampleEntity>(
                    new EntityName(PersistentTests.EntityNameString1));
        }

        public IPersistentSet<BadSampleEntity> BadSampleEntities { get; private set; }
    }

    public class SampleEntity
    {
        public SampleEntity()
        {
            Strings = new List<string>();
        }

        public List<string> Strings { get; set; }

        public string String { get; set; }
    }

    public class BadSampleEntity
    {
        public BadSampleEntity()
        {
            Strings = new List<DateTime>();
        }

        public List<DateTime> Strings { get; set; }

        public DateTime String { get; set; }
    }
}
