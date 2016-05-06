using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Areas.WurmApi.Modules
{
    class WurmInstallDirectoryOverride : IWurmClientInstallDirectory
    {
        public string FullPath { get; set; }
    }
}