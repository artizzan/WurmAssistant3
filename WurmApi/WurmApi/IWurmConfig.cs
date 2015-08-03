using System;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Allows reading and observing changes of a single wurm config.
    /// </summary>
    public interface IWurmConfig
    {
        LogsLocation CustomTimerSource { get; }

        LogsLocation ExecSource { get; }

        LogsLocation KeyBindSource { get; }

        LogsLocation AutoRunSource { get; }

        LogSaveMode IrcLoggingType { get; }

        LogSaveMode OtherLoggingType { get; }

        LogSaveMode EventLoggingType { get; }

        SkillGainRate SkillGainRate { get; }

        bool? NoSkillMessageOnAlignmentChange { get; }

        bool? NoSkillMessageOnFavorChange { get; }

        bool? SaveSkillsOnQuit { get; }

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
    }
}