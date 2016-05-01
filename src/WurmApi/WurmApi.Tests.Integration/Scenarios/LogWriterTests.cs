using System;
using System.IO;
using AldursLab.WurmApi.Tests.Integration.Helpers;
using AldursLab.WurmApi.Tests.Integration.Scenarios.Modules;
using AldursLab.WurmApi.Tests.Integration.TempDirs;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios
{
    [TestFixture]
    internal class LogWriterTests : TestsBase
    {
        DirectoryHandle testDir;
        LogWriter monthlyWriter;
        LogWriter dailyWriter;
        string pathDaily;
        string pathMonthly;

        [SetUp]
        public void Setup()
        {
            testDir = TempDirectoriesFactory.CreateEmpty();
            pathDaily = Path.Combine(testDir.AbsolutePath, "_event.2014-01-01.txt");
            pathMonthly = Path.Combine(testDir.AbsolutePath, "_event.2014-01.txt");
            monthlyWriter = new LogWriter(pathDaily, new DateTime(2014, 1, 1), false);
            dailyWriter = new LogWriter(pathMonthly, new DateTime(2014, 1, 1), false);
        }

        [Test]
        public void WritesCorrectlyFormattedEntries()
        {
            var validLogEntry1 = new LogEntry(new DateTime(2014, 1, 1, 1, 1, 1), "Source", "Contents1");
            var validLogEntry2 = new LogEntry(new DateTime(2014, 1, 1, 1, 1, 2), string.Empty, "Contents2");
            dailyWriter.WriteSection(new[] { validLogEntry1 }, true);
            dailyWriter.WriteSection(new[] { validLogEntry2 });
            monthlyWriter.WriteSection(new [] { validLogEntry1 }, true);
            monthlyWriter.WriteSection(new [] { validLogEntry2 });

            var dailyContents = File.ReadAllText(pathDaily);
            var monthlyContents = File.ReadAllText(pathMonthly);

            const string expectedDailyContents = "Logging started 2014-01-01\r\n[01:01:01] <Source> Contents1\r\n[01:01:02] Contents2\r\n";
            const string expectedMonthlyContents = "Logging started 2014-01-01\r\n[01:01:01] <Source> Contents1\r\n[01:01:02] Contents2\r\n";

            Expect(dailyContents, EqualTo(expectedDailyContents));
            Expect(monthlyContents, EqualTo(expectedMonthlyContents));
        }
    }
}
