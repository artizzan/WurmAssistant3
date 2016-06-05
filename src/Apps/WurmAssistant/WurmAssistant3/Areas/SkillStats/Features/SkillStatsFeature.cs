using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SkillStats.Parts;
using AldursLab.WurmAssistant3.Properties;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.SkillStats.Features
{
    public class SkillStatsFeature : IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly SkillStatsFeatureForm form;

        public SkillStatsFeature(IWurmApi wurmApi, ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmApi = wurmApi;
            this.logger = logger;
            form = new SkillStatsFeatureForm(this, wurmApi, logger);
        }

        #region IFeature

        void IFeature.Show()
        {
            form.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
        }

        string IFeature.Name
        {
            get { return "Skill Stats"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.Improvement; }
        }

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        void IFeature.PopulateDto(WurmAssistantDto dto)
        {
        }

        async Task IFeature.ImportDataFromWa2Async(WurmAssistantDto dto)
        {
            await Task.FromResult(true);
        }

        int IFeature.DataImportOrder
        {
            get { return 0; }
        }

        #endregion
    }
}
