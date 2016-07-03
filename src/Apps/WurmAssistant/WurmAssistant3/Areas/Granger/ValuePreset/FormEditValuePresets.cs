using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    public partial class FormEditValuePresets : ExtendedForm
    {
        readonly GrangerContext context;

        string currentValueMapId = null;
        TraitValueMap currentTraitValueMap;

        public FormEditValuePresets([NotNull] GrangerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            this.context = context;
            InitializeComponent();
            RebuildAllPresetsList();
        }

        void RebuildAllPresetsList()
        {
            listBox1.Items.Clear();

            var allPresets = new List<string> {TraitValuator.DefaultId};
            allPresets.AddRange(context.TraitValues.AsEnumerable().Select(x => x.ValueMapId).Distinct().OrderBy(x => x));

            listBox1.Items.AddRange(allPresets.Cast<object>().ToArray());
        }

        string CurrentValueMapId
        {
            get { return currentValueMapId; }
            set
            {
                if (value == null)
                {
                    currentValueMapId = null;
                    currentTraitValueMap = null;
                    UpdateTraitView();
                }
                else if (listBox1.SelectedItem.ToString() != CurrentValueMapId)
                {
                    currentValueMapId = listBox1.SelectedItem.ToString();
                    currentTraitValueMap = new TraitValueMap(context, CurrentValueMapId);
                    UpdateTraitView();
                }
            }
        }

        private void UpdateTraitView()
        {
            flowLayoutPanel1.Controls.Clear();
            if (currentTraitValueMap != null)
            {
                foreach (var keyval in currentTraitValueMap.ValueMap)
                {
                    CreatureTrait trait = new CreatureTrait(keyval.Key);
                    var control = new UcGrangerTraitValueEditBox()
                    {
                        Trait = trait,
                        Value = keyval.Value,
                        ReadOnly = currentTraitValueMap.ReadOnly
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
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentValueMapId = listBox1.SelectedIndex > -1 ? listBox1.SelectedItem.ToString() : null;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (currentTraitValueMap == null) return;

            if (!currentTraitValueMap.ReadOnly)
            {
                //iterate over controls and update the current value map
                foreach (UcGrangerTraitValueEditBox control in flowLayoutPanel1.Controls)
                {
                    currentTraitValueMap.ModifyTraitValue(control.Trait.CreatureTraitId, control.Value);
                }
                currentTraitValueMap.Save();
            }

            labelSaveWarn.Visible = false;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) MessageBox.Show("Please select a preset from which  to copy initial values.");
            else
            {
                FormEditValuePresetsNewNameDialog ui = new FormEditValuePresetsNewNameDialog(this, context);
                if (ui.ShowDialogCenteredOnForm(this) == System.Windows.Forms.DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(ui.Result))
                    {
                        TraitValueMap map = new TraitValueMap(context, listBox1.SelectedItem.ToString());
                        context.UpdateOrCreateTraitValueMap(map.ValueMap, ui.Result);
                    }
                    listBox1.SelectedIndex = -1;
                    RebuildAllPresetsList();
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) MessageBox.Show("Please select a preset to delete.");
            else if (listBox1.SelectedItem.ToString() == TraitValuator.DefaultId) MessageBox.Show("Default preset can't be deleted.");
            else
            {
                if (MessageBox.Show("Are you sure to delete this preset?", "Confirm", MessageBoxButtons.OKCancel)
                    == System.Windows.Forms.DialogResult.OK)
                {
                    context.DeleteTraitValueMap(listBox1.SelectedItem.ToString());
                    listBox1.SelectedIndex = -1;
                    RebuildAllPresetsList();
                }
            }
        }
    }
}
