using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.ViewModels;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Views;
using AldursLab.WurmAssistant3.Core.WinForms;
using BrightIdeasSoftware;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Views
{
    public partial class CombatResultsView : ExtendedForm
    {
        readonly ICombatDataSource combatDataSource;
        readonly FeatureSettings featureSettings;
        readonly IHostEnvironment hostEnvironment;

        bool refreshRequired = false;

        readonly Dictionary<Tuple<CombatActor, CombatActor>, CombatActorViewModel> viewModelsMap =
            new Dictionary<Tuple<CombatActor, CombatActor>, CombatActorViewModel>();

        readonly TextMatchFilter filter;
        readonly IModelFilter defaultFilter;

        byte[] initialOlvState;

        public CombatResultsView(ICombatDataSource combatDataSource, FeatureSettings featureSettings,
            IHostEnvironment hostEnvironment)
        {
            if (combatDataSource == null) throw new ArgumentNullException("combatDataSource");
            if (featureSettings == null) throw new ArgumentNullException("featureSettings");
            if (hostEnvironment == null) throw new ArgumentNullException("hostEnvironment");
            this.combatDataSource = combatDataSource;
            this.featureSettings = featureSettings;
            this.hostEnvironment = hostEnvironment;
            InitializeComponent();

            combatDataSource.DataChanged += CombatResultsOnDataChanged;

            filter = new TextMatchFilter(objectListView1,
                "",
                StringComparison.InvariantCultureIgnoreCase);
            filter.Columns = new[] {olvColumn0};

            defaultFilter = objectListView1.ModelFilter;
            rowHeightNup.Value = objectListView1.RowHeight.ConstrainToRange(1, 10000);

            if (featureSettings.CombatResultViewState.Any())
            {
                objectListView1.RestoreState(featureSettings.CombatResultViewState);
            }

            initialOlvState = objectListView1.SaveState();

            hostEnvironment.HostClosing += HostEnvironmentOnHostClosing;

            RefreshData();
        }

        void HostEnvironmentOnHostClosing(object sender, EventArgs eventArgs)
        {
            PersistOlvState();
        }

        void CombatResultsOnDataChanged(object sender, EventArgs eventArgs)
        {
            refreshRequired = true;
        }

        private void CombatResultsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            combatDataSource.DataChanged -= CombatResultsOnDataChanged;
            hostEnvironment.HostClosing -= HostEnvironmentOnHostClosing;
            PersistOlvState();
        }

        void PersistOlvState()
        {
            // persist the state only if changed
            // this will still cause conficts when multiple states are modified in multiple windows,
            // but it's not an easy problem to fix
            var olvState = objectListView1.SaveState();
            if (!olvState.SequenceEqual(initialOlvState))
            {
                featureSettings.CombatResultViewState = objectListView1.SaveState();
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (refreshRequired)
            {
                RefreshData();

                refreshRequired = false;
            }
        }

        void RefreshData()
        {
            foreach (var pairStats in combatDataSource.CombatStatus.AllStats)
            {
                {
                    var key = new Tuple<CombatActor, CombatActor>(pairStats.ActorOne, pairStats.ActorTwo);
                    if (!viewModelsMap.ContainsKey(key))
                    {
                        viewModelsMap[key] = new CombatActorViewModel(pairStats, key.Item1.Name);
                    }
                }

                {
                    var key = new Tuple<CombatActor, CombatActor>(pairStats.ActorTwo, pairStats.ActorOne);
                    if (!viewModelsMap.ContainsKey(key))
                    {
                        viewModelsMap[key] = new CombatActorViewModel(pairStats, key.Item1.Name);
                    }
                }
            }

            objectListView1.SetObjects(viewModelsMap.Values.ToArray(), preserveState: true);
        }

        private void nameFilterTbox_TextChanged(object sender, EventArgs e)
        {
            var text = nameFilterTbox.Text ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                filter.ContainsStrings = new List<string>() { text };
                objectListView1.ModelFilter = filter;
            }
            else
            {
                objectListView1.ModelFilter = defaultFilter;
            }

            objectListView1.BuildList(true);
        }

        private void rowHeightNup_ValueChanged(object sender, EventArgs e)
        {
            objectListView1.RowHeight = (int)rowHeightNup.Value;
        }

        private void legendBtn_Click(object sender, EventArgs e)
        {
            UniversalTextDisplayView view = new UniversalTextDisplayView();
            view.Text = "Combat statistics legend";
            view.ContentText = Resources.CombatStatisticsLegend;
            view.ShowCenteredOnForm(this);
        }

        private void CombatResultsView_Load(object sender, EventArgs e)
        {

        }
    }
}
