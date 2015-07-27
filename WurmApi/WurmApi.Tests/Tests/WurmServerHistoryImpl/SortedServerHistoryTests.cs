using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Modules.DataContext;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.ServerHistoryModel;
using AldurSoft.WurmApi.Modules.Wurm.ServerHistory;
using Moq;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.WurmServerHistoryImpl
{
    [TestFixture]
    class SortedServerHistoryTests : FixtureBase
    {
        protected TestPak DataDir;
        protected SortedServerHistory System;

        [SetUp]
        public void Setup()
        {
            DataDir = CreateTestPakEmptyDir();
            IWurmApiDataContext dataContext = new WurmApiDataContext(DataDir.DirectoryFullPath, Mock.Of<ISimplePersistLogger>());
            System = new SortedServerHistory(dataContext.ServerHistory.Get(new EntityKey("Sample")));
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
