using AldursLab.PersistentObjects;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistantLite.Bootstrapping.Persistent
{
    public class WurmAssistantLiteSettings : PersistentEntityBase<GlobalSettingsEntity>
    {
        public WurmAssistantLiteSettings(IPersistent<GlobalSettingsEntity> persistent) : base(persistent)
        {
        }

        public string DataDirectoryFullPath
        {
            get { return Entity.DataDirectoryFullPath; }
            set
            {
                if (value == Entity.DataDirectoryFullPath) return;
                Entity.DataDirectoryFullPath = value;
                FlagAsChanged();
            }
        }

        public string WurmGameClientInstallDirectory
        {
            get { return Entity.WurmGameClientInstallDirectory; }
            set
            {
                if (value == Entity.WurmGameClientInstallDirectory) return;
                Entity.WurmGameClientInstallDirectory = value;
                FlagAsChanged();
            }
        }

        public Platform RunningPlatform
        {
            get { return Entity.RunningPlatform; }
            set
            {
                if (value == Entity.RunningPlatform) return;
                Entity.RunningPlatform = value;
                FlagAsChanged();
            }
        }

        public bool SetupRequired
        {
            get { return Entity.SetupRequired; }
            set
            {
                if (value == Entity.SetupRequired) return;
                Entity.SetupRequired = value;
                FlagAsChanged();
            }
        }
    }
}