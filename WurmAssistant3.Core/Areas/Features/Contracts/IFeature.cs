using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.Features.Contracts
{
    public interface IFeature
    {
        void Show();

        void Hide();

        string Name { get; }

        Image Icon { get; }

        Task InitAsync();

        void PopulateDto(WurmAssistantDto dto);

        void ImportFromDto(WurmAssistantDto dto);

        int DataImportOrder { get; }
    }
}
