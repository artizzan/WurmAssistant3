using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Persistence.WurmApiDataContextModule;
using AldurSoft.WurmApi.Validation;
using AldurSoft.WurmApi.Wurm.CharacterDirectories.WurmCharacterDirectoriesModule;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Logs;
using AldurSoft.WurmApi.Wurm.Logs.Searching;
using AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogDefinitionsModule;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogFilesModule;
using AldurSoft.WurmApi.Wurm.Paths;
using AldurSoft.WurmApi.Wurm.Paths.WurmPathsModule;
using Moq;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.WurmLogsHistoryImpl
{
    [TestFixture]
    class WurmLogsHistoryTests : WurmApiFixtureBase
    {
        protected TestPak DataDir;
        protected TestPak WurmDir;

        protected WurmLogsHistory System;
        private WurmLogFiles wurmLogFiles;
        private WurmCharacterDirectories wurmCharacterDirectories;

        Mock<ILogger> logger;

        [SetUp]
        public void Setup()
        {
            DataDir = CreateTestPakEmptyDir();
            WurmDir = CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "logs-samples-realdata"));

            var installDir = new Mock<IWurmInstallDirectory>();
            installDir.Setup(directory => directory.FullPath).Returns(WurmDir.DirectoryFullPath);

            wurmCharacterDirectories = new WurmCharacterDirectories(new WurmPaths(installDir.Object));


            logger = new Mock<ILogger>();
            logger.Setup(logger1 => logger1.Log(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<object>()))
                .Callback<LogLevel, string, object>(
                    (level, s, arg3) => Trace.WriteLine(string.Format("{0} {1} {2}", level, arg3, s)));

            wurmLogFiles = new WurmLogFiles(
                wurmCharacterDirectories,
                logger.Object,
                new WurmLogDefinitions());
            System =
                new WurmLogsHistory(
                    new WurmApiDataContext(DataDir.DirectoryFullPath, Mock.Of<ISimplePersistLogger>()),
                    wurmLogFiles,
                    logger.Object,
                    Mock.Of<IThreadGuard>());
        }

        [TearDown]
        public override void Teardown()
        {
            wurmCharacterDirectories.Dispose();
            wurmLogFiles.Dispose();
            base.Teardown();
        }

        [Test]
        public async Task Scans()
        {
            var results = await System.Scan(new LogSearchParameters()
            {
                DateFrom = DateTime.MinValue,
                DateTo = DateTime.MaxValue,
                CharacterName = new CharacterName("Testguy"),
                LogType = LogType.Skills
            });
            Expect(results.Any(), True);

            VerifyLoggedErrors(9);
        }

        [Test]
        public async Task RetrievesCorrectData_MonthlyFile_FullMonth()
        {
            var results = await System.Scan(new LogSearchParameters()
            {
                DateFrom = new DateTime(2012, 8, 1),
                DateTo = new DateTime(2012, 8, 31),
                CharacterName = new CharacterName("Testguy"),
                LogType = LogType.Skills
            });
            Expect(results.Count, EqualTo(62));
            var firstResult = results.First();
            var lastResult = results.Last();
            Expect(firstResult.Timestamp, EqualTo(new DateTime(2012,8,18,17,28,19)));
            Expect(firstResult.Content, EqualTo("First aid increased by 0,0124 to 23,392"));
            Expect(lastResult.Timestamp, EqualTo(new DateTime(2012,8,27,1,17,51)));
            Expect(lastResult.Content, EqualTo("Paving increased  to 19"));

            VerifyLoggedErrors(9);
        }

        [Test]
        public async Task RetrievesCorrectData_MonthlyFile_SingleDay()
        {
            var results = await System.Scan(new LogSearchParameters()
            {
                DateFrom = new DateTime(2012, 8, 19),
                DateTo = new DateTime(2012, 8, 19),
                CharacterName = new CharacterName("Testguy"),
                LogType = LogType.Skills
            });
            Expect(results.Count, EqualTo(8));
            var firstResult = results.First();
            var lastResult = results.Last();
            Expect(firstResult.Timestamp, EqualTo(new DateTime(2012, 8, 19, 0, 9, 44)));
            Expect(firstResult.Content, EqualTo("Miscellaneous items increased by 0,0105 to 52,467"));
            Expect(lastResult.Timestamp, EqualTo(new DateTime(2012, 8, 19, 23, 53, 27)));
            Expect(lastResult.Content, EqualTo("Mind increased  to 27"));

            VerifyLoggedErrors();
        }

        [Test]
        public async Task RetrievesCorrectData_DailyFile_SingleDay()
        {
            var results = await System.Scan(new LogSearchParameters()
            {
                DateFrom = new DateTime(2012, 9, 22),
                DateTo = new DateTime(2012, 9, 22),
                CharacterName = new CharacterName("Testguy"),
                LogType = LogType.Skills
            });
            Expect(results.Count, EqualTo(18));
            var firstResult = results.First();
            var lastResult = results.Last();
            Expect(firstResult.Timestamp, EqualTo(new DateTime(2012, 9, 22, 19, 05, 44)));
            Expect(firstResult.Content, EqualTo("Mining increased by 0,104 to 47,472"));
            Expect(lastResult.Timestamp, EqualTo(new DateTime(2012, 9, 22, 22, 51, 57)));
            Expect(lastResult.Content, EqualTo("Healing increased by 0,104 to 12,295"));

            VerifyLoggedErrors();
        }

        [Test]
        public async Task RetrievesCorrectData_MixedFiles_ManyDays()
        {
            var results = await System.Scan(new LogSearchParameters()
            {
                DateFrom = new DateTime(2011, 8, 22),
                DateTo = new DateTime(2013, 9, 22),
                CharacterName = new CharacterName("Testguy"),
                LogType = LogType.Skills
            });
            Expect(results.Count, EqualTo(62 + 57 + 18 + 9 + 142));
            var firstResult = results.First();
            var lastResult = results.Last();
            Expect(firstResult.Timestamp, EqualTo(new DateTime(2012, 8, 18, 17, 28, 19)));
            Expect(firstResult.Content, EqualTo("First aid increased by 0,0124 to 23,392"));
            Expect(lastResult.Timestamp, EqualTo(new DateTime(2012, 9, 23, 23, 37, 13)));
            Expect(lastResult.Content, EqualTo("Gardening increased by 0,106 to 26,977"));

            VerifyLoggedErrors(9);
        }

        private void VerifyLoggedErrors(int count = 0)
        {
            logger.Verify(logger1 => logger1.Log(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(count));
        }
    }
}
