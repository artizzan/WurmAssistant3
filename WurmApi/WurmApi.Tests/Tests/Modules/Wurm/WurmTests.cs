using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            TestPaksDirFullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "TestPaks");
            Fixture = new WurmApiFixtureV2();
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
