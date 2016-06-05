using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Parts.TriggersManager;
using AldursLab.WurmAssistant3.Areas.Triggers.Transients;
using AldursLab.WurmAssistant3.Properties;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Parts
{
    public partial class FormTriggersConfig : ExtendedForm
    {
        readonly TriggerManager triggerManager;
        readonly ISoundManager soundManager;
        private const string DisplayName = "Triggers";

        public FormTriggersConfig([NotNull] TriggerManager triggerManager, [NotNull] ISoundManager soundManager)
        {
            if (triggerManager == null) throw new ArgumentNullException("triggerManager");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            InitializeComponent();
            this.triggerManager = triggerManager;
            this.soundManager = soundManager;
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
                    triggerManager.FlagAsChanged();
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
            triggerManager.FlagAsChanged();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            RemoveCurrentItem();
        }

        void RemoveCurrentItem()
        {
            var selected = TriggersListView.SelectedObject;
            if (selected != null)
            {
                triggerManager.RemoveTrigger((ITrigger)selected);
                RefreshBankAndList();
                triggerManager.FlagAsChanged();
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
            triggerManager.FlagAsChanged();
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
            triggerManager.FlagAsChanged();
        }

        private void TriggersListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveCurrentItem();
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
    }
}
