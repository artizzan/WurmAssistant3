using AldursLab.WurmAssistant3.DataContext.Entities;

namespace AldursLab.WurmAssistant3.DataContext
{
    public interface IWurmAssistantDataContext
    {
        IPersistent<WurmAssistantConfiguration> GlobalConfig { get; }
    }
}
