using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Modules.Wurm.LogDefinitions;
using AldurSoft.WurmApi.Modules.Wurm.LogFiles;
using AldurSoft.WurmApi.Modules.Wurm.LogsMonitor;
using AldurSoft.WurmApi.Modules.Wurm.Paths;
using Moq;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.WurmLogsMonitorImpl
{
    [TestFixture]
    class WurmLogsMonitorTests : WurmApiFixtureBase
    {
        protected TestPak WurmDir;
        protected WurmLogsMonitor System;
        private WurmCharacterDirectories wurmCharacterDirectories;
        private WurmLogFiles wurmLogFiles;

        [SetUp]
        public void Setup()
        {
            WurmDir = CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "logs-samples-forLogsMonitor"));

            IWurmInstallDirectory installDirectory = Mock.Of<IWurmInstallDirectory>();
            installDirectory.GetMock()
                .Setup(directory => directory.FullPath)
                .Returns(WurmDir.DirectoryFullPath);
            var internalEventAggregator = new InternalEventAggregator();
            //wurmCharacterDirectories = new WurmCharacterDirectories(new WurmPaths(installDirectory),
            //    new PublicEventInvoker(new SimpleMarshaller(), new LoggerStub()), internalEventAggregator);
            //wurmLogFiles = new WurmLogFiles(
            //    wurmCharacterDirectories,
            //    Mock.Of<ILogger>(),
            //    new WurmLogDefinitions(),
            //    internalEventAggregator,
            //    new PublicEventInvoker(new SimpleMarshaller(), new LoggerStub()));
            //System =
            //    new WurmLogsMonitor(
            //        wurmLogFiles,
            //        Mock.Of<ILogger>());
        }

        [Test]
        public void SubscribeSingleLogType_TriggerWithSingleEvent()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNowOffset = new DateTimeOffset(2014, 1, 1, 0, 0, 0, TimeSpan.Zero);
                scope.LocalNow = new DateTime(2014, 1, 1, 0, 0, 0);

                List<LogsMonitorEventArgs> events = new List<LogsMonitorEventArgs>();
                WriteToLogFile("_Event.2014-01-01.txt", "Logging started 2014-01-01");

                Thread.Sleep(20);
                RefreshSystems();

                EventHandler<LogsMonitorEventArgs> eventHandler = (sender, args) => events.Add(args);
                System.Subscribe(new CharacterName("Testguy"), LogType.Event, eventHandler);
                WriteToLogFile("_Event.2014-01-01.txt", "[00:00:12] Horses like this one have many uses.");

                Thread.Sleep(20);
                RefreshSystems();

                Expect(events.Count, EqualTo(1));
                Expect(events.Single().CharacterName, EqualTo(new CharacterName("Testguy")));
                Expect(events.Single().WurmLogEntries.Count(), EqualTo(1));
                Expect(events.Single().WurmLogEntries.Single().Timestamp, EqualTo(MockableClock.LocalNow));
                Expect(events.Single().WurmLogEntries.Single().Content, EqualTo("Horses like this one have many uses."));

                //System.Unsubscribe(TODO, eventHandler);

                WriteToLogFile("_Event.2014-01-01.txt", "[00:00:13] Horses like this one have many uses.");

                Thread.Sleep(20);
                RefreshSystems();

                Expect(events.Count, EqualTo(1));
            }
        }

        [Test]
        public void SubscribePmLog_TriggerWithSingleEvent()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNowOffset = new DateTimeOffset(2014, 1, 1, 0, 0, 0, TimeSpan.Zero);
                scope.LocalNow = new DateTime(2014, 1, 1, 0, 0, 0);

                List<LogsMonitorEventArgs> anotherGuyEvents = new List<LogsMonitorEventArgs>();
                List<LogsMonitorEventArgs> unrelatedGuyEvents = new List<LogsMonitorEventArgs>();
                WriteToLogFile("PM__Anotherguy.2014-01-01.txt", "Logging started 2014-01-01");

                Thread.Sleep(20);
                RefreshSystems();

                EventHandler<LogsMonitorEventArgs> anotherGuyEventHandler = (sender, args) => anotherGuyEvents.Add(args);
                EventHandler<LogsMonitorEventArgs> unrelatedGuyEventHandler = (sender, args) => unrelatedGuyEvents.Add(args);
                //System.SubscribePm(new CharacterName("Testguy"), "Anotherguy", anotherGuyEventHandler);
                //System.SubscribePm(new CharacterName("Testguy"), "Unrelatedguy", unrelatedGuyEventHandler);
                WriteToLogFile("PM__Anotherguy.2014-01-01.txt", "[00:00:12] <Anotherguy> Horses like this one have many uses.");

                Thread.Sleep(20);
                RefreshSystems();

                Expect(anotherGuyEvents.Count, EqualTo(1));
                Expect(unrelatedGuyEvents.Count, EqualTo(0));
                Expect(anotherGuyEvents.Single().CharacterName, EqualTo(new CharacterName("Testguy")));
                Expect(anotherGuyEvents.Single().WurmLogEntries.Count(), EqualTo(1));
                Expect(anotherGuyEvents.Single().WurmLogEntries.Single().Timestamp, EqualTo(MockableClock.LocalNow));
                Expect(anotherGuyEvents.Single().WurmLogEntries.Single().Content, EqualTo("Horses like this one have many uses."));
                Expect(anotherGuyEvents.Single().WurmLogEntries.Single().Source, EqualTo("Anotherguy"));

                //System.UnsubscribePm(TODO, TODO, anotherGuyEventHandler);

                WriteToLogFile("PM__Anotherguy.2014-01-01.txt", "[00:00:13] <Anotherguy> Horses like this one have many uses.");

                Thread.Sleep(20);
                RefreshSystems();

                Expect(anotherGuyEvents.Count, EqualTo(1));
                Expect(unrelatedGuyEvents.Count, EqualTo(0));
            }
        }

        [Test]
        public void SubscribeAll_TriggerWithSingleEvent()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNowOffset = new DateTimeOffset(2014, 1, 1, 0, 0, 0, TimeSpan.Zero);
                scope.LocalNow = new DateTime(2014, 1, 1, 0, 0, 0);

                List<LogsMonitorEventArgs> allEvents = new List<LogsMonitorEventArgs>();
                EventHandler<LogsMonitorEventArgs> eventHandler = (sender, args) => allEvents.Add(args);
                System.SubscribeAllActive(eventHandler);

                // should not gather this, since no normal subscription for these events yet
                WriteToLogFile("_Event.2014-01-01.txt", "Logging started 2014-01-01");

                Thread.Sleep(20);
                RefreshSystems();

                EventHandler<LogsMonitorEventArgs> regularSubHandler = (sender, args) => DoNothing();
                System.Subscribe(new CharacterName("Testguy"), LogType.Event, regularSubHandler);

                // should gather this, since now a subscription active
                WriteToLogFile("_Event.2014-01-01.txt", "[00:00:12] Horses like this one have many uses.");

                Thread.Sleep(20);
                RefreshSystems();

                Expect(allEvents.Count, EqualTo(1));
                Expect(allEvents.Single().CharacterName, EqualTo(new CharacterName("Testguy")));
                Expect(allEvents.Single().WurmLogEntries.Count(), EqualTo(1));
                Expect(allEvents.Single().WurmLogEntries.Single().Timestamp, EqualTo(MockableClock.LocalNow));
                Expect(allEvents.Single().WurmLogEntries.Single().Content, EqualTo("Horses like this one have many uses."));

                Thread.Sleep(20);
                RefreshSystems();

                //System.Unsubscribe(TODO, regularSubHandler);

                // should not gather this, since all subs are unsubscribed
                WriteToLogFile("_Event.2014-01-01.txt", "[00:00:13] Horses like this one have many uses.");

                Thread.Sleep(20);
                RefreshSystems();

                Expect(allEvents.Count, EqualTo(1));
            }
        }

        [Test]
        public void GathersEventsOnNewDay_StopsGatheringOnOldDay()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNowOffset = new DateTimeOffset(2014, 1, 1, 0, 0, 0, TimeSpan.Zero);
                scope.LocalNow = new DateTime(2014, 1, 1, 0, 0, 0);

                List<LogsMonitorEventArgs> events = new List<LogsMonitorEventArgs>();
                WriteToLogFile("_Event.2014-01-01.txt", "Logging started 2014-01-01");

                Thread.Sleep(20);
                RefreshSystems();

                EventHandler<LogsMonitorEventArgs> eventHandler = (sender, args) => events.Add(args);
                System.Subscribe(new CharacterName("Testguy"), LogType.Event, eventHandler);
                WriteToLogFile("_Event.2014-01-01.txt", "[00:00:12] Horses like this one have many uses.");

                Thread.Sleep(20);
                RefreshSystems();

                scope.LocalNowOffset = new DateTimeOffset(2014, 1, 2, 0, 0, 0, TimeSpan.Zero);
                scope.LocalNow = new DateTime(2014, 1, 2, 0, 0, 0);

                WriteToLogFile("_Event.2014-01-01.txt", "[02:12:22] Should not log this, because its yesterday log.");
                WriteToLogFile("_Event.2014-01-02.txt", "Logging started 2014-01-01");
                WriteToLogFile("_Event.2014-01-02.txt", "[00:00:01] Qwerty.");

                Thread.Sleep(20);
                RefreshSystems();

                Expect(events.Count, EqualTo(2));
                Expect(events.Any(args => args.WurmLogEntries.Any(entry => entry.Content == "Qwerty.")), True);
                Expect(events.Any(args => args.WurmLogEntries.Any(entry => entry.Content == "Should not log this, because its yesterday log.")), False);
            }
        }

        private void DoNothing()
        {
        }

        private void RefreshSystems()
        {
            //wurmCharacterDirectories.Refresh();
            //wurmLogFiles.Refresh();
            //System.Refresh();
        }

        [TearDown]
        public override void Teardown()
        {
            ExecuteAll(wurmCharacterDirectories.Dispose, wurmLogFiles.Dispose, base.Teardown);
        }

        private void WriteToLogFile(string fileName, string contents)
        {
            var dirpath = Path.Combine(WurmDir.DirectoryFullPath, "players", "Testguy", "logs");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            var filepath = Path.Combine(dirpath, fileName);
            if (!File.Exists(filepath))
            {
                File.Create(filepath).Dispose();
            }
            File.AppendAllLines(filepath, new[] { contents });
        }
    }
}
