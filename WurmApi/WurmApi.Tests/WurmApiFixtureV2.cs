using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Testing;
using AldurSoft.WurmApi.Tests.Builders;
using AldurSoft.WurmApi.Tests.Builders.WurmClient;
using Telerik.JustMock;

namespace AldurSoft.WurmApi.Tests
{
    class WurmApiFixtureV2
    {
        public WurmApiFixtureV2()
        {
            var handle = TempDirectoriesFactory.CreateEmpty();
            WurmApiDataDir = new DirectoryInfo(handle.AbsolutePath);
            WurmClientMock = WurmClientMockBuilder.Create();
            WurmApiManager = new WurmApiManager(WurmApiDataDir.FullName, WurmClientMock.InstallDirectory,
                Mock.Create<IHttpWebRequests>(), Mock.Create<ILogger>().RedirectToTraceOut());
        }

        public DirectoryInfo WurmApiDataDir { get; private set; }

        public WurmApiManager WurmApiManager { get; private set; }
        public WurmClientMock WurmClientMock { get; private set; }
    }
}
