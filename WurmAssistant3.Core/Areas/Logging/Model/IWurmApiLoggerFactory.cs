namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Model
{
    public interface IWurmApiLoggerFactory
    {
        AldursLab.WurmApi.ILogger Create();
    }
}