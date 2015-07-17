using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;

namespace AldurSoft.WurmApi.Tests
{
    public abstract class WurmApiFixtureBase : FixtureBase
    {
        public static string TestPaksDirFullPath { get; private set; }
        public const int FileSystemDelayMillis = 20;

        static WurmApiFixtureBase()
        {
            TestPaksDirFullPath = Path.Combine(TestEnv.BinDirectory, "Resources", "TestPaks");
        }

        protected void WaitForFileSystem()
        {
            Thread.Sleep(FileSystemDelayMillis);
        }
    }
}
