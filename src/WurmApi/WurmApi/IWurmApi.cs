using System;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Provides an API to work with many Wurm Online related data and functions.
    /// See remarks for more details.
    /// </summary>
    public interface IWurmApi : IDisposable
    {
        /// <summary>
        /// Wurm client autorun files. Check for commands, add commands.
        /// </summary>
        IWurmAutoruns Autoruns { get; }
        /// <summary>
        /// State of in-game wurm characters.
        /// </summary>
        IWurmCharacters Characters { get; }
        /// <summary>
        /// State of wurm client configs.
        /// </summary>
        IWurmConfigs Configs { get; }
        /// <summary>
        /// Definitions of game log types supported by WurmApi.
        /// </summary>
        IWurmLogDefinitions LogDefinitions { get; }
        /// <summary>
        /// Enables searching through game log files.
        /// </summary>
        IWurmLogsHistory LogsHistory { get; }
        /// <summary>
        /// Enables monitoring of game log files for new events in real time.
        /// </summary>
        IWurmLogsMonitor LogsMonitor { get; }
        /// <summary>
        /// Provides information about specific wurm servers.
        /// </summary>
        IWurmServers Servers { get; }

        /// <summary>
        /// Directory paths to Wurm Game Client.
        /// </summary>
        IWurmPaths Paths { get; }

        /// <summary>
        /// Definitions for Wurm Online server groups.
        /// </summary>
        IWurmServerGroups ServerGroups { get; }

        /// <summary>
        /// Low level access to server history in the context of particular character.
        /// Similar information can be obtained through IWurmCharacter.
        /// </summary>
        IWurmServerHistory WurmServerHistory { get; }

        /// <summary>
        /// Low level access to wurm client characters directory.
        /// </summary>
        IWurmCharacterDirectories WurmCharacterDirectories { get; }

        /// <summary>
        /// Low level access to wurm client configs directory.
        /// </summary>
        IWurmConfigDirectories WurmConfigDirectories { get; }

        /// <summary>
        /// Low level access to wurm client logs directory.
        /// </summary>
        IWurmLogFiles WurmLogFiles { get; }

        /// <summary>
        /// Internal WurmApi logger.
        /// </summary>
        IWurmApiLogger Logger { get; }

        /// <summary>
        /// Defined Wurm servers (Online and Unlimited).
        /// </summary>
        IWurmServerList ServersList { get; }
    }
}
