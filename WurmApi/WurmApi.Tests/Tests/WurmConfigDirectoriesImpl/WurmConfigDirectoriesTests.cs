using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Paths;
using AldurSoft.WurmApi.Tests.Tests.WurmCharacterDirectoriesImpl;
using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmApi.Tests.Tests.WurmConfigDirectoriesImpl
{
    public class WurmConfigDirectoriesTests : WurmApiFixtureBase
    {
        private WurmConfigDirectories system;
        TestPak testDir;

        private DirectoryInfo configsDirInfo;

        [SetUp]
        public void Setup()
        {
            testDir = CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "WurmDir-configs"));
            var installDir = Automocker.Create<IWurmInstallDirectory>();
            Mock.Get(installDir)
                .Setup(directory => directory.FullPath)
                .Returns(testDir.DirectoryFullPath);
            system = new WurmConfigDirectories(new WurmPaths(installDir), new LoggerStub());

            configsDirInfo = new DirectoryInfo(Path.Combine(testDir.DirectoryFullPath, "configs"));
        }

        [TearDown]
        public override void Teardown()
        {
            system.Dispose();
            base.Teardown();
        }

        public class AllDirectoryNamesNormalized : WurmConfigDirectoriesTests
        {
            [Test]
            public void ReturnsNormalizedNames()
            {
                var realdirnames = configsDirInfo.GetDirectories().Select(s => s.Name.ToUpperInvariant()).OrderBy(s => s).ToArray();
                var dirnames = system.AllDirectoryNamesNormalized.OrderBy(s => s).ToArray();
                Expect(dirnames, EqualTo(realdirnames));
            }
        }

        public class AllDirectoriesFullPaths : WurmConfigDirectoriesTests
        {
            [Test]
            public void ReturnsFullPaths()
            {
                var realdirfullpaths = configsDirInfo.GetDirectories().Select(s => s.FullName).OrderBy(s => s).ToArray();
                var dirpaths = system.AllDirectoriesFullPaths.OrderBy(s => s).ToArray();
                Expect(dirpaths, EqualTo(realdirfullpaths));
            }
        }

        public class DirectoriesChanged : WurmConfigDirectoriesTests
        {
            [Test]
            public void TriggersOnChanged()
            {
                bool changed = false;
                system.DirectoriesChanged += (sender, args) => changed = true;
                var dir = configsDirInfo.CreateSubdirectory("newconfig");
                Thread.Sleep(10); // might require more delay
                //system.Refresh();
                Expect(changed, True);
                var allDirFullPaths = system.AllDirectoriesFullPaths.ToList();
                var allDirNames = system.AllDirectoryNamesNormalized.ToList();
                Expect(allDirFullPaths, Member(dir.FullName).And.Count.EqualTo(7));
                Expect(allDirNames, Member(dir.Name.ToUpperInvariant()).And.Count.EqualTo(7));
            }
        }

        public class GetGameSettingsFileFullPathForConfigName : WurmConfigDirectoriesTests
        {
            [Test]
            public void GetsWhenFileExists()
            {
                var realPath =
                    configsDirInfo.GetDirectories()
                        .Single(d => d.Name.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                        .GetFiles("gamesettings.txt")
                        .Single()
                        .FullName;
                var dir = system.GetGameSettingsFileFullPathForConfigName("default");
                Expect(dir, EqualTo(realPath));
            }

            [Test]
            public void ThrowsWhenNotExists()
            {
                Assert.Catch<WurmApiException>(() => system.GetGameSettingsFileFullPathForConfigName("notexisting"));
            }
        }
    }
}
