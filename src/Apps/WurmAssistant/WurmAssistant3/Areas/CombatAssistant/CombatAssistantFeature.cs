using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Properties;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant
{
    [KernelBind(BindingHint.Singleton)]
    public class CombatAssistantFeature : IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly FeatureSettings featureSettings;
        readonly IProcessStarter processStarter;

        readonly CombatAssistantFeatureForm form;

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
            form = new CombatAssistantFeatureForm(wurmApi, logger, featureSettings, processStarter);
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

        #endregion
    }
}
