using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    public interface IWurmAutoruns
    {
        /// <summary>
        /// Appends given command to all autorun files found in Wurm dir.
        /// Commands are added only if they do not yet exist.
        /// </summary>
        /// <param name="command"></param>
        void MergeCommandToAllAutoruns(string command);

        /// <summary>
        /// Scans all autorun files and verifies each has specified command.
        /// Returns list of autorun file paths, where command is missing.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IEnumerable<string> FindIfMissingCommandInAnyAutorun(string command);
    }
}
