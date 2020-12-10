using System;

namespace AldursLab.WurmAssistant3.Areas.Config
{
    public interface IWurmAssistantConfig
    {
        string WurmGameClientInstallDirectory { get; set; }
        bool WurmApiResetRequested { get; set; }
        bool DropAllWurmApiCachesToggle { get; set; }
        bool WurmUnlimitedMode { get; }
        bool SkipWurmConfigsValidation { get; set; }
        Guid InstallationId { get; set; }
        bool AllowInsights { get; set; }
        bool UseTopRightPopupStrategy { get; set; }
    }
}