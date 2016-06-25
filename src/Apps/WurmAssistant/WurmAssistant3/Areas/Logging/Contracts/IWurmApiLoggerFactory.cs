namespace AldursLab.WurmAssistant3.Areas.Logging.Contracts
{
    public interface IWurmApiLoggerFactory
    {
        AldursLab.WurmApi.IWurmApiLogger Create();
    }
}