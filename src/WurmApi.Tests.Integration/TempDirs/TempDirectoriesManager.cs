using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AldursLab.WurmApi.FileSystem;
using NUnit.Framework;
using SevenZip;

namespace AldursLab.WurmApi.Tests.TempDirs
{
    class TempDirectoriesManager
    {
        readonly HashSet<DirectoryHandle> directoryHandles = new HashSet<DirectoryHandle>();

        const string DirName = "TempDirectories";
        readonly string tempDirsPath;

        internal TempDirectoriesManager()
        {
            tempDirsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, DirName);
            if (IntPtr.Size == 4)
            {
                // 32-bit
                SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "x86", "7z.dll"));
            }
            else if (IntPtr.Size == 8)
            {
                // 64-bit
                SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "x64", "7z.dll"));
            }
            else
            {
                throw new Exception("The future is now!");
            }


            if (Directory.Exists(tempDirsPath))
            {
                Directory.Delete(tempDirsPath, true);
                Directory.CreateDirectory(tempDirsPath);
            }
        }

        internal DirectoryHandle CreateEmpty()
        {
            return new DirectoryHandle(this, CreateTempDirectory());
        }

        internal DirectoryHandle CreateByCopy(string sourceDirectoryPath)
        {
            var dir = CreateTempDirectory();
            sourceDirectoryPath = AbsolutizePath(sourceDirectoryPath);
            DirectoryOps.CopyRecursively(sourceDirectoryPath, dir.FullName);
            return new DirectoryHandle(this, dir);
        }

        internal DirectoryHandle CreateByUnzippingFile(string zipFilePath)
        {
            var dir = CreateTempDirectory();
            zipFilePath = AbsolutizePath(zipFilePath);
            var extractor =
                new SevenZipExtractor(File.OpenRead(zipFilePath));
            extractor.ExtractArchive(dir.FullName);
            return new DirectoryHandle(this, dir);
        }

        DirectoryInfo CreateTempDirectory()
        {
            var id = Guid.NewGuid();
            var directoryFullPath = Path.Combine(tempDirsPath, id.ToString());

            return Directory.CreateDirectory(directoryFullPath);
        }

        string AbsolutizePath(string zipFilePath)
        {
            var rootedPath = Path.IsPathRooted(zipFilePath);
            if (!rootedPath)
            {
                zipFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, zipFilePath);
            }
            return zipFilePath;
        }

        internal void RegisterHandle(DirectoryHandle directoryHandle)
        {
            directoryHandles.Add(directoryHandle);
        }

        internal void FreeHandle(DirectoryHandle directoryHandle)
        {
            if (!directoryHandle.IsDisposed && directoryHandle.Dir.Exists)
            {
                directoryHandle.Dir.Delete(true);
            }
            directoryHandles.Remove(directoryHandle);
        }

        ~TempDirectoriesManager()
        {
            foreach (var handle in directoryHandles.ToArray())
            {
                try
                {
                    FreeHandle(handle);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine("Error during disposal of DirectoryHandle: " + exception);
                }
            }
        }
    }
}