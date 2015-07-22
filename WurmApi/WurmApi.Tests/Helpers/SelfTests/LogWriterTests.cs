using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Wurm.Logs;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Helpers.SelfTests
{
    [TestFixture]
    class LogWriterTests : WurmApiFixtureBase
    {
        private TestPak testDir;
        private LogWriter monthlyWriter;
        private LogWriter dailyWriter;
        private string pathDaily;
        private string pathMonthly;

        [SetUp]
        public void Setup()
        {
            testDir = CreateTestPakEmptyDir();
            this.pathDaily = Path.Combine(testDir.DirectoryFullPath, "_event.2014-01-01.txt");
            this.pathMonthly = Path.Combine(testDir.DirectoryFullPath, "_event.2014-01.txt");
            monthlyWriter = new LogWriter(pathDaily, new DateTime(2014, 1, 1), false);
            dailyWriter = new LogWriter(pathMonthly, new DateTime(2014, 1, 1), false);
        }

        [Test]
        public void WritesCorrectlyFormattedEntries()
        {
            var validLogEntry1 = new LogEntry()
            {
                Content = "Contents1",
                Source = "Source",
                Timestamp = new DateTime(2014, 1, 1, 1, 1, 1)
            };
            var validLogEntry2 = new LogEntry()
            {
                Content = "Contents2",
                Timestamp = new DateTime(2014, 1, 1, 1, 1, 2)
            };
            dailyWriter.WriteSection(new[] { validLogEntry1 }, true);
            dailyWriter.WriteSection(new[] { validLogEntry2 });
            monthlyWriter.WriteSection(new [] { validLogEntry1 }, true);
            monthlyWriter.WriteSection(new [] { validLogEntry2 });

            var dailyContents = File.ReadAllText(pathDaily);
            var monthlyContents = File.ReadAllText(pathMonthly);

            const string ExpectedDailyContents = "Logging started 2014-01-01\r\n[01:01:01] <Source> Contents1\r\n[01:01:02] Contents2\r\n";
            const string ExpectedMonthlyContents = "Logging started 2014-01-01\r\n[01:01:01] <Source> Contents1\r\n[01:01:02] Contents2\r\n";

            Expect(dailyContents, EqualTo(ExpectedDailyContents));
            Expect(monthlyContents, EqualTo(ExpectedMonthlyContents));
        }
    }
}
