using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.LogSearcher
{
    [KernelBind(BindingHint.Singleton)]
    class LogSearchFeature : IFeature
    {
        private readonly IWurmApi wurmApi;
        private readonly ILogger logger;

        private WeakReference<LogSearchForm> currentView;

        public LogSearchFeature([NotNull] IWurmApi wurmApi, [NotNull] ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmApi = wurmApi;
            this.logger = logger;
        }

        #region IFeature

        void IFeature.Show()
        {
            LogSearchForm form;
            if (currentView != null)
            {
                if (currentView.TryGetTarget(out form))
                {
                    if (!form.IsDisposed)
                    {
                        form.ShowAndBringToFront();
                        return;
                    }
                }
            }

            form = new LogSearchForm(wurmApi, logger);
            currentView = new WeakReference<LogSearchForm>(form);
            form.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
        }

        string IFeature.Name { get { return "Log Searcher"; } }

        Image IFeature.Icon { get { return Resources.LogSearcherIcon; } }

        async Task IFeature.InitAsync()
        {
            // no initialization required
            await Task.FromResult(true);
        }

        public void PopulateDto(WurmAssistantDto dto)
        {
        }

        public async Task ImportDataFromWa2Async(WurmAssistantDto dto)
        {
            await Task.FromResult(true);
        }

        public int DataImportOrder { get { return 0; } }

        #endregion

    }
}
