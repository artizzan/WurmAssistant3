using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AldursLab.WurmAssistant.Launcher
{
    class LocalLauncherSettings
    {
        readonly Dictionary<string, string> settings = new Dictionary<string, string>(); 

        public LocalLauncherSettings()
        {
            var assemblyDir = Path.GetDirectoryName(this.GetType().Assembly.Location);
            if (assemblyDir == null)
            {
                throw new NullReferenceException("assemblyDir is null");
            }
            var settingsFile = Path.Combine(assemblyDir, "LauncherSettings.txt");
            var lines = File.ReadAllLines(settingsFile).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            ParseLines(lines);
        }

        void ParseLines(string[] lines)
        {
            foreach (var line in lines)
            {
                var delimitedIndex = line.IndexOf("=", StringComparison.InvariantCulture);
                var key = line.Substring(0, delimitedIndex).Trim();
                var value = line.Substring(delimitedIndex + 1).Trim();
                settings.Add(key, value);
            }
        }

        public string GetSetting(string key)
        {
            return settings[key];
        }
    }
}