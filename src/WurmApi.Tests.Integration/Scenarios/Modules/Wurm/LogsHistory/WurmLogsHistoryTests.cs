using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.Modules.Wurm.LogsHistory
{
    [TestFixture(Platform.Windows)]
    [TestFixture(Platform.Linux)]
    class WurmLogsHistoryTests : WurmTests
    {
        public WurmLogsHistoryTests(Platform targetPlatform) : base(targetPlatform)
        {}

        public IWurmLogsHistory System => Fixture.WurmApiManager.LogsHistory;

        [SetUp]
        public void Setup()
        {
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
                MinDate = DateTime.MinValue,
                MaxDate = DateTime.MaxValue,
                CharacterName = "Testguy",
                LogType = LogType.Skills
            });
            Expect(results.Any(), True);
        }

        [Test]
        public async Task RetrievesCorrectData_MonthlyFile_FullMonth(
            [Values(ScanResultOrdering.Ascending, ScanResultOrdering.Descending)] ScanResultOrdering ordering)
        {
            var results = await System.ScanAsync(new LogSearchParameters()
            {
                MinDate = new DateTime(2012, 8, 1),
                MaxDate = new DateTime(2012, 8, 31),
                CharacterName = "testguy",
                LogType = LogType.Skills,
                ScanResultOrdering = ordering
            });
            Expect(results.Count, EqualTo(62));

            LogEntry alphaBoundary;
            LogEntry betaBoundary;
            if (ordering == ScanResultOrdering.Ascending)
            {
                alphaBoundary = results.First();
                betaBoundary = results.Last();
            }
            else
            {
                alphaBoundary = results.Last();
                betaBoundary = results.First();
            }

            Expect(betaBoundary.Timestamp, EqualTo(new DateTime(2012, 8, 27, 1, 17, 51)));
            Expect(betaBoundary.Content, EqualTo("Paving increased  to 19"));
            Expect(alphaBoundary.Timestamp, EqualTo(new DateTime(2012, 8, 18, 17, 28, 19)));
            Expect(alphaBoundary.Content, EqualTo("First aid increased by 0,0124 to 23,392"));
        }

        [Test]
        public async Task RetrievesCorrectData_MonthlyFile_SingleDay([Values(ScanResultOrdering.Ascending, ScanResultOrdering.Descending)] ScanResultOrdering ordering)
        {
            var results = await System.ScanAsync(new LogSearchParameters()
            {
                MinDate = new DateTime(2012, 8, 19),
                MaxDate = new DateTime(2012, 8, 19),
                CharacterName = "Testguy",
                LogType = LogType.Skills,
                ScanResultOrdering = ordering
            });
            Expect(results.Count, EqualTo(8));

            LogEntry alphaBoundary;
            LogEntry betaBoundary;
            if (ordering == ScanResultOrdering.Ascending)
            {
                alphaBoundary = results.First();
                betaBoundary = results.Last();
            }
            else
            {
                alphaBoundary = results.Last();
                betaBoundary = results.First();
            }

            Expect(betaBoundary.Timestamp, EqualTo(new DateTime(2012, 8, 19, 23, 53, 27)));
            Expect(betaBoundary.Content, EqualTo("Mind increased  to 27"));
            Expect(alphaBoundary.Timestamp, EqualTo(new DateTime(2012, 8, 19, 0, 9, 44)));
            Expect(alphaBoundary.Content, EqualTo("Miscellaneous items increased by 0,0105 to 52,467"));
        }

        [Test]
        public async Task RetrievesCorrectData_DailyFile_SingleDay([Values(ScanResultOrdering.Ascending, ScanResultOrdering.Descending)] ScanResultOrdering ordering)
        {
            var results = await System.ScanAsync(new LogSearchParameters()
            {
                MinDate = new DateTime(2012, 9, 22),
                MaxDate = new DateTime(2012, 9, 22),
                CharacterName = "Testguy",
                LogType = LogType.Skills,
                ScanResultOrdering = ordering
            });
            Expect(results.Count, EqualTo(18));

            LogEntry alphaBoundary;
            LogEntry betaBoundary;
            if (ordering == ScanResultOrdering.Ascending)
            {
                alphaBoundary = results.First();
                betaBoundary = results.Last();
            }
            else
            {
                alphaBoundary = results.Last();
                betaBoundary = results.First();
            }

            Expect(betaBoundary.Timestamp, EqualTo(new DateTime(2012, 9, 22, 22, 51, 57)));
            Expect(betaBoundary.Content, EqualTo("Healing increased by 0,104 to 12,295"));
            Expect(alphaBoundary.Timestamp, EqualTo(new DateTime(2012, 9, 22, 19, 05, 44)));
            Expect(alphaBoundary.Content, EqualTo("Mining increased by 0,104 to 47,472"));
        }

        [Test]
        public async Task RetrievesCorrectData_MixedFiles_ManyDays([Values(ScanResultOrdering.Ascending, ScanResultOrdering.Descending)] ScanResultOrdering ordering)
        {
            var results = await System.ScanAsync(new LogSearchParameters()
            {
                MinDate = new DateTime(2011, 8, 22),
                MaxDate = new DateTime(2013, 9, 22),
                CharacterName = "Testguy",
                LogType = LogType.Skills,
                ScanResultOrdering = ordering
            });
            Expect(results.Count, EqualTo(62 + 57 + 18 + 9 + 142));

            LogEntry alphaBoundary;
            LogEntry betaBoundary;
            if (ordering == ScanResultOrdering.Ascending)
            {
                alphaBoundary = results.First();
                betaBoundary = results.Last();
            }
            else
            {
                alphaBoundary = results.Last();
                betaBoundary = results.First();
            }

            Expect(betaBoundary.Timestamp, EqualTo(new DateTime(2012, 9, 23, 23, 37, 13)));
            Expect(betaBoundary.Content, EqualTo("Gardening increased by 0,106 to 26,977"));
            Expect(alphaBoundary.Timestamp, EqualTo(new DateTime(2012, 8, 18, 17, 28, 19)));
            Expect(alphaBoundary.Content, EqualTo("First aid increased by 0,0124 to 23,392"));
        }

        [Test]
        public async Task ScansAreCancellable()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var resultsAwaiter = System.ScanAsync(new LogSearchParameters()
            {
                MinDate = DateTime.MinValue,
                MaxDate = DateTime.MaxValue,
                CharacterName = "Testguy",
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
                MinDate = DateTime.MinValue,
                MaxDate = DateTime.MaxValue,
                CharacterName = "Testguy",
                LogType = LogType.Skills
            });
            Expect(results.Any(), True);
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
                    MinDate = DateTime.MinValue,
                    MaxDate = DateTime.MaxValue,
                    CharacterName = "Testguy",
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
    }
}
