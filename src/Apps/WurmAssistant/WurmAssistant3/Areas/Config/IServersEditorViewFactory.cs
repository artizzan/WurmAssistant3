namespace AldursLab.WurmAssistant3.Areas.Config
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface IServersEditorViewFactory
    {
        ServersEditorForm CreateServersEditorView();
    }
}