namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts
{
    public interface ILoggingConfig
    {
        string GetCurrentReadableLogFileFullPath();
        string GetCurrentVerboseLogFileFullPath();
    }
}