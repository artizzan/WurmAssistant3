namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IGui : IProgressReporter
    {
        void ShowGui();

        void AddUserMessage(string message);

        void HideGui();

        void SetState(LauncherState state);
    }

    public enum LauncherState
    {
        Running,
        Error
    }
}