namespace AldursLab.WurmAssistant3.Areas.Logging
{
    public interface ILoggingConfig
    {
        string GetCurrentReadableLogFileFullPath();
        string GetCurrentVerboseLogFileFullPath();
        string LogsDirectoryFullPath { get; }
    }
}