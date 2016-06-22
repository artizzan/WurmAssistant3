namespace AldursLab.WurmAssistant3.Areas.Config.Contracts
{
    public interface IWurmAssistantConfig
    {
        string WurmGameClientInstallDirectory { get; set; }
        bool WurmApiResetRequested { get; set; }
        bool DropAllWurmApiCachesToggle { get; set; }
        bool WurmUnlimitedMode { get; }
    }
}