namespace AldursLab.WurmAssistant.LauncherCore
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