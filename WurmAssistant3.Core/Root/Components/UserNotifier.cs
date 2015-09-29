using AldursLab.WurmAssistant3.Core.Root.Contracts;

namespace AldursLab.WurmAssistant3.Core.Root.Components
{
    public class UserNotifier : IUserNotifier
    {
        public void Notify(string text)
        {
            System.Windows.Forms.MessageBox.Show(text);
        }
    }
}