using System;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means of reading some of wurm config properties.
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
        /// Directory path, where this config is located
        /// </summary>
        string ConfigDirectoryFullPath { get; }

        /// <summary>
        /// If event handler throws exception, it will be logged and ignored. 
        /// </summary>
        event EventHandler ConfigChanged;

        /// <summary>
        /// Wurm Game Client identificator of this config.
        /// </summary>
        string Name { get; }
    }
}