using System;
using System.IO;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.Persistence;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Persistence.Components;
using AldursLab.WurmAssistant3.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.IoC;
using AldursLab.WurmAssistant3.Root.Contracts;
using JetBrains.Annotations;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Strategies;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    public class PersistenceSetup
    {
        public static void BindPersistenceSystems(IKernel kernel)
        {
            // Special way of persisting data is used in this app.
            // All dependencies may implement IPersistentObject (or inherit from PersistentObjectBase).
            // This method sets up Ninject to wire any such object.
            // These objects members will be automatically persisted and then loaded on next app start,
            // based on their serialization config (for example, fields marked as [JsonProperty]).

            // Note: any object created with new() operator (as opposed to through kernel resolution) 
            // will not be wired automatically.

            // To pass specific persistent object id at runtime and still use kernel persistence activation,
            // use IPersistentObjectProvider<T>. 
            // Resolved class must have a parameter of type string named exactly: persistentObjectId.

            // Data saving relies on application events, wired via PersistentDataManager.

            var dataDir = kernel.Get<IWurmAssistantDataDirectory>();
            var logger = kernel.Get<ILogger>();
            var host = kernel.Get<IHostEnvironment>();
            var updateLoop = kernel.Get<IUpdateLoop>();

            var config = new PersistenceManagerConfig()
            {
                DataStoreDirectoryPath = Path.Combine(dataDir.DirectoryPath, "Data")
            };
            var errorStrategy = new JsonExtendedErrorHandlingStrategy(logger, host);
            var serializationStrategy = new JsonSerializationStrategy
            {
                ErrorStrategy = errorStrategy
            };
            var persistenceManager = new PersistenceManager(config,
                serializationStrategy,
                new FlatFilesPersistenceStrategy(config));

            kernel.Bind<PersistenceManager>().ToConstant(persistenceManager);

            PreInitializeActionsStrategy str =
                (PreInitializeActionsStrategy)
                    kernel.Components
                          .GetAll<IActivationStrategy>().Single(strategy => strategy is PreInitializeActionsStrategy);

            var persistentDataManager = new PersistenceSetup(persistenceManager,
                host, updateLoop);
            persistentDataManager.SetupPersistenceActivation(str);

            kernel.Bind<PersistenceSetup>().ToConstant(persistentDataManager);

            kernel.Bind<IPersistentObjectResolver>().To<PersistentObjectResolver>().InSingletonScope();
            kernel.Bind(typeof (IPersistentObjectResolver<>)).To(typeof (PersistentObjectResolver<>)).InSingletonScope();
        }

        readonly PersistenceManager persistenceManager;
        readonly object locker = new object();

        PersistenceSetup([NotNull] PersistenceManager persistenceManager, IHostEnvironment hostEnvironment,
            IUpdateLoop updateLoop)
        {
            if (persistenceManager == null)
                throw new ArgumentNullException("persistenceManager");
            this.persistenceManager = persistenceManager;
            updateLoop.Updated += UpdateLoopOnUpdated;
            hostEnvironment.LateHostClosing += HostEnvironmentOnHostClosing;
        }

        void UpdateLoopOnUpdated(object sender, EventArgs eventArgs)
        {
            persistenceManager.SavePending();
        }

        void HostEnvironmentOnHostClosing(object sender, EventArgs eventArgs)
        {
            // for debugging save only changed, so issues can be spotted.
#if DEBUG
            persistenceManager.SavePending();
#else
            persistenceManager.SaveAll();
#endif
        }

        void SetupPersistenceActivation(PreInitializeActionsStrategy str)
        {
            str.AddActivationAction(Action);
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
    }
}
