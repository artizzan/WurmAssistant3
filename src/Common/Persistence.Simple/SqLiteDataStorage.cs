using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using AldursLab.Essentials.Synchronization;
using Dapper;

namespace AldursLab.Persistence.Simple
{
    public class SqLiteDataStorage : IDataStorage, IDisposable
    {
        readonly string databasePath;
        readonly FileLock fileLock;
        bool disposed;

        public SqLiteDataStorage(string rootPath)
        {
            if (rootPath == null) throw new ArgumentNullException(nameof(rootPath));

            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            fileLock = FileLock.EnterWithCreateWait(Path.Combine(rootPath, "dir.lock"), TimeSpan.FromSeconds(15));

            databasePath = Path.Combine(rootPath, "database.sqlite");

            UpdateSchema();
            ImportFromObsoleteFlatFiles(rootPath);
        }

        void ImportFromObsoleteFlatFiles(string rootPath)
        {
            var logFilePath = Path.Combine(rootPath, "migration.log");

            var allObjects = new FlatFilesDataStorageImporter(rootPath).GetAllObjects().ToList();
            if (allObjects.Any())
            {
                if (!File.Exists(logFilePath))
                {
                    foreach (var dataObject in allObjects)
                    {
                        Save(dataObject.SetId, dataObject.ObjectId, dataObject.Data);
                    }
                    File.WriteAllText(logFilePath, $"{DateTime.Now} : migrated data from flat files to sqlite");
                }

                foreach (var directory in new DirectoryInfo(rootPath).GetDirectories())
                {
                    directory.Delete(recursive:true);
                }
            }
        }

        private void UpdateSchema()
        {
            ExecDbAction(connection =>
            {
                connection.Execute(
                    @"create table if not exists PersistentObjects
                      (
                         Id          integer primary key AUTOINCREMENT,
                         SetId       text not null,
                         ObjectId    text not null,
                         Data        text null
                      );
                      create unique index if not exists PersistentObjectsSetObjectIdIndex on PersistentObjects ( SetId, ObjectId );
                    ");
            });
        }

        public void Save(string setId, string objectId, string data)
        {
            ThrowIfDisposed();

            ExecDbAction(connection =>
            {
                var recordId = connection.QueryFirstOrDefault<int?>(
                    "select Id from PersistentObjects where SetId = @SetId and ObjectId = @ObjectId",
                    new {SetId = setId, ObjectId = objectId});

                if (recordId == null)
                {
                    connection.Execute(
                        "insert into PersistentObjects (SetId, ObjectId, Data) values (@SetId, @ObjectId, @Data)",
                        new { SetId = setId, ObjectId = objectId, Data = data });
                }
                else
                {
                    connection.Execute(
                        "update PersistentObjects set Data = @Data where SetId = @SetId and ObjectId = @ObjectId",
                        new { SetId = setId, ObjectId = objectId, Data = data });
                }
            });
        }

        public string TryLoad(string setId, string objectId)
        {
            ThrowIfDisposed();

            string result = null;

            ExecDbAction(connection =>
                {
                    result = connection.QuerySingleOrDefault<string>(
                        "select Data from PersistentObjects where SetId = @SetId and ObjectId = @ObjectId",
                        new {SetId = setId, ObjectId = objectId});
                });

            return result;
        }

        public void Delete(string setId, string objectId)
        {
            ThrowIfDisposed();

            ExecDbAction(connection =>
            {
                connection.Execute(
                    "delete from PersistentObjects where SetId = @SetId and ObjectId = @ObjectId",
                    new { SetId = setId, ObjectId = objectId });
            });
        }

        public void DeleteObjectSet(string setId)
        {
            ThrowIfDisposed();

            ExecDbAction(connection =>
            {
                connection.Execute(
                    "delete from PersistentObjects where SetId = @SetId",
                    new { SetId = setId });
            });
        }

        public void DeleteAllObjectSets()
        {
            ThrowIfDisposed();

            ExecDbAction(connection =>
            {
                connection.Execute(
                    "delete from PersistentObjects");
            });
        }

        public void Dispose()
        {
            // this is necessary because System.Data.Sqlite does not release handle on SQLiteConnection Dispose, only on object finalization.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            fileLock.Dispose();
            disposed = true;
        }

        SQLiteConnection CreateDbConnection()
        {
            return new SQLiteConnection("Data Source=" + databasePath + ";Version=3;Pooling=True;Max Pool Size=100;");
        }

        void ExecDbAction(Action<SQLiteConnection> action)
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

        void ThrowIfDisposed()
        {
            if (disposed) throw new ObjectDisposedException(nameof(SqLiteDataStorage));
        }
    }
}
