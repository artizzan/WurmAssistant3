using System;
using System.IO;
using System.Linq;
using System.Threading;
using AldursLab.Testing;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.CharacterDirectories
{
    public class WurmConfigDirectoriesTests : WurmTests
    {
        // assumption: normalization convention for WurmApi is ToUpperInvariant

        IWurmConfigDirectories configDirectories;
        Subscriber<ConfigDirectoriesChanged> subscriber;

        [SetUp]
        public void Setup()
        {
            configDirectories = Fixture.WurmApiManager.WurmConfigDirectories;
            subscriber = new Subscriber<ConfigDirectoriesChanged>(Fixture.WurmApiManager.InternalEventAggregator);
        }

        [Test]
        public void ReturnsNormalizedNames()
        {
            var realdirnames = SetupDefaultConfigs().Select(s => s.ToUpperInvariant()).OrderBy(s => s).ToArray();
            var dirnames = configDirectories.AllDirectoryNamesNormalized.OrderBy(s => s).ToArray();
            Expect(dirnames, EqualTo(realdirnames));
        }

        [Test]
        public void ReturnsFullPaths()
        {
            SetupDefaultConfigs();
            var realdirfullpaths = Fixture.WurmClientMock.Configs.Select(s => s.ConfigDir.FullName).OrderBy(s => s).ToArray();
            var dirpaths = configDirectories.AllDirectoriesFullPaths.OrderBy(s => s).ToArray();
            Expect(dirpaths, EqualTo(realdirfullpaths));
        }

        [Test]
        public void TriggersOnChanged()
        {
            var batman = ClientMock.AddConfig("batmobile");
            subscriber.WaitMessages(1);

            // verifying event sent
            Expect(subscriber.ReceivedMessages.Count(), GreaterThan(0));

            // verifying data updated
            var allConfigs = configDirectories.AllConfigNames.ToList();
            var allDirFullPaths = configDirectories.AllDirectoriesFullPaths.ToList();
            var allDirNames = configDirectories.AllDirectoryNamesNormalized.ToList();
            Expect(allConfigs, Member(batman.NameNormalized).And.Count.EqualTo(1));
            Expect(allDirFullPaths, Member(batman.ConfigDir.FullName).And.Count.EqualTo(1));
            Expect(allDirNames, Member(batman.ConfigDir.Name.ToUpperInvariant()).And.Count.EqualTo(1));
        }

        [Test]
        public void GetsGamesettingsWhenFileExists()
        {
            var config = ClientMock.AddConfig("batmobile");
            var realPath = config.GameSettings.FileInfo.FullName;
            var dir1 = configDirectories.GetGameSettingsFileFullPathForConfigName("batmobile");
            var dir2 = configDirectories.GetGameSettingsFileFullPathForConfigName("Batmobile");
            var dir3 = configDirectories.GetGameSettingsFileFullPathForConfigName("BATMOBILE");
            Expect(dir1, EqualTo(realPath));
            Expect(dir2, EqualTo(realPath));
            Expect(dir3, EqualTo(realPath));
        }

        [Test]
        public void ThrowsWhenGamesettingsNotExist()
        {
            Assert.Catch<WurmApiException>(() => configDirectories.GetGameSettingsFileFullPathForConfigName("notexisting"));
        }

        string[] SetupDefaultConfigs()
        {
            ClientMock.AddConfig("foo");
            ClientMock.AddConfig("bar");
            return new string[] { "foo", "bar" };
        }
    }
}
