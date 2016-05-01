using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AldursLab.Testing.Tests.TempDirectoriesTests
{
    [TestFixture]
    public class UnitTests : AssertionHelper
    {
        [Test]
        public void CreateEmpty_CreatesAndCleanups()
        {
            DirectoryHandle dirhandle;
            using (
                 dirhandle = TempDirectoriesFactory.CreateEmpty())
            {
                Expect(dirhandle.Exists, "Directory not created");
            }
            Expect(!dirhandle.Exists, "Directory not deleted");
        }

        [Test]
        public void CreateByCopy_WithRelativePath_CreatesAndCleanups()
        {
            DirectoryHandle dirhandle;
            using (
                dirhandle =
                    TempDirectoriesFactory.CreateByCopy(Path.Combine("TempDirectoriesTests", "SampleSourceDir")))
            {
                Expect(dirhandle.Exists, "Directory not created");
                Expect(dirhandle.GetFiles().Count(), GreaterThan(0), "No files in temp directory");
            }
            Expect(!dirhandle.Exists, "Directory not deleted");
        }

        [Test]
        public void CreateByCopy_WithAbsolutePath_CreatesAndCleanups()
        {
            DirectoryHandle dirhandle;
            using (
                dirhandle =
                    TempDirectoriesFactory.CreateByCopy(Path.Combine(TestContext.CurrentContext.TestDirectory,
                        "TempDirectoriesTests", "SampleSourceDir")))
            {
                Expect(dirhandle.Exists, "Directory not created");
                Expect(dirhandle.GetFiles().Count(), GreaterThan(0), "No files in temp directory");
            }
            Expect(!dirhandle.Exists, "Directory not deleted");
        }

        [Test]
        public void CreateByUnzippingFile_WithRelativePath_CreatesAndCleanups()
        {
            DirectoryHandle dirhandle;
            using (
                dirhandle =
                    TempDirectoriesFactory.CreateByUnzippingFile(Path.Combine("TempDirectoriesTests",
                        "SampleZippedDir.7z")))
            {
                Expect(dirhandle.Exists, "Directory not created");
                Expect(dirhandle.GetFiles().Count(), GreaterThan(0), "No files in temp directory");
            }
            Expect(!dirhandle.Exists, "Directory not deleted");
        }

        [Test]
        public void CreateByUnzippingFile_WithAbsolutePath_CreatesAndCleanups()
        {
            DirectoryHandle dirhandle;
            using (
                dirhandle =
                    TempDirectoriesFactory.CreateByUnzippingFile(Path.Combine(TestContext.CurrentContext.TestDirectory,
                        "TempDirectoriesTests", "SampleZippedDir.7z")))
            {
                Expect(dirhandle.Exists, "Directory not created");
                Expect(dirhandle.GetFiles().Count(), GreaterThan(0), "No files in temp directory");
            }
            Expect(!dirhandle.Exists, "Directory not deleted");
        }

        [Test]
        [Ignore("for manual testing only")]
        [Category("Manual")]
        public void AutoCleanupsOnAppDomainExit()
        {
            TempDirectoriesFactory.CreateEmpty();
            // dir should not be there after testing finishes and finalizers run on manager
        }
    }
}
