using AldursLab.PersistentObjects;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistantLite.Bootstrapping.Persistent
{
    public class GlobalSettingsEntity : Entity
    {
        public GlobalSettingsEntity()
        {
            SetupRequired = true;
        }

        public string DataDirectoryFullPath { get; set; }
        public string WurmGameClientInstallDirectory { get; set; }
        public Platform RunningPlatform { get; set; }

        public bool SetupRequired { get; set; }
    }
}