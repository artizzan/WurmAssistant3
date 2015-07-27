using System;
using System.IO;

using AldurSoft.Core.Testing;
using Moq;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests
{
    public abstract class WurmApiIntegrationFixtureBase : WurmApiFixtureBase
    {
        private TestPak dataDir;
        private TestPak wurmDir;

        protected Mock<IWurmInstallDirectory> WurmInstallDirectoryMock;
        protected Mock<IHttpWebRequests> HttpWebRequestsMock;
        protected Mock<ILogger> LoggerMock;

        protected WurmApiManager WurmApiManager;

        public TestPak WurmDir
        {
            get { return wurmDir; }
        }

        public TestPak DataDir
        {
            get { return dataDir; }
        }

        protected void ConstructApi(string wurmDirFullPath)
        {
            if (wurmDirFullPath == null) throw new ArgumentNullException("wurmDirFullPath");
            wurmDir = CreateTestPakFromDir(wurmDirFullPath);
            dataDir = CreateTestPakEmptyDir();

            WurmInstallDirectoryMock = new Mock<IWurmInstallDirectory>();
            WurmInstallDirectoryMock.Setup(directory => directory.FullPath).Returns(WurmDir.DirectoryFullPath);

            HttpWebRequestsMock = new Mock<IHttpWebRequests>();

            LoggerMock = new Mock<ILogger>();

            WurmApiManager apiManager = new WurmApiManager(
                DataDir.DirectoryFullPath,
                WurmInstallDirectoryMock.Object,
                HttpWebRequestsMock.Object,
                LoggerMock.Object);

            WurmApiManager = apiManager;
        }

        public override void Teardown()
        {
            ExecuteAll(WurmApiManager.Dispose, base.Teardown);
        }
    }
}