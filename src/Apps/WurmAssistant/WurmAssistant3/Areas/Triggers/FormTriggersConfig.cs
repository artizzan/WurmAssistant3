using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.Triggers.ImportExport;
using AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager;
using AldursLab.WurmAssistant3.Properties;
using AldursLab.WurmAssistant3.Utils.WinForms;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers
{
    partial class FormTriggersConfig : ExtendedForm
    {
        readonly TriggerManager triggerManager;
        readonly ISoundManager soundManager;
        readonly IWindowManager windowManager;
        readonly IExporterFactory exporterFactory;
        readonly IImporterFactory importerFactory;

        private const string DisplayName = "Triggers";

        public FormTriggersConfig(
            [NotNull] TriggerManager triggerManager, 
            [NotNull] ISoundManager soundManager,
            [NotNull] IWindowManager windowManager,
            [NotNull] IExporterFactory exporterFactory,
            [NotNull] IImporterFactory importerFactory)
        {
            if (triggerManager == null) throw new ArgumentNullException(nameof(triggerManager));
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (windowManager == null) throw new ArgumentNullException(nameof(windowManager));
            if (exporterFactory == null) throw new ArgumentNullException(nameof(exporterFactory));
            if (importerFactory == null) throw new ArgumentNullException(nameof(importerFactory));
            InitializeComponent();
            this.triggerManager = triggerManager;
            this.soundManager = soundManager;
            this.windowManager = windowManager;
            this.exporterFactory = exporterFactory;
            this.importerFactory = importerFactory;
            BuildFormText();
            UpdateMutedState();
            TriggersListView.SetObjects(this.triggerManager.Triggers);
            timer1.Enabled = true;
        }

        private void RefreshBankAndList()
        {
            TriggersListView.SetObjects(this.triggerManager.Triggers, true);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshBankAndList();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var ui = new TriggerChoice(triggerManager);
            if (ui.ShowDialogCenteredOnForm(this) == DialogResult.OK)
            {
                var trigger = ui.Result;
                trigger.MuteChecker = triggerManager.GetMutedEvaluator();
                var ui2 = ui.Result.ShowAndGetEditUi(this);
                ui2.Closed += (o, args) =>
                {
                    RefreshBankAndList();
                };
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            EditCurrentItem();
        }

        void EditCurrentItem()
        {
            var selected = TriggersListView.SelectedObject;
            if (selected != null)
            {
                var ui = ((ITrigger)selected).ShowAndGetEditUi(this);
                ui.Closed -= EditTriggerClosed; //in case this is already hooked
                ui.Closed += EditTriggerClosed;
            }
        }

        private void EditTriggerClosed(object o, EventArgs args)
        {
            TriggersListView.BuildList(true);
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            RemoveCurrentItems();
        }

        void RemoveCurrentItems()
        {
            var selected = TriggersListView.SelectedObjects.Cast<ITrigger>().ToArray();
            if (selected.Any())
            {
                if (selected.Length > 1)
                {
                    if (MessageBox.Show($"Are you sure to delete {selected.Length} triggers?",
                        "Confirm delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Asterisk) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                foreach (var trigger in selected)
                {
                    triggerManager.RemoveTrigger(trigger);
                }
                RefreshBankAndList();
            }
        }

        private void FormSoundNotifyConfig_Load(object sender, EventArgs e)
        {
            RefreshBankAndList();
            TriggersListView.RestoreState(triggerManager.TriggerListState);
        }

        public void RestoreFromMin()
        {
            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
        }

        private void buttonMute_Click(object sender, EventArgs e)
        {
            triggerManager.Muted = !triggerManager.Muted;
            UpdateMutedState();
        }

        public void UpdateMutedState()
        {
            if (!triggerManager.Muted)
            {
                buttonMute.Image = Resources.SoundEnabledSmall;

                BuildFormText();
            }
            else
            {
                soundManager.StopAllSounds();
                buttonMute.Image = Resources.SoundDisabledSmall;
                BuildFormText(true);
                this.Text = String.Format("{1} ({0}) [MUTED]", triggerManager.CharacterName, DisplayName);
            }
            triggerManager.UpdateMutedState();
        }

        void BuildFormText(bool muted = false)
        {
            this.Text = String.Format("{1} ({0}){2}",
                triggerManager.CharacterName,
                DisplayName,
                muted ? " [MUTED]" : "");
        }

        private void buttonManageSNDBank_Click(object sender, EventArgs e)
        {
            soundManager.ShowSoundManager();
        }

        private void FormSoundNotifyConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }

            SaveStateToSettings();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible) RefreshBankAndList();
        }

        private void FormTriggersConfig_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) RefreshBankAndList();
        }

        private void TriggersListView_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            var trigger = (ITrigger)e.Model;
            if (!trigger.Active) e.Item.BackColor = Color.LightGray;
        }

        private void TriggersListView_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
        }

        private void TriggersListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selected = TriggersListView.SelectedObject;
                if (selected != null)
                {
                    var trigger = (ITrigger)selected;
                    trigger.Active = !trigger.Active;
                    RefreshBankAndList();
                    TriggersListView.DeselectAll();
                }
            }
        }

        private void TriggersListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                EditCurrentItem();
            }
        }

        void SaveStateToSettings()
        {
            var settings = TriggersListView.SaveState();
            triggerManager.TriggerListState = settings;
        }

        private void TriggersListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveCurrentItems();
            }
        }

        private void TriggersListView_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Column == olvColumnSound)
            {
                var trigger = (ITrigger)e.Model;
                switch (trigger.HasSoundAspect)
                {
                    case ThreeStateBool.True:
                        e.SubItem.BackColor = Color.LightGreen;
                        break;
                    case ThreeStateBool.Error:
                        e.SubItem.BackColor = Color.PeachPuff;
                        break;
                }
            }
            else if (e.Column == olvColumnPopup)
            {
                var trigger = (ITrigger)e.Model;
                if (trigger.HasPopupAspect == ThreeStateBool.True)
                {
                    e.SubItem.BackColor = Color.LightGreen;
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {

        }

        private void buttonExportSelected_Click(object sender, EventArgs e)
        {
            var selectedTriggers = TriggersListView.SelectedObjects.Cast<ITrigger>().ToList();
            if (selectedTriggers.Any())
            {
                var exporter = exporterFactory.CreateExporter();
                exporter.Export(selectedTriggers);
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            var importer = importerFactory.CreateImporter();
            importer.Import(triggerManager);
        }
    }
}
