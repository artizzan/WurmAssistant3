using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core;
using AldurSoft.Core.Testing;
using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Tests.Helpers;
using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmApi.Tests.Tests.WurmServerHistoryImpl
{
    [TestFixture]
    class WurmServerHistoryTests : WurmApiIntegrationFixtureBase
    {
        protected IWurmServerHistory ServerHistory;
        protected MockableClock.MockedScope ClockScope;

        private readonly CharacterName characterNameTestguy = new CharacterName("Testguy");
        private readonly CharacterName characterNameTestguytwo = new CharacterName("Testguytwo");

        [SetUp]
        public void Init()
        {
            ClockScope = MockableClock.CreateScope();
            ClockScope.LocalNow = new DateTime(2014, 12, 14, 17, 10, 0);
            ClockScope.LocalNowOffset = new DateTime(2014, 12, 14, 17, 10, 0);

            ConstructApi(Path.Combine(TestPaksDirFullPath, "WurmServerHistory-wurmdir"));
            ServerHistory = WurmApiManager.WurmServerHistory;
            //WurmApiManager.Update();
        }

        [TestFixture]
        public class TryGetServer : WurmServerHistoryTests
        {
            [Test]
            public async Task Gets()
            {
                var server = await ServerHistory.GetServerAsync(characterNameTestguy, new DateTime(2014, 12, 14, 17, 3, 0));
                Expect(server, EqualTo(new ServerName("Exodus")));
            }

            [Test]
            public async Task NullWhenNoData()
            {
                var server = await ServerHistory.GetServerAsync(characterNameTestguy, new DateTime(2014, 12, 14, 17, 2, 0));
                Expect(server, Null);
            }

            [Test]
            public async Task GetsWhenMoreData()
            {
                var server1 = await ServerHistory.GetServerAsync(characterNameTestguytwo, new DateTime(2014, 12, 14, 17, 0, 0));
                Expect(server1, Null);
                var server2 = await ServerHistory.GetServerAsync(characterNameTestguytwo, new DateTime(2014, 12, 14, 17, 5, 0));
                Expect(server2, EqualTo(new ServerName("Exodus")));
                var server3 = await ServerHistory.GetServerAsync(characterNameTestguytwo, new DateTime(2014, 12, 14, 17, 9, 59));
                Expect(server3, EqualTo(new ServerName("Chaos")));
                var currentServer1 = await ServerHistory.GetCurrentServerAsync(characterNameTestguy);
                Expect(currentServer1, EqualTo(new ServerName("Exodus")));
                var currentServer2 = await ServerHistory.GetCurrentServerAsync(characterNameTestguytwo);
                Expect(currentServer2, EqualTo(new ServerName("Chaos")));
            }

            [Test]
            public async Task GetsAfterLiveEvent()
            {
                //WurmApiManager.Update();
                // next day
                ClockScope.LocalNow = new DateTime(2014, 12, 15, 3, 5, 0);
                ClockScope.LocalNowOffset = new DateTime(2014, 12, 15, 3, 5, 0);

                // verify current
                var nameCurrent1 = await ServerHistory.GetCurrentServerAsync(characterNameTestguy);
                Expect(nameCurrent1, EqualTo(new ServerName("Exodus")));

                // add live event
                var path = Path.Combine(
                    WurmDir.DirectoryFullPath,
                    "players",
                    "Testguy",
                    "logs",
                    "_Event.2014-12.txt");
                var logwriter = new LogWriter(path, new DateTime(2014, 12, 1), true);
                //Trace.WriteLine("----- before write");
                //Trace.Write(File.ReadAllText(path));
                //Trace.WriteLine("-----");
                logwriter.WriteSection(
                    new Collection<LogEntry>()
                    {
                        new LogEntry(new DateTime(2014, 12, 15, 3, 4, 0), String.Empty,
                            "42 other players are online. You are on Abuzabi (765 totally in Wurm).")
                    }, true);
                //Trace.WriteLine("----- after write");
                //Trace.Write(File.ReadAllText(path));
                //Trace.WriteLine("-----");

                // refresh and assert
                //WurmApiManager.Update();
                var nameBefore = await ServerHistory.GetServerAsync(characterNameTestguy, new DateTime(2014, 12, 15, 3, 3, 0));
                Expect(nameBefore, EqualTo(new ServerName("Exodus")));
                var nameAfter = await ServerHistory.GetServerAsync(characterNameTestguy, new DateTime(2014, 12, 15, 3, 5, 0));
                Expect(nameAfter, EqualTo(new ServerName("Abuzabi")));
                var nameCurrent2 = await ServerHistory.GetCurrentServerAsync(characterNameTestguy);
                Expect(nameCurrent2, EqualTo(new ServerName("Abuzabi")));
            }
        }
    }
}
