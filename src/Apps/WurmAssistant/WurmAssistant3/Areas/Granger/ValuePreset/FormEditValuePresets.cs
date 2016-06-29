using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    public partial class FormEditValuePresets : ExtendedForm
    {
        private GrangerContext Context;

        public FormEditValuePresets(GrangerContext Context)
        {
            this.Context = Context;
            InitializeComponent();
            RebuildAllPresetsList();
        }

        void RebuildAllPresetsList()
        {
            listBox1.Items.Clear();

            var allPresets = new List<string>();
            allPresets.Add(TraitValuator.DefaultId);
            allPresets.AddRange(Context.TraitValues.AsEnumerable().Select(x => x.ValueMapID).Distinct().OrderBy(x => x));

            listBox1.Items.AddRange(allPresets.ToArray());
        }

        string _CurrentValueMapID = null;
        string CurrentValueMapID
        {
            get { return _CurrentValueMapID; }
            set
            {
                if (value == null)
                {
                    _CurrentValueMapID = null;
                    CurrentTraitValueMap = null;
                    UpdateTraitView();
                }
                else if (listBox1.SelectedItem.ToString() != CurrentValueMapID)
                {
                    _CurrentValueMapID = listBox1.SelectedItem.ToString();
                    CurrentTraitValueMap = new TraitValueMap(Context, CurrentValueMapID);
                    UpdateTraitView();
                }
            }
        }

        TraitValueMap CurrentTraitValueMap;

        private void UpdateTraitView()
        {
            flowLayoutPanel1.Controls.Clear();
            if (CurrentTraitValueMap != null)
            {
                foreach (var keyval in CurrentTraitValueMap.ValueMap)
                {
                    CreatureTrait trait = new CreatureTrait(keyval.Key);
                    var control = new UCGrangerTraitValueEditBox()
                    {
                        Trait = trait,
                        Value = keyval.Value,
                        ReadOnly = CurrentTraitValueMap.ReadOnly
                    };
                    flowLayoutPanel1.Controls.Add(control);
                    control.TraitValueChanged += control_TraitValueChanged;
                }
            }
        }

        void control_TraitValueChanged(object sender, EventArgs e)
        {
            labelSaveWarn.Visible = true;
        }

        private void FormEditValuePresets_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                CurrentValueMapID = listBox1.SelectedItem.ToString();
            }
            else CurrentValueMapID = null;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (CurrentTraitValueMap == null) return;

            if (!CurrentTraitValueMap.ReadOnly)
            {
                //iterate over controls and update the current value map
                foreach (UCGrangerTraitValueEditBox control in flowLayoutPanel1.Controls)
                {
                    CurrentTraitValueMap.ModifyTraitValue(control.Trait.Trait, control.Value);
                }
                CurrentTraitValueMap.Save();
            }

            labelSaveWarn.Visible = false;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) MessageBox.Show("select a preset to copy values from");
            else
            {
                FormEditValuePresetsNewNameDialog ui = new FormEditValuePresetsNewNameDialog(this, Context);
                if (ui.ShowDialogCenteredOnForm(this) == System.Windows.Forms.DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(ui.Result))
                    {
                        TraitValueMap map = new TraitValueMap(Context, listBox1.SelectedItem.ToString());
                        Context.UpdateOrCreateTraitValueMap(map.ValueMap, ui.Result);
                    }
                    listBox1.SelectedIndex = -1;
                    RebuildAllPresetsList();
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) MessageBox.Show("select a preset to copy values from");
            else if (listBox1.SelectedItem.ToString() == TraitValuator.DefaultId) MessageBox.Show("default preset can't be deleted");
            else
            {
                if (MessageBox.Show("Are you sure to delete this preset?", "Confirm", MessageBoxButtons.OKCancel)
                    == System.Windows.Forms.DialogResult.OK)
                {
                    Context.DeleteTraitValueMap(listBox1.SelectedItem.ToString());
                    listBox1.SelectedIndex = -1;
                    RebuildAllPresetsList();
                }
            }
        }
    }
}
