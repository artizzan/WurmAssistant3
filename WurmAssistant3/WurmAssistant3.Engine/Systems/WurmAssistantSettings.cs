using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.DataContext;
using AldurSoft.WurmAssistant3.DataContext.Entities;
using AldurSoft.WurmAssistant3.Systems;

namespace AldurSoft.WurmAssistant3.Engine.Systems
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
