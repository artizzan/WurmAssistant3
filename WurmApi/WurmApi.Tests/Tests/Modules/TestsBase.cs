using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.Modules
{
    public class TestsBase : AssertionHelper
    {
        internal string TestPaksDirFullPath { get; private set; }

        public const int FileSystemDelayMillis = 20;

        public static StubbableTime TimeStub { get; private set; }

        static TestsBase()
        {
            TimeStub = new StubbableTime();
            Time.SetProvider(TimeStub);
        }

        [SetUp]
        public void BaseSetup()
        {
            TestPaksDirFullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "TestPaks");
        }

        [TearDown]
        public void BaseTeardown()
        {
            if (TimeStub.CurrentScope != null) TimeStub.CurrentScope.Dispose();
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
