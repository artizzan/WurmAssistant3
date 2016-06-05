using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Areas.Config.Contracts
{
    public interface IWurmAssistantConfig
    {
        string WurmGameClientInstallDirectory { get; set; }
        Platform RunningPlatform { get; }
        bool WurmApiResetRequested { get; set; }
        bool DropAllWurmApiCachesToggle { get; set; }
        bool WurmUnlimitedMode { get; }
    }
}