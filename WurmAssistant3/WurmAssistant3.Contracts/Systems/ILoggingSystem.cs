namespace AldursLab.WurmAssistant3.Systems
{
    public interface ILoggingSystem
    {
        /// <summary>
        /// Logs everything, everywhere and to everywhere.
        /// </summary>
        void EnableGlobalLogging();

        /// <summary>
        /// File may not exist.
        /// </summary>
        string FullPathToCurrentLogFile { get; }
    }
}
