using AldursLab.WurmAssistant3.Core.Areas.Config.Views;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Contracts
{
    public interface IServersEditorViewFactory
    {
        ServersEditorView CreateServersEditorView();
    }
}