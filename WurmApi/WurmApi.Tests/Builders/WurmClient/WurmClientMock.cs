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
    class WurmClientMock
    {
        readonly DirectoryHandle dir;

        DirectoryInfo WurmDir { get; set; }
        DirectoryInfo ConfigsDir { get; set; }
        DirectoryInfo PacksDir { get; set; }
        DirectoryInfo PlayersDir { get; set; }

        readonly List<WurmPlayer> players = new List<WurmPlayer>();

        readonly List<WurmConfig> configs = new List<WurmConfig>();

        public WurmClientMock([NotNull] DirectoryHandle dir)
        {
            if (dir == null) throw new ArgumentNullException("dir");
            this.dir = dir;

            InstallDirectory = Mock.Create<IWurmInstallDirectory>();
            InstallDirectory.Arrange(directory => directory.FullPath).Returns(Path.Combine(dir.AbsolutePath, "wurm"));

            var dirinfo = new DirectoryInfo(dir.AbsolutePath);
            WurmDir = dirinfo.CreateSubdirectory("wurm");
            ConfigsDir = WurmDir.CreateSubdirectory("configs");
            PacksDir = WurmDir.CreateSubdirectory("packs");
            PlayersDir = WurmDir.CreateSubdirectory("players");
        }

        public IWurmInstallDirectory InstallDirectory { get; private set; }

        public List<WurmPlayer> Players
        {
            get { return players; }
        }

        public List<WurmConfig> Configs
        {
            get { return configs; }
        }

        public WurmPlayer AddPlayer(string name)
        {
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
            var p = new WurmConfig(ConfigsDir.CreateSubdirectory(name), name);
            if (configs.Any(config => config.Name == name))
            {
                throw new InvalidOperationException("config already exists");
            }
            configs.Add(p);
            return p;
        }
    }
}