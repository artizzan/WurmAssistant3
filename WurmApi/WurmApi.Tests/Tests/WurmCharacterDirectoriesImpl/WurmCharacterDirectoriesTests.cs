using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Paths;
using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmApi.Tests.Tests.WurmCharacterDirectoriesImpl
{
    public class WurmCharacterDirectoriesTests : WurmApiFixtureBase
    {
        private readonly WurmCharacterDirectories system;
        readonly TestPak testDir;

        private readonly DirectoryInfo playersDirInfo;

        public WurmCharacterDirectoriesTests()
        {
            
            testDir = CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "WurmDir-PlayersDirWithPlayers"));
            var installDir = Automocker.Create<IWurmInstallDirectory>();
            Mock.Get(installDir)
                .Setup(directory => directory.FullPath)
                .Returns(testDir.DirectoryFullPath);
            system = new WurmCharacterDirectories(new WurmPaths(installDir),
                new PublicEventInvoker(new SimpleMarshaller(), new LoggerStub()), new InternalEventAggregator());

            playersDirInfo = new DirectoryInfo(Path.Combine(testDir.DirectoryFullPath, "players"));
        }

        [TearDown]
        public override void Teardown()
        {
            system.Dispose();
            base.Teardown();
        }

        public class AllDirectoryNamesNormalized : WurmCharacterDirectoriesTests
        {
            [Test]
            public void ReturnsNormalizedNames()
            {
                var realdirnames = playersDirInfo.GetDirectories().Select(s => s.Name.ToUpperInvariant()).OrderBy(s => s).ToArray();
                var dirnames = system.AllDirectoryNamesNormalized.OrderBy(s => s).ToArray();
                Expect(dirnames, EqualTo(realdirnames));
            }
        }

        public class AllDirectoriesFullPaths : WurmCharacterDirectoriesTests
        {
            [Test]
            public void ReturnsFullPaths()
            {
                var realdirfullpaths = playersDirInfo.GetDirectories().Select(s => s.FullName).OrderBy(s => s).ToArray();
                var dirpaths = system.AllDirectoriesFullPaths.OrderBy(s => s).ToArray();
                Expect(dirpaths, EqualTo(realdirfullpaths));
            }
        }

        public class DirectoriesChanged : WurmCharacterDirectoriesTests
        {
            [Test]
            public void TriggersOnChanged()
            {
                bool changed = false;
                system.DirectoriesChanged += (sender, args) => changed = true;
                var dir = playersDirInfo.CreateSubdirectory("Newplayer");
                Thread.Sleep(10); // might require more delay
                //system.Refresh();
                Expect(changed, True);
                var allChars = system.GetAllCharacters().ToList();
                var allDirFullPaths = system.AllDirectoriesFullPaths.ToList();
                var allDirNames = system.AllDirectoryNamesNormalized.ToList();
                Expect(allChars, Member(new CharacterName("Newplayer")).And.Count.EqualTo(3));
                Expect(allDirFullPaths, Member(dir.FullName).And.Count.EqualTo(3));
                Expect(allDirNames, Member(dir.Name.ToUpperInvariant()).And.Count.EqualTo(3));
            }
        }

        public class GetFullDirPathForCharacter : WurmCharacterDirectoriesTests
        {
            [Test]
            public void GetsDirForCharacter()
            {
                var realPath = playersDirInfo.GetDirectories().Single(d => d.Name.Equals("someplayer", StringComparison.InvariantCultureIgnoreCase));
                var dir = system.GetFullDirPathForCharacter(new CharacterName("someplayer"));
                Expect(dir, EqualTo(realPath.FullName));
            }
        }

        public class GetAllCharacters : WurmCharacterDirectoriesTests
        {
            [Test]
            public void GetsAllCharacterNames()
            {
                var realdirnames = playersDirInfo.GetDirectories().Select(s => s.Name.ToUpperInvariant()).OrderBy(s => s).ToArray();
                var allNames = system.GetAllCharacters().OrderBy(name => name.Normalized).Select(name => name.Normalized);
                Expect(allNames, EqualTo(realdirnames));
            }
        }
    }
}
