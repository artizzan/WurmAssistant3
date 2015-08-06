using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AldursLab.Testing;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
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
        public IWurmConfigs System { get { return Fixture.WurmApiManager.WurmConfigs; } }
        EventAwaiter<EventArgs> awaiter;
        Builders.WurmClient.WurmConfig configMock;

        [SetUp]
        public void Setup()
        {
            var batmobile = "batmobile";
            configMock = ClientMock.AddConfig(batmobile);

            awaiter = new EventAwaiter<EventArgs>();
            System.AnyConfigChanged += awaiter.Handle;
        }

        [Test]
        public void ReadingConfig()
        {
            IWurmConfig config = System.GetConfig(configMock.Name);

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

            Assert.AreEqual(true, config.HasBeenRead);
        }

        [Test]
        public void TriggersEventOnChanged()
        {
            IWurmConfig config = System.GetConfig(configMock.Name);

            Expect(config.NoSkillMessageOnFavorChange, False);

            var configAwaiter = new EventAwaiter<EventArgs>();
            config.ConfigChanged += configAwaiter.Handle;

            configMock.GameSettings.ChangeValue("skillgain_no_favor", "true");

            awaiter.WaitInvocations(1);

            Expect(config.NoSkillMessageOnFavorChange, True);
        }

        [Test]
        public void GetsName()
        {
            IWurmConfig config = System.GetConfig(configMock.Name);
            Expect(config.Name, EqualTo(configMock.Name));
        }

        [Test]
        public void AddsNewConfig()
        {
            var configMock2 = ClientMock.AddConfig("batmobile2");
            var config2 = WaitUntilConfigAvailable(configMock2.Name);
            Expect(config2.Name, EqualTo(configMock2.Name).IgnoreCase);
        }

        IWurmConfig WaitUntilConfigAvailable(string configName)
        {
            IWurmConfig config = null;
            awaiter.WaitUntilMatch(list =>
            {
                try
                {
                    config = System.GetConfig(configName);
                    return true;
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception.Message);
                    return false;
                }
            });
            return config;
        }
    }
}
