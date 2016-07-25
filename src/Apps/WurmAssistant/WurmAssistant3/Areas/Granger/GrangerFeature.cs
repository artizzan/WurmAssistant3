using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
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

        readonly ITimer updateLoop;

        public GrangerFeature(
            [NotNull] ILogger logger, 
            [NotNull] IWurmAssistantDataDirectory dataDirectory,
            [NotNull] ISoundManager soundManager, 
            [NotNull] ITrayPopups trayPopups, 
            [NotNull] IWurmApi wurmApi,
            [NotNull] GrangerSettings grangerSettings,
            [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions,
            [NotNull] GrangerSimpleDb grangerSimpleDb,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig,
            [NotNull] ITimerFactory timerFactory)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (dataDirectory == null) throw new ArgumentNullException(nameof(dataDirectory));
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (grangerSettings == null) throw new ArgumentNullException(nameof(grangerSettings));
            if (defaultBreedingEvaluatorOptions == null) throw new ArgumentNullException(nameof(defaultBreedingEvaluatorOptions));
            if (grangerSimpleDb == null) throw new ArgumentNullException(nameof(grangerSimpleDb));
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));

            this.logger = logger;
            this.dataDirectory = dataDirectory;
            this.soundManager = soundManager;
            this.trayPopups = trayPopups;

            settings = grangerSettings;
            this.defaultBreedingEvaluatorOptions = defaultBreedingEvaluatorOptions;
            this.grangerSimpleDb = grangerSimpleDb;

            context = new GrangerContext(grangerSimpleDb);

            grangerUi = new FormGrangerMain(this, settings, context, logger, wurmApi, defaultBreedingEvaluatorOptions);

            logsFeedMan = new LogsFeedManager(this, context, wurmApi, logger, trayPopups, wurmAssistantConfig);
            logsFeedMan.UpdatePlayers(settings.CaptureForPlayers);
            grangerUi.GrangerPlayerListChanged += GrangerUI_Granger_PlayerListChanged;
            
            updateLoop = timerFactory.CreateUiThreadTimer();
            updateLoop.Interval = TimeSpan.FromMilliseconds(500);
            updateLoop.Tick += (sender, args) => Update();
            updateLoop.Start();
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

        string IFeature.Name => "Granger";

        Image IFeature.Icon => Resources.GrangerIcon;

        public GrangerSettings Settings => settings;

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        #endregion IFeature

        public void Dispose()
        {
            updateLoop.Stop();
            if (grangerUi != null)
            {
                grangerUi.SaveAllState();
            }
            else
            {
                logger.Error("Granger UI null at saving state on Stop");
            }
            logsFeedMan.Dispose();
        }
    }
}
