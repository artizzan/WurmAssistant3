using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.CreatureEdit;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.Views.HorseEdit;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Views
{
    public partial class UCGrangerHerdList : UserControl
    {
        FormGrangerMain MainForm;
        GrangerContext Context;
        ILogger logger;
        IWurmApi wurmApi;

        public UCGrangerHerdList()
        {
            InitializeComponent();
        }

        public void Init(FormGrangerMain mainForm, GrangerContext context, [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            MainForm = mainForm;
            Context = context;
            this.logger = logger;
            this.wurmApi = wurmApi;

            objectListView1.BooleanCheckStateGetter = new BrightIdeasSoftware.BooleanCheckStateGetterDelegate(x =>
                {
                    HerdEntity entity = (HerdEntity)x;
                    return entity.Selected;
                });
            objectListView1.BooleanCheckStatePutter = new BrightIdeasSoftware.BooleanCheckStatePutterDelegate((x, y) =>
                {
                    HerdEntity entity = (HerdEntity)x;
                    Context.UpdateHerdSelectedState(entity.HerdID, y);
                    return entity.Selected;
                });

            Context.OnHerdsModified += RefreshHerdList;
            RefreshHerdList(this, new EventArgs());
        }

        public void RefreshHerdList(object sender, EventArgs e)
        {
            HerdEntity[] Herds = Context.Herds.ToArray();
            objectListView1.SetObjects(Herds, true);
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHerdName ui = new FormHerdName(Context, MainForm, logger);
            ui.ShowDialogCenteredOnForm(MainForm);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selHerd = selectedHerd;
            if (selHerd == null) MessageBox.Show("select a herd first");
            else
            {
                FormHerdName ui = new FormHerdName(Context, MainForm, logger, selHerd.HerdID);
                ui.ShowDialogCenteredOnForm(MainForm);
            }
        }

        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selHerd = selectedHerd;
            if (selHerd == null) MessageBox.Show("select a herd first");
            else
            {
                FormHerdMerge ui = new FormHerdMerge(Context, MainForm, selHerd.HerdID, logger);
                ui.ShowDialogCenteredOnForm(MainForm);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selHerd = selectedHerd;
            if (selHerd == null) MessageBox.Show("select a herd first");
            else
            {
                CreatureEntity[] creatures = Context.Creatures.Where(x => x.Herd == selHerd.HerdID).ToArray();
                if (MessageBox.Show("Following herd will be deleted: " + selHerd + "\r\n" + "all creatures in this herd will also be deleted:" + "\r\n"
                    + (creatures.Length == 0 ? "no creatures in this herd" : string.Join(", ", (IEnumerable<CreatureEntity>)creatures)) + "\r\n\r\n" +
                    "Continue?", "confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    Context.DeleteHerd(selHerd.HerdID);
                }
            }
        }

        HerdEntity selectedHerd { get { return (HerdEntity)objectListView1.SelectedObject; } }

        private void addCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selHerd = selectedHerd;
            if (selHerd == null) MessageBox.Show("select a herd first");
            else
            {
                FormCreatureViewEdit ui = new FormCreatureViewEdit(MainForm, null, Context, CreatureViewEditOpType.New, selHerd.HerdID, logger, wurmApi);
                ui.ShowDialog();
            }
        }

        //olv
        private void objectListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {

        }

        private void objectListView1_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
        {
            if (e.Model == null)
            {
                addCreatureToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
                combineToolStripMenuItem.Enabled = false;
                renameToolStripMenuItem.Enabled = false;
            }
            else
            {
                addCreatureToolStripMenuItem.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
                combineToolStripMenuItem.Enabled = true;
                renameToolStripMenuItem.Enabled = true;
            }
        }
    }
}
