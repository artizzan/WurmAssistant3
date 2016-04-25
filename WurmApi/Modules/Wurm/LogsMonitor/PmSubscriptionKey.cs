using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    sealed class PmSubscriptionKey : IEquatable<PmSubscriptionKey>
    {
        public PmSubscriptionKey([NotNull] EventHandler<LogsMonitorEventArgs> eventHandler, [NotNull] string pmRecipient)
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
            if (pmRecipient == null) throw new ArgumentNullException(nameof(pmRecipient));
            EventHandler = eventHandler;
            PmRecipient = pmRecipient;
        }

        public EventHandler<LogsMonitorEventArgs> EventHandler { get; }
        public string PmRecipient { get; }

        public bool Equals(PmSubscriptionKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventHandler.Equals(other.EventHandler) && string.Equals(PmRecipient, other.PmRecipient);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is PmSubscriptionKey && Equals((PmSubscriptionKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventHandler.GetHashCode()*397) ^ PmRecipient.GetHashCode();
            }
        }

        public static bool operator ==(PmSubscriptionKey left, PmSubscriptionKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PmSubscriptionKey left, PmSubscriptionKey right)
        {
            return !Equals(left, right);
        }
    }
}