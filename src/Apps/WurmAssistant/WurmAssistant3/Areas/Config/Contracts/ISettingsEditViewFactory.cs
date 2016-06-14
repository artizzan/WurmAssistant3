using AldursLab.WurmAssistant3.Areas.Config.Services;

namespace AldursLab.WurmAssistant3.Areas.Config.Contracts
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface ISettingsEditViewFactory
    {
        SettingsEditForm CreateSettingsEditView();
    }
}
