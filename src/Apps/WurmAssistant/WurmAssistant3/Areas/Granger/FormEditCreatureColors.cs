using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using BrightIdeasSoftware;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [KernelBind(), UsedImplicitly]
    public partial class FormEditCreatureColors : Form
    {
        readonly CreatureColorDefinitions creatureColorDefinitions;

        public FormEditCreatureColors([NotNull] CreatureColorDefinitions creatureColorDefinitions)
        {
            if (creatureColorDefinitions == null) throw new ArgumentNullException(nameof(creatureColorDefinitions));
            this.creatureColorDefinitions = creatureColorDefinitions;
            InitializeComponent();
            RefreshView();
        }

        void RefreshView()
        {
            var allItems =
                creatureColorDefinitions.GetColors().Select(color => new CreatureColorViewObject(color)).ToArray();
            objectListView.SetObjects(allItems);
            objectListView.DisableObjects(allItems.Where(o => o.BlockedForEditing));
            // refreshing to force FormatCell
            objectListView.RefreshObjects(objectListView.Objects.Cast<object>().ToList());
        }

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            var newId = textBoxId.Text.Trim();

            if (string.IsNullOrWhiteSpace(newId))
            {
                MessageBox.Show("New Id must not be empty");
            }
            else if (creatureColorDefinitions.GetForId(newId) != CreatureColor.GetDefaultColor())
            {
                MessageBox.Show($"CreatureColor with Id {newId} already exists on the list.");
            }
            else
            {
                creatureColorDefinitions.AddNew(newId);
                RefreshView();
            }
        }

        private void objectListView_ButtonClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            var obj = (CreatureColorViewObject) e.Model;
            if (e.Column == olvColumnChangeColor)
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    var color = colorDialog.Color;
                    creatureColorDefinitions.UpdateColor(obj.Id, color);
                    RefreshView();
                }
            }
            else if (e.Column == olvColumnRemove)
            {
                if (MessageBox.Show("Removing this color will reset it on all creatures using it. Continue?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    creatureColorDefinitions.Remove(obj.Id);
                    RefreshView();
                }
            }
        }

        private void objectListView_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Column == olvColumnColor)
            {
                var obj = (CreatureColorViewObject)e.Model;
                e.SubItem.BackColor = obj.Color;
                e.SubItem.ForeColor = e.SubItem.BackColor.GetContrastingBlackOrWhiteColor();
            }
        }
    }

    class CreatureColorViewObject
    {
        public CreatureColorViewObject(CreatureColor creatureColor)
        {
            Id = creatureColor.CreatureColorId;
            Color = creatureColor.SystemDrawingColor;
            BlockedForEditing = creatureColor.IsReadOnly;
        }

        public CreatureColorViewObject([NotNull] string newId)
        {
            if (newId == null) throw new ArgumentNullException(nameof(newId));
            Id = newId;
        }

        public string Id { get; private set; } = string.Empty;
        public Color Color { get; set; } = Color.Empty;
        public bool BlockedForEditing { get; private set; } = false;

        public string ChangeColorButtonAspect { get; } = "change color";
        public string RemoveButtonAspect { get; } = "remove";
    }
}
