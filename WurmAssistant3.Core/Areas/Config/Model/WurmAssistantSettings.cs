using System;
using System.IO;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Model
{
    public class WurmAssistantSettings : PersistentEntityBase<WurmAssistantSettings.PersistentData>
    {
        public WurmAssistantSettings(IPersistent<PersistentData> persistent)
            : base(persistent)
        {
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

        public bool RecondingRequested
        {
            get { return Entity.RecondingRequested; }
            set
            {
                if (value == Entity.RecondingRequested)
                    return;
                Entity.RecondingRequested = value;
                FlagAsChanged();
            }
        }

        public class PersistentData : Entity
        {
            public string WurmGameClientInstallDirectory { get; set; }
            public Platform RunningPlatform { get; set; }
            public bool RecondingRequested { get; set; }
        }
    }
}