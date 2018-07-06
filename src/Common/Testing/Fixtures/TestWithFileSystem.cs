using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AldursLab.Testing.Fixtures
{
    public abstract class TestWithFileSystem : AssertionHelper
    {
        DirectoryHandle directoryHandle;

        public DirectoryHandle TempDir
        {
            get { return directoryHandle; }
        }

        [SetUp]
        public void BaseSetup()
        {
            directoryHandle = TempDirectoriesFactory.CreateEmpty();
        }

        [TearDown]
        public void BaseTeardown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            directoryHandle.Dispose();
        }
    }
}
