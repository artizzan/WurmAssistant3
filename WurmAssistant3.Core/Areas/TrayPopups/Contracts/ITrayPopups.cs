using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts
{
    public interface  ITrayPopups
    {
        /// <summary>
        /// Add message to Popup queue
        /// </summary>
        /// <param name="title">title of the message</param>
        /// <param name="content">content of the message</param>
        /// <param name="timeToShowMillis">how long should this popup be visible, in milliseconds</param>
        void Schedule(string title, string content, int timeToShowMillis = 3000);

        /// <summary>
        /// Add message to Popup queue with default title
        /// </summary>
        /// <param name="content">content of the message</param>
        /// <param name="timeToShowMillis">how long should this popup be visible, in milliseconds</param>
        void Schedule(string content, int timeToShowMillis = 3000);
    }
}
