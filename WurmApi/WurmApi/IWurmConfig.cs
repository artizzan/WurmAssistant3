using System;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Allows reading and observing changes of a single wurm config.
    /// </summary>
    public interface IWurmConfig
    {
        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        LogsLocation CustomTimerSource { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        LogsLocation ExecSource { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        LogsLocation KeyBindSource { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        LogsLocation AutoRunSource { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        LogSaveMode IrcLoggingType { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        LogSaveMode OtherLoggingType { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        LogSaveMode EventLoggingType { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        SkillGainRate SkillGainRate { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        bool? NoSkillMessageOnAlignmentChange { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        bool? NoSkillMessageOnFavorChange { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        bool? SaveSkillsOnQuit { get; }

        /// <exception cref="Exception">An error prevented refresh of config values.</exception>
        bool? TimestampMessages { get; }

        /// <summary>
        /// Directory absolute path, where this gamesettings.txt is located
        /// </summary>
        string ConfigDirectoryFullPath { get; }

        /// <summary>
        /// Triggered when config is modified. 
        /// </summary>
        event EventHandler<EventArgs> ConfigChanged;

        /// <summary>
        /// Name of this config. Not normalized.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Informs if the config has had it's values read at least once.
        /// </summary>
        bool HasBeenRead { get; }
    }
}