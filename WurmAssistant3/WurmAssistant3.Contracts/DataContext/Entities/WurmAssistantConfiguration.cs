using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.DataContext.Entities
{
    [JsonObject(MemberSerialization.Fields)]
    public class WurmAssistantConfiguration
    {
        private WurmClientConfig wurmClientConfig;

        public WurmAssistantConfiguration()
        {
            wurmClientConfig = new WurmClientConfig();
        }

        public WurmClientConfig WurmClientConfig
        {
            get { return wurmClientConfig; }
        }
    }

    [JsonObject(MemberSerialization.Fields)]
    public class WurmClientConfig
    {
        private LogSaveMode logSaveMode = LogSaveMode.Daily;
        private SkillGainRate skillGainRate = SkillGainRate.Per0D001;
        private string wurmClientInstallDirOverride;
        private bool doNotAskToSyncWurmClients;

        /// <summary>
        /// Null indicates "do not change this setting".
        /// </summary>
        public WurmApi.LogSaveMode LogSaveMode
        {
            get { return logSaveMode; }
            set { logSaveMode = value; }
        }

        /// <summary>
        /// Null indicates "do not change this setting".
        /// </summary>
        public WurmApi.SkillGainRate SkillGainRate
        {
            get { return skillGainRate; }
            set { skillGainRate = value; }
        }

        /// <summary>
        /// Full directory path or null, if not overriding.
        /// </summary>
        public string WurmClientInstallDirOverride
        {
            get { return wurmClientInstallDirOverride; }
            set { wurmClientInstallDirOverride = value; }
        }

        public bool DoNotAskToSyncWurmClients
        {
            get { return doNotAskToSyncWurmClients; }
            set { doNotAskToSyncWurmClients = value; }
        }
    }
}
