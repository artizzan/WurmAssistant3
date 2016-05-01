using System;
using System.Diagnostics;
using AldursLab.WurmApi.Tests.Integration.Builders.WurmClient;
using AldursLab.WurmApi.Tests.Integration.Helpers;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.Modules.Wurm.Configs
{
    class WurmConfigsTests : WurmTests
    {
        public IWurmConfigs System => Fixture.WurmApiManager.Configs;
        EventAwaiter<EventArgs> awaiter;
        WurmConfig configMock;

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

            awaiter.WaitInvocations(2, 10000);

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
