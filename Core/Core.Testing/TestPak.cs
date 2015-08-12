using System;
using System.IO;

namespace AldursLab.Deprec.Core.Testing
{
    public abstract class TestPak
    {
        private Guid id;
        private string directoryFullPath;

        private static readonly bool Initialized = false;
        protected static readonly string TestPakTempDirFullPath;

        static TestPak()
        {
            if (!Initialized)
            {
                TestPakTempDirFullPath = Path.Combine(TestEnv.BinDirectory, "TestPakTempDir");
                SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(TestEnv.BinDirectory, "7z.dll"));

                if (Directory.Exists(TestPakTempDirFullPath))
                {
                    Directory.Delete(TestPakTempDirFullPath, true);
                    Directory.CreateDirectory(TestPakTempDirFullPath);
                }

                Initialized = true;
            }
        }

        protected TestPak()
        {
            CreateDirectoryForPackage();
        }

        private void CreateDirectoryForPackage()
        {
            id = Guid.NewGuid();
            directoryFullPath = Path.Combine(TestPakTempDirFullPath, id.ToString());

            Directory.CreateDirectory(directoryFullPath);
        }

        public Guid Id
        {
            get { return id; }
        }

        public string DirectoryFullPath
        {
            get { return directoryFullPath; }
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(DirectoryFullPath))
                {
                    Directory.Delete(DirectoryFullPath, recursive: true);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("error while disposing {0}, exception: {1}", this, exception));
            }
        }

        public override string ToString()
        {
            return string.Format("[TestPak; Id: {0}, DirectoryFullPath: {1}]", Id, DirectoryFullPath);
        }
    }

    internal class DirCopyTestPak : TestPak
    {
        public DirCopyTestPak(string sourceDirFullPath)
        {
            var dirInfo = new DirectoryInfo(sourceDirFullPath);

            DirectoryCopier copier = new DirectoryCopier();
            copier.DirectoryCopy(dirInfo.FullName, DirectoryFullPath);
        }
    }

    internal class UnzipFileTestPak : TestPak
    {
        public UnzipFileTestPak(string sevenZipFileFullPath)
        {
            var extractor =
                new SevenZipExtractor(File.OpenRead(sevenZipFileFullPath));
            extractor.ExtractArchive(DirectoryFullPath);
        }
    }

    internal class EmptyDirTestPak : TestPak
    {
        // base creates and disposes of this dir
    }
}
