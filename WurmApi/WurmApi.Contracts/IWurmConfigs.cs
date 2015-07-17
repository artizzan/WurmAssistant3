using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means of working with wurm configs.
    /// </summary>
    public interface IWurmConfigs
    {
        /// <summary>
        /// Triggered, when config is edited in any way.
        /// </summary>
        event EventHandler<EventArgs> ConfigsChanged;

        /// <summary>
        /// Enumerates all configs detected and managed by WurmApi.
        /// </summary>
        IEnumerable<IWurmConfig> All { get; }
        
        /// <summary>
        /// Gets config by its name, case insensitive.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="WurmApiException">Config with this name was not found.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        IWurmConfig GetConfig(string name);
    }
}