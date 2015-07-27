using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AldurSoft.WurmApi.Modules.Wurm.Autoruns
{
    public class WurmAutoruns : IWurmAutoruns
    {
        private readonly IWurmConfigs wurmConfigs;
        private readonly IWurmCharacterDirectories wurmCharacterDirectories;
        private readonly object locker = new object();

        public WurmAutoruns(
            IWurmConfigs wurmConfigs,
            IWurmCharacterDirectories wurmCharacterDirectories)
        {
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            this.wurmConfigs = wurmConfigs;
            this.wurmCharacterDirectories = wurmCharacterDirectories;
        }

        public void AppendCommandToAllAutoruns(string command)
        {
            lock (locker)
            {
                var allPossiblePaths = GetAllAutorunFullFilePaths();
                foreach (var path in allPossiblePaths)
                {
                    var file = new FileInfo(Path.Combine(path, "autorun.txt"));
                    if (file.Exists)
                    {
                        AppendCommandIfNotInFile(file, command);
                    }
                }
            }
        }

        public IEnumerable<string> FindIfMissingCommandInAnyAutorun(string command)
        {
            lock (locker)
            {
                List<string> filesMissingCommand = new List<string>();

                var allPossiblePaths = GetAllAutorunFullFilePaths();
                foreach (var path in allPossiblePaths)
                {
                    var file = new FileInfo(Path.Combine(path, "autorun.txt"));
                    if (file.Exists)
                    {
                        if (!CommandExists(file, command))
                        {
                            filesMissingCommand.Add(file.FullName);
                        }
                    }
                }
                return filesMissingCommand;
            }
        }

        private IEnumerable<string> GetAllAutorunFullFilePaths()
        {
            var allCharactersDirPaths = wurmCharacterDirectories.AllDirectoriesFullPaths.ToArray();
            var allConfigs = wurmConfigs.All.ToArray().Select(config => config.ConfigDirectoryFullPath);
            return allCharactersDirPaths.AsEnumerable().Concat(allConfigs).ToArray();
        }

        /// <summary>
        /// Appends a command to autoexec.txt, throws exceptions on errors
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void AppendCommandIfNotInFile(FileInfo file, string execCommand)
        {
            //read the exec file
            bool exists = CommandExists(file, execCommand);

            //if not exists, append as new line
            if (!exists)
            {
                using (var sw = new StreamWriter(file.FullName, true))
                {
                    sw.WriteLine(); //this ensures command is always written on new line
                    sw.WriteLine(execCommand);
                }
            }
        }

        private static bool CommandExists(FileInfo file, string command)
        {
            using (var sr = new StreamReader(file.FullName))
            {
                //try to find the command text on each line
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.StartsWith("//", StringComparison.InvariantCulture) && line.Contains(command))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
