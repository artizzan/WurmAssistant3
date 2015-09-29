namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts
{
    public interface IWurmApiLoggerFactory
    {
        AldursLab.WurmApi.ILogger Create();
    }
}