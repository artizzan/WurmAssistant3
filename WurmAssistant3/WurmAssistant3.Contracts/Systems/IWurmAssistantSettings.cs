using AldurSoft.WurmAssistant3.DataContext.Entities;

namespace AldurSoft.WurmAssistant3.Systems
{
    public interface IWurmAssistantSettings
    {
        WurmAssistantConfiguration Entity { get; }

        void Save();
    }
}
