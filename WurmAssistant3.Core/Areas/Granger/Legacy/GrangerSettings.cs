using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.LogFeedManager;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
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

            ahSkillInfos = new List<AhSkillInfo>();

            genesisLog = new Dictionary<string, DateTime>();

            showGroomingTime = TimeSpan.FromMinutes(60);
            updateHorseDataFromAnyEventLine = true;
        }

        [JsonProperty]
        TimeSpan showGroomingTime;

        public TimeSpan ShowGroomingTime
        {
            get { return showGroomingTime; }
            set { showGroomingTime = value; FlagAsChanged(); }
        }

        /// <summary>
        /// By default horses can't be updated if wrong herds are selected.
        /// This option makes update possible as long, as horse name
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
        bool updateHorseDataFromAnyEventLine;

        public bool UpdateHorseDataFromAnyEventLine
        {
            get { return updateHorseDataFromAnyEventLine; }
            set { updateHorseDataFromAnyEventLine = value; FlagAsChanged(); }
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
        byte[] horseListState;

        public byte[] HorseListState
        {
            get { return horseListState; }
            set { horseListState = value; FlagAsChanged(); }
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

        // todo: json.net does not allow composite dictionary keys without type converted
        // quickly fixed until refactoring
        [JsonProperty] readonly List<AhSkillInfo> ahSkillInfos;

        public bool TryGetAHSkill([NotNull] string player, ServerGroupId serverGroupId, out float result)
        {
            if (player == null) throw new ArgumentNullException("player");
            var info = GetSkillInfo(player, serverGroupId);
            result = info.SkillValue;
            return result != 0f;
        }

        public void SetAHSkill([NotNull] string player, ServerGroupId serverGroupId, float AHValue)
        {
            if (player == null) throw new ArgumentNullException("player");
            var info = GetSkillInfo(player, serverGroupId);
            info.SkillValue = AHValue;
            FlagAsChanged();
        }

        public bool TryGetAHCheckDate([NotNull] string player, ServerGroupId serverGroupId, out DateTime result)
        {
            if (player == null) throw new ArgumentNullException("player");
            var info = GetSkillInfo(player, serverGroupId);
            result = info.LastCheck;
            return result != DateTime.MinValue;
        }

        public void SetAHCheckDate([NotNull] string player, ServerGroupId serverGroupId, DateTime AHValue)
        {
            if (player == null) throw new ArgumentNullException("player");
            var info = GetSkillInfo(player, serverGroupId);
            info.LastCheck = AHValue;
            FlagAsChanged();
        }

        AhSkillInfo GetSkillInfo(string player, ServerGroupId serverGroupId)
        {
            var info =
                ahSkillInfos.FirstOrDefault(
                    x =>
                        x.PlayerName.ToUpperInvariant().Equals(player.ToUpperInvariant())
                        && x.ServerGroupId == serverGroupId);
            if (info == null)
            {
                info = new AhSkillInfo(serverGroupId, player, 0, DateTime.MinValue);
                ahSkillInfos.Add(info);
            }
            return info;
        }

        [JsonProperty] readonly Dictionary<string, DateTime> genesisLog;

        internal void AddGenesisCast(DateTime castDate, string horseName)
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
                foreach (var horsename in keysToRemove)
                {
                    genesisLog.Remove(horsename);
                    logger.Info(string.Format("Removed cached genesis cast data for {0}", horsename));
                }
            }
            genesisLog[horseName] = castDate;
            FlagAsChanged();
        }

        internal bool HasGenesisCast(string horseName)
        {
            DateTime castTime;
            if (genesisLog.TryGetValue(horseName, out castTime))
            {
                if (castTime > DateTime.Now - TimeSpan.FromHours(1))
                {
                    return true;
                }
            }
            return false;
        }

        internal void RemoveGenesisCast(string horseName)
        {
            genesisLog.Remove(horseName);
            FlagAsChanged();
        }

        [JsonProperty] List<string> captureForPlayers;

        public IEnumerable<string> CaptureForPlayers
        {
            get { return captureForPlayers; }
            set { captureForPlayers = new List<string>(value); FlagAsChanged(); }
        }
    }
}
