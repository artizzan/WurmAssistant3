using System;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Contracts;
using AldursLab.WurmAssistantDataTransfer;

namespace AldursLab.WurmAssistant3.Areas.Wa2DataImport.Modules
{
    public class Wa2DataImporter : IWa2DataImporter
    {
        private readonly IFeaturesManager featuresManager;
        private readonly IDataTransferManager transferManager = new DataTransferManager();

        public Wa2DataImporter(IFeaturesManager featuresManager)
        {
            if (featuresManager == null) throw new ArgumentNullException("featuresManager");
            this.featuresManager = featuresManager;
        }

        public async Task ImportFromFileAsync(string filePath)
        {
            var dto = transferManager.LoadFromFile(filePath);
            foreach (var feature in featuresManager.Features.OrderBy(feature => feature.DataImportOrder).ToArray())
            {
                await feature.ImportDataFromWa2Async(dto);
            }
        }
    }
}