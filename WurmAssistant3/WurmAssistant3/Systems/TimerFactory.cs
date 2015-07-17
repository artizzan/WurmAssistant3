using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using AldurSoft.Core;

using Core.AppFramework.Wpf;

namespace AldurSoft.WurmAssistant3.Systems
{
    public class TimerFactory : ITimerFactory
    {
        public ITimer Create()
        {
            return new DispatcherTimerProxy();
        }
    }
}
