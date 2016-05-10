using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Messages.Lifecycle;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Components.Singletons
{
    class UpdateLoop : IUpdateLoop, IHandle<AppBootstrapped>, IHandle<AppClosing>
    {
        readonly IHostEnvironment hostEnvironment;
        readonly IEventBus eventBus;
        readonly DispatcherTimer timer;
        public event EventHandler<EventArgs> Updated;

        public UpdateLoop([NotNull] IHostEnvironment hostEnvironment, [NotNull] IEventBus eventBus)
        {
            if (hostEnvironment == null) throw new ArgumentNullException(nameof(hostEnvironment));
            if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
            this.hostEnvironment = hostEnvironment;
            this.eventBus = eventBus;
            eventBus.Subscribe(this);
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += (sender, args) => OnUpdated();
        }

        public void Start()
        {
            timer.Start();
        }

        protected virtual void OnUpdated()
        {
            if (!hostEnvironment.Closing)
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public void Handle(AppBootstrapped message)
        {
            timer.Start();
        }

        public void Handle(AppClosing message)
        {
            timer.Stop();
        }
    }
}
