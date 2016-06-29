namespace AldursLab.WurmAssistant3.Areas.Config
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface ISettingsEditViewFactory
    {
        SettingsEditForm CreateSettingsEditView();
    }
}
