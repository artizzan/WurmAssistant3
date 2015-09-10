using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.FlatFiles;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using AldursLab.WurmAssistant3.Core.Areas.PersistentData.Model;
using AldursLab.WurmAssistant3.Core.Areas.Root.Model;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.PersistentData
{
    public class PersistentDataManager : IPersistentFactory, IPersistentLibrary
    {
        public static void BindPersistentLibrary(IKernel kernel)
        {
            var dataDir = kernel.Get<IWurmAssistantDataDirectory>();

            var persistentLibrary = new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(
                Path.Combine(dataDir.DirectoryPath, "WurmAssistantSettings")));
            kernel.Bind<PersistentCollectionsLibrary>().ToConstant(persistentLibrary);

            var timerService = kernel.Get<ITimerService>();
            var factory = new PersistentDataManager(persistentLibrary, timerService);
            kernel.Bind<IPersistentFactory>().ToConstant(factory);
        }

        readonly PersistentCollectionsLibrary persistentCollectionsLibrary;

        private PersistentDataManager(PersistentCollectionsLibrary persistentCollectionsLibrary, ITimerService timerService)
        {
            this.persistentCollectionsLibrary = persistentCollectionsLibrary;
            timerService.Updated += (sender, args) => persistentCollectionsLibrary.SaveChanged();
        }

        public IPersistent<T> Get<T>() where T : Entity, new()
        {
            return persistentCollectionsLibrary.DefaultCollection.GetObject<T>(typeof (T).FullName);
        }
        public void SavePending()
        {
            persistentCollectionsLibrary.SaveChanged();
        }

        public void SaveAll()
        {
            persistentCollectionsLibrary.SaveAll();
        }
    }
}
