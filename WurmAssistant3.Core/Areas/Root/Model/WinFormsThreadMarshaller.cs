using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Eventing;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Root.Model
{
    class WinFormsThreadMarshaller : IEventMarshaller, IWurmApiEventMarshaller
    {
        readonly Control mainControl;

        public WinFormsThreadMarshaller([NotNull] Control mainControl)
        {
            if (mainControl == null) throw new ArgumentNullException("mainControl");
            this.mainControl = mainControl;
        }

        void IEventMarshaller.Marshal(Action action)
        {
            if (mainControl.InvokeRequired)
            {
                mainControl.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        void IWurmApiEventMarshaller.Marshal(Action action)
        {
            if (mainControl.InvokeRequired)
            {
                mainControl.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
