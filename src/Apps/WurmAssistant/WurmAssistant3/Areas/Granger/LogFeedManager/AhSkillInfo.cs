using System;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AhSkillInfo
    {
        [JsonProperty]
        readonly string serverGroupId;
        [JsonProperty]
        readonly string playerName;
        [JsonProperty]
        float skillValue;
        [JsonProperty] 
        DateTime lastCheck;

        public AhSkillInfo(string serverGroupId, string playerName, float skillValue, DateTime lastCheck)
        {
            this.serverGroupId = serverGroupId;
            this.playerName = playerName;
            this.skillValue = skillValue;
            this.lastCheck = lastCheck;
        }

        public string ServerGroupId => serverGroupId;

        public string PlayerName => playerName;

        public float SkillValue
        {
            get { return skillValue; }
            set { skillValue = value; }
        }

        public DateTime LastCheck
        {
            get { return lastCheck; }
            set { lastCheck = value; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AhSkillInfo)) return false;
            var other = (AhSkillInfo)obj;
            return Equals(other);
        }

        public bool Equals(AhSkillInfo other)
        {
            return serverGroupId == other.serverGroupId && playerName == other.playerName;
        }

        public override int GetHashCode()
        {
            return unchecked((playerName?.GetHashCode() ?? String.Empty.GetHashCode())*serverGroupId.GetHashCode());
        }
    }
}