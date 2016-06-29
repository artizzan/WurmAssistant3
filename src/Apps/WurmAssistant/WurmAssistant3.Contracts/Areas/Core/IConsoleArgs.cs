namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface IConsoleArgs
    {
        bool WurmUnlimitedMode { get; }
        bool UseRelativeDataDir { get; }
    }
}
