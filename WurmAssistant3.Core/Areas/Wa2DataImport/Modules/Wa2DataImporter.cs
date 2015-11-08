using System;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Contracts;
using AldursLab.WurmAssistantDataTransfer;

namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules
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

        public void ImportFromFile(string filePath)
        {
            var dto = transferManager.LoadFromFile(filePath);
            foreach (var feature in featuresManager.Features.OrderBy(feature => feature.DataImportOrder).ToArray())
            {
                feature.ImportFromDto(dto);
            }
        }
    }
}