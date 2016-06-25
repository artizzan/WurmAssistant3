namespace AldursLab.WurmAssistant3.Areas.Logging.Contracts
{
    public interface ILoggerFactory
    {
        ILogger Create(string category);
    }
}