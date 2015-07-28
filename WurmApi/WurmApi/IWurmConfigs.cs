using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides read-only access to game client configs.
    /// </summary>
    public interface IWurmConfigs
    {
        /// <summary>
        /// Triggered, when a config is added, renamed or removed.
        /// </summary>
        event EventHandler<EventArgs> AvailableConfigsChanged;

        /// <summary>
        /// Triggered when any config is modified.
        /// </summary>
        event EventHandler<EventArgs> AnyConfigChanged;

        /// <summary>
        /// Retrieves all configs.
        /// </summary>
        IEnumerable<IWurmConfig> All { get; }

        /// <summary>
        /// Gets config by its name. Case insensitive.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException">Config with this name does not exist</exception>
        IWurmConfig GetConfig(string name);
    }
}