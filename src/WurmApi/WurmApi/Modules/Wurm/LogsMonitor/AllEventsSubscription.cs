using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    sealed class AllEventsSubscription : IEquatable<AllEventsSubscription>
    {
        public EventHandler<LogsMonitorEventArgs> EventHandler { get; }
        public bool InternalSubscription { get; }

        public AllEventsSubscription([NotNull] EventHandler<LogsMonitorEventArgs> eventHandler, bool internalSubscription)
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
            EventHandler = eventHandler;
            InternalSubscription = internalSubscription;
        }

        public bool Equals(AllEventsSubscription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventHandler.Equals(other.EventHandler) && InternalSubscription == other.InternalSubscription;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AllEventsSubscription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EventHandler.GetHashCode()*397) ^ InternalSubscription.GetHashCode();
            }
        }

        public static bool operator ==(AllEventsSubscription left, AllEventsSubscription right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AllEventsSubscription left, AllEventsSubscription right)
        {
            return !Equals(left, right);
        }
    }
}