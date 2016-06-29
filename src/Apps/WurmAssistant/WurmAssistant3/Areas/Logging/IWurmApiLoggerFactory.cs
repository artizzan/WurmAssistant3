namespace AldursLab.WurmAssistant3.Areas.Logging
{
    public interface IWurmApiLoggerFactory
    {
        AldursLab.WurmApi.IWurmApiLogger Create();
    }
}