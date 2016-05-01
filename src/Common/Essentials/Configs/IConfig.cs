using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.Essentials.Configs
{
    /// <summary>
    /// Provides access to custom configuration values
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// </summary>
        /// <param name="key">Case insensitive, cannot contain = (equals) sign</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        string GetValue(string key);

        /// <summary>
        /// </summary>
        /// <param name="key">Case insensitive, cannot contain = (equals) sign</param>
        /// <returns></returns>
        bool HasValue(string key);
    }

    /// <summary>
    /// Reads a file matching name pattern: [name].cfg
    /// then optionally overrides values from file mathing name pattern: [name].cfg.usr
    /// See remarks for details
    /// </summary>
    /// <remarks>
    /// File contents should match following rules:
    /// <list type="bullet">
    ///     <item>lines starting with # (hash) or empty lines, are ignored</item>
    ///     <item>each non-ignored line is split on first = (equals) character, left side is key, right side is value</item>
    ///     <item>each non-ignored line must have a delimiter = (equals) sign</item>
    ///     <item>key or value can't evaluate to empty string</item>
    ///     <item>keys and values are trimmed of any leading and trailing whitespace characters (enables config file formatting)</item>
    ///     <item>keys are case insensitive, values are case sensitive</item>
    ///     <item>values can contain = (equals) signs (only first is used to delimit current line key)</item>
    /// </list>
    /// </remarks>
    public class FileSimpleConfig : IConfig
    {
        readonly FileInfo configFile;
        readonly FileInfo overrideFile;

        Dictionary<string, string> values = new Dictionary<string, string>();

        /// <summary>
        /// </summary>
        /// <param name="configFileFullPath">Absolute path to config file *.cfg (required to exist)</param>
        /// <param name="alternateOverrideConfigFileFullPath">
        /// Absolute path to override file *.cfg.usr (optional, allowed to not exist)</param>
        public FileSimpleConfig([NotNull] string configFileFullPath, string alternateOverrideConfigFileFullPath = null)
        {
            ValidateConfigPath(configFileFullPath);
            configFile = new FileInfo(configFileFullPath);
            if (!configFile.Exists)
            {
                throw new ArgumentException("Config file does not exist. Expected: " + configFile.FullName);
            }

            string overrideFilePath;
            if (alternateOverrideConfigFileFullPath != null)
            {
                ValidateOverridePath(alternateOverrideConfigFileFullPath);
                overrideFilePath = alternateOverrideConfigFileFullPath;
            }
            else
            {
                overrideFilePath = configFileFullPath + ".usr";
            }
            overrideFile = new FileInfo(overrideFilePath);

            BuildValues();
        }

        static void ValidateConfigPath(string configFileFullPath)
        {
            if (configFileFullPath == null) throw new ArgumentNullException("configFileFullPath");
            if (!Path.IsPathRooted(configFileFullPath))
            {
                throw new ArgumentException("Path is not rooted, actual path: " + configFileFullPath);
            }
            if (!configFileFullPath.EndsWith(".cfg"))
            {
                throw new ArgumentException("Expected file path in format of rootedpath\\*.cfg, actual path: "
                                            + configFileFullPath);
            }
        }

        static void ValidateOverridePath(string configFileFullPath)
        {
            if (configFileFullPath == null)
                throw new ArgumentNullException("configFileFullPath");
            if (!Path.IsPathRooted(configFileFullPath))
            {
                throw new ArgumentException("Override path is not rooted, actual path: " + configFileFullPath);
            }
        }

        void BuildValues()
        {
            values = ParseLines(configFile.FullName);
            if (overrideFile.Exists)
            {
                var overrideValues = ParseLines(overrideFile.FullName);
                foreach (var pair in overrideValues)
                {
                    if (!values.ContainsKey(pair.Key))
                    {
                        throw new InvalidOperationException("Original config does not contain overriden key: " + pair.Key);
                    }
                    values[pair.Key] = pair.Value;
                }
            }
        }

        Dictionary<string,string> ParseLines(string filePath)
        {
            var lines = File.ReadAllLines(filePath).ToArray();
            var result = new Dictionary<string, string>();
            int lineCounter = 0;
            foreach (var line in lines)
            {
                lineCounter++;
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                {
                    // is comment
                    continue;
                }
                var delimitedIndex = line.IndexOf("=", StringComparison.InvariantCulture);
                if (delimitedIndex == -1)
                {
                    throw new InvalidOperationException(
                        string.Format("Non-empty non-comment line has no delimiter, line number: {0}, file: {1}",
                            lineCounter,
                            filePath));
                }
                var key = line.Substring(0, delimitedIndex).Trim();
                key = PrepareKey(key, filePath);
                var value = line.Substring(delimitedIndex + 1).Trim();
                value = PrepareValue(value, key, filePath);
                result.Add(key, value);
            }
            return result;
        }

        public string GetValue(string key)
        {
            var normalizedKey = PrepareKey(key, null);
            string result;
            if (!values.TryGetValue(normalizedKey, out result))
            {
                throw new KeyNotFoundException(string.Format("Config value for key {0} does not exist", key));
            }
            return result;
        }

        public bool HasValue(string key)
        {
            key = PrepareKey(key, null);
            return values.ContainsKey(key);
        }

        string PrepareKey([NotNull] string key, string source)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(String.Format("key {0} cannot be empty{1}",
                    key,
                    source != null ? ", source file: {1}" : string.Empty));
            }
            return key.Trim().ToUpperInvariant();
        }

        string PrepareValue([NotNull] string value, string key, string source)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(String.Format("value for key {0} is empty, source file: {1}", key, source));
            }
            return value.Trim();
        }

        public override string ToString()
        {
            return string.Format("{0}\r\n{1}\r\n{2}\r\nEnd",
                "Main config file: " + configFile.FullName,
                "Override config file: " + (overrideFile.Exists ? overrideFile.FullName : "N/A"),
                "Values:\r\n" + string.Join("\r\n", values.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value))));
        }
    }
}
