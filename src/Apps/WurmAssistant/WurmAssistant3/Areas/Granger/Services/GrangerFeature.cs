using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Granger.Parts;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.LogFeedManager;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Services
{
    [KernelBind(BindingHint.Singleton), PersistentObject("GrangerFeature")]
    public class GrangerFeature : PersistentObjectBase, IFeature, IDisposable
    {
        readonly ILogger logger;
        readonly IWurmAssistantDataDirectory dataDirectory;
        readonly ISoundManager soundManager;
        readonly ITrayPopups trayPopups;

        readonly GrangerSettings settings;
        readonly DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions;
        readonly GrangerSimpleDb grangerSimpleDb;
        readonly FormGrangerMain grangerUi;

        readonly LogsFeedManager logsFeedMan;
        readonly GrangerContext context;

        public GrangerFeature([NotNull] ILogger logger, 
            [NotNull] IWurmAssistantDataDirectory dataDirectory,
            [NotNull] ISoundManager soundManager, 
            [NotNull] ITrayPopups trayPopups, 
            [NotNull] IWurmApi wurmApi, 
            GrangerSettings grangerSettings,
            [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions,
            [NotNull] GrangerSimpleDb grangerSimpleDb,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            this.logger = logger;
            this.dataDirectory = dataDirectory;
            this.soundManager = soundManager;
            this.trayPopups = trayPopups;
            if (logger == null) throw new ArgumentNullException("logger");
            if (dataDirectory == null) throw new ArgumentNullException("dataDirectory");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (defaultBreedingEvaluatorOptions == null)
                throw new ArgumentNullException("defaultBreedingEvaluatorOptions");
            if (grangerSimpleDb == null) throw new ArgumentNullException("grangerSimpleDb");
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));

            settings = grangerSettings;
            this.defaultBreedingEvaluatorOptions = defaultBreedingEvaluatorOptions;
            this.grangerSimpleDb = grangerSimpleDb;

            context = new GrangerContext(grangerSimpleDb);

            grangerUi = new FormGrangerMain(this, settings, context, logger, wurmApi, defaultBreedingEvaluatorOptions);

            logsFeedMan = new LogsFeedManager(this, context, wurmApi, logger, trayPopups, wurmAssistantConfig);
            logsFeedMan.UpdatePlayers(settings.CaptureForPlayers);
            grangerUi.Granger_PlayerListChanged += GrangerUI_Granger_PlayerListChanged;
        }

        void GrangerUI_Granger_PlayerListChanged(object sender, EventArgs e)
        {
            logsFeedMan.UpdatePlayers(settings.CaptureForPlayers);
        }

        private void Update()
        {
            logsFeedMan.Update();
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

        public void Dispose()
        {
            if (grangerUi != null) grangerUi.SaveAllState();
            else logger.Error("Granger UI null when trying to save state on Stop");
            logsFeedMan.Dispose();
        }
    }
}
