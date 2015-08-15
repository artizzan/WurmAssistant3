using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistantLite
{
    class SampleViewModel : IHandle<string>
    {
        readonly Form1 form;
        int counter = 0;

        public SampleViewModel([NotNull] Form1 form)
        {
            if (form == null) throw new ArgumentNullException("form");
            this.form = form;
        }


        public void Handle(string message)
        {
            counter++;
            form.textBox1.Text = message + " " + counter;
        }
    }
}
