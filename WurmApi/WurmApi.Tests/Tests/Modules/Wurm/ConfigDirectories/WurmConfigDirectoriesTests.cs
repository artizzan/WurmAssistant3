using System.Linq;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.ConfigDirectories
{
    public class WurmConfigDirectoriesTests : WurmTests
    {
        // assumption: normalization convention for WurmApi is ToUpperInvariant

        // property-redirect to enable some setup, before wurmapi init 
        IWurmConfigDirectories System
        {
            get { return Fixture.WurmApiManager.WurmConfigDirectories; }
        }

        [Test]
        public void ReturnsNormalizedNames()
        {
            var realdirnames = SetupDefaultConfigs().Select(s => s.ToUpperInvariant()).OrderBy(s => s).ToArray();
            var dirnames = System.AllDirectoryNamesNormalized.OrderBy(s => s).ToArray();
            Expect(dirnames, EqualTo(realdirnames));
        }

        [Test]
        public void ReturnsFullPaths()
        {
            SetupDefaultConfigs();
            var realdirfullpaths = Fixture.WurmClientMock.Configs.Select(s => s.ConfigDir.FullName).OrderBy(s => s).ToArray();
            var dirpaths = System.AllDirectoriesFullPaths.OrderBy(s => s).ToArray();
            Expect(dirpaths, EqualTo(realdirfullpaths));
        }

        [Test]
        public void TriggersOnChanged()
        {
            var subscriber = new Subscriber<ConfigDirectoriesChanged>(Fixture.WurmApiManager.InternalEventAggregator);
            var batman = ClientMock.AddConfig("batmobile");
            subscriber.WaitMessages(1);

            // verifying event sent
            Expect(subscriber.ReceivedMessages.Count(), GreaterThan(0));

            // verifying data updated
            var allConfigs = System.AllConfigNames.ToList();
            var allDirFullPaths = System.AllDirectoriesFullPaths.ToList();
            var allDirNames = System.AllDirectoryNamesNormalized.ToList();
            Expect(allConfigs, Member(batman.NameNormalized).And.Count.EqualTo(1));
            Expect(allDirFullPaths, Member(batman.ConfigDir.FullName).And.Count.EqualTo(1));
            Expect(allDirNames, Member(batman.ConfigDir.Name.ToUpperInvariant()).And.Count.EqualTo(1));
        }

        [Test]
        public void GetsGamesettingsWhenFileExists()
        {
            var config = ClientMock.AddConfig("batmobile");
            var realPath = config.GameSettings.GameSettingsTxt.FullName;
            var dir1 = System.GetGameSettingsFileFullPathForConfigName("batmobile");
            var dir2 = System.GetGameSettingsFileFullPathForConfigName("Batmobile");
            var dir3 = System.GetGameSettingsFileFullPathForConfigName("BATMOBILE");
            Expect(dir1, EqualTo(realPath));
            Expect(dir2, EqualTo(realPath));
            Expect(dir3, EqualTo(realPath));
        }

        [Test]
        public void ThrowsWhenGamesettingsNotExist()
        {
            Assert.Catch<WurmApiException>(() => System.GetGameSettingsFileFullPathForConfigName("notexisting"));
        }

        string[] SetupDefaultConfigs()
        {
            ClientMock.AddConfig("foo");
            ClientMock.AddConfig("bar");
            return new string[] { "foo", "bar" };
        }
    }
}
