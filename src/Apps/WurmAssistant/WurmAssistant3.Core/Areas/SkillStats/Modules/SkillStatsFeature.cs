using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SkillStats.Data;
using AldursLab.WurmAssistant3.Core.Areas.SkillStats.Views;
using AldursLab.WurmAssistant3.Core.Properties;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.SkillStats.Modules
{
    public class SkillStatsFeature : IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly SkillStatsFeatureView view;

        public SkillStatsFeature(IWurmApi wurmApi, ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmApi = wurmApi;
            this.logger = logger;
            view = new SkillStatsFeatureView(this, wurmApi, logger);
        }

        #region IFeature

        void IFeature.Show()
        {
            view.ShowAndBringToFront();
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
