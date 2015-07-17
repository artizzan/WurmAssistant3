using System;
using System.Diagnostics;
using System.IO;

using Aldurcraft.Core;

using SevenZip;

namespace WurmAssistant3.TestResources
{
    public abstract class TestingResourcesPackage : IDisposable
    {
        private Guid id;
        private string directoryFullPath;

        protected static readonly string ActivePackagesDirectory;
        protected static readonly string BlueprintPackagesDirectory;
        private static readonly bool Initialized = false;

        static TestingResourcesPackage()
        {
            if (!Initialized)
            {
                ActivePackagesDirectory = Path.Combine(TestingEnvironment.BinDirectory, "ActiveTestingResourcePackages");
                BlueprintPackagesDirectory = Path.Combine(TestingEnvironment.BinDirectory, "BlueprintResourcePackages");
                SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(TestingEnvironment.BinDirectory, "7z.dll"));

                // cleanup lost packages
                if (Directory.Exists(ActivePackagesDirectory))
                    Directory.Delete(ActivePackagesDirectory, recursive: true);

                Initialized = true;
            }
        }

        protected TestingResourcesPackage()
        {
            CreateDirectoryForPackage();
        }

        private void CreateDirectoryForPackage()
        {
            id = Guid.NewGuid();
            directoryFullPath = Path.Combine(ActivePackagesDirectory, id.ToString());

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
                Directory.Delete(DirectoryFullPath, recursive: true);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("error while disposing {0}, exception: {1}", this, exception));
            }
        }

        public override string ToString()
        {
            return string.Format("[TestingResourcesPackage; Id: {0}, DirectoryFullPath: {1}]", Id, DirectoryFullPath);
        }
    }

    /// <summary>
    /// Prepares a set of files contained in 7z archive at WurmApi.TestResources.BlueprintResourcePackages
    /// </summary>
    public class SevenZipResourcePackage : TestingResourcesPackage
    {
        public SevenZipResourcePackage(string sevenZipFilePath)
        {
            var extractor =
                new SevenZipExtractor(File.OpenRead(Path.Combine(BlueprintPackagesDirectory, sevenZipFilePath)));
            extractor.ExtractArchive(DirectoryFullPath);
        }
    }

    /// <summary>
    /// Prepares a set of files contained in specified test resource directory at WurmApi.TestResources.BlueprintResourcePackages
    /// </summary>
    public class DirectoryResourcePackage : TestingResourcesPackage
    {
        public DirectoryResourcePackage(string packageSourceDirectory)
        {
            var dirInfo = new DirectoryInfo(Path.Combine(BlueprintPackagesDirectory, packageSourceDirectory));

            DirectoryCopier copier = new DirectoryCopier();
            copier.DirectoryCopy(dirInfo.FullName, DirectoryFullPath);
        }
    }

    public class EmptyDirectoryResourcePackage : TestingResourcesPackage
    {
    }
}
