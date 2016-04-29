using System.IO;
using AldursLab.WurmApi.Tests.Integration.Builders;
using AldursLab.WurmApi.Tests.Integration.Builders.WurmClient;
using AldursLab.WurmApi.Tests.Integration.TempDirs;
using Telerik.JustMock;

namespace AldursLab.WurmApi.Tests
{
    class WurmApiIntegrationFixtureV2
    {
        readonly Platform targetPlatform;
        // note: 
        // using default ThreadPool marshallers for both internal and public events
        // while simple marshaller would speed tests, using thread pool can potentially uncover more bugs

        WurmApiManager wurmApiManager;
        readonly object locker = new object();

        public WurmApiIntegrationFixtureV2(Platform targetPlatform)
        {
            this.targetPlatform = targetPlatform;

            var handle = TempDirectoriesFactory.CreateEmpty();
            WurmApiDataDir = new DirectoryInfo(handle.AbsolutePath);
            WurmClientMock = WurmClientMockBuilder.Create(targetPlatform);
            LoggerMock = Mock.Create<IWurmApiLogger>().RedirectToTraceOut(true);
            HttpWebRequestsMock = Mock.Create<IHttpWebRequests>();
        }

        public DirectoryInfo WurmApiDataDir { get; private set; }

        public WurmClientMock WurmClientMock { get; private set; }

        public IWurmApiLogger LoggerMock { get; set; }

        public WurmApiManager WurmApiManager
        {
            get
            {
                lock (locker)
                {
                    if (wurmApiManager == null)
                    {
                        wurmApiManager = new WurmApiManager(
                            WurmApiDataDir.FullName,
                            WurmClientMock.InstallDirectory,
                            HttpWebRequestsMock,
                            LoggerMock,
                            new WurmApiConfig() { Platform = targetPlatform });
                    }
                    return wurmApiManager;
                }
            }
        }

        public IHttpWebRequests HttpWebRequestsMock { get; private set; }
    }
}
