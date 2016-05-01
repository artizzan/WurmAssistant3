using System;
using System.IO;

namespace AldursLab.WurmApi
{
    public class WurmApiDataDirectory
    {
        public WurmApiDataDirectory(
            [JetBrains.Annotations.NotNull] string fullPath,
            bool createIfNotExists = false)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
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

            FullPath = fullPath;
        }

        public string FullPath { get; }
    }
}