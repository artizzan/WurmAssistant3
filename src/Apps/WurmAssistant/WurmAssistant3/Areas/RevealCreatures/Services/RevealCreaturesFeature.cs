using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.RevealCreatures.Parts;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.RevealCreatures.Services
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class RevealCreaturesFeature : IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly RevealCreaturesForm form;

        public RevealCreaturesFeature(IWurmApi wurmApi, ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmApi = wurmApi;
            this.logger = logger;

            form = new RevealCreaturesForm(wurmApi, logger);
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
            get { return "Reveal Creatures Parser"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.Radar; }
        }

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        #endregion
    }
}
