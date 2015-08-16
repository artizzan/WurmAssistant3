using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Infrastructure
{
    class WurmInstallDirectoryOverride : IWurmInstallDirectory
    {
        public string FullPath { get; set; }
    }
}