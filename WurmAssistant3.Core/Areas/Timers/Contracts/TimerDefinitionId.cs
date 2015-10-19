using System;
using AldursLab.WurmApi;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimerDefinitionId : IEquatable<TimerDefinitionId>
    {
        public TimerDefinitionId([NotNull] string name, ServerGroupId serverGroupId)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            ServerGroupId = serverGroupId;
        }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("serverGroupId")]
        public ServerGroupId ServerGroupId { get; private set; }

        public bool Equals(TimerDefinitionId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Equals(Name, (string) other.Name) && ServerGroupId == other.ServerGroupId;
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
                return (Name.GetHashCode()*397) ^ (int) ServerGroupId;
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
            return Name + "-" + (int)ServerGroupId;
        }
    }
}