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
    public abstract class WurmTests : TestsBase
    {
        internal WurmApiFixtureV2 Fixture { get; private set; }
        internal WurmClientMock ClientMock { get { return Fixture.WurmClientMock; } }

        [SetUp]
        public void WurmTestsSetup()
        {
            //int maxo, maxioo, mino, minioo;
            //ThreadPool.GetMaxThreads(out maxo, out maxioo);
            //ThreadPool.GetMinThreads(out mino, out minioo);
            //Trace.WriteLine(string.Format("ThreadPool: Max worker threads: {0}, Max IO ops: {1}", maxo, maxioo));
            //Trace.WriteLine(string.Format("ThreadPool: Min worker threads: {0}, Min IO ops: {1}", mino, minioo));

            Fixture = new WurmApiFixtureV2();
            WurmApiTuningParams.PublicEventMarshallerDelay = TimeSpan.FromMilliseconds(1);
        }

        [TearDown]
        public void WurmTestsTeardown()
        {
            //Fixture.WurmApiManager.Dispose();
            //Fixture.WurmClientMock.Dispose();
        }

        protected void WaitUntilTrue(Func<bool> conditionFunc, int timeoutMillis = 5000)
        {
            int currentWait = 0;
            while (true)
            {
                const int loopMillis = 50;
                Thread.Sleep(loopMillis);
                currentWait += loopMillis;
                var result = conditionFunc();
                if (result) break;
                if (currentWait > timeoutMillis)
                {
                    throw new Exception("timeout");
                }
            }
        }
    }
}
