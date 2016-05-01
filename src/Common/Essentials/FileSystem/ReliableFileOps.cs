using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.FileSystem
{
    public static class ReliableFileOps
    {
        private const string FileExtensionNew = ".new";
        private const string FileExtensionOld = ".old";

        /// <summary>
        /// Null if file not found.
        /// </summary>
        public static string TryReadFileContents(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                // file does not exist, attempt to recover
                if (File.Exists(absolutePath + FileExtensionOld))
                {
                    // .old file exists, this means .new file might not have finished saving and must be ignored
                    File.Move(absolutePath + FileExtensionOld, absolutePath);
                }
                else if (File.Exists(absolutePath + FileExtensionNew))
                {
                    // since .old file does not exist, but .new file was found, it should be complete and we can use it.
                    File.Move(absolutePath + FileExtensionNew, absolutePath);
                }
                else
                {
                    return null;
                }
            }
            using (FileStream fileStream = File.OpenRead(absolutePath))
            {
                using (var sr = new StreamReader(fileStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static void SaveFileContents(string absolutePath, string newContents)
        {
            var dir = Path.GetDirectoryName(absolutePath);
            if (dir == null)
            {
                throw new InvalidOperationException("Path.GetDirectoryName(absolutePath) returned null for filepath: " + absolutePath);
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(absolutePath + FileExtensionNew))
            {
                File.Delete(absolutePath + FileExtensionNew);
            }
            if (File.Exists(absolutePath + FileExtensionOld))
            {
                File.Delete(absolutePath + FileExtensionOld);
            }

            // write down temporary new file
            using (
                var fs = new FileStream(absolutePath + FileExtensionNew,
                    FileMode.Create,
                    FileAccess.ReadWrite,
                    FileShare.None))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(newContents);
                    // forcing hard flushing to avoid incomplete physical file write, which can cause corruption on system crash
                    fs.Flush(true);
                }
            }

            if (File.Exists(absolutePath))
            {
                // demote current file to a backup file
                File.Move(absolutePath, absolutePath + FileExtensionOld);
            }
            //promote temporary file to current file
            File.Move(absolutePath + FileExtensionNew, absolutePath);
            //operation succeeded, cleanup backup file
            File.Delete(absolutePath + FileExtensionOld);
        }
    }
}
