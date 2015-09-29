using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.WinForms
{
    public class MinimizationManager
    {
        readonly Form form;

        FormWindowState lastState;

        public MinimizationManager([NotNull] Form form)
        {
            if (form == null) throw new ArgumentNullException("form");
            this.form = form;
            SaveLastNonMinimizedState();
            form.Resize += (sender, args) =>
            {
                SaveLastNonMinimizedState();
            };
        }

        void SaveLastNonMinimizedState()
        {
            if (form.WindowState != FormWindowState.Minimized)
            {
                lastState = form.WindowState;
            }
        }

        public void Restore()
        {
            form.Show();
            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = lastState;
            }
        }

        public void Minimize()
        {
            form.WindowState = FormWindowState.Minimized;
        }
    }
}
