using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Networking;
using AldursLab.WurmApi.Tests.Helpers;
using AldursLab.WurmApi.Tests.TempDirs;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldursLab.WurmApi.Tests.Tests.Modules.Wurm.Servers
{
    [TestFixture]
    class WurmServersTests : WurmTests
    {
        protected IWurmServers System => Fixture.WurmApiManager.Servers;
        public StubbableTime.StubScope Timescope;
        protected readonly CharacterName TestGuyCharacterName = new CharacterName("Testguy");
        protected DateTime MockedNow = new DateTime(2014, 12, 15, 0, 0, 0, DateTimeKind.Local);

        protected DirectoryHandle HtmlWebRequestsDir;

        [SetUp]
        public virtual void Setup()
        {
            // gotcha: this will spam trace output with exceptions:
            Fixture.HttpWebRequestsMock.Arrange(requests => requests.GetResponseAsync(Arg.IsAny<string>()))
                   .Throws<NotSupportedException>();

            HtmlWebRequestsDir =
                TempDirectoriesFactory.CreateByUnzippingFile(Path.Combine(TestPaksZippedDirFullPath,
                    "WurmServerTests-wurmdir-webrequests.7z"));

            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "WurmServerTests-wurmdir.7z"));

            Timescope = TimeStub.CreateStubbedScope();
            Timescope.SetAllLocalTimes(MockedNow);
        }

        [TearDown]
        public void Teardown()
        {
            Timescope.Dispose();
        }

        [Test]
        public void TryGetByName_Gets()
        {
            var server = System.GetByName(new ServerName("Exodus"));
            Expect(server, Not.Null);
        }

        [Test]
        public void All_Gets()
        {
            var servers = System.All.ToArray();
            Expect(servers.Length, GreaterThan(0));
        }

        [Test]
        public void GetsForUnknownServer()
        {
            var server = System.GetByName(new ServerName("Idonotexist"));
            Expect(server.ServerGroup, EqualTo(new ServerGroup("SERVERSCOPED:IDONOTEXIST")));
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
                event201412Writer =
                    new LogWriter(
                        Path.Combine(ClientMock.InstallDirectory.FullPath,
                            "players",
                            "Testguy",
                            "Logs",
                            "_Event.2014-12.txt"),
                        new DateTime(2014, 12, 1),
                        true);

                server = System.GetByName(serverName);
            }

            [Test]
            public void GetsServer_HasCorrectProperties()
            {
                Expect(server.ServerGroup, EqualTo(new ServerGroup("FREEDOM")));
                Expect(server.ServerName, EqualTo(serverName));
            }

            [Test]
            public async Task ObtainsServerTimesFromLogHistory()
            {
                var uptime = await server.TryGetCurrentUptimeAsync();
                var datetime = await server.TryGetCurrentTimeAsync();
                Expect(uptime, GreaterThanOrEqualTo(new TimeSpan(1, 23, 22)));
                Expect(datetime, GreaterThanOrEqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Luck, 21, 07, 50)));
            }

            [Test]
            public async Task ObtainsServerTimesFromLogHistory_AdjustedForRealTime()
            {
                var uptime = await server.TryGetCurrentUptimeAsync();
                var datetime = await server.TryGetCurrentTimeAsync();
                Expect(uptime, EqualTo(new TimeSpan(2, 6, 19, 32)));
                Expect(datetime, EqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Wurm, 04, 05, 22)));
            }

            [Test]
            public async Task ObtainsServerTimesFromLiveLogs()
            {
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
                
                Thread.Sleep(1000);

                var uptime = await server.TryGetCurrentUptimeAsync();
                var datetime = await server.TryGetCurrentTimeAsync();
                Expect(uptime, EqualTo(new TimeSpan(3, 15, 30, 0)));
                Expect(datetime, EqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Ant, 12, 01, 40)));
            }

            [Test]
            public async Task ObtainsServerTimesFromWeb()
            {
                // move forward in time, so that log history data becomes too outdated to be valid
                var newMockedNow = new DateTime(2014, 12, 30, 0, 0, 0);
                Timescope.SetAllLocalTimes(newMockedNow);

                var responseMock = Mock.Create<HttpWebResponse>();
                var htmlBytes = File.ReadAllBytes(Path.Combine(HtmlWebRequestsDir.AbsolutePath, "Exodus.txt"));
                responseMock.Arrange(response => response.GetResponseStream())
                    .Returns(() => new MemoryStream(htmlBytes));
                responseMock.Arrange(response => response.LastModified).Returns(newMockedNow);
                Fixture.HttpWebRequestsMock.Arrange(requests => requests.GetResponseAsync(Arg.IsAny<string>()))
                    .Returns(() => Task.FromResult(responseMock));

                var uptime = await server.TryGetCurrentUptimeAsync();
                var datetime = await server.TryGetCurrentTimeAsync();
                Expect(uptime, EqualTo(new TimeSpan(3, 15, 30, 0)));
                Expect(datetime, EqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Ant, 12, 01, 40)));
            }

            [Test]
            public async Task ObtainsServerTime_WhenUnknownServer()
            {
                var logWriter =
                    new LogWriter(
                        Path.Combine(ClientMock.InstallDirectory.FullPath,
                            "players",
                            "Testguy",
                            "Logs",
                            "_Event.2014-12.txt"),
                        new DateTime(2014, 12, 1),
                        true);

                var unknownServer = System.GetByName("Idonotexist");

                logWriter.WriteSection(
                    new Collection<LogEntry>()
                    {
                        new LogEntry(MockedNow, string.Empty, "42 other players are online. You are on Idonotexist (765 totally in Wurm).")
                    },
                    true);
                logWriter.WriteSection(
                    new Collection<LogEntry>()
                    {
                        new LogEntry(MockedNow, string.Empty, "The server has been up 3 days, 15 hours and 30 minutes.")
                    },
                    true);
                logWriter.WriteSection(
                    new Collection<LogEntry>()
                    {
                        new LogEntry(MockedNow,
                            String.Empty,
                            "It is 12:01:40 on day of the Ant in week 2 of the Snake's starfall in the year of 1045.")
                    });

                Thread.Sleep(1000);

                var uptime = await unknownServer.TryGetCurrentUptimeAsync();
                var datetime = await unknownServer.TryGetCurrentTimeAsync();
                Expect(uptime, EqualTo(new TimeSpan(3, 15, 30, 0)));
                Expect(datetime, EqualTo(new WurmDateTime(1045, WurmStarfall.Snake, 2, WurmDay.Ant, 12, 01, 40)));
            }
        }

        [TearDown]
        public void FixtureTeardown()
        {
            Timescope.Dispose();
        }
    }
}
