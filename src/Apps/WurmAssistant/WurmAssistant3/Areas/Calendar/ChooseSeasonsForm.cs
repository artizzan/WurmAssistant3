using System;
using System.Linq;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Calendar
{
    public partial class ChooseSeasonsForm : Form
    {
        public ChooseSeasonsForm(string[] items, string[] tracked)
        {
            InitializeComponent();
            int indexcount = 0;

            foreach (string item in items)
            {
                checkedListBox1.Items.Add(item);
                checkedListBox1.SetItemChecked(indexcount, tracked.Contains(item, StringComparer.InvariantCultureIgnoreCase));
                indexcount++;
            }
        }
    }
}
