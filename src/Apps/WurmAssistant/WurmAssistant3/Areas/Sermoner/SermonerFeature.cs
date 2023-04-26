using System;
using System.Threading.Tasks;
using System.Drawing;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Properties;
using AldursLab.WurmAssistant3.Areas.Insights;
using JetBrains.Annotations;
using ILogger = AldursLab.WurmAssistant3.Areas.Logging.ILogger;

namespace AldursLab.WurmAssistant3.Areas.Sermoner
{
    [KernelBind(BindingHint.Singleton), PersistentObject("SermonerFeature")]
    public sealed class SermonerFeature : PersistentObjectBase, IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly ITelemetry telemetry;

        public SermonerForm sermonerForm;
        readonly SermonerSettings sermonerSettings;

        public SermonerFeature(
            [NotNull] IWurmApi wurmApi,
            [NotNull] ILogger logger,
            [NotNull] SermonerSettings sermonerSettings,
            [NotNull] ITelemetry telemetry)
        {
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (telemetry == null) throw new ArgumentNullException(nameof(telemetry));
            if (sermonerSettings == null) throw new ArgumentNullException("sermonerSettings");
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.telemetry = telemetry;
            this.sermonerSettings = sermonerSettings;

            sermonerForm = new SermonerForm(logger, wurmApi, sermonerSettings);
        }

        #region IFeature

        void IFeature.Show()
        {
            if(sermonerForm == null)
            {
                sermonerForm = new SermonerForm(logger, wurmApi, sermonerSettings);
            }
            sermonerForm.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
            sermonerForm.Hide();
        }

        string IFeature.Name
        {
            get { return "Sermoner"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.Sermoner; }
        }

        async Task IFeature.InitAsync()
        {
            // no async inits required
            await Task.FromResult(true);
        }

        #endregion IFeature
    }
}
