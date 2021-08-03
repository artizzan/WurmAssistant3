using System;
using System.Collections.Generic;
using System.Drawing;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [KernelBind(BindingHint.Singleton), PersistentObject("GrangerFeature_GrangerSettings")]
    public class GrangerSettings : PersistentObjectBase
    {
        readonly ILogger logger;

        public GrangerSettings([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
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

            showGroomingTime = TimeSpan.FromMinutes(60);
            updateCreatureDataFromAnyEventLine = true;

            updateCreatureColorOnSmilexamines = true;
            requireServerAndSkillToBeKnownForSmilexamine = true;
        }

        [JsonProperty]
        System.Drawing.Size mainWindowSize;

        [JsonProperty]
        TimeSpan showGroomingTime;

        [JsonProperty]
        bool doNotBlockDataUpdateUnlessMultiplesInEntireDb;

        [JsonProperty]
        bool updateCreatureDataFromAnyEventLine;

        [JsonProperty]
        bool doNotShowReadFirstWindow;

        [JsonProperty]
        bool traitViewVisible;

        [JsonProperty]
        bool herdViewVisible;

        [JsonProperty]
        bool logCaptureEnabled;

        [JsonProperty]
        string valuePresetId;

        [JsonProperty]
        string advisorId;

        [JsonProperty]
        byte[] creatureListState;

        [JsonProperty]
        TraitDisplayMode traitViewDisplayMode;

        [JsonProperty]
        int herdViewSplitterPosition;

        [JsonProperty]
        byte[] traitViewState;

        [JsonProperty]
        bool disableRowColoring;

        [JsonProperty]
        bool adjustForDarkThemes;

        [JsonProperty]
        List<string> captureForPlayers;

        [JsonProperty]
        bool useServerNameAsCreatureIdComponent;

        [JsonProperty]
        bool hideLiveTrackerPopups;

        [JsonProperty]
        bool doNotMatchCreaturesByBrandName;

        [JsonProperty]
        bool updateCreatureColorOnSmilexamines;

        [JsonProperty]
        bool requireServerAndSkillToBeKnownForSmilexamine;

        public TimeSpan ShowGroomingTime
        {
            get { return showGroomingTime; }
            set { showGroomingTime = value; FlagAsChanged(); }
        }

        /// <summary>
        /// This option changes how Granger does creature updates.
        /// By default, only creatures from selected herds are considered for update.
        /// This option will skip this check and all creatures in the database will be considered.
        /// However, the unique creature identity constraint is not bypassed by this setting.
        /// </summary>
        public bool DoNotBlockDataUpdateUnlessMultiplesInEntireDb
        {
            get { return doNotBlockDataUpdateUnlessMultiplesInEntireDb; }
            set { doNotBlockDataUpdateUnlessMultiplesInEntireDb = value; FlagAsChanged(); }
        }

        public bool UpdateCreatureDataFromAnyEventLine
        {
            get { return updateCreatureDataFromAnyEventLine; }
            set { updateCreatureDataFromAnyEventLine = value; FlagAsChanged(); }
        }

        public bool DoNotShowReadFirstWindow
        {
            get { return doNotShowReadFirstWindow; }
            set { doNotShowReadFirstWindow = value; FlagAsChanged(); }
        }

        public bool TraitViewVisible
        {
            get { return traitViewVisible; }
            set { traitViewVisible = value; FlagAsChanged(); }
        }

        public bool HerdViewVisible
        {
            get { return herdViewVisible; }
            set { herdViewVisible = value; FlagAsChanged(); }
        }

        public bool LogCaptureEnabled
        {
            get { return logCaptureEnabled; }
            set { logCaptureEnabled = value; FlagAsChanged(); }
        }

        public Size MainWindowSize
        {
            get { return mainWindowSize; }
            set { mainWindowSize = value; FlagAsChanged(); }
        }

        public string ValuePresetId
        {
            get { return valuePresetId; }
            set { valuePresetId = value; FlagAsChanged(); }
        }

        public string AdvisorId
        {
            get { return advisorId; }
            set { advisorId = value; FlagAsChanged(); }
        }

        public byte[] CreatureListState
        {
            get { return creatureListState; }
            set { creatureListState = value; FlagAsChanged(); }
        }

        public TraitDisplayMode TraitViewDisplayMode
        {
            get { return traitViewDisplayMode; }
            set { traitViewDisplayMode = value; FlagAsChanged(); }
        }

        public int HerdViewSplitterPosition
        {
            get { return herdViewSplitterPosition; }
            set { herdViewSplitterPosition = value; FlagAsChanged(); }
        }

        public byte[] TraitViewState
        {
            get { return traitViewState; }
            set { traitViewState = value; FlagAsChanged(); }
        }

        public bool DisableRowColoring
        {
            get { return disableRowColoring; }
            set { disableRowColoring = value; FlagAsChanged(); }
        }

        public bool AdjustForDarkThemes
        {
            get { return adjustForDarkThemes; }
            set { adjustForDarkThemes = value; FlagAsChanged(); }
        }

        public IEnumerable<string> CaptureForPlayers
        {
            get { return captureForPlayers; }
            set { captureForPlayers = new List<string>(value); FlagAsChanged(); }
        }

        public bool UseServerNameAsCreatureIdComponent
        {
            get { return useServerNameAsCreatureIdComponent; }
            set { useServerNameAsCreatureIdComponent = value; FlagAsChanged(); }
        }

        public bool HideLiveTrackerPopups
        {
            get { return hideLiveTrackerPopups; }
            set { hideLiveTrackerPopups = value; FlagAsChanged(); }
        }

        public bool DoNotMatchCreaturesByBrandName
        {
            get { return doNotMatchCreaturesByBrandName; }
            set { doNotMatchCreaturesByBrandName = value; FlagAsChanged(); }
        }

        public bool UpdateCreatureColorOnSmilexamines
        {
            get { return updateCreatureColorOnSmilexamines; }
            set { updateCreatureColorOnSmilexamines = value; FlagAsChanged(); }
        }

        public bool RequireServerAndSkillToBeKnownForSmilexamine
        {
            get { return requireServerAndSkillToBeKnownForSmilexamine; }
            set { requireServerAndSkillToBeKnownForSmilexamine = value; FlagAsChanged(); }
        }
    }
}
