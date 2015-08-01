using System;
using System.IO;
using AldursLab.Testing;

using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests
{
    public abstract class WurmApiIntegrationFixtureBase : WurmApiFixtureBase
    {
        private DirectoryHandle dataDir;
        private DirectoryHandle wurmDir;

        protected IWurmInstallDirectory WurmInstallDirectoryMock;
        protected IHttpWebRequests HttpWebRequestsMock;
        protected ILogger LoggerMock;

        protected WurmApiManager WurmApiManager;

        public DirectoryHandle WurmDir
        {
            get { return wurmDir; }
        }

        public DirectoryHandle DataDir
        {
            get { return dataDir; }
        }

        protected void ConstructApi(string wurmDirFullPath)
        {
            if (wurmDirFullPath == null) throw new ArgumentNullException("wurmDirFullPath");
            wurmDir = TempDirectoriesFactory.CreateByCopy(wurmDirFullPath);
            dataDir = TempDirectoriesFactory.CreateEmpty();

            WurmInstallDirectoryMock = Mock.Create<IWurmInstallDirectory>();
            WurmInstallDirectoryMock.Arrange(directory => directory.FullPath).Returns(WurmDir.AbsolutePath);

            HttpWebRequestsMock = Mock.Create<IHttpWebRequests>();

            LoggerMock = Mock.Create<ILogger>();

            WurmApiManager apiManager = new WurmApiManager(
                DataDir.AbsolutePath,
                WurmInstallDirectoryMock,
                HttpWebRequestsMock,
                LoggerMock);

            WurmApiManager = apiManager;
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
            WurmApiManager.Dispose();
            DataDir.Dispose();
            wurmDir.Dispose();
        }
    }
}