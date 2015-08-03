using System;
using System.IO;
using System.Threading.Tasks;
using AldursLab.Testing;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Configs;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.Configs
{
    public class WurmConfigsTests : WurmTests
    {
        //private const string CompactConfigName = "compact";

        public IWurmConfigs System { get { return Fixture.WurmApiManager.WurmConfigs; } }

        [Test]
        public void ReadingConfig()
        {
            var batmobile = "batmobile";
            ClientMock.AddConfig(batmobile);

            var config = System.GetConfig(batmobile);

            // verifying against values in default config template attached to this project

            Assert.AreEqual(LogsLocation.ProfileFolder, config.AutoRunSource);
            Assert.AreEqual(LogsLocation.PlayerFolder, config.CustomTimerSource);
            Assert.AreEqual(LogsLocation.ProfileFolder, config.ExecSource);

            Assert.AreEqual(LogSaveMode.Daily, config.EventLoggingType);
            Assert.AreEqual(LogSaveMode.Daily, config.IrcLoggingType);
            Assert.AreEqual(LogSaveMode.Daily, config.OtherLoggingType);

            Assert.AreEqual(SkillGainRate.Per0D001, config.SkillGainRate);

            Assert.AreEqual(true, config.SaveSkillsOnQuit);
            Assert.AreEqual(true, config.TimestampMessages);
            Assert.AreEqual(false, config.NoSkillMessageOnAlignmentChange);
            Assert.AreEqual(false, config.NoSkillMessageOnFavorChange);
        }

        [Test]
        public void TriggersEventOnChanged()
        {
            var batmobile = "batmobile";
            var configMock = ClientMock.AddConfig(batmobile);

            var config = System.GetConfig(batmobile);
            Expect(config.NoSkillMessageOnFavorChange, False);

            var awaiter = new EventAwaiter<EventArgs>();
            config.ConfigChanged += awaiter.Handle;

            configMock.GameSettings.ChangeValue("skillgain_no_favor", "true");

            awaiter.WaitInvocations(1);

            Expect(config.NoSkillMessageOnFavorChange, True);
        }

        [Test]
        public void GetsName()
        {
            var batmobile = "batmobile";
            ClientMock.AddConfig(batmobile);

            var config = System.GetConfig(batmobile);
            Expect(config.Name, EqualTo(batmobile));
        }
    }
}
