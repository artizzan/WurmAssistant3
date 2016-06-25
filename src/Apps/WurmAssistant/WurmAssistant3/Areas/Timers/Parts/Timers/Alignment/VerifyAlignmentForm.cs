using System;
using System.Linq;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers.Alignment
{
    public partial class VerifyAlignmentForm : Form
    {
        readonly string[] allAlignments = null;

        public VerifyAlignmentForm(string[] allalignments)
        {
            InitializeComponent();
            this.allAlignments = allalignments;
        }

        private void FormVerifyAlignment_Load(object sender, EventArgs e)
        {
            listBox1.Items.Add("list as of date: " + DateTime.Now.ToString());

            if (allAlignments != null) listBox1.Items.AddRange(allAlignments.Cast<object>().ToArray()); 
            else listBox1.Items.Add("no data available");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
