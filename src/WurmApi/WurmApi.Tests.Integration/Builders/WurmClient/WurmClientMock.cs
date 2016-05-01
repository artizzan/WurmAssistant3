using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.WurmApi.Tests.Integration.TempDirs;
using JetBrains.Annotations;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldursLab.WurmApi.Tests.Integration.Builders.WurmClient
{
    class WurmClientMock : IDisposable
    {
        readonly DirectoryHandle dir;
        readonly Platform targetPlatform;

        DirectoryInfo WurmDir { get; set; }
        DirectoryInfo ConfigsDir { get; set; }
        DirectoryInfo PacksDir { get; set; }
        DirectoryInfo PlayersDir { get; set; }

        readonly List<WurmPlayer> players = new List<WurmPlayer>();

        readonly List<WurmConfig> configs = new List<WurmConfig>();

        public  WurmClientMock([NotNull] DirectoryHandle dir, bool createBasicDirs, Platform targetPlatform)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            this.dir = dir;
            this.targetPlatform = targetPlatform;

            var dirinfo = new DirectoryInfo(dir.AbsolutePath);
            WurmDir = dirinfo.CreateSubdirectory("wurm");

            if (createBasicDirs)
            {
                CreateBasicDirectories();
            }

            InstallDirectory = Mock.Create<IWurmClientInstallDirectory>();
            InstallDirectory.Arrange(directory => directory.FullPath).Returns(Path.Combine(dir.AbsolutePath, "wurm"));
        }

        public IWurmClientInstallDirectory InstallDirectory { get; private set; }

        public IEnumerable<WurmPlayer> Players => players;

        public IEnumerable<WurmConfig> Configs => configs;

        public WurmClientMock CreateBasicDirectories()
        {
            CreateConfigsDir();
            CreatePacksDir();
            CreatePlayersDir();
            return this;
        }

        void CreatePacksDir()
        {
            if (PacksDir == null)
            {
                PacksDir = WurmDir.CreateSubdirectory("packs");
            }
        }

        void CreatePlayersDir()
        {
            if (PlayersDir == null)
            {
                PlayersDir = WurmDir.CreateSubdirectory("players");
            }
        }

        void CreateConfigsDir()
        {
            if (ConfigsDir == null)
            {
                ConfigsDir = WurmDir.CreateSubdirectory("configs");
            }
        }

        public WurmPlayer AddPlayer(string name)
        {
            CreatePlayersDir();
            var p = new WurmPlayer(PlayersDir.CreateSubdirectory(name), name, targetPlatform);
            if (players.Any(player => player.Name == name))
            {
                throw new InvalidOperationException("player already exists");
            }
            players.Add(p);
            p.SetConfigName("default");
            return p;
        }

        public WurmConfig AddConfig(string name)
        {
            CreateConfigsDir();
            var p = new WurmConfig(ConfigsDir.CreateSubdirectory(name), name);
            if (configs.Any(config => config.Name == name))
            {
                throw new InvalidOperationException("config already exists");
            }
            configs.Add(p);
            return p;
        }

        /// <summary>
        /// Copies / overwrites contents of wurm directory from source directory.
        /// </summary>
        /// <param name="sourceDirFullPath"></param>
        /// <returns></returns>
        public WurmClientMock PopulateFromDir(string sourceDirFullPath)
        {
            dir.AmmendFromSourceDirectory(sourceDirFullPath, "wurm");
            if (targetPlatform != Platform.Windows)
            {
                // replace all CRLF with LF, to simulate Linux log files
                var allLogFiles =
                    WurmDir.EnumerateDirectories("players")
                           .SelectMany(di => di.GetDirectories())
                           .Select(di => di.GetDirectories("logs").SingleOrDefault())
                           .Where(di => di != null)
                           .SelectMany(di => di.GetFiles());

                foreach (var fi in allLogFiles)
                {
                    var content = File.ReadAllText(fi.FullName);
                    content = content.Replace("\r\n", "\n");
                    File.WriteAllText(fi.FullName, content);
                }
            }
            return this;
        }


        public WurmClientMock PopulateFromZip(string zipFileFullPath)
        {
            using (var handle = TempDirectoriesFactory.CreateByUnzippingFile(zipFileFullPath))
            {
                PopulateFromDir(handle.AbsolutePath);
            }
            return this;
        }

        public void Dispose()
        {
            dir.Dispose();
        }
    }
}