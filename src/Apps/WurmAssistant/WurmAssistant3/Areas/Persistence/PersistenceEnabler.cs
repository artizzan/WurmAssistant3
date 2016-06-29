using System;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Core;
using JetBrains.Annotations;
using Ninject.Activation;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    class PersistenceEnabler : IDisposable
    {
        readonly ITimer updateTimer;
        readonly PersistenceManager persistenceManager;
        readonly IKernelConfig kernelConfig;
        readonly object locker = new object();

        public PersistenceEnabler([NotNull] PersistenceManager persistenceManager, [NotNull] ITimerFactory timerFactory,
            [NotNull] IKernelConfig kernelConfig)
        {
            if (persistenceManager == null) throw new ArgumentNullException(nameof(persistenceManager));
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));
            if (kernelConfig == null) throw new ArgumentNullException(nameof(kernelConfig));
            this.updateTimer = timerFactory.CreateUiThreadTimer();
            this.updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            this.updateTimer.Tick += UpdateLoopOnUpdated;

            this.persistenceManager = persistenceManager;
            this.kernelConfig = kernelConfig;

            this.updateTimer.Start();
        }

        void UpdateLoopOnUpdated(object sender, EventArgs eventArgs)
        {
            persistenceManager.SavePending();
        }

        public void SetupPersistenceActivation()
        {
            kernelConfig.AddPreInitializeActivations(Action, null);
        }

        void Action(IContext context, InstanceReference instanceReference)
        {
            lock (locker)
            {
                var @object = instanceReference.Instance as IPersistentObject;
                if (@object != null)
                {
                    @object = persistenceManager.LoadAndStartTracking(@object, returnExistingInsteadOfException: true);
                    instanceReference.Instance = @object;
                }
            }
        }

        public void Dispose()
        {
            updateTimer.Stop();
            // for debugging save only changed, so issues can be spotted.
#if DEBUG
            persistenceManager.SavePending();
#else
            persistenceManager.SaveAll();
#endif
        }
    }
}
