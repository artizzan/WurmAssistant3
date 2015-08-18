using System.Windows.Forms;

namespace AldursLab.WurmAssistant.LauncherCore
{
    public interface IGuiHost
    {
        void ShowHostWindow();
        void HideHostWindow();
        void SetContent(UserControl userControl);
        void Close();
    }
}