using System;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Insights;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main
{
    [KernelBind(BindingHint.Singleton)]
    public class AppRuntimeManager
    {
        readonly IUserNotifier userNotifier;
        readonly ILogger logger;
        readonly INewsViewModelFactory newsViewModelFactory;
        readonly ITelemetry telemetry;
        readonly IWaVersionInfoProvider waVersionInfoProvider;

        public AppRuntimeManager(
            [NotNull] IUserNotifier userNotifier,
            [NotNull] ILogger logger,
            [NotNull] INewsViewModelFactory newsViewModelFactory,
            [NotNull] ITelemetry telemetry,
            [NotNull] IWaVersionInfoProvider waVersionInfoProvider)
        {
            if (userNotifier == null) throw new ArgumentNullException(nameof(userNotifier));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (newsViewModelFactory == null) throw new ArgumentNullException(nameof(newsViewModelFactory));
            if (telemetry == null) throw new ArgumentNullException(nameof(telemetry));
            if (waVersionInfoProvider == null) throw new ArgumentNullException(nameof(waVersionInfoProvider));
            this.userNotifier = userNotifier;
            this.logger = logger;
            this.newsViewModelFactory = newsViewModelFactory;
            this.telemetry = telemetry;
            this.waVersionInfoProvider = waVersionInfoProvider;
        }

        public void ExecuteAfterStartupSteps()
        {
            try
            {
                var vm = newsViewModelFactory.CreateNewsViewModel();
                vm.ShowIfAnyUnshownNews();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error at showing news");
                userNotifier.NotifyWithMessageBox("Error at showing news, see logs for details.", NotifyKind.Warning);
            }

            var version = waVersionInfoProvider.Get();
            telemetry.TrackEvent($"Started: " + version);
        }
    }
}
