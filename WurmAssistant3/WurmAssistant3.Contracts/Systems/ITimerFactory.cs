using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core;

namespace AldurSoft.WurmAssistant3.Systems
{
    public interface ITimerFactory
    {
        ITimer Create();
    }
}
