using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core;
using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Modules.Networking;
using AldurSoft.WurmApi.Tests.Helpers;
using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmApi.Tests.Tests.WurmServersImpl
{
    [TestFixture]
    class WurmServersTests : WurmApiIntegrationFixtureBase
    {
        protected IWurmServers Servers;
        public MockableClock.MockedScope Timescope;
        protected readonly CharacterName TestGuyCharacterName = new CharacterName("Testguy");
        protected DateTime MockedNow = new DateTime(2014, 12, 15, 0, 0, 0);

        protected TestPak HtmlWebRequestsDir;

        [SetUp]
        public virtual void Setup()
        {
            HtmlWebRequestsDir =
                CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "WurmServerTests-wurmdir-webrequests"));

            Timescope = MockableClock.CreateScope();
            Timescope.SetAllLocalTimes(MockedNow);

            ConstructApi(Path.Combine(TestPaksDirFullPath, "WurmServerTests-wurmdir"));
            Servers = WurmApiManager.WurmServers;
        }

        [Test]
        public void TryGetByName_Gets()
        {
            var server = Servers.GetByName(new ServerName("Exodus"));
            Expect(server, !Null);
        }

        [Test]
        public void All_Gets()
        {
            var servers = Servers.All.ToArray();
            Expect(servers.Length, GreaterThan(0));
        }

        [TestFixture]
        public class WurmServerTests : WurmServersTests
        {
            private readonly ServerName serverName = new ServerName("Exodus");
            private IWurmServer server;
            private LogWriter event201412Writer;

            [SetUp]
            public override void Setup()
            {
                base.Setup();
                server = Servers.GetByName(serverName);
                event201412Writer =
                    new LogWriter(
                        Path.Combine(WurmDir.DirectoryFullPath, "players", "Testguy", "Logs", "_Event.2014-12.txt"),
                        new DateTime(2014, 12, 1),
                        true);
            }

            [Test]
            public void GetsServer_HasCorrectProperties()
            {
                Expect(server.ServerGroup.ServerGroupId, EqualTo(ServerGroupId.Freedom));
                Expect(server.ServerName, EqualTo(serverName));
            }

            [Test]
            public async Task ObtainsServerTimesFromLogHistory()
            {
                var uptime = await server.TryGetCurrentUptime();
                var datetime = await server.TryGetCurrentTime();
                Expect(uptime, GreaterThanOrEqualTo(new TimeSpan(1, 23, 22)));
                Expect(datetime, GreaterThanOrEqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Luck, 21, 07, 50)));
            }

            [Test]
            public async Task ObtainsServerTimesFromLogHistory_AdjustedForRealTime()
            {
                var uptime = await server.TryGetCurrentUptime();
                var datetime = await server.TryGetCurrentTime();
                Expect(uptime, EqualTo(new TimeSpan(2, 6, 19, 32)));
                Expect(datetime, EqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Wurm, 04, 05, 22)));
            }

            [Test]
            public async Task ObtainsServerTimesFromLiveLogs()
            {
                WurmApiManager.WurmLogsMonitor.Subscribe(TestGuyCharacterName, LogType.Skills, DoNothing);
                event201412Writer.WriteSection(
                    new Collection<LogEntry>()
                    {
                        new LogEntry(MockedNow, string.Empty, "The server has been up 3 days, 15 hours and 30 minutes.")
                    }, true);
                event201412Writer.WriteSection(
                    new Collection<LogEntry>()
                    {
                        new LogEntry(MockedNow, String.Empty,
                            "It is 12:01:40 on day of the Ant in week 2 of the Snake's starfall in the year of 1045.")
                    });
                //WurmApiManager.Update();
                var uptime = await server.TryGetCurrentUptime();
                var datetime = await server.TryGetCurrentTime();
                Expect(uptime, EqualTo(new TimeSpan(3, 15, 30, 0)));
                Expect(datetime, EqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Ant, 12, 01, 40)));
            }

            [Test]
            public async Task ObtainsServerTimesFromWeb()
            {
                // move forward in time, so that log history data becomes too outdated to be valid
                var newMockedNow = new DateTime(2014, 12, 30, 0, 0, 0);
                Timescope.SetAllLocalTimes(newMockedNow);

                var responseMock = Automocker.Create<HttpWebResponse>().GetMock();
                var htmlBytes = File.ReadAllBytes(Path.Combine(HtmlWebRequestsDir.DirectoryFullPath, "Exodus.txt"));
                responseMock.Setup(response => response.GetResponseStream())
                    .Returns(() => new MemoryStream(htmlBytes));
                responseMock.Setup(response => response.LastModified).Returns(newMockedNow);
                base.HttpWebRequestsMock.Setup(requests => requests.GetResponse(It.IsAny<string>()))
                    .Returns(() => Task.FromResult(responseMock.Object));

                var uptime = await server.TryGetCurrentUptime();
                var datetime = await server.TryGetCurrentTime();
                Expect(uptime, EqualTo(new TimeSpan(3, 15, 30, 0)));
                Expect(datetime, EqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Ant, 12, 01, 40)));
            }
        }

        [TearDown]
        public override void Teardown()
        {
            ExecuteAll(Timescope.Dispose, base.Teardown);
        }
    }
}
