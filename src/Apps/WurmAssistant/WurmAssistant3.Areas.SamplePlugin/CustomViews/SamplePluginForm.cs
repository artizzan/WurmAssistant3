using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.SamplePlugin.Contracts;
using AldursLab.WurmAssistant3.Areas.SamplePlugin.Factories;
using AldursLab.WurmAssistant3.Utils.WinForms;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.SamplePlugin.CustomViews
{
    [KernelBind(BindingHint.Singleton)]
    public partial class SamplePluginForm : ExtendedForm, ISampleContract
    {
        readonly ISomethingViewModelFactory somethingViewModelFactory;
        readonly IWindowManager windowManager;

        public SamplePluginForm(
            [NotNull] ISomethingViewModelFactory somethingViewModelFactory,
            [NotNull] IWindowManager windowManager)
        {
            if (somethingViewModelFactory == null) throw new ArgumentNullException(nameof(somethingViewModelFactory));
            if (windowManager == null) throw new ArgumentNullException(nameof(windowManager));
            this.somethingViewModelFactory = somethingViewModelFactory;
            this.windowManager = windowManager;
            InitializeComponent();
            SetupHideInsteadOfCloseIfUserReason();
        }

        private void btnShowSomething_Click(object sender, EventArgs e)
        {
            var view = somethingViewModelFactory.CreatesSomethingViewModel();
            windowManager.ShowWindow(view);
        }
    }
}
