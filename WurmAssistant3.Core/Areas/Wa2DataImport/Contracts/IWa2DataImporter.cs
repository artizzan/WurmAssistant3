using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Contracts
{
    public interface IWa2DataImporter
    {
        Task ImportFromFileAsync(string filePath);
    }
}