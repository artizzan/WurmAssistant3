using System;

namespace AldurSoft.WurmApi
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
        /// <summary>
        /// Indicates if editing this config is currently possible.
        /// </summary>
        /// <remarks>
        /// Note that this value can change between reading it and attempting actual modification on any property.
        /// </remarks>
        bool CanChangeConfig { get; }

        LogsLocation CustomTimerSource { get; }

        LogsLocation ExecSource { get; }

        LogsLocation KeyBindSource { get; }

        LogsLocation AutoRunSource { get; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        LogSaveMode IrcLoggingType { get; set; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        LogSaveMode OtherLoggingType { get; set; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        LogSaveMode EventLoggingType { get; set; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        SkillGainRate SkillGainRate { get; set; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        bool? NoSkillMessageOnAlignmentChange { get; set; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        bool? NoSkillMessageOnFavorChange { get; set; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        bool? SaveSkillsOnQuit { get; set; }

        /// <summary></summary>
        /// <exception cref="WurmApiException">Setting this value is currently not allowed.</exception>
        bool? TimestampMessages { get; set; }

        /// <summary>
        /// Directory path, where this config is located
        /// </summary>
        string ConfigDirectoryFullPath { get; }

        event EventHandler ConfigChanged;

        string Name { get; }
    }
}