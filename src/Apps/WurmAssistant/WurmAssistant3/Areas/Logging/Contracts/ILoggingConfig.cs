namespace AldursLab.WurmAssistant3.Areas.Logging.Contracts
{
    public interface ILoggingConfig
    {
        string GetCurrentReadableLogFileFullPath();
        string GetCurrentVerboseLogFileFullPath();
    }
}