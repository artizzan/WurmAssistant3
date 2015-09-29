namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts
{
    public interface ILoggerFactory
    {
        ILogger Create(string category);
    }
}