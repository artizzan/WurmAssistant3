using System;
using System.IO;
using Microsoft.Win32;

namespace AldursLab.WurmApi.Modules.Wurm.InstallDirectory
{
    public class WurmClientInstallDirectory : IWurmClientInstallDirectory
    {
        private WurmClientInstallDirectory(string path)
        {
            FullPath = path;
        }

        /// <summary>
        /// Full directory path to the Wurm client directory
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Attempts to autodetect install directory.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="WurmGameClientInstallDirectoryValidationException">Autodetection failed.</exception>
        public static IWurmClientInstallDirectory AutoDetect()
        {
            var fullPath = GetFromRegistry();
            if (fullPath == null)
            {
                throw new WurmGameClientInstallDirectoryValidationException(
                    "Failed to find install directory in system registry.");
            }

            var pathExists = Directory.Exists(fullPath);
            if (!pathExists)
            {
                throw new WurmGameClientInstallDirectoryValidationException(
                    "Detected directory does not exist: " + fullPath);
            }
            var expectedSubdirs = new[] { "configs", "packs", "players" };
            foreach (var expectedSubdir in expectedSubdirs)
            {
                if (!Directory.Exists(Path.Combine(fullPath, expectedSubdir)))
                {
                    throw new WurmGameClientInstallDirectoryValidationException(
                        "Detected directory does not have expected subdirectory: " + expectedSubdir);
                }
            }

            return new WurmClientInstallDirectory(fullPath);
        }

        private static string GetFromRegistry()
        {
            object regObj = Registry.GetValue(@"HKEY_CURRENT_USER\Software\JavaSoft\Prefs\com\wurmonline\client", "wurm_dir", null);
            if (regObj == null)
            {
                return null;
            }

            var wurmdir = Convert.ToString(regObj);
            wurmdir = wurmdir.Replace(@"//", @"\");
            wurmdir = wurmdir.Replace(@"/", @"");
            wurmdir = wurmdir.Trim();
            if (!wurmdir.EndsWith(@"\", StringComparison.Ordinal))
                wurmdir += @"\";
            return wurmdir;
        }
    }
}