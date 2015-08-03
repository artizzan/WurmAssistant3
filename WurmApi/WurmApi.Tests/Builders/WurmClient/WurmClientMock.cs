using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.Testing;
using JetBrains.Annotations;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.Builders.WurmClient
{
    class WurmClientMock : IDisposable
    {
        readonly DirectoryHandle dir;

        DirectoryInfo WurmDir { get; set; }
        DirectoryInfo ConfigsDir { get; set; }
        DirectoryInfo PacksDir { get; set; }
        DirectoryInfo PlayersDir { get; set; }

        readonly List<WurmPlayer> players = new List<WurmPlayer>();

        readonly List<WurmConfig> configs = new List<WurmConfig>();

        public  WurmClientMock([NotNull] DirectoryHandle dir, bool createBasicDirs)
        {
            if (dir == null) throw new ArgumentNullException("dir");
            this.dir = dir;

            var dirinfo = new DirectoryInfo(dir.AbsolutePath);
            WurmDir = dirinfo.CreateSubdirectory("wurm");

            if (createBasicDirs)
            {
                CreateBasicDirectories();
            }

            InstallDirectory = Mock.Create<IWurmInstallDirectory>();
            InstallDirectory.Arrange(directory => directory.FullPath).Returns(Path.Combine(dir.AbsolutePath, "wurm"));
        }

        public IWurmInstallDirectory InstallDirectory { get; private set; }

        public IEnumerable<WurmPlayer> Players
        {
            get
            {
                return players;
            }
        }

        public IEnumerable<WurmConfig> Configs
        {
            get { return configs; }
        }

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
            var p = new WurmPlayer(PlayersDir.CreateSubdirectory(name), name);
            if (players.Any(player => player.Name == name))
            {
                throw new InvalidOperationException("player already exists");
            }
            players.Add(p);
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
            return this;
        }

        public void Dispose()
        {
            dir.Dispose();
        }
    }
}