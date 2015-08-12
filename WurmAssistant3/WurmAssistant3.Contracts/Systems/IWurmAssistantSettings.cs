using AldursLab.WurmAssistant3.DataContext.Entities;

namespace AldursLab.WurmAssistant3.Systems
{
    public interface IWurmAssistantSettings
    {
        WurmAssistantConfiguration Entity { get; }

        void Save();
    }
}
