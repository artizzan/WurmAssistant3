using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Autoruns
{
    public class WurmAutoruns : IWurmAutoruns
    {
        readonly IWurmConfigs wurmConfigs;
        readonly IWurmCharacterDirectories wurmCharacterDirectories;
        readonly ILogger logger;
        readonly object locker = new object();

        public WurmAutoruns(
            [NotNull] IWurmConfigs wurmConfigs,
            [NotNull] IWurmCharacterDirectories wurmCharacterDirectories, 
            [NotNull] ILogger logger)
        {
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmConfigs = wurmConfigs;
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.logger = logger;
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
                    else
                    {
                        LogThatFileNotFound(file);
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
                    else
                    {
                        LogThatFileNotFound(file);
                    }
                }
                return filesMissingCommand;
            }
        }

        void LogThatFileNotFound(FileInfo file)
        {
            logger.Log(LogLevel.Warn, "autorun.txt file was not found at path " + file.FullName, this, null);
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
