using System;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.WurmApi
{
    class WurmInstallDirectoryOverride : IWurmClientInstallDirectory
    {
        public WurmInstallDirectoryOverride([NotNull] string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            this.FullPath = fullPath;
        }

        public string FullPath { get; }
    }
}