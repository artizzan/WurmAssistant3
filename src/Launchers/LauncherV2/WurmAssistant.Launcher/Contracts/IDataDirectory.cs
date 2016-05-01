namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    public interface IDataDirectory
    {
        void EnterWa3Lock();
        void ReleaseWa3Lock();
    }
}