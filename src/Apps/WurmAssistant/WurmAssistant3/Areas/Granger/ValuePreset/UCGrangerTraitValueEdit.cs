using System;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    public partial class UCGrangerTraitValueEditBox : UserControl
    {
        private CreatureTrait _Trait;

        public CreatureTrait Trait { get { return _Trait; } set { _Trait = value; textBox1.Text = value.ToString(); } }
        public int Value { get { return (int)numericUpDown1.Value; } set { numericUpDown1.Value = value.ConstrainToRange(-1000, 1000); } }
        public bool ReadOnly { set { numericUpDown1.Enabled = !value; } }

        public UCGrangerTraitValueEditBox()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (TraitValueChanged != null) TraitValueChanged(this, EventArgs.Empty);
        }

        public event EventHandler TraitValueChanged;
    }
}
