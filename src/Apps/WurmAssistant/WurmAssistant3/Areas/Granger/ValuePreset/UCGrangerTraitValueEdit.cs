using System;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    public partial class UcGrangerTraitValueEditBox : UserControl
    {
        CreatureTrait trait;

        public UcGrangerTraitValueEditBox()
        {
            InitializeComponent();
        }

        public CreatureTrait Trait { get { return trait; } set { trait = value; textBox1.Text = value.ToString(); } }
        public int Value { get { return (int)numericUpDown1.Value; } set { numericUpDown1.Value = value.ConstrainToRange(-1000, 1000); } }
        public bool ReadOnly { set { numericUpDown1.Enabled = !value; } }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            TraitValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TraitValueChanged;
    }
}
