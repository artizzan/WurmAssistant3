using System.Drawing;
using System.Threading.Tasks;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.Features.Contracts
{
    public interface IFeature
    {
        void Show();

        void Hide();

        string Name { get; }

        Image Icon { get; }

        Task InitAsync();

        void PopulateDto(WurmAssistantDto dto);

        Task ImportDataFromWa2Async(WurmAssistantDto dto);

        int DataImportOrder { get; }
    }
}
