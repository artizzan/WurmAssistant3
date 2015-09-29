using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.Features.Views
{
    public partial class FeatureView : UserControl, IFeatureView
    {
        public FeatureView()
        {
            InitializeComponent();
        }
    }
}
