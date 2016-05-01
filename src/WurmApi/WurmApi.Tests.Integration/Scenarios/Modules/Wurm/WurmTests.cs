using System;
using System.Threading;
using AldursLab.WurmApi.Tests.Integration.Builders.WurmClient;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.Modules.Wurm
{
    abstract class WurmTests : TestsBase
    {
        readonly Platform targetPlatform;

        internal WurmApiIntegrationFixtureV2 Fixture { get; private set; }
        internal WurmClientMock ClientMock => Fixture.WurmClientMock;

        protected WurmTests() : this(Platform.Windows) 
        {}

        protected WurmTests(Platform targetPlatform)
        {
            this.targetPlatform = targetPlatform;
        }

        [SetUp]
        public void WurmTestsSetup()
        {
            //int maxo, maxioo, mino, minioo;
            //ThreadPool.GetMaxThreads(out maxo, out maxioo);
            //ThreadPool.GetMinThreads(out mino, out minioo);
            //Trace.WriteLine(string.Format("ThreadPool: Max worker threads: {0}, Max IO ops: {1}", maxo, maxioo));
            //Trace.WriteLine(string.Format("ThreadPool: Min worker threads: {0}, Min IO ops: {1}", mino, minioo));

            Fixture = new WurmApiIntegrationFixtureV2(targetPlatform);
            WurmApiTuningParams.PublicEventMarshallerDelay = TimeSpan.FromMilliseconds(1);
        }

        [TearDown]
        public void WurmTestsTeardown()
        {
            Fixture.WurmApiManager.Dispose();
            Fixture.WurmClientMock.Dispose();
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
