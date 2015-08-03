using System;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Events.Public
{
    class PublicEventImpl : PublicEvent
    {
        readonly PublicEventInvoker publicEventInvoker;

        public PublicEventImpl([NotNull] PublicEventInvoker publicEventInvoker)
        {
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            this.publicEventInvoker = publicEventInvoker;
        }

        public override void Trigger()
        {
            publicEventInvoker.Trigger(this);
        }

        public override void Detach()
        {
            publicEventInvoker.Detach(this);
        }

        public override string ToString()
        {
            return publicEventInvoker.GetEventInfoString(this);
        }
    }
}