using System;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Events.Internal
{
    class InternalEventImpl : InternalEvent
    {
        readonly InternalEventInvoker invoker;

        public InternalEventImpl([NotNull] InternalEventInvoker invoker)
        {
            if (invoker == null) throw new ArgumentNullException("invoker");
            this.invoker = invoker;
        }

        public override void Trigger()
        {
            invoker.Trigger(this);
        }

        public override void Detach()
        {
            invoker.Detach(this);
        }
    }
}