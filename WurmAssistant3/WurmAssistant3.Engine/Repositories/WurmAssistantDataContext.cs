using System;
using AldursLab.WurmAssistant3.DataContext;
using AldursLab.WurmAssistant3.DataContext.Entities;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Repositories
{
    public class WurmAssistantDataContext : IWurmAssistantDataContext
    {
        public WurmAssistantDataContext(
            IPersistentManager persistentManager, 
            IScheduleEngine scheduleEngine)
        {
            GlobalConfig = persistentManager.GetPersistentCollection<WurmAssistantConfiguration>("WurmAssistantConfiguration").Get("#single");
            scheduleEngine.RegisterForUpdates(
                TimeSpan.FromMilliseconds(500),
                info => persistentManager.SaveAllChanged());
        }

        public IPersistent<WurmAssistantConfiguration> GlobalConfig { get; private set; }
    }
}
