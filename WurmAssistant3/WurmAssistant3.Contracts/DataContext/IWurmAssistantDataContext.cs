using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.DataContext.Entities;

namespace AldurSoft.WurmAssistant3.DataContext
{
    public interface IWurmAssistantDataContext
    {
        IPersistent<WurmAssistantConfiguration> GlobalConfig { get; }
    }
}
