using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.WinForms.Reusables
{
    public partial class UniversalTextDisplayView : ExtendedForm
    {
        public UniversalTextDisplayView(params Button[] extraButtons)
        {
            InitializeComponent();

            foreach (var extraButton in extraButtons)
            {
                extraButtonsFlowPanel.Controls.Add(extraButton);
            }
        }

        public string ContentText
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
    }
}
