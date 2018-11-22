using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects.Persistence;
using AldursLab.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace AldursLab.PersistentObjects.Tests
{
    public class PersistenceStrategyTests : AssertionHelper
    {
        SqlitePersistenceStrategy strategy;

        readonly string entityName1 = "EntityName1";
        readonly string entityName2 = "EntityName2";
        readonly string entityKey1 = "EntityKey1";
        readonly string entityKey2 = "EntityKey2";

        const string Contents1 = "Contents1";
        const string Contents2 = "Contents2";
        const string Contents3 = "Contents3";

        DirectoryHandle dirHandle;

        [SetUp]
        public void Setup()
        {
            dirHandle = TempDirectoriesFactory.CreateEmpty();
            strategy = CreatePersistenceStrategy();
        }

        SqlitePersistenceStrategy CreatePersistenceStrategy()
        {
            return new SqlitePersistenceStrategy(new PersistenceManagerConfig() { DataStoreDirectoryPath = dirHandle.FullName });
        }

        [TearDown]
        public void Teardown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            dirHandle.Dispose();
        }

        [Test]
        public void SavesAndLoads()
        {
            strategy.Save(entityName1, entityKey1, Contents1);
            strategy.Save(entityName1, entityKey2, Contents2);
            strategy.Save(entityName2, entityKey1, Contents3);

            var data1 = strategy.TryLoad(entityName1, entityKey1);
            var data2 = strategy.TryLoad(entityName1, entityKey2);
            var data3 = strategy.TryLoad(entityName2, entityKey1);

            Expect(data1, EqualTo(Contents1));
            Expect(data2, EqualTo(Contents2));
            Expect(data3, EqualTo(Contents3));
        }

        [Test]
        public void RestoresMapFromState()
        {
            strategy.Save(entityName1, entityKey1, Contents1);

            var newStrategy = CreatePersistenceStrategy();
            var data = newStrategy.TryLoad(entityName1, entityKey1);
            Expect(data, EqualTo(Contents1));
        }
    }
}
