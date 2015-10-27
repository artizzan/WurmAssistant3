using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.Advisor.Default;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.Utils;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
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

        public GrangerSettings Settings;
        readonly DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions;
        FormGrangerMain GrangerUI;
        GrangerContext Context;

        LogFeedManager.LogFeedManager LogFeedMan;

        public GrangerFeature([NotNull] ILogger logger, [NotNull] IWurmAssistantDataDirectory dataDirectory,
            [NotNull] IUpdateLoop updateLoop, [NotNull] IHostEnvironment hostEnvironment,
            [NotNull] ISoundEngine soundEngine, [NotNull] ITrayPopups trayPopups, [NotNull] IWurmApi wurmApi, GrangerSettings grangerSettings,
            [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions)
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

            Settings = grangerSettings;
            this.defaultBreedingEvaluatorOptions = defaultBreedingEvaluatorOptions;

            var dbDir = new DirectoryInfo(Path.Combine(dataDirectory.DirectoryPath, "GrangerData"));
            dbDir.Create();
            //init database
            DBSchema.SetConnectionString(Path.Combine(dbDir.FullName, "grangerDB.s3db"));

            SQLiteHelper.CreateTableIfNotExists(DBSchema.HorsesSchema, DBSchema.HorsesTableName, DBSchema.ConnectionString);
            SQLiteHelper.ValidateTable(DBSchema.HorsesSchema, DBSchema.HorsesTableName, DBSchema.ConnectionString);

            SQLiteHelper.CreateTableIfNotExists(DBSchema.TraitValuesSchema, DBSchema.TraitValuesTableName, DBSchema.ConnectionString);
            SQLiteHelper.ValidateTable(DBSchema.TraitValuesSchema, DBSchema.TraitValuesTableName, DBSchema.ConnectionString);

            SQLiteHelper.CreateTableIfNotExists(DBSchema.HerdsSchema, DBSchema.HerdsTableName, DBSchema.ConnectionString);
            SQLiteHelper.ValidateTable(DBSchema.HerdsSchema, DBSchema.HerdsTableName, DBSchema.ConnectionString);

            Context = new GrangerContext(new SQLiteConnection(DBSchema.ConnectionString));

            GrangerUI = new FormGrangerMain(this, Settings, Context, logger, wurmApi, defaultBreedingEvaluatorOptions);

            LogFeedMan = new LogFeedManager.LogFeedManager(this, Context, wurmApi, logger, trayPopups);
            LogFeedMan.UpdatePlayers(Settings.CaptureForPlayers);
            GrangerUI.Granger_PlayerListChanged += GrangerUI_Granger_PlayerListChanged;

            updateLoop.Updated += (sender, args) => Update();
            hostEnvironment.HostClosing += (sender, args) => Stop();
        }

        void GrangerUI_Granger_PlayerListChanged(object sender, EventArgs e)
        {
            LogFeedMan.UpdatePlayers(Settings.CaptureForPlayers);
        }

        private void Update()
        {
            LogFeedMan.Update();
        }

        private void Stop()
        {
            if (GrangerUI != null) GrangerUI.SaveAllState();
            else logger.Error("Granger UI null when trying to save state on Stop");
            LogFeedMan.Dispose();
        }

        internal void ShowPopup(string p)
        {
            trayPopups.Schedule("Granger", p);
        }

        #region IFeature

        void IFeature.Show()
        {
            if (GrangerUI != null)
            {
                GrangerUI.ShowAndBringToFront();
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

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        #endregion IFeature
    }
}
