using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Events.Internal
{
    class InternalEventImpl : InternalEvent
    {
        readonly InternalEventInvoker invoker;

        public InternalEventImpl([NotNull] InternalEventInvoker invoker)
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
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