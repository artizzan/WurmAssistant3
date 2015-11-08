using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Views;

namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Contracts
{
    public interface IDataImportViewFactory
    {
        Wa2DataImportView CreateDataImportView();
    }
}