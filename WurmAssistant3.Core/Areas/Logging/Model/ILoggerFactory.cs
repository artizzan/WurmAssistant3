namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Model
{
    public interface ILoggerFactory
    {
        ILogger Create(string category);
    }
}