using System;
using System.Collections.Generic;
using System.Drawing;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.Modules
{
    [PersistentObject("GrangerFeature_GrangerSettings")]
    public class GrangerSettings : PersistentObjectBase
    {
        readonly ILogger logger;

        public GrangerSettings([NotNull] ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            this.logger = logger;

            doNotShowReadFirstWindow = false;

            traitViewVisible = true;
            herdViewVisible = true;
            logCaptureEnabled = true;

            mainWindowSize = new System.Drawing.Size(784, 484);

            captureForPlayers = new List<string>();

            valuePresetId = string.Empty;
            advisorId = string.Empty;

            herdViewSplitterPosition = 250;

            genesisLog = new Dictionary<string, DateTime>();

            showGroomingTime = TimeSpan.FromMinutes(60);
            updateCreatureDataFromAnyEventLine = true;
        }

        [JsonProperty]
        TimeSpan showGroomingTime;

        public TimeSpan ShowGroomingTime
        {
            get { return showGroomingTime; }
            set { showGroomingTime = value; FlagAsChanged(); }
        }

        /// <summary>
        /// By default creatures can't be updated if wrong herds are selected.
        /// This option makes update possible as long, as creature name
        /// is unique in entire database
        /// </summary>
        [JsonProperty]
        bool doNotBlockDataUpdateUnlessMultiplesInEntireDb;

        public bool DoNotBlockDataUpdateUnlessMultiplesInEntireDb
        {
            get { return doNotBlockDataUpdateUnlessMultiplesInEntireDb; }
            set { doNotBlockDataUpdateUnlessMultiplesInEntireDb = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool updateCreatureDataFromAnyEventLine;

        public bool UpdateCreatureDataFromAnyEventLine
        {
            get { return updateCreatureDataFromAnyEventLine; }
            set { updateCreatureDataFromAnyEventLine = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool doNotShowReadFirstWindow;

        public bool DoNotShowReadFirstWindow
        {
            get { return doNotShowReadFirstWindow; }
            set { doNotShowReadFirstWindow = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool traitViewVisible;

        public bool TraitViewVisible
        {
            get { return traitViewVisible; }
            set { traitViewVisible = value; FlagAsChanged(); }
        }
        [JsonProperty]
        bool herdViewVisible;

        public bool HerdViewVisible
        {
            get { return herdViewVisible; }
            set { herdViewVisible = value; FlagAsChanged(); }
        }
        [JsonProperty]
        bool logCaptureEnabled;

        public bool LogCaptureEnabled
        {
            get { return logCaptureEnabled; }
            set { logCaptureEnabled = value; FlagAsChanged(); }
        }

        [JsonProperty]
        System.Drawing.Size mainWindowSize;

        public Size MainWindowSize
        {
            get { return mainWindowSize; }
            set { mainWindowSize = value; FlagAsChanged(); }
        }

        [JsonProperty]
        string valuePresetId;

        public string ValuePresetId
        {
            get { return valuePresetId; }
            set { valuePresetId = value; FlagAsChanged(); }
        }

        [JsonProperty]
        string advisorId;

        public string AdvisorId
        {
            get { return advisorId; }
            set { advisorId = value; FlagAsChanged(); }
        }

        [JsonProperty]
        byte[] creatureListState;

        public byte[] CreatureListState
        {
            get { return creatureListState; }
            set { creatureListState = value; FlagAsChanged(); }
        }

        [JsonProperty]
        TraitViewManager.TraitDisplayMode traitViewDisplayMode;

        public TraitViewManager.TraitDisplayMode TraitViewDisplayMode
        {
            get { return traitViewDisplayMode; }
            set { traitViewDisplayMode = value; FlagAsChanged(); }
        }

        [JsonProperty]
        int herdViewSplitterPosition;

        public int HerdViewSplitterPosition
        {
            get { return herdViewSplitterPosition; }
            set { herdViewSplitterPosition = value; FlagAsChanged(); }
        }

        [JsonProperty]
        byte[] traitViewState;

        public byte[] TraitViewState
        {
            get { return traitViewState; }
            set { traitViewState = value; FlagAsChanged(); }
        }

        [JsonProperty] 
        bool disableRowColoring;

        public bool DisableRowColoring
        {
            get { return disableRowColoring; }
            set { disableRowColoring = value; FlagAsChanged(); }
        }

        [JsonProperty] 
        bool adjustForDarkThemes;

        public bool AdjustForDarkThemes
        {
            get { return adjustForDarkThemes; }
            set { adjustForDarkThemes = value; FlagAsChanged(); }
        }

        [JsonProperty] 
        readonly Dictionary<string, DateTime> genesisLog;

        internal void AddGenesisCast(DateTime castDate, string creatureName)
        {
            List<string> keysToRemove = null;
            var dtTreshhold = DateTime.Now - TimeSpan.FromHours(1);
            foreach (var keyval in genesisLog)
            {
                if (keyval.Value < dtTreshhold)
                {
                    if (keysToRemove == null)
                        keysToRemove = new List<string>();
                    keysToRemove.Add(keyval.Key);
                }
            }
            if (keysToRemove != null)
            {
                foreach (var creaturename in keysToRemove)
                {
                    genesisLog.Remove(creaturename);
                    logger.Info(string.Format("Removed cached genesis cast data for {0}", creaturename));
                }
            }
            genesisLog[creatureName] = castDate;
            FlagAsChanged();
        }

        internal bool HasGenesisCast(string creatureName)
        {
            DateTime castTime;
            if (genesisLog.TryGetValue(creatureName, out castTime))
            {
                if (castTime > DateTime.Now - TimeSpan.FromHours(1))
                {
                    return true;
                }
            }
            return false;
        }

        internal void RemoveGenesisCast(string creatureName)
        {
            genesisLog.Remove(creatureName);
            FlagAsChanged();
        }

        [JsonProperty] List<string> captureForPlayers;

        public IEnumerable<string> CaptureForPlayers
        {
            get { return captureForPlayers; }
            set { captureForPlayers = new List<string>(value); FlagAsChanged(); }
        }

        [JsonProperty]
        bool useServerNameAsCreatureIdComponent;

        public bool UseServerNameAsCreatureIdComponent
        {
            get { return useServerNameAsCreatureIdComponent; }
            set { useServerNameAsCreatureIdComponent = value; FlagAsChanged(); }
        }
    }
}
