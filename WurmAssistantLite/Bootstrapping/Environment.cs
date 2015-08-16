using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using AldursLab.WurmAssistantLite.Views;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistantLite.Bootstrapping
{
    class Environment : IEnvironment
    {
        readonly MainForm mainForm;

        public Environment([NotNull] MainForm mainForm)
        {
            if (mainForm == null) throw new ArgumentNullException("mainForm");
            this.mainForm = mainForm;
        }

        public bool Closing { get; set; }

        public void RequestRestart()
        {
            mainForm.Close();
            Application.Restart();
        }
    }
}