using System;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    public interface IHostEnvironment
    {
        /// <summary>
        /// Indicates that the host enviroment is currently shutting down.
        /// </summary>
        bool Closing { get; }

        /// <summary>
        /// Requests application restart.
        /// </summary>
        void Restart();

        /// <summary>
        /// Requests application to shut down.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Current platform, if known. Else Unknown.
        /// </summary>
        Platform Platform { get; }

        event EventHandler<EventArgs> HostClosing;

        /// <summary>
        /// Called after HostClosing. 
        /// Note: This is used for persistent object saving and should not be used otherwise.
        /// </summary>
        event EventHandler<EventArgs> LateHostClosing;
    }
}
