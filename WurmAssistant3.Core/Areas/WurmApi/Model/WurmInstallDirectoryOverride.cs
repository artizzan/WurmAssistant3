using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Areas.WurmApi.Model
{
    class WurmInstallDirectoryOverride : IWurmClientInstallDirectory
    {
        public string FullPath { get; set; }
    }
}