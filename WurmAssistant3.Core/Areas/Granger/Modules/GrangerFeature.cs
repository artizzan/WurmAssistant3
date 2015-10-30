using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.Advisor.Default;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.LogFeedManager;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules
{
    [PersistentObject("GrangerFeature")]
    public class GrangerFeature : PersistentObjectBase, IFeature
    {
        readonly ILogger logger;
        readonly IWurmAssistantDataDirectory dataDirectory;
        readonly IUpdateLoop updateLoop;
        readonly IHostEnvironment hostEnvironment;
        readonly ISoundEngine soundEngine;
        readonly ITrayPopups trayPopups;

        readonly GrangerSettings settings;
        readonly DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions;
        readonly GrangerSimpleDb grangerSimpleDb;
        readonly FormGrangerMain grangerUi;

        readonly LogsFeedManager logsFeedMan;

        public GrangerFeature([NotNull] ILogger logger, [NotNull] IWurmAssistantDataDirectory dataDirectory,
            [NotNull] IUpdateLoop updateLoop, [NotNull] IHostEnvironment hostEnvironment,
            [NotNull] ISoundEngine soundEngine, [NotNull] ITrayPopups trayPopups, [NotNull] IWurmApi wurmApi, GrangerSettings grangerSettings,
            [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions,
            [NotNull] GrangerSimpleDb grangerSimpleDb)
        {
            this.logger = logger;
            this.dataDirectory = dataDirectory;
            this.updateLoop = updateLoop;
            this.hostEnvironment = hostEnvironment;
            this.soundEngine = soundEngine;
            this.trayPopups = trayPopups;
            if (logger == null) throw new ArgumentNullException("logger");
            if (dataDirectory == null) throw new ArgumentNullException("dataDirectory");
            if (updateLoop == null) throw new ArgumentNullException("updateLoop");
            if (hostEnvironment == null) throw new ArgumentNullException("hostEnvironment");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (defaultBreedingEvaluatorOptions == null)
                throw new ArgumentNullException("defaultBreedingEvaluatorOptions");
            if (grangerSimpleDb == null) throw new ArgumentNullException("grangerSimpleDb");

            settings = grangerSettings;
            this.defaultBreedingEvaluatorOptions = defaultBreedingEvaluatorOptions;
            this.grangerSimpleDb = grangerSimpleDb;

            var context = new GrangerContext(grangerSimpleDb);

            grangerUi = new FormGrangerMain(this, settings, context, logger, wurmApi, defaultBreedingEvaluatorOptions);

            logsFeedMan = new LogsFeedManager(this, context, wurmApi, logger, trayPopups);
            logsFeedMan.UpdatePlayers(settings.CaptureForPlayers);
            grangerUi.Granger_PlayerListChanged += GrangerUI_Granger_PlayerListChanged;

            updateLoop.Updated += (sender, args) => Update();
            hostEnvironment.HostClosing += (sender, args) => Stop();
        }

        void GrangerUI_Granger_PlayerListChanged(object sender, EventArgs e)
        {
            logsFeedMan.UpdatePlayers(settings.CaptureForPlayers);
        }

        private void Update()
        {
            logsFeedMan.Update();
        }

        private void Stop()
        {
            if (grangerUi != null) grangerUi.SaveAllState();
            else logger.Error("Granger UI null when trying to save state on Stop");
            logsFeedMan.Dispose();
        }

        #region IFeature

        void IFeature.Show()
        {
            if (grangerUi != null)
            {
                grangerUi.ShowAndBringToFront();
            }
            else
            {
                logger.Error("GrangerUI was null");
            }
        }

        void IFeature.Hide()
        {
        }

        string IFeature.Name
        {
            get { return "Granger"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.GrangerIcon; }
        }

        public GrangerSettings Settings
        {
            get { return settings; }
        }

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        #endregion IFeature
    }
}
