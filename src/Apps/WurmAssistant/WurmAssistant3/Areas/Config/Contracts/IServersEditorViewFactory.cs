using AldursLab.WurmAssistant3.Areas.Config.Transients;

namespace AldursLab.WurmAssistant3.Areas.Config.Contracts
{
    [NinjectFactory]
    public interface IServersEditorViewFactory
    {
        ServersEditorForm CreateServersEditorView();
    }
}