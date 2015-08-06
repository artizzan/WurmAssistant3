using System;
using System.IO;
using System.Linq;
using System.Threading;
using AldursLab.Essentials;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.LogsMonitor
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
            //ClientMock.PopulateFromDir(Path.Combine(TestPaksDirFullPath, "logs-samples-forLogsMonitor"));
            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "logs-samples-forLogsMonitor.7z"));
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
            EventAwaiter<LogsMonitorEventArgs> anotherGuyAwaiter = new EventAwaiter<LogsMonitorEventArgs>();
            EventAwaiter<LogsMonitorEventArgs> unrelatedGuyAwaiter = new EventAwaiter<LogsMonitorEventArgs>();
            EventHandler<LogsMonitorEventArgs> anotherGuyEventHandler = anotherGuyAwaiter.GetEventHandler();
            EventHandler<LogsMonitorEventArgs> unrelatedGuyEventHandler = unrelatedGuyAwaiter.GetEventHandler();

            System.SubscribePm(TestGuyCharacterName, new CharacterName("Anotherguy"), anotherGuyEventHandler);
            System.SubscribePm(TestGuyCharacterName, new CharacterName("Unrelatedguy"), unrelatedGuyEventHandler);

            WriteToLogFile("PM__Anotherguy.2014-01-01.txt", "Logging started 2014-01-01");
            WriteToLogFile("PM__Anotherguy.2014-01-01.txt",
                "[00:00:12] <Anotherguy> Horses like this one have many uses.");

            anotherGuyAwaiter.WaitInvocations(1);
            anotherGuyAwaiter.WaitUntilMatch(list =>
            {
                var match1 = list.Any(
                    args =>
                        args.CharacterName == TestGuyCharacterName
                        && args.ConversationNameNormalized == "ANOTHERGUY"
                        && args.WurmLogEntries.Any(
                            entry =>
                                entry.Content == "Horses like this one have many uses."
                                && entry.Timestamp == new DateTime(2014, 1, 1, 0, 0, 12)
                                && entry.Source == "Anotherguy"));
                return match1;
            });

            Expect(unrelatedGuyAwaiter.Invocations.Count(), EqualTo(0));

            System.UnsubscribePm(TestGuyCharacterName, new CharacterName("Anotherguy"), anotherGuyEventHandler);

            WriteToLogFile("PM__Anotherguy.2014-01-01.txt",
                "[00:00:13] <Anotherguy> Horses like this one have many uses.");

            Thread.Sleep(1000);
            var unexpected1 = anotherGuyAwaiter.Invocations.Any(
                args =>
                    args.WurmLogEntries.Any(
                        entry =>
                            entry.Timestamp == new DateTime(2014, 1, 1, 0, 0, 13)));
            var unexpected2 = anotherGuyAwaiter.Invocations.Any(
                args =>
                    args.WurmLogEntries.Any(
                        entry =>
                            entry.Timestamp == new DateTime(2014, 1, 1, 0, 0, 13)));
            Expect(unexpected1, False);
            Expect(unexpected2, False);
        }

        [Test]
        public void SubscribeAll_TriggerWithSingleEvent()
        {
            EventAwaiter<LogsMonitorEventArgs> allAwaiter = new EventAwaiter<LogsMonitorEventArgs>();
            EventHandler<LogsMonitorEventArgs> allEventHandler = allAwaiter.GetEventHandler();
            System.SubscribeAllActive(allEventHandler);

            WriteToLogFile("_Event.2014-01-01.txt", "Logging started 2014-01-01");
            WriteToLogFile("_Event.2014-01-01.txt", "[00:00:12] Horses like this one have many uses.");

            allAwaiter.WaitInvocations(1);
            allAwaiter.WaitUntilMatch(list =>
            {
                var match1 = list.Any(
                    args =>
                        args.CharacterName == TestGuyCharacterName
                        && args.WurmLogEntries.Any(
                            entry =>
                                entry.Content == "Horses like this one have many uses."
                                && entry.Timestamp == new DateTime(2014, 1, 1, 0, 0, 12)));
                return match1;
            });
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
