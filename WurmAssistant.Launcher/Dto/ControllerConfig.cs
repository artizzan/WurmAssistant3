using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Dto
{
    public class ControllerConfig
    {
        public string LauncherBinDirFullPath { get; set; }
        public string WebServiceRootUrl { get; set; }
        public string WurmAssistantExeFileName { get; set; }
        public string BuildCode { get; set; }
        [CanBeNull] public string BuildNumber { get; set; }
        public bool WurmUnlimitedMode { get; set; }
        public bool UseRelativeDataDirPath { get; set; }
    }
}