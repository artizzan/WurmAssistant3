using System;
using System.IO;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.Persistence;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Components;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Core.IoC;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Strategies;

namespace AldursLab.WurmAssistant3.Core.Areas.Persistence
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

            var config = new PersistenceManagerConfig()
            {
                DataStoreDirectoryPath = Path.Combine(dataDir.DirectoryPath, "Data")
            };
            var persistenceManager = new PersistenceManager(config,
                new JsonSerializationStrategy(),
                new FlatFilesPersistenceStrategy(config));

            kernel.Bind<PersistenceManager>().ToConstant(persistenceManager);

            PreInitializeActionsStrategy str =
                (PreInitializeActionsStrategy)
                    kernel.Components
                          .GetAll<IActivationStrategy>().Single(strategy => strategy is PreInitializeActionsStrategy);

            var persistentDataManager = new PersistenceSetup(persistenceManager,
                kernel.Get<IHostEnvironment>(),
                kernel.Get<IUpdateLoop>());
            persistentDataManager.SetupPersistenceActivation(str);

            kernel.Bind<PersistenceSetup>().ToConstant(persistentDataManager);

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
            updateLoop.Updated += (sender, args) => persistenceManager.SavePending();
            hostEnvironment.HostClosing += (sender, args) => persistenceManager.SaveAll();
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
                    persistenceManager.LoadAndStartTracking(@object);
                }
            }
        }
    }
}
