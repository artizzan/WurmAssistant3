namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface  IWaVersionInfoProvider
    {
        /// <summary>
        /// Returns string representing currently running version and the configured platform (OS).
        /// </summary>
        /// <returns></returns>
        string Get();
    }
}
