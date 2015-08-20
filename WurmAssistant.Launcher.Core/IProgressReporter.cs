namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IProgressReporter
    {
        /// <summary>
        /// Set current status message.
        /// </summary>
        /// <param name="message"></param>
        void SetProgressStatus(string message);

        /// <summary>
        /// Sets progress to whole percent value or set "indeterminate" if null value is passed.
        /// </summary>
        /// <param name="percent"></param>
        void SetProgressPercent(byte? percent);
    }
}