using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Root.Model
{
    public interface ITimerService
    {
        event EventHandler<EventArgs> Updated;
    }
}
