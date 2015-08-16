using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Infrastructure
{
    public interface IWurmAssistantConfig
    {
        string DataDirectoryFullPath { get; }
        string WurmGameClientInstallDirectory { get; }
        Platform RunningPlatform { get;}
    }
}