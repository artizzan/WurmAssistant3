using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Areas.WurmApi.Modules
{
    class WurmInstallDirectoryOverride : IWurmClientInstallDirectory
    {
        public string FullPath { get; set; }
    }
}