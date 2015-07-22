using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Wurm.Logs.Parsing;
using AldurSoft.WurmApi.Wurm.Logs.Reading;
using AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule.Heuristics;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogDefinitionsModule;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogFilesModule;
using Moq;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.WurmLogsHistoryImpl
{
    [TestFixture]
    class MonthlyHeuristicsExtractorTests : WurmApiFixtureBase
    {
        private FileInfo testFile;
        private FileInfo emptyTestFile;
        private FileInfo invalidTestFile;
        private FileInfo unrecognizedTestFile;
        private FileInfo dailyLogFile;
        private FileInfo fileWithBadStamp;
        private FileInfo fileThatGoesBeyondMonthDays;
        private FileInfo fileEvent201412;

        private TestPak logsDir;

        [SetUp]
        public void SetUp()
        {
            logsDir = CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "MonthlyHeuristicsExtractor-sample-logs"));
            string basePath = logsDir.DirectoryFullPath;

            testFile = new FileInfo(Path.Combine(basePath, "Village.2013-03.txt"));
            emptyTestFile = new FileInfo(Path.Combine(basePath, "Village.2013-03.empty.txt"));
            invalidTestFile = new FileInfo(Path.Combine(basePath, "Village.2013-03.invaliddata.txt"));
            unrecognizedTestFile = new FileInfo(Path.Combine(basePath, "unrecognized.txt"));
            dailyLogFile = new FileInfo(Path.Combine(basePath, "Village.2012-10-24.txt"));
            fileWithBadStamp = new FileInfo(Path.Combine(basePath, "_Skills.2012-08.txt"));
            fileThatGoesBeyondMonthDays = new FileInfo(Path.Combine(basePath, "Village.2013-04.txt"));
            fileEvent201412 = new FileInfo(Path.Combine(basePath, "_Event.2014-12.txt"));
        }

        private MonthlyHeuristicsExtractor ConstructForFilePath(FileInfo info)
        {
            LogFileInfoFactory factory = new LogFileInfoFactory(new WurmLogDefinitions(), Mock.Of<ILogger>());
            
            return new MonthlyHeuristicsExtractor(
                factory.Create(info), 
                new ParsingHelper(),
                new LogFileStreamReaderFactory(),
                Mock.Of<ILogger>());
        }

        [Test]
        public void ExtractDataToPositionMap()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNow = new DateTime(2014, 1, 1);

                MonthlyHeuristicsExtractor extractor = ConstructForFilePath(testFile);

                HeuristicsExtractionResult result = extractor.ExtractDayToPositionMap();

                Expect(result.Heuristics.Count, EqualTo(31));
                Expect(result.Heuristics.Keys.Min(), EqualTo(1));
                Expect(result.Heuristics.Keys.Max(), EqualTo(31));
                Expect(result.LogDate, EqualTo(new DateTime(2013, 3, 1, 0, 0, 0)));

                AssertData(result, 1, 0, 0);
                AssertData(result, 2, 0, 0);
                AssertData(result, 3, 0, 8);
                AssertData(result, 4, 449, 0);
                AssertData(result, 6, 449, 4);
                AssertData(result, 7, 665, 2);
                AssertData(result, 15, 775, 2);
                AssertData(result, 16, 890, 0);
                AssertData(result, 31, 890, 0);
            }
        }

        [Test]
        public void ExtractDataToPositionMap_ForCurrentMonth()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNow = new DateTime(2013, 3, 15);

                MonthlyHeuristicsExtractor extractor = ConstructForFilePath(testFile);

                HeuristicsExtractionResult result = extractor.ExtractDayToPositionMap();

                Expect(result.Heuristics.Count, EqualTo(15));
                Expect(result.Heuristics.Keys.Min(), EqualTo(1));
                Expect(result.Heuristics.Keys.Max(), EqualTo(15));
                Expect(result.LogDate, EqualTo(new DateTime(2013, 3, 1, 0, 0, 0)));

                AssertData(result, 1, 0, 0);
                AssertData(result, 2, 0, 0);
                AssertData(result, 3, 0, 8);
                AssertData(result, 4, 449, 0);
                AssertData(result, 6, 449, 4);
                AssertData(result, 7, 665, 2);
                AssertData(result, 15, 775, int.MaxValue);
                Assert.Throws<KeyNotFoundException>(() => AssertData(result, 16, 890, 0));
            }
        }

        [Test]
        public void ExtractDataToPositionMap_InvalidFile_ShouldThrow()
        {
            MonthlyHeuristicsExtractor extractor = ConstructForFilePath(emptyTestFile);
            Assert.Throws<WurmApiException>(() => extractor.ExtractDayToPositionMap());
        }
        [Test]
        public void ExtractDataToPositionMap_EmptyFile_ShouldThrow()
        {
            MonthlyHeuristicsExtractor extractor = ConstructForFilePath(invalidTestFile);
            Assert.Catch<Exception>(() => extractor.ExtractDayToPositionMap());
        }

        [Test]
        public void ExtractDataToPositionMap_UnrecognizedFileName_ShouldThrow()
        {
            MonthlyHeuristicsExtractor extractor = ConstructForFilePath(unrecognizedTestFile);
            Assert.Throws<WurmApiException>(() => extractor.ExtractDayToPositionMap());
        }

        [Test]
        public void ExtractDataToPositionMap_DailyFile_ShouldThrow()
        {
            MonthlyHeuristicsExtractor extractor = ConstructForFilePath(dailyLogFile);
            Assert.Throws<WurmApiException>(() => extractor.ExtractDayToPositionMap());
        }

        [Test]
        public void ExtractFromFileWithBadStamps_ShouldNotAffectAllResults()
        {
            MonthlyHeuristicsExtractor extractor = ConstructForFilePath(fileWithBadStamp);
            var result = extractor.ExtractDayToPositionMap();

            AssertData(result, 17, 0);
            AssertData(result, 18, 22);
            AssertData(result, 19, 12);
            AssertData(result, 20, 9);
            AssertData(result, 21, 7);
            AssertData(result, 22, 3);
            AssertData(result, 23, 9);
            AssertData(result, 24, 11);
            AssertData(result, 25, 4);
            AssertData(result, 26, 2);
            AssertData(result, 27, 1);
            AssertData(result, 28, 0);
        }

        [Test]
        public void ExtractForBadEntriesAtEndOfFile()
        {
            MonthlyHeuristicsExtractor extractor = ConstructForFilePath(fileThatGoesBeyondMonthDays);
            var result = extractor.ExtractDayToPositionMap();
            AssertData(result, 29, 0);
            AssertData(result, 30, 4);
            Assert.Catch<Exception>(() => { var impossibleDay = result.Heuristics[31]; });
        }

        [Test]
        public void ExtractForEvent201412()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.SetAllLocalTimes(new DateTime(2014,12,15));
                MonthlyHeuristicsExtractor extractor = ConstructForFilePath(fileEvent201412);
                var result = extractor.ExtractDayToPositionMap();
                AssertData(result, 14, 0, 18);
                AssertData(result, 15, 830, int.MaxValue);
            }
        }

        private void AssertData(
            HeuristicsExtractionResult result,
            int day,
            long filePosition,
            int linesCount)
        {
            Expect(result.Heuristics[day].FilePositionInBytes, EqualTo(filePosition));
            Expect(result.Heuristics[day].LinesCount, EqualTo(linesCount));
        }

        private void AssertData(
            HeuristicsExtractionResult result,
            int day,
            int linesCount)
        {
            Expect(result.Heuristics[day].LinesCount, EqualTo(linesCount));
        }
    }
}
