using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class EventHandlerExtensions
    {
        public static string MethodInformationToString<T>(this EventHandler<T> eventHandler) where T : EventArgs
        {
            return string.Format("{0}.{1}",
                eventHandler.Method.DeclaringType != null
                    ? eventHandler.Method.DeclaringType.FullName
                    : string.Empty,
                eventHandler.Method.Name);
        }

        /// <summary>
        /// Invokes event handler in a thread safe way. Defaults source to null and event args to Empty.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="source"></param>
        /// <param name="eventArgs"></param>
        public static void SafeInvoke(this EventHandler<EventArgs> handler, object source = null, EventArgs eventArgs = null)
        {
            if (handler != null)
                handler(source, eventArgs ?? EventArgs.Empty);
        }
    }
}
