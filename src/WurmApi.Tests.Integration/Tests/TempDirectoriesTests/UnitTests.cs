using System.IO;
using System.Linq;
using AldursLab.WurmApi.Tests.TempDirs;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.TempDirectoriesTests
{
    [TestFixture]
    public class UnitTests : AssertionHelper
    {
        static readonly string LocalTestResourcePath = Path.Combine("Tests", "TempDirectoriesTests");

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
                    TempDirectoriesFactory.CreateByCopy(Path.Combine(LocalTestResourcePath, "SampleSourceDir")))
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
                        LocalTestResourcePath, "SampleSourceDir")))
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
                    TempDirectoriesFactory.CreateByUnzippingFile(Path.Combine(LocalTestResourcePath,
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
                        LocalTestResourcePath, "SampleZippedDir.7z")))
            {
                Expect(dirhandle.Exists, "Directory not created");
                Expect(dirhandle.GetFiles().Count(), GreaterThan(0), "No files in temp directory");
            }
            Expect(!dirhandle.Exists, "Directory not deleted");
        }
    }
}
