using System;
using System.IO;

namespace AldurSoft.WurmApi
{
    public class WurmApiDataDirectory
    {
        private readonly string fullPath;

        public WurmApiDataDirectory(
            [JetBrains.Annotations.NotNull] string fullPath,
            bool createIfNotExists = false)
        {
            if (fullPath == null) throw new ArgumentNullException("fullPath");
            if (!Directory.Exists(fullPath))
            {
                if (createIfNotExists)
                {
                    Directory.CreateDirectory(fullPath);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Specified directory does not exist. Call this constructor with createIfNotExists to create directory on demand.");
                }
            }

            this.fullPath = fullPath;
        }

        public string FullPath
        {
            get { return fullPath; }
        }
    }
}