using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Contracts
{
    public interface IWurmAssistantConfig
    {
        string WurmGameClientInstallDirectory { get; set; }
        Platform RunningPlatform { get; set; }
        bool ReSetupRequested { get; set; }
        bool DropAllWurmApiCachesToggle { get; set; }
        bool MinimizeToTrayEnabled { get; set; }
    }
}