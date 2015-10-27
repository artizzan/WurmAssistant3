using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.Advisor;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.Advisor.Default;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.ValuePreset;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
{
    public partial class FormGrangerMain : ExtendedForm
    {
        public class UserViewChangedEventArgs : EventArgs
        {
            public readonly bool HerdViewVisible;
            public readonly bool TraitViewVisible;
            public UserViewChangedEventArgs(bool herdViewVisible, bool traitViewVisible)
            {
                this.HerdViewVisible = herdViewVisible;
                this.TraitViewVisible = traitViewVisible;
            }
        }
        public event EventHandler<UserViewChangedEventArgs> Granger_UserViewChanged;

        public event EventHandler Granger_ValuatorChanged;
        public event EventHandler Granger_AdvisorChanged;

        public event EventHandler Granger_PlayerListChanged;
        public event EventHandler Granger_SelectedSingleHorseChanged;
        public event EventHandler Granger_TraitViewDisplayModeChanged;

        public GrangerSettings Settings;

        private GrangerFeature ParentModule;
        private GrangerContext Context;
        readonly ILogger logger;
        readonly IWurmApi wurmApi;
        readonly DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions;

        public TraitValuator CurrentValuator { get; private set; }
        public BreedingAdvisor CurrentAdvisor { get; private set; }

        /// <summary>
        /// null if none or more horses selected, else ref to horse
        /// </summary>
        public Horse SelectedSingleHorse { get { return ucGrangerHorseList1.SelectedSingleHorse; } }

        bool _WindowInitCompleted = false;
        public FormGrangerMain(GrangerFeature grangerFeature, GrangerSettings settings, GrangerContext context,
            [NotNull] ILogger logger, [NotNull] IWurmApi wurmApi,
            [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (defaultBreedingEvaluatorOptions == null)
                throw new ArgumentNullException("defaultBreedingEvaluatorOptions");
            this.ParentModule = grangerFeature;
            this.Settings = settings;
            this.Context = context;
            this.logger = logger;
            this.wurmApi = wurmApi;
            this.defaultBreedingEvaluatorOptions = defaultBreedingEvaluatorOptions;

            InitializeComponent();

            RebuildValuePresets();
            RefreshValuator();
            RebuildAdvisors();
            RefreshAdvisor();

            ucGrangerHerdList1.Init(this, context, logger);
            ucGrangerHorseList1.Init(this, context, logger);
            ucGrangerTraitView1.Init(this, context, logger);

            Context.OnTraitValuesModified += Context_OnTraitValuesModified;

            this.Size = Settings.MainWindowSize;

            this.checkBoxCapturingEnabled.Checked = Settings.LogCaptureEnabled;
            this.UpdateViewsVisibility();
            this.Update_textBoxCaptureForPlayers();

            _WindowInitCompleted = true;
        }

        /// <summary>
        /// triggers Granger_SelectedSingleHorseChanged event
        /// </summary>
        internal void TriggerSelectedSingleHorseChanged()
        {
            if (Granger_SelectedSingleHorseChanged != null) 
                Granger_SelectedSingleHorseChanged(this, EventArgs.Empty);
        }

        #region TRAIT VALUES

        internal void InvalidateTraitValuator()
        {
            RebuildValuePresets();
            RefreshValuator();
        }

        void Context_OnTraitValuesModified(object sender, EventArgs e)
        {
            RebuildValuePresets();
            RefreshValuator();
        }

        bool _rebuildingValuePresets = false;
        private void comboBoxValuePreset_TextChanged(object sender, EventArgs e)
        {
            if (!_rebuildingValuePresets)
            {
                RefreshValuator();
            }
        }

        void RebuildValuePresets()
        {
            _rebuildingValuePresets = true;
            comboBoxValuePreset.Items.Clear();
            var valuemaps = Context.TraitValues.AsEnumerable().Select(x => x.ValueMapID).Distinct().ToArray();
            comboBoxValuePreset.Items.Add(TraitValuator.DefaultId);
            comboBoxValuePreset.Items.AddRange(valuemaps);
            if (valuemaps.Contains(Settings.ValuePresetId))
                comboBoxValuePreset.Text = Settings.ValuePresetId;
            else comboBoxValuePreset.Text = TraitValuator.DefaultId;
            _rebuildingValuePresets = false;
        }

        void RefreshValuator()
        {
            try
            {
                CurrentValuator = new TraitValuator(this, comboBoxValuePreset.Text, Context);
            }
            catch (Exception _e)
            {
                CurrentValuator = new TraitValuator(this);
                logger.Error(_e, "failed to create TraitValuator for valuemapid: " + comboBoxValuePreset.Text + "; using defaults");
            }
            Settings.ValuePresetId = CurrentValuator.ValueMapId;

            if (Granger_ValuatorChanged != null) Granger_ValuatorChanged(this, new EventArgs());
        }

        private void buttonEditValuePreset_Click(object sender, EventArgs e)
        {
            //dialog to edit value presets
            FormEditValuePresets ui = new FormEditValuePresets(Context);
            ui.ShowCenteredOnForm(this);
        }

        #endregion

        #region ADVISORS

        bool _rebuildingAdvisors = false;
        void RebuildAdvisors()
        {
            _rebuildingAdvisors = true;
            comboBoxAdvisor.Items.Clear();
            var advisors = BreedingAdvisor.DefaultAdvisorIDs;
            comboBoxAdvisor.Items.AddRange(advisors);
            if (advisors.Contains(Settings.AdvisorId))
            {
                comboBoxAdvisor.Text = Settings.AdvisorId;
            }
            else comboBoxAdvisor.Text = BreedingAdvisor.DEFAULT_id;
            _rebuildingAdvisors = false;
        }

        void RefreshAdvisor()
        {
            try
            {
                CurrentAdvisor = new BreedingAdvisor(this,
                    comboBoxAdvisor.Text,
                    Context,
                    logger,
                    defaultBreedingEvaluatorOptions);
            }
            catch (Exception _e)
            {
                CurrentAdvisor = new BreedingAdvisor(this,
                    BreedingAdvisor.DEFAULT_id,
                    Context,
                    logger,
                    defaultBreedingEvaluatorOptions);
                logger.Error(_e,
                    "failed to create BreedingAdvisor for advisorid: " + comboBoxAdvisor.Text + "; using defaults");
            }
            Settings.AdvisorId = CurrentAdvisor.AdvisorID;
            if (Granger_AdvisorChanged != null) Granger_AdvisorChanged(this, new EventArgs());
        }

        private void comboBoxAdvisor_TextChanged(object sender, EventArgs e)
        {
            if (!_rebuildingAdvisors) RefreshAdvisor();
        }

        private void buttonAdvisorOptions_Click(object sender, EventArgs e)
        {
            if (CurrentAdvisor != null)
            {
                if (CurrentAdvisor.ShowOptions(this))
                {
                    //do not call refresh advisor, rebuilds are not needed if options are changed!
                    if (Granger_AdvisorChanged != null) Granger_AdvisorChanged(this, new EventArgs());
                }
            }
        }

        #endregion

        #region MISC

        FormGrangerNewInfo NewcomerHelpUI;

        
        private void FormGrangerMain_Load(object sender, EventArgs e)
        {
            try
            {
                splitContainer2.SplitterDistance = Settings.HerdViewSplitterPosition;
                splitContainer2.Panel2Collapsed = Settings.TraitViewVisible;
            }
            catch (Exception _e)
            {
                logger.Error(_e, "FormGrangerMain_Load");
                throw;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible && !Settings.DoNotShowReadFirstWindow)
            {
                NewcomerHelpUI = new FormGrangerNewInfo(Settings);
                NewcomerHelpUI.Show(); //show here so its in front
                //TODO remove this when wiki complete
                timer1.Enabled = false;
            }
        }

        internal void BringBackFromAbyss()
        {
            if (this.Visible)
            {
                this.BringToFront();
            }
            else
            {
                this.Show();
                this.RestoreFromMin();
            }
        }

        internal void RestoreFromMin()
        {
            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
        }

        private void FormGrangerMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    this.Hide();
                }
                SaveAllState();
            }
            catch (Exception _e)
            {
                logger.Error(_e, "FormGrangerMain_FormClosing");
                throw;
            }
        }

        public void SaveAllState()
        {
            try
            {
                if (!this.IsDisposed)
                {
                    //show panel if hidden, so correct SplitterDistance can be saved
                    ucGrangerTraitView1.SaveStateToSettings();
                    ucGrangerHorseList1.SaveStateToSettings();
                    splitContainer2.Panel2Collapsed = false;
                    Settings.HerdViewSplitterPosition = splitContainer2.SplitterDistance;
                }
                else
                    logger.Error("SaveAllState when already disposed");
            }
            catch (Exception _e)
            {
                logger.Error(_e, "SaveAllState");
                throw;
            }
        }

        private void FormGrangerMain_Resize(object sender, EventArgs e)
        {
            try
            {
                if (_WindowInitCompleted)
                {
                    Settings.MainWindowSize = this.Size;
                }
            }
            catch (Exception _e)
            {
                logger.Error(_e, "FormGrangerMain_Resize");
                throw;
            }
        }

        #endregion

        #region VIEWS

        private void buttonHerdView_Click(object sender, EventArgs e)
        {
            Settings.HerdViewVisible = !Settings.HerdViewVisible;
            UpdateViewsVisibility();
        }

        private void buttonTraitView_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
            Settings.TraitViewVisible = splitContainer2.Panel2Collapsed;
            UpdateViewsVisibility();
        }

        void UpdateViewsVisibility()
        {
            if (Settings.HerdViewVisible)
                tableLayoutPanel1.ColumnStyles[0].Width = 150;
            else tableLayoutPanel1.ColumnStyles[0].Width = 0;

            if (Granger_UserViewChanged != null)
                Granger_UserViewChanged(this, new UserViewChangedEventArgs(
                    Settings.HerdViewVisible,
                    Settings.TraitViewVisible));
        }

        public TraitViewManager.TraitDisplayMode TraitViewDisplayMode
        {
            get
            {
                return Settings.TraitViewDisplayMode;
            }
            set
            {
                Settings.TraitViewDisplayMode = value;
                if (Granger_TraitViewDisplayModeChanged != null) Granger_TraitViewDisplayModeChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region LOG TRACKING

        private void checkBoxCapturingEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Settings.LogCaptureEnabled = checkBoxCapturingEnabled.Checked;
        }

        private void buttonChangePlayers_Click(object sender, EventArgs e)
        {
            FormChoosePlayers dialog = new FormChoosePlayers((Settings.CaptureForPlayers ?? new List<string>()).ToArray(), wurmApi);
            if (dialog.ShowDialogCenteredOnForm(this) == System.Windows.Forms.DialogResult.OK)
            {
                Settings.CaptureForPlayers = dialog.Result.ToList();
                Update_textBoxCaptureForPlayers();
                if (Granger_PlayerListChanged != null) Granger_PlayerListChanged(this, new EventArgs());
            }
        }

        void Update_textBoxCaptureForPlayers()
        {
            textBoxCaptureForPlayers.Text = String.Join(", ", Settings.CaptureForPlayers);
        }

        #endregion

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            
        }

        private void buttonOptions_Click(object sender, EventArgs e)
        {

        }

        private void buttonGrangerGeneralOptions_Click(object sender, EventArgs e)
        {
            var ui = new FormGrangerGeneralOptions(Settings);
            if (ui.ShowDialogCenteredOnForm(this) == DialogResult.OK)
            {
                //do something with results?
                //nope
            }
        }

        private void buttonImportExport_Click(object sender, EventArgs e)
        {
            var ui = new FormGrangerImportExport(Context, logger);
            ui.ShowDialogCenteredOnForm(this);
        }
    }
}
