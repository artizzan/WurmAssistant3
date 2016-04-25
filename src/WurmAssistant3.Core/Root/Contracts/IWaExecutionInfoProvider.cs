namespace AldursLab.WurmAssistant3.Core.Root.Contracts
{
    public interface  IWaExecutionInfoProvider
    {
        /// <summary>
        /// Returns string representing currently running version and the configured platform (OS).
        /// </summary>
        /// <returns></returns>
        string Get();
    }
}
