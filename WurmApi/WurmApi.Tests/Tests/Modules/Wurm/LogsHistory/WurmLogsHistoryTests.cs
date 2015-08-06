using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Testing;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Modules.Wurm.LogFiles;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.LogsHistory
{
    [TestFixture]
    class WurmLogsHistoryTests : WurmTests
    {
        public IWurmLogsHistory System { get { return Fixture.WurmApiManager.WurmLogsHistory; } }

        [SetUp]
        public void Setup()
        {
            //ClientMock.PopulateFromDir(Path.Combine(TestPaksDirFullPath, "logs-samples-realdata"));
            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "logs-samples-realdata.7z"));
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public async Task Scans()
        {
            var results = await System.ScanAsync(new LogSearchParameters()
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
            var results = await System.ScanAsync(new LogSearchParameters()
            {
                DateFrom = new DateTime(2012, 8, 1),
                DateTo = new DateTime(2012, 8, 31),
                CharacterName = new CharacterName("Testguy"),
                LogType = LogType.Skills
            });
            Expect(results.Count, EqualTo(62));
            var firstResult = results.First();
            var lastResult = results.Last();
            Expect(firstResult.Timestamp, EqualTo(new DateTime(2012, 8, 18, 17, 28, 19)));
            Expect(firstResult.Content, EqualTo("First aid increased by 0,0124 to 23,392"));
            Expect(lastResult.Timestamp, EqualTo(new DateTime(2012, 8, 27, 1, 17, 51)));
            Expect(lastResult.Content, EqualTo("Paving increased  to 19"));

            VerifyLoggedErrors(9);
        }

        [Test]
        public async Task RetrievesCorrectData_MonthlyFile_SingleDay()
        {
            var results = await System.ScanAsync(new LogSearchParameters()
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
            var results = await System.ScanAsync(new LogSearchParameters()
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
            var results = await System.ScanAsync(new LogSearchParameters()
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

        [Test]
        public async Task ScansAreCancellable()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var resultsAwaiter = System.ScanAsync(new LogSearchParameters()
            {
                DateFrom = DateTime.MinValue,
                DateTo = DateTime.MaxValue,
                CharacterName = new CharacterName("Testguy"),
                LogType = LogType.Skills
            }, cts.Token);
            try
            {
                await resultsAwaiter;
                Assert.Fail("no exception thrown");
            }
            catch (Exception exception)
            {
                Expect(exception, TypeOf<OperationCanceledException>());
            }
        }

        [Test]
        public void Scans_Synchronously()
        {
            var results = System.Scan(new LogSearchParameters()
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
        public void Scans_SynchronousScansAreCancellable()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            try
            {
                System.Scan(new LogSearchParameters()
                {
                    DateFrom = DateTime.MinValue,
                    DateTo = DateTime.MaxValue,
                    CharacterName = new CharacterName("Testguy"),
                    LogType = LogType.Skills
                },
                    cts.Token);
                Assert.Fail("no exception thrown");
            }
            catch (Exception exception)
            {
                Expect(exception, TypeOf<OperationCanceledException>());
            }
        }

        private void VerifyLoggedErrors(int count = 0)
        {
            Fixture.LoggerMock.Assert(
                logger1 =>
                    logger1.Log(Arg.IsAny<LogLevel>(), Arg.IsAny<string>(), Arg.IsAny<object>(), Arg.IsAny<Exception>()),
                Occurs.Exactly(count));
        }
    }
}
