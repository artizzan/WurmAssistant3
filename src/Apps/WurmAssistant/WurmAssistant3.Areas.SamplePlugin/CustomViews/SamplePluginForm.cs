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
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.SamplePlugin.CustomViews
{
    public partial class SamplePluginForm : ExtendedForm, ISampleContract
    {
        public SamplePluginForm()
        {
            InitializeComponent();
            SetupHideInsteadOfCloseIfUserReason();
        }
    }
}
