using System.Windows.Forms;
using AldursLab.Essentials.Debugging;
using AldursLab.WurmAssistant3.Core.Root.Contracts;

namespace AldursLab.WurmAssistant3.Core.Root.Components
{
    public class UserNotifier : IUserNotifier
    {
        public void NotifyWithMessageBox(string text, NotifyKind notifyKind = NotifyKind.Info)
        {
            switch (notifyKind)
            {
                 case NotifyKind.Error:
                    MessageBox.Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                 case NotifyKind.Warning:
                    MessageBox.Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case NotifyKind.Info:
                    MessageBox.Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                 default:
                    Assertions.ThrowIfDebug();
                    MessageBox.Show(text);
                    break;
            }
            
        }
    }
}