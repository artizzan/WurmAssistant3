using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Tests.Builders.WurmClient;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm
{
    public abstract class WurmTests : AssertionHelper
    {
        internal WurmApiFixtureV2 Fixture { get; private set; }
        internal WurmClientMock ClientMock { get { return Fixture.WurmClientMock; } }

        internal string TestPaksDirFullPath { get; private set; }

        [SetUp]
        public void WurmTestsSetup()
        {
            //int maxo, maxioo, mino, minioo;
            //ThreadPool.GetMaxThreads(out maxo, out maxioo);
            //ThreadPool.GetMinThreads(out mino, out minioo);
            //Trace.WriteLine(string.Format("ThreadPool: Max worker threads: {0}, Max IO ops: {1}", maxo, maxioo));
            //Trace.WriteLine(string.Format("ThreadPool: Min worker threads: {0}, Min IO ops: {1}", mino, minioo));

            TestPaksDirFullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "TestPaks");
            Fixture = new WurmApiFixtureV2();
            WurmApiTuningParams.PublicEventMarshallerDelay = TimeSpan.FromMilliseconds(1);
        }

        [TearDown]
        public void WurmTestsTeardown()
        {
            Fixture.WurmApiManager.Dispose();
            Fixture.WurmClientMock.Dispose();
        }

        public void DoNothing() { }
        public void DoNothing<T1>(T1 arg1) { }
        public void DoNothing<T1, T2>(T1 arg1, T2 arg2) { }
    }
}
