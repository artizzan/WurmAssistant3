using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Modules.Events
{
    public static class EventHandlerExtensions
    {
        public static void SafeInvoke(this EventHandler<EventArgs> handler, object source, EventArgs eventArgs = null)
        {
            if (handler != null)
                handler(source, eventArgs ?? EventArgs.Empty);
        }
    }
}
