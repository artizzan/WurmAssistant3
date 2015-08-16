using System;
using System.Windows.Forms;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistantLite.Bootstrapping
{
    class WinFormsMainThreadEventMarshaller : IEventMarshaller
    {
        readonly Form form;

        public WinFormsMainThreadEventMarshaller([NotNull] Form form)
        {
            if (form == null) throw new ArgumentNullException("form");
            this.form = form;
        }

        public void Marshal(Action action)
        {
            form.BeginInvoke(action);
        }
    }
}