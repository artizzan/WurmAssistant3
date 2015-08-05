using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Testing;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Modules.Wurm.LogDefinitions;
using AldurSoft.WurmApi.Modules.Wurm.LogFiles;
using AldurSoft.WurmApi.Modules.Wurm.LogsMonitor;
using AldurSoft.WurmApi.Modules.Wurm.Paths;
using AldurSoft.WurmApi.Tests.Helpers;
using AldurSoft.WurmApi.Tests.Tests.Modules.Wurm;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.Tests.WurmLogsMonitorImpl
{
    [TestFixture]
    class WurmLogsMonitorTests : WurmTests
    {
        protected IWurmLogsMonitor System;
        protected readonly CharacterName TestGuyCharacterName = new CharacterName("Testguy");
        StubbableTime.StubScope scope;

        [SetUp]
        public void Setup()
        {
            scope = TimeStub.CreateStubbedScope();
            scope.OverrideNowOffset(new DateTimeOffset(2014, 1, 1, 0, 0, 0, TimeSpan.Zero));
            scope.OverrideNow(new DateTime(2014, 1, 1, 0, 0, 0));
            ClientMock.PopulateFromDir(Path.Combine(TestPaksDirFullPath, "logs-samples-forLogsMonitor"));
            System = Fixture.WurmApiManager.WurmLogsMonitor;
        }

        [TearDown]
        public void Teardown()
        {
            scope.Dispose();
        }

        [Test]
        public void SubscribeSingleLogType_TriggerWithSingleEvent()
        {
            EventAwaiter<LogsMonitorEventArgs> awaiter = new EventAwaiter<LogsMonitorEventArgs>();
            var handler = awaiter.GetEventHandler();
            System.Subscribe(TestGuyCharacterName, LogType.Event, handler);

            WriteToLogFile("_Event.2014-01-01.txt", "Logging started 2014-01-01");
            WriteToLogFile("_Event.2014-01-01.txt", "[00:00:12] Horses like this one have many uses.");

            awaiter.WaitInvocations(1);
            awaiter.WaitUntilMatch(list =>
            {
                var match1 = list.Any(
                    args =>
                        args.CharacterName == TestGuyCharacterName
                        && args.WurmLogEntries.Any(
                            entry =>
                                entry.Content == "Horses like this one have many uses."
                                && entry.Timestamp == new DateTime(2014,1,1, 0,0,12)));
                return match1;
            });

            System.Unsubscribe(TestGuyCharacterName, handler);

            WriteToLogFile("_Event.2014-01-01.txt", "[00:00:13] Horses like this one have many uses.");

            Thread.Sleep(1000);
            var unexpected = awaiter.Invocations.Any(
                args =>
                    args.WurmLogEntries.Any(
                        entry =>
                            entry.Timestamp == new DateTime(2014,1,1, 0,0,13)));

            Expect(unexpected, False);
        }

        [Test]
        public void SubscribePmLog_TriggerWithSingleEvent()
        {
            List<LogsMonitorEventArgs> anotherGuyEvents = new List<LogsMonitorEventArgs>();
            List<LogsMonitorEventArgs> unrelatedGuyEvents = new List<LogsMonitorEventArgs>();
            WriteToLogFile("PM__Anotherguy.2014-01-01.txt", "Logging started 2014-01-01");

            Thread.Sleep(20);

            EventHandler<LogsMonitorEventArgs> anotherGuyEventHandler = (sender, args) => anotherGuyEvents.Add(args);
            EventHandler<LogsMonitorEventArgs> unrelatedGuyEventHandler = (sender, args) => unrelatedGuyEvents.Add(args);
            System.SubscribePm(TestGuyCharacterName, new CharacterName("Anotherguy"), anotherGuyEventHandler);
            System.SubscribePm(TestGuyCharacterName, new CharacterName("Unrelatedguy"), unrelatedGuyEventHandler);
            WriteToLogFile("PM__Anotherguy.2014-01-01.txt",
                "[00:00:12] <Anotherguy> Horses like this one have many uses.");

            Thread.Sleep(20);

            Expect(anotherGuyEvents.Count, EqualTo(1));
            Expect(unrelatedGuyEvents.Count, EqualTo(0));
            Expect(anotherGuyEvents.Single().CharacterName, EqualTo(TestGuyCharacterName));
            Expect(anotherGuyEvents.Single().WurmLogEntries.Count(), EqualTo(1));
            Expect(anotherGuyEvents.Single().WurmLogEntries.Single().Timestamp, EqualTo(TimeStub.LocalNow));
            Expect(anotherGuyEvents.Single().WurmLogEntries.Single().Content,
                EqualTo("Horses like this one have many uses."));
            Expect(anotherGuyEvents.Single().WurmLogEntries.Single().Source, EqualTo("Anotherguy"));

            System.UnsubscribePm(TestGuyCharacterName, new CharacterName("Anotherguy"), anotherGuyEventHandler);

            WriteToLogFile("PM__Anotherguy.2014-01-01.txt",
                "[00:00:13] <Anotherguy> Horses like this one have many uses.");

            Thread.Sleep(20);

            Expect(anotherGuyEvents.Count, EqualTo(1));
            Expect(unrelatedGuyEvents.Count, EqualTo(0));
        }

        [Test]
        public void SubscribeAll_TriggerWithSingleEvent()
        {
            List<LogsMonitorEventArgs> allEvents = new List<LogsMonitorEventArgs>();
            EventHandler<LogsMonitorEventArgs> eventHandler = (sender, args) => allEvents.Add(args);
            System.SubscribeAllActive(eventHandler);

            // should not gather this, since no normal subscription for these events yet
            WriteToLogFile("_Event.2014-01-01.txt", "Logging started 2014-01-01");

            Thread.Sleep(20);

            EventHandler<LogsMonitorEventArgs> regularSubHandler = (sender, args) => DoNothing();
            System.Subscribe(TestGuyCharacterName, LogType.Event, regularSubHandler);

            // should gather this, since now a subscription active
            WriteToLogFile("_Event.2014-01-01.txt", "[00:00:12] Horses like this one have many uses.");

            Thread.Sleep(20);

            Expect(allEvents.Count, EqualTo(1));
            Expect(allEvents.Single().CharacterName, EqualTo(TestGuyCharacterName));
            Expect(allEvents.Single().WurmLogEntries.Count(), EqualTo(1));
            Expect(allEvents.Single().WurmLogEntries.Single().Timestamp, EqualTo(TimeStub.LocalNow));
            Expect(allEvents.Single().WurmLogEntries.Single().Content, EqualTo("Horses like this one have many uses."));

            Thread.Sleep(20);

            System.Unsubscribe(TestGuyCharacterName, regularSubHandler);

            // should not gather this, since all subs are unsubscribed
            WriteToLogFile("_Event.2014-01-01.txt", "[00:00:13] Horses like this one have many uses.");

            Thread.Sleep(20);

            Expect(allEvents.Count, EqualTo(1));
        }

        void WriteToLogFile(string fileName, string contents)
        {
            var dirpath = Path.Combine(ClientMock.InstallDirectory.FullPath, "players", "Testguy", "logs");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            var filepath = Path.Combine(dirpath, fileName);
            if (!File.Exists(filepath))
            {
                File.Create(filepath).Dispose();
            }
            File.AppendAllLines(filepath, new[] {contents});
        }
    }
}
