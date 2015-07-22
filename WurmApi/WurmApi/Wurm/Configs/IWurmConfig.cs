using System;

namespace AldurSoft.WurmApi.Wurm.Configs
{
    /// <summary>
    /// Provides means of reading and modifying some of wurm config properties.
    /// </summary>
    /// <remarks>
    /// Modifying config may be locked under certain conditions. For example, when game clients are running.
    /// Attempting to modify config, when its locked, results in an <see cref="WurmApiException"/>.
    /// </remarks>
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

        event EventHandler ConfigChanged;

        /// <summary>
        /// Wurm Game Client identificator of this config.
        /// </summary>
        string Name { get; }
    }
}