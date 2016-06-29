using System.Diagnostics;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    [KernelBind(BindingHint.Singleton)]
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
                    Debug.Fail("unexpected NotifyKind");
                    MessageBox.Show(text);
                    break;
            }
        }
    }
}