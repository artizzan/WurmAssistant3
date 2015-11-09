using System;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules
{
    class GrangerWa2Importer
    {
        private readonly GrangerFeature grangerFeature;

        public GrangerWa2Importer(GrangerFeature grangerFeature)
        {
            if (grangerFeature == null) throw new ArgumentNullException("grangerFeature");
            this.grangerFeature = grangerFeature;
        }

        public void ImportFromDto(WurmAssistantDto dto)
        {
        }
    }
}