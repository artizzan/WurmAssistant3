using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.ViewModels;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistantLite.Views
{
    public partial class AppStartView : UserControl
    {
        readonly AppStartViewModel viewModel;

        public AppStartView()
        {
            InitializeComponent();
        }

        public AppStartView([NotNull] AppStartViewModel viewModel) 
            : this()
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            this.viewModel = viewModel;
        }
    }
}
