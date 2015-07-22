using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Wurm.Autoruns;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Configs;
using AldurSoft.WurmApi.Wurm.GameClients;
using AldurSoft.WurmApi.Wurm.Logs;
using AldurSoft.WurmApi.Wurm.Logs.Monitoring;
using AldurSoft.WurmApi.Wurm.Logs.Searching;
using AldurSoft.WurmApi.Wurm.Servers;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides an API to work with many Wurm Online related data and functions.
    /// See remarks for more details.
    /// </summary>
    public interface IWurmApi
    {
        /// <summary>
        /// API that allows to interact and edit wurm client autorun files.
        /// </summary>
        IWurmAutoruns WurmAutoruns { get; }
        /// <summary>
        /// API that allows to obtain information about state of wurm characters.
        /// </summary>
        IWurmCharacters WurmCharacters { get; }
        /// <summary>
        /// API that allows to read and edit wurm client configs.
        /// </summary>
        IWurmConfigs WurmConfigs { get; }
        /// <summary>
        /// API that provides information about running game clients.
        /// </summary>
        IWurmGameClients WurmGameClients { get; }
        /// <summary>
        /// API that defines game log types supported by WurmApi.
        /// </summary>
        IWurmLogDefinitions WurmLogDefinitions { get; }
        /// <summary>
        /// API that allows searching through game log files.
        /// </summary>
        IWurmLogsHistory WurmLogsHistory { get; }
        /// <summary>
        /// API that monitors game log files in real time.
        /// </summary>
        IWurmLogsMonitor WurmLogsMonitor { get; }
        /// <summary>
        /// API that provides information about wurm servers.
        /// </summary>
        IWurmServers WurmServers { get; }
    }

    public interface IWurmApiController
    {
        /// <summary>
        /// Update the state of the API
        /// </summary>
        void Update();
    }
}
