using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Infrastructure;

namespace AldursLab.WurmAssistant3.Infrastructure
{
    class WurmAssistantConfig : IWurmAssistantConfig
    {
        public string DataDirectoryFullPath { get; set; }
        public string WurmGameClientInstallDirectory { get; set; }
        public Platform RunningPlatform { get; set; }
    }
}