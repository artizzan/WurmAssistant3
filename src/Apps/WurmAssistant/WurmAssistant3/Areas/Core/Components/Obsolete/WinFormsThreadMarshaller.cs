using System;
using System.Windows.Forms;
using AldursLab.Essentials.Eventing;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Components.Obsolete
{
    [Obsolete]
    class WinFormsThreadMarshaller : IThreadMarshaller, IWurmApiEventMarshaller
    {
        readonly Control mainControl;

        public WinFormsThreadMarshaller([NotNull] Control mainControl)
        {
            if (mainControl == null) throw new ArgumentNullException(nameof(mainControl));
            this.mainControl = mainControl;
        }

        void IThreadMarshaller.Marshal(Action action)
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
