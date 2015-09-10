using System;
using System.Windows.Forms;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Areas.Root.Model
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
    }
}
