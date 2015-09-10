using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Root.ViewModels;

namespace AldursLab.WurmAssistant3.Core.Areas.Root.Views
{
    public partial class MenuView : UserControl
    {
        public MenuView(MenuViewModel menuViewModel)
        {
            InitializeComponent();
        }
    }
}
