using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Alignment
{
    public partial class VerifyAlignmentForm : Form
    {
        string[] AllAlignments = null;

        public VerifyAlignmentForm(string[] allalignments)
        {
            InitializeComponent();
            this.AllAlignments = allalignments;
        }

        private void FormVerifyAlignment_Load(object sender, EventArgs e)
        {
            listBox1.Items.Add("list as of date: " + DateTime.Now.ToString());

            if (AllAlignments != null) listBox1.Items.AddRange(AllAlignments); 
            else listBox1.Items.Add("no data available");
        }
    }
}
