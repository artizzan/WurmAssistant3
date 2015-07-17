using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.SimplePersist;
using AldurSoft.SimplePersist.Persistence.FlatFiles;

using Xunit;

namespace SimplePersist.Tests.PersistenceFlatFiles
{
    public class FlatFilesPersistenceStrategyTests
    {
        private readonly FlatFilesPersistenceStrategy strategy;

        private readonly EntityName entityName1 = new EntityName("EntityName1");
        private readonly EntityName entityName2 = new EntityName("EntityName2");
        private readonly EntityKey entityKey1 = new EntityKey("EntityKey1");
        private readonly EntityKey entityKey2 = new EntityKey("EntityKey2");

        private const string Contents1 = "Contents1";
        private const string Contents2 = "Contents2";
        private const string Contents3 = "Contents3";
        
        public FlatFilesPersistenceStrategyTests()
        {
            var dirPath = Path.Combine(TestingEnvironment.BinDirectory, "FlatFilesStrategyTestDataStore" + "674288F9-2B93-48BF-8483-3C1F3510B744");

            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, recursive: true);
            }

            strategy = new FlatFilesPersistenceStrategy(dirPath);
        }

        [Fact]
        public void SavesAndLoads()
        {
            strategy.Save(entityName1, entityKey1, Contents1);
            strategy.Save(entityName1, entityKey2, Contents2);
            strategy.Save(entityName2, entityKey1, Contents3);

            var data1 = strategy.Load(entityName1, entityKey1);
            var data2 = strategy.Load(entityName1, entityKey2);
            var data3 = strategy.Load(entityName2, entityKey1);

            Assert.Equal(Contents1, data1);
            Assert.Equal(Contents2, data2);
            Assert.Equal(Contents3, data3);
        }

        [Fact]
        public void ChecksIfExists()
        {
            Assert.False(strategy.Exists(entityName1, entityKey1));
            strategy.Save(entityName1, entityKey1, Contents1);
            Assert.True(strategy.Exists(entityName1, entityKey1));
            strategy.Delete(entityName1, entityKey1);
            Assert.False(strategy.Exists(entityName1, entityKey1));
        }

        [Fact]
        public void GetsAllKeys()
        {
            strategy.Save(entityName1, entityKey1, Contents1);
            strategy.Save(entityName1, entityKey2, Contents2);
            strategy.Save(entityName2, entityKey1, Contents3);

            var allkeys1 = strategy.GetAllPersistedKeys(entityName1).ToArray();
            Assert.Equal(2, allkeys1.Count());
            Assert.Equal(1, allkeys1.Count(key => key == entityKey1));
            Assert.Equal(1, allkeys1.Count(key => key == entityKey2));

            var allkeys2 = strategy.GetAllPersistedKeys(entityName2).ToArray();
            Assert.Equal(1, allkeys2.Count());
            Assert.Equal(1, allkeys2.Count(key => key == entityKey1));
            Assert.Equal(0, allkeys2.Count(key => key == entityKey2));
        }

        [Fact]
        public void Deletes()
        {
            strategy.Save(entityName1, entityKey1, Contents1);
            strategy.Save(entityName1, entityKey2, Contents2);
            strategy.Save(entityName2, entityKey1, Contents3);

            strategy.Delete(entityName1, entityKey1);

            var data1 = strategy.Load(entityName1, entityKey1);
            var data2 = strategy.Load(entityName1, entityKey2);
            var data3 = strategy.Load(entityName2, entityKey1);

            Assert.True(string.IsNullOrEmpty(data1));
            Assert.False(string.IsNullOrEmpty(data2));
            Assert.False(string.IsNullOrEmpty(data3));
        }

        [Fact]
        public void CreatesBackup()
        {
            strategy.Save(entityName1, entityKey1, Contents1);
            var filePath = strategy.CreateCopy(entityName1, entityKey1);
            Trace.WriteLine("filepath: " + filePath);
            Assert.True(File.Exists(filePath));
            Assert.Equal(Contents1, File.ReadAllText(filePath));
        }
    }
}
