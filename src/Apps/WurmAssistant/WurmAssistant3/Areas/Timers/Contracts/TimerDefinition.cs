using System;
using AldursLab.WurmAssistant3.Areas.Timers.Parts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Timers.Contracts
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class TimerDefinition : IEquatable<TimerDefinition>
    {
        public TimerDefinition(Guid id)
        {
            Id = id;
        }

        [JsonProperty("id")]
        public Guid Id { get; private set; }

        [JsonProperty("typeId")]
        public RuntimeTypeId RuntimeTypeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("customTimerConfig"), CanBeNull]
        public CustomTimerDefinition CustomTimerConfig { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string ToVerboseString()
        {
            return string.Format("Id: {0}, RuntimeTypeId: {1}, Name: {2}", Id, RuntimeTypeId, Name);
        }

        public bool Equals(TimerDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TimerDefinition && Equals((TimerDefinition) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(TimerDefinition left, TimerDefinition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TimerDefinition left, TimerDefinition right)
        {
            return !Equals(left, right);
        }
    }
}