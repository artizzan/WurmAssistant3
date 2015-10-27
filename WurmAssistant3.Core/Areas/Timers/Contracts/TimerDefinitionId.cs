using System;
using AldursLab.WurmApi;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimerDefinitionId : IEquatable<TimerDefinitionId>
    {
        public TimerDefinitionId([NotNull] string name, [NotNull] string serverGroupId)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (serverGroupId == null) throw new ArgumentNullException("serverGroupId");
            Name = name;
            ServerGroupId = serverGroupId;
        }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("serverGroupId")]
        public string ServerGroupId { get; private set; }

        public override string ToString()
        {
            return Name + " (" + ServerGroupId.ToString() + ")";
        }

        public string ToCompactString()
        {
            return string.Format("{0} ({1})", Name, ServerGroupId.ToString().Substring(0, 1));
        }

        public string ToPersistentIdString()
        {
            return Name + "-" + ServerGroupId;
        }

        public bool Equals(TimerDefinitionId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(ServerGroupId, other.ServerGroupId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TimerDefinitionId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (ServerGroupId != null ? ServerGroupId.GetHashCode() : 0);
            }
        }

        public static bool operator ==(TimerDefinitionId left, TimerDefinitionId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TimerDefinitionId left, TimerDefinitionId right)
        {
            return !Equals(left, right);
        }
    }
}