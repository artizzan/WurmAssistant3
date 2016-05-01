namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    public interface IProcessRunner
    {
        void Start(string filePath, string args);
    }
}
