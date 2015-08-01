using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests
{
    public abstract class WurmApiFixtureBase : AssertionHelper
    {
        public static string TestPaksDirFullPath { get; private set; }
        public const int FileSystemDelayMillis = 20;
        public static StubbableTime TimeStub { get; private set; }

        static WurmApiFixtureBase()
        {
            TestPaksDirFullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "TestPaks");
            TimeStub = new StubbableTime();
            Time.SetProvider(TimeStub);
        }

        protected void WaitForFileSystem()
        {
            Thread.Sleep(FileSystemDelayMillis);
        }

        [TearDown]
        public virtual void Teardown() { }

        public void DoNothing() { }
        public void DoNothing<T1>(T1 arg1) { }
        public void DoNothing<T1, T2>(T1 arg1, T2 arg2) { }
    }
}
