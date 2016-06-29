using AldursLab.Persistence;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    public interface IPersistentContextProvider
    {
        /// <summary>
        /// </summary>
        /// <param name="contextId">Case insensitive Id of the context, only letters, numbers and dashes, no whitespaces.</param>
        /// <param name="options">Custom configuration for the context.</param>
        /// <returns></returns>
        IPersistentContext GetPersistentContext(string contextId, PersistentContextOptions options);

        /// <summary>
        /// </summary>
        /// <param name="contextId">Case insensitive Id of the context, only letters, numbers and dashes, no whitespaces.</param>
        /// <returns></returns>
        IPersistentContext GetPersistentContext(string contextId);
    }
}