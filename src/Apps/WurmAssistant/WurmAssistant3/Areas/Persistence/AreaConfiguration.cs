using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.Persistence;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Persistence.Components;
using AldursLab.WurmAssistant3.Areas.Persistence.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            // this relies on specific Area binding order, as set in Bootstrapper.
            // todo: remove once current persistence is deprecated.

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
