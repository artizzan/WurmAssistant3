using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.SimplePersist.Persistence.FlatFiles
{
    class TransactionalFileOps
    {
        private const string FileExtensionNew = ".new";
        private const string FileExtensionOld = ".old";

        /// <summary>
        /// Empty string if nothing found.
        /// </summary>
        public string LoadFileContents(string filePath)
        {
            if (!File.Exists(filePath))
            {
                if (File.Exists(filePath + FileExtensionNew))
                {
                    File.Move(filePath + FileExtensionNew, filePath);
                }
                else if (File.Exists(filePath + FileExtensionOld))
                {
                    File.Move(filePath + FileExtensionOld, filePath);
                }
                else
                {
                    return string.Empty;
                }
            }
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                using (var sr = new StreamReader(fileStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public void SaveFileContents(string filePath, string newContents)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (dir == null)
            {
                throw new InvalidOperationException("Path.GetDirectoryName(filePath) returned null for filepath: " + filePath);
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(filePath + FileExtensionNew))
            {
                File.Delete(filePath + FileExtensionNew);
            }
            if (File.Exists(filePath + FileExtensionOld))
            {
                File.Delete(filePath + FileExtensionOld);
            }

            File.WriteAllText(filePath + FileExtensionNew, newContents, Encoding.UTF8);

            if (File.Exists(filePath))
            {
                File.Move(filePath, filePath + FileExtensionOld);
            }
            File.Move(filePath + FileExtensionNew, filePath);
            File.Delete(filePath + FileExtensionOld);
        }
    }
}
