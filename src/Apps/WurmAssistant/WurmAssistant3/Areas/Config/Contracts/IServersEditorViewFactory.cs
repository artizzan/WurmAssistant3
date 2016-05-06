using AldursLab.WurmAssistant3.Areas.Config.Views;

namespace AldursLab.WurmAssistant3.Areas.Config.Contracts
{
    public interface IServersEditorViewFactory
    {
        ServersEditorView CreateServersEditorView();
    }
}