using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.Views;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Properties;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant.Modules
{
    public class CombatAssistantFeature : IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly FeatureSettings featureSettings;
        readonly IProcessStarter processStarter;

        readonly CombatAssistantFeatureView view;

        public CombatAssistantFeature(IWurmApi wurmApi, ILogger logger, FeatureSettings featureSettings,
            IProcessStarter processStarter)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (featureSettings == null) throw new ArgumentNullException("featureSettings");
            if (processStarter == null) throw new ArgumentNullException("processStarter");
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.featureSettings = featureSettings;
            this.processStarter = processStarter;
            view = new CombatAssistantFeatureView(wurmApi, logger, featureSettings, processStarter);
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
            get { return "Combat Assistant"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.Combat; }
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
