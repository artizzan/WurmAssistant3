using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3
{
    public class EventMarshaller : IEventMarshaller
    {
        public void Marshal(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}
