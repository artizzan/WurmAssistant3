using System;
using AldursLab.WurmAssistant3.DataContext;
using AldursLab.WurmAssistant3.DataContext.Entities;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Systems
{
    public class WurmAssistantSettings : IWurmAssistantSettings
    {
        private readonly IPersistent<WurmAssistantConfiguration> globalConfig; 

        public WurmAssistantSettings(IWurmAssistantDataContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            globalConfig = context.GlobalConfig;
        }

        public WurmAssistantConfiguration Entity
        {
            get { return globalConfig.Entity; }
        }

        public void Save()
        {
            globalConfig.Save();
        }
    }
}
