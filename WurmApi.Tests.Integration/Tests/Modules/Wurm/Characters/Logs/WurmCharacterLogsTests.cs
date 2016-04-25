using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.Modules.Wurm.Characters.Logs
{
    class WurmCharacterLogsTests : WurmTests
    {
        StubbableTime.StubScope timeScope;

        public IWurmCharacterLogs TestguyLogs => Fixture.WurmApiManager.Characters.Get("Testguy").Logs;

        [SetUp]
        public void Setup()
        {
            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "WurmCharacterLogsTests-wurmdir.7z"));

            if (timeScope != null)
            {
                timeScope.Dispose();
                timeScope = null;
            }
            timeScope = TimeStub.CreateStubbedScope();
            timeScope.OverrideNow(new DateTime(2012, 09, 23, 23, 37, 13));
        }

        [TearDown]
        public void Teardown()
        {
            timeScope.Dispose();
            timeScope = null;
        }

        [Test]
        public async Task Scans_WhenTwoServerGroups()
        {
            var serverCrossDate = new DateTime(2012, 09, 15, 17, 02, 21);

            var results = await TestguyLogs.ScanLogsServerGroupRestrictedAsync(new DateTime(2012, 08, 01),
                new DateTime(2012, 09, 24),
                LogType.Skills,
                new ServerGroup("FREEDOM"));
            Expect(results.Any(), "No log entries found");
            Expect(results.All(entry => entry.Timestamp < serverCrossDate));
            Expect(!results.Any(entry => entry.Timestamp > serverCrossDate));

            var results2 = await TestguyLogs.ScanLogsServerGroupRestrictedAsync(new DateTime(2012, 08, 01),
                new DateTime(2012, 09, 24),
                LogType.Skills,
                new ServerGroup("EPIC"));
            Expect(results.Any(), "No log entries found");
            Expect(results2.All(entry => entry.Timestamp > serverCrossDate));
            Expect(!results2.Any(entry => entry.Timestamp < serverCrossDate));
        }

        [Test]
        public async Task Scans()
        {
            var results = await TestguyLogs.ScanLogsAsync(new DateTime(2012, 08, 01),
                new DateTime(2012, 09, 24),
                LogType.Skills);
            Expect(results.Any(), "No log entries found");
        }
    }
}
