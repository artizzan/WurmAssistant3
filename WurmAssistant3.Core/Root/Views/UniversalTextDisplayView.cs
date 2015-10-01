using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Core.Root.Views
{
    public partial class UniversalTextDisplayView : Form
    {
        public UniversalTextDisplayView()
        {
            InitializeComponent();
        }

        public string ContentText
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
    }
}
