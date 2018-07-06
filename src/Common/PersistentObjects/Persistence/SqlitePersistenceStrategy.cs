using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using JetBrains.Annotations;

namespace AldursLab.PersistentObjects.Persistence
{
    public class SqlitePersistenceStrategy : IPersistenceStrategy
    {
        readonly string databasePath;

        public SqlitePersistenceStrategy([NotNull] PersistenceManagerConfig persistenceManagerConfig)
        {
            if (persistenceManagerConfig == null) throw new ArgumentNullException(nameof(persistenceManagerConfig));

            string rootPath = persistenceManagerConfig.DataStoreDirectoryPath;

            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            databasePath = Path.Combine(rootPath, "database.sqlite");

            UpdateSchema();
            ImportFromObsoleteFlatFiles(persistenceManagerConfig);
        }

        private void UpdateSchema()
        {
            ExecDbAction(connection =>
            {
                connection.Execute(
                    @"create table if not exists PersistentObjects
                      (
                         Id            integer primary key AUTOINCREMENT,
                         CollectionId  text not null,
                         Key           text not null,
                         Content       text null
                      );
                      create unique index if not exists PersistentObjectsSetObjectIdIndex on PersistentObjects ( CollectionId, Key );
                    ");
            });
        }

        void ImportFromObsoleteFlatFiles(PersistenceManagerConfig config)
        {
            var logFilePath = Path.Combine(config.DataStoreDirectoryPath, "migration.log");

            var allObjects = new FlatFilesDataImporter(config).GetAllObjects().ToList();
            if (allObjects.Any())
            {
                if (!File.Exists(logFilePath))
                {
                    foreach (var dataObject in allObjects)
                    {
                        Save(dataObject.CollectionId, dataObject.Key, dataObject.Content);
                    }
                    File.WriteAllText(logFilePath, $"{DateTime.Now} : migrated data from flat files to sqlite");
                }

                foreach (var directory in new DirectoryInfo(config.DataStoreDirectoryPath).GetDirectories())
                {
                    directory.Delete(recursive: true);
                }

                var mapFile = Path.Combine(config.DataStoreDirectoryPath, "data.map");
                if (File.Exists(mapFile)) File.Delete(mapFile);
            }
        }

        public string TryLoad(string collectionId, string key)
        {
            string result = null;

            ExecDbAction(connection =>
            {
                result = connection.QuerySingleOrDefault<string>(
                    "select Content from PersistentObjects where CollectionId = @CollectionId and Key = @Key",
                    new { CollectionId = collectionId, Key = key });
            });

            return result;
        }

        public void Save(string collectionId, string key, string content)
        {
            ExecDbAction(connection =>
            {
                var recordId = connection.QueryFirstOrDefault<int?>(
                    "select Id from PersistentObjects where CollectionId = @CollectionId and Key = @Key",
                    new { CollectionId = collectionId, Key = key });

                if (recordId == null)
                {
                    connection.Execute(
                        "insert into PersistentObjects (CollectionId, Key, Content) values (@CollectionId, @Key, @Content)",
                        new { CollectionId = collectionId, Key = key, Content = content });
                }
                else
                {
                    connection.Execute(
                        "update PersistentObjects set Content = @Content where CollectionId = @CollectionId and Key = @Key",
                        new { CollectionId = collectionId, Key = key, Content = content });
                }
            });
        }

        public void TryDeleteData(string collectionId, string key)
        {

            ExecDbAction(connection =>
            {
                connection.Execute(
                    "delete from PersistentObjects where CollectionId = @CollectionId and Key = @Key",
                    new { CollectionId = collectionId, Key = key });
            });
        }

        private void ExecDbAction(Action<SQLiteConnection> action)
        {
            using (var cnn = CreateDbConnection())
            {
                cnn.Open();
                using (var t = cnn.BeginTransaction(IsolationLevel.Serializable))
                {
                    action(cnn);
                    t.Commit();
                }
            }
        }

        SQLiteConnection CreateDbConnection()
        {
            return new SQLiteConnection("Data Source=" + databasePath + ";Version=3;Pooling=True;Max Pool Size=100;");
        }
    }
}