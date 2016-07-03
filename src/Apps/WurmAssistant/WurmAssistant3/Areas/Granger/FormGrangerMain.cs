using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Granger.Advisor;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.ValuePreset;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormGrangerMain : ExtendedForm
    {
        public event EventHandler<UserViewChangedEventArgs> GrangerUserViewChanged;

        public event EventHandler GrangerValuatorChanged;
        public event EventHandler GrangerAdvisorChanged;

        public event EventHandler GrangerPlayerListChanged;
        public event EventHandler GrangerSelectedSingleCreatureChanged;
        public event EventHandler GrangerTraitViewDisplayModeChanged;

        GrangerSettings settings;

        GrangerFeature parentModule;
        readonly GrangerContext context;
        readonly ILogger logger;
        readonly IWurmApi wurmApi;
        readonly DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions;

        readonly bool _windowInitCompleted = false;
        bool _rebuildingValuePresets = false;
        bool _rebuildingAdvisors = false;
        FormGrangerNewInfo newcomerHelpUi;

        public FormGrangerMain(
            [NotNull] GrangerFeature grangerFeature,
            [NotNull] GrangerSettings settings,
            [NotNull] GrangerContext context,
            [NotNull] ILogger logger, 
            [NotNull] IWurmApi wurmApi,
            [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions)
        {
            if (grangerFeature == null) throw new ArgumentNullException(nameof(grangerFeature));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (defaultBreedingEvaluatorOptions == null) throw new ArgumentNullException(nameof(defaultBreedingEvaluatorOptions));
            this.parentModule = grangerFeature;
            this.settings = settings;
            this.context = context;
            this.logger = logger;
            this.wurmApi = wurmApi;
            this.defaultBreedingEvaluatorOptions = defaultBreedingEvaluatorOptions;

            InitializeComponent();

            RebuildValuePresets();
            RefreshValuator();
            RebuildAdvisors();
            RefreshAdvisor();

            ucGrangerHerdList1.Init(this, context, logger, wurmApi);
            ucGrangerCreatureList1.Init(this, context, logger, wurmApi);
            ucGrangerTraitView1.Init(this, context, logger);

            this.context.OnTraitValuesModified += Context_OnTraitValuesModified;

            this.Size = this.settings.MainWindowSize;

            this.checkBoxCapturingEnabled.Checked = this.settings.LogCaptureEnabled;
            this.UpdateViewsVisibility();
            this.Update_textBoxCaptureForPlayers();

            _windowInitCompleted = true;
        }

        public TraitValuator CurrentValuator { get; private set; }
        public BreedingAdvisor CurrentAdvisor { get; private set; }

        /// <summary>
        /// This value should be null unless exactly one creature is selected in the list.
        /// </summary>
        public Creature SelectedSingleCreature => ucGrangerCreatureList1.SelectedSingleCreature;

        /// <summary>
        /// triggers Granger_SelectedSingleCreatureChanged event
        /// </summary>
        internal void TriggerSelectedSingleCreatureChanged()
        {
            GrangerSelectedSingleCreatureChanged?.Invoke(this, EventArgs.Empty);
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

        void comboBoxValuePreset_TextChanged(object sender, EventArgs e)
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
            var valuemaps = context.TraitValues.AsEnumerable().Select(x => x.ValueMapId).Distinct().ToArray();
            comboBoxValuePreset.Items.Add(TraitValuator.DefaultId);
            comboBoxValuePreset.Items.AddRange(valuemaps.Cast<object>().ToArray());
            comboBoxValuePreset.Text = valuemaps.Contains(settings.ValuePresetId)
                ? settings.ValuePresetId
                : TraitValuator.DefaultId;
            _rebuildingValuePresets = false;
        }

        void RefreshValuator()
        {
            try
            {
                CurrentValuator = new TraitValuator(this, comboBoxValuePreset.Text, context);
            }
            catch (Exception exception)
            {
                CurrentValuator = new TraitValuator();
                logger.Error(exception, "TraitValuator creation failed for valuemapid: " + comboBoxValuePreset.Text + "; reverting to defaults");
            }
            settings.ValuePresetId = CurrentValuator.ValueMapId;

            GrangerValuatorChanged?.Invoke(this, new EventArgs());
        }

        private void buttonEditValuePreset_Click(object sender, EventArgs e)
        {
            //dialog to edit value presets
            FormEditValuePresets ui = new FormEditValuePresets(context);
            ui.ShowCenteredOnForm(this);
        }

        #endregion

        #region ADVISORS

        void RebuildAdvisors()
        {
            _rebuildingAdvisors = true;
            comboBoxAdvisor.Items.Clear();
            var advisors = BreedingAdvisor.DefaultAdvisorIDs;
            comboBoxAdvisor.Items.AddRange(advisors.Cast<object>().ToArray());
            comboBoxAdvisor.Text = advisors.Contains(settings.AdvisorId)
                ? settings.AdvisorId
                : BreedingAdvisor.DefaultId;
            _rebuildingAdvisors = false;
        }

        void RefreshAdvisor()
        {
            try
            {
                CurrentAdvisor = new BreedingAdvisor(this,
                    comboBoxAdvisor.Text ?? string.Empty,
                    context,
                    logger,
                    defaultBreedingEvaluatorOptions);
            }
            catch (Exception exception)
            {
                CurrentAdvisor = new BreedingAdvisor(this,
                    BreedingAdvisor.DefaultId,
                    context,
                    logger,
                    defaultBreedingEvaluatorOptions);
                logger.Error(exception,
                    "BreedingAdvisor creation failed for advisorid: " + comboBoxAdvisor.Text + "; reverting to defaults");
            }
            settings.AdvisorId = CurrentAdvisor.AdvisorId;
            GrangerAdvisorChanged?.Invoke(this, new EventArgs());
        }

        private void comboBoxAdvisor_TextChanged(object sender, EventArgs e)
        {
            if (!_rebuildingAdvisors) RefreshAdvisor();
        }

        private void buttonAdvisorOptions_Click(object sender, EventArgs e)
        {
            if (CurrentAdvisor == null) return;

            if (CurrentAdvisor.ShowOptions(this))
            {
                GrangerAdvisorChanged?.Invoke(this, new EventArgs());
            }
        }

        #endregion

        #region MISC
        
        private void FormGrangerMain_Load(object sender, EventArgs e)
        {
            try
            {
                splitContainer2.SplitterDistance = settings.HerdViewSplitterPosition;
                splitContainer2.Panel2Collapsed = settings.TraitViewVisible;
            }
            catch (Exception exception)
            {
                logger.Error(exception, "FormGrangerMain_Load");
                throw;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.Visible || settings.DoNotShowReadFirstWindow) return;

            newcomerHelpUi = new FormGrangerNewInfo(settings);
            newcomerHelpUi.Show();
            timer1.Enabled = false;
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
            catch (Exception exception)
            {
                logger.Error(exception, "FormGrangerMain_FormClosing");
                throw;
            }
        }

        public void SaveAllState()
        {
            try
            {
                if (!this.IsDisposed)
                {

                    ucGrangerTraitView1.SaveStateToSettings();
                    ucGrangerCreatureList1.SaveStateToSettings();

                    //Showing panel in necessary to save correct SplitterDistance.
                    splitContainer2.Panel2Collapsed = false;

                    settings.HerdViewSplitterPosition = splitContainer2.SplitterDistance;
                }
                else
                {
                    logger.Error("SaveAllState when already disposed");
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "SaveAllState");
                throw;
            }
        }

        private void FormGrangerMain_Resize(object sender, EventArgs e)
        {
            try
            {
                if (_windowInitCompleted)
                {
                    settings.MainWindowSize = this.Size;
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "FormGrangerMain_Resize");
                throw;
            }
        }

        #endregion

        #region VIEWS

        private void buttonHerdView_Click(object sender, EventArgs e)
        {
            settings.HerdViewVisible = !settings.HerdViewVisible;
            UpdateViewsVisibility();
        }

        private void buttonTraitView_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
            settings.TraitViewVisible = splitContainer2.Panel2Collapsed;
            UpdateViewsVisibility();
        }

        void UpdateViewsVisibility()
        {
            tableLayoutPanel1.ColumnStyles[0].Width = settings.HerdViewVisible ? 150 : 0;

            GrangerUserViewChanged?.Invoke(this,
                new UserViewChangedEventArgs(
                    settings.HerdViewVisible,
                    settings.TraitViewVisible));
        }

        public TraitDisplayMode TraitViewDisplayMode
        {
            get
            {
                return settings.TraitViewDisplayMode;
            }
            set
            {
                settings.TraitViewDisplayMode = value;
                GrangerTraitViewDisplayModeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public GrangerSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        #endregion

        #region LOG TRACKING

        private void checkBoxCapturingEnabled_CheckedChanged(object sender, EventArgs e)
        {
            settings.LogCaptureEnabled = checkBoxCapturingEnabled.Checked;
        }

        private void buttonChangePlayers_Click(object sender, EventArgs e)
        {
            var dialog = new FormChoosePlayers((settings.CaptureForPlayers ?? new List<string>()).ToArray(), wurmApi);
            if (dialog.ShowDialogCenteredOnForm(this) == System.Windows.Forms.DialogResult.OK)
            {
                settings.CaptureForPlayers = dialog.Result.ToList();
                Update_textBoxCaptureForPlayers();
                GrangerPlayerListChanged?.Invoke(this, new EventArgs());
            }
        }

        void Update_textBoxCaptureForPlayers()
        {
            textBoxCaptureForPlayers.Text = string.Join(", ", settings.CaptureForPlayers);
        }

        #endregion

        private void buttonGrangerGeneralOptions_Click(object sender, EventArgs e)
        {
            var ui = new FormGrangerGeneralOptions(settings);
            ui.ShowDialogCenteredOnForm(this);
        }

        private void buttonImportExport_Click(object sender, EventArgs e)
        {
            var ui = new FormGrangerImportExport(context, logger);
            ui.ShowDialogCenteredOnForm(this);
        }
    }
}
