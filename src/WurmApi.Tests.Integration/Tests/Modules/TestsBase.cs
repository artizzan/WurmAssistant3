using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.Modules
{
    class TestsBase : AssertionHelper
    {
        internal string TestPaksDirFullPath { get; private set; }
        internal string TestPaksZippedDirFullPath { get; private set; }

        public const int FileSystemDelayMillis = 20;

        public static StubbableTime TimeStub { get; private set; }

        static TestsBase()
        {
            TimeStub = new StubbableTime();
            Time.SetProvider(TimeStub);

            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        [SetUp]
        public void BaseSetup()
        {
            TestPaksDirFullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "TestPaks");
            TestPaksZippedDirFullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "TestPaksZipped");
        }

        [TearDown]
        public void BaseTeardown()
        {
            TimeStub.CurrentScope?.Dispose();
        }

        public void DoNothing() { }
        public void DoNothing<T1>(T1 arg1) { }
        public void DoNothing<T1, T2>(T1 arg1, T2 arg2) { }

        protected void WaitForFileSystem()
        {
            Thread.Sleep(FileSystemDelayMillis);
        }
    }
}
