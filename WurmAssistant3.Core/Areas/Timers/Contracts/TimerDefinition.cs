using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class TimerDefinition : IEquatable<TimerDefinition>
    {
        // note: equality is still limited to TimerDefinitionId, due to legacy code

        [JsonProperty("typeId")]
        public RuntimeTypeId RuntimeTypeId { get; private set; }

        [JsonProperty("timerDefinitionId")]
        public TimerDefinitionId TimerDefinitionId { get; private set; }

        public TimerDefinition([NotNull] TimerDefinitionId timerDefinitionId, RuntimeTypeId runtimeTypeId)
        {
            if (timerDefinitionId == null) throw new ArgumentNullException("timerDefinitionId");
            RuntimeTypeId = runtimeTypeId;
            TimerDefinitionId = timerDefinitionId;
        }

        public override string ToString()
        {
            return TimerDefinitionId.ToString();
        }

        public string ToCompactString()
        {
            return TimerDefinitionId.ToCompactString();
        }

        public object ToDebugString()
        {
            return TimerDefinitionId.ToString() + ", Runtime Type Id " + RuntimeTypeId;
        }

        public string ToPersistentIdString()
        {
            return TimerDefinitionId.ToPersistentIdString();
        }

        public bool Equals(TimerDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TimerDefinitionId.Equals(other.TimerDefinitionId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TimerDefinition && Equals((TimerDefinition) obj);
        }

        public override int GetHashCode()
        {
            return TimerDefinitionId.GetHashCode();
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