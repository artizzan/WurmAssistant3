using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Contracts;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Modules;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Views;
using Ninject;
using Ninject.Extensions.Factory;

namespace AldursLab.WurmAssistant3.Areas.Wa2DataImport
{
    public static class Wa2DataImportSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IWa2DataImporter>().To<Wa2DataImporter>();
            kernel.Bind<Wa2DataImportView>().ToSelf();
            kernel.Bind<IDataImportViewFactory>().ToFactory();
        }
    }
}
