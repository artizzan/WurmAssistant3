namespace AldursLab.WurmAssistant3.Areas.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create(string category);
    }
}