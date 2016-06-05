using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Parts;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Transients;

namespace AldursLab.WurmAssistant3.Areas.Wa2DataImport.Contracts
{
    [NinjectFactory]
    public interface IDataImportFormFactory
    {
        Wa2DataImportForm CreateDataImportForm();
    }
}