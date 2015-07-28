using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Configs;
using AldurSoft.WurmApi.Modules.Wurm.Paths;
using AldurSoft.WurmApi.Utility;
using Moq;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.WurmConfigsImpl
{
    public class WurmConfigsTests : WurmApiFixtureBase
    {
        private const string CompactConfigName = "compact";

        [Test]
        public void ReadingConfig()
        {
            using (var frame = new MockFrame(this))
            {
                var wurmConfigs = frame.System;
                var config = wurmConfigs.GetConfig(CompactConfigName);
                Assert.AreEqual(LogsLocation.ProfileFolder, config.AutoRunSource);
                Assert.AreEqual(LogsLocation.PlayerFolder, config.CustomTimerSource);
                Assert.AreEqual(LogsLocation.ProfileFolder, config.ExecSource);

                Assert.AreEqual(LogSaveMode.Monthly, config.EventLoggingType);
                Assert.AreEqual(LogSaveMode.Monthly, config.IrcLoggingType);
                Assert.AreEqual(LogSaveMode.Monthly, config.OtherLoggingType);

                Assert.AreEqual(SkillGainRate.Per0D001, config.SkillGainRate);

                Assert.AreEqual(true, config.SaveSkillsOnQuit);
                Assert.AreEqual(true, config.TimestampMessages);
                Assert.AreEqual(false, config.NoSkillMessageOnAlignmentChange);
                Assert.AreEqual(false, config.NoSkillMessageOnFavorChange);
            }
        }

        [Test]
        public async Task ChangeEvents()
        {
            var wurmDir = CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "WurmDir-configs"));
            // wurm dir has to be reused
            {
                using (var frame = new MockFrame(this, wurmDir))
                {
                    var wurmConfigs = frame.System;
                    var config = (WurmConfig)wurmConfigs.GetConfig(CompactConfigName);
                    //config.SkillGainRate = SkillGainRate.PerInteger;
                    await Task.Delay(TimeSpan.FromMilliseconds(5));
                    // expecting changewatcher event may fail, depends on file system response time
                    ((IRequireRefresh)config).Refresh();
                }
            }

            {
                using (var frame = new MockFrame(this, wurmDir))
                {
                    var wurmConfigs = frame.System;
                    var config = wurmConfigs.GetConfig(CompactConfigName);
                    Assert.AreEqual(SkillGainRate.PerInteger, config.SkillGainRate);
                }
            }
        }

        // since detection method is gone, checking if wurm is running is no longer applicable
        //[Test]
        //public void CantChangeIfWurmRunning()
        //{
        //    using (var frame = new MockFrame(this))
        //    {
        //        frame.WurmGameClients.Setup(clients => clients.AnyRunning).Returns(true);
        //        var wurmConfigs = frame.System;
        //        var config = wurmConfigs.GetConfig(CompactConfigName);
        //        Assert.Throws<WurmApiException>(() => config.EventLoggingType = LogSaveMode.Daily);
        //        Assert.Throws<WurmApiException>(() => config.IrcLoggingType = LogSaveMode.Daily);
        //        Assert.Throws<WurmApiException>(() => config.OtherLoggingType = LogSaveMode.Daily);
        //        Assert.Throws<WurmApiException>(() => config.SkillGainRate = SkillGainRate.PerInteger);
        //        Assert.Throws<WurmApiException>(() => config.SaveSkillsOnQuit = false);
        //        Assert.Throws<WurmApiException>(() => config.TimestampMessages = false);
        //        Assert.Throws<WurmApiException>(() => config.NoSkillMessageOnAlignmentChange = true);
        //        Assert.Throws<WurmApiException>(() => config.NoSkillMessageOnFavorChange = true);
        //    }
        //}

        [Test]
        public void Name_Gets()
        {
            using (var frame = new MockFrame(this))
            {
                var wurmConfigs = frame.System;
                var config = wurmConfigs.GetConfig(CompactConfigName);
                Expect(config.Name, EqualTo(CompactConfigName));
            }
        }

        private class MockFrame : IDisposable
        {
            private readonly WurmConfigsTests baseFixture;
            private TestPak wurmDir;

            public WurmConfigs System { get; private set; }
            public Mock<IWurmInstallDirectory> WurmInstallDirectory = new Mock<IWurmInstallDirectory>();
            private readonly WurmConfigDirectories wurmConfigDirectories;

            public MockFrame(WurmConfigsTests baseFixture, TestPak wurmDir = null)
            {
                if (baseFixture == null)
                {
                    throw new ArgumentNullException("baseFixture");
                }

                if (wurmDir != null)
                {
                    this.wurmDir = wurmDir;
                }
                else
                {
                    this.wurmDir = baseFixture.CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "WurmDir-configs"));
                }

                this.baseFixture = baseFixture;
                WurmInstallDirectory.Setup(directory => directory.FullPath)
                    .Returns(this.wurmDir.DirectoryFullPath);
                var publicEventInvoker = new PublicEventInvoker(new SimpleMarshaller(), new LoggerStub());
                var internalEventAggregator = new InternalEventAggregator();
                //wurmConfigDirectories = new WurmConfigDirectories(new WurmPaths(WurmInstallDirectory.Object), publicEventInvoker, internalEventAggregator);
                System = new WurmConfigs(
                    wurmConfigDirectories,
                    Mock.Of<ILogger>(),
                    publicEventInvoker,
                    internalEventAggregator);
            }

            public void Dispose()
            {
                System.Dispose();
                wurmConfigDirectories.Dispose();
            }
        }
    }
}
