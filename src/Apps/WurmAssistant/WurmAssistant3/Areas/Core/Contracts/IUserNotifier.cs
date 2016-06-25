namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    public interface IUserNotifier
    {
        /// <summary>
        /// Shows a notification to the user with MessageBox.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="notifyKind"></param>
        void NotifyWithMessageBox(string text, NotifyKind notifyKind = NotifyKind.Info);
    }

    public enum NotifyKind
    {
        Info = 0, // default
        Warning,
        Error
    }
}
