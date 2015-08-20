using System.Windows.Forms;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IGuiHost
    {
        void ShowHostWindow();
        void HideHostWindow();
        void SetContent(UserControl userControl);
        void Close();
    }
}