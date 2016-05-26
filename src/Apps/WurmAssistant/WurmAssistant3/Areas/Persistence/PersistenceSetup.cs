using System;
using System.IO;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.Persistence;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Persistence.Components;
using AldursLab.WurmAssistant3.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Strategies;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    public static class PersistenceSetup
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

            kernel.Bind<PersistenceManager>().ToMethod(context =>
            {
                var dataDir = kernel.Get<IWurmAssistantDataDirectory>();
                var logger = kernel.Get<ILogger>();

                var config = new PersistenceManagerConfig()
                {
                    DataStoreDirectoryPath = Path.Combine(dataDir.DirectoryPath, "Data")
                };
                var errorStrategy = new JsonExtendedErrorHandlingStrategy(logger);
                var serializationStrategy = new JsonSerializationStrategy
                {
                    ErrorStrategy = errorStrategy
                };
                var persistenceManager = new PersistenceManager(config,
                    serializationStrategy,
                    new FlatFilesPersistenceStrategy(config));

                return persistenceManager;
            }).InSingletonScope();

            kernel.Bind<PersistenceEnabler>().ToSelf().InSingletonScope();

            kernel.Bind<IPersistentObjectResolver>().To<PersistentObjectResolver>().InSingletonScope();
            kernel.Bind(typeof(IPersistentObjectResolver<>)).To(typeof(PersistentObjectResolver<>)).InSingletonScope();

            // this step is required, so that objects resolved after this step, can be properly populated.
            var persistentDataManager = kernel.Get<PersistenceEnabler>();
            persistentDataManager.SetupPersistenceActivation();
        }
    }
}
