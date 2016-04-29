using System;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;
using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.PersistentObjects.FlatFiles;
using AldursLab.WurmApi.Tests.Integration.TempDirs;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.Modules.Wurm.ServerHistory
{
    [TestFixture]
    class SortedServerHistoryTests : AssertionHelper
    {
        protected DirectoryHandle DataDir;
        protected SortedServerHistory System;

        [SetUp]
        public void Setup()
        {
            DataDir = TempDirectoriesFactory.CreateEmpty();
            var lib = new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(DataDir.AbsolutePath));
            System = new SortedServerHistory(lib.DefaultCollection.GetObject<WurmApi.Modules.Wurm.ServerHistory.PersistentModel.ServerHistory>("default"));
        }

        [Test]
        public void Recalculates()
        {
            var dateTime1 = new DateTime(2014, 1, 1, 1, 0, 0);
            var dateTime2 = new DateTime(2014, 1, 1, 1, 1, 0);
            var dateTime3 = new DateTime(2014, 1, 1, 2, 2, 2);
            var serverNameA = new ServerName("A");
            var serverNameB = new ServerName("B");
            var serverNameC = new ServerName("C");

            System.Insert(
                new ServerStamp() { ServerName = serverNameA, Timestamp = dateTime1 },
                new ServerStamp() { ServerName = serverNameA, Timestamp = dateTime1.AddSeconds(1) },
                new ServerStamp() { ServerName = serverNameB, Timestamp = dateTime2 },
                new ServerStamp() { ServerName = serverNameB, Timestamp = dateTime2.AddSeconds(1) },
                new ServerStamp() { ServerName = serverNameC, Timestamp = dateTime3 },
                new ServerStamp() { ServerName = serverNameC, Timestamp = dateTime3.AddSeconds(1) }
                );

            Expect(System.TryGetServerAtStamp(dateTime1.AddSeconds(-1)), Null);
            Expect(System.TryGetServerAtStamp(dateTime1), EqualTo(serverNameA));
            Expect(System.TryGetServerAtStamp(dateTime1.AddSeconds(1)), EqualTo(serverNameA));

            Expect(System.TryGetServerAtStamp(dateTime2.AddSeconds(-1)), EqualTo(serverNameA));
            Expect(System.TryGetServerAtStamp(dateTime2), EqualTo(serverNameB));
            Expect(System.TryGetServerAtStamp(dateTime2.AddSeconds(1)), EqualTo(serverNameB));

            Expect(System.TryGetServerAtStamp(dateTime3.AddSeconds(-1)), EqualTo(serverNameB));
            Expect(System.TryGetServerAtStamp(dateTime3), EqualTo(serverNameC));
            Expect(System.TryGetServerAtStamp(dateTime3.AddSeconds(1)), EqualTo(serverNameC));
        }
    }
}
