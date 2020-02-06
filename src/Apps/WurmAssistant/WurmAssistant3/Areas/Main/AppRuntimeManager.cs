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
        readonly WurmUnlimitedLogsDirChecker wurmUnlimitedLogsDirChecker;

        public AppRuntimeManager(
            [NotNull] IUserNotifier userNotifier,
            [NotNull] ILogger logger,
            [NotNull] INewsViewModelFactory newsViewModelFactory,
            [NotNull] ITelemetry telemetry,
            [NotNull] IWaVersionInfoProvider waVersionInfoProvider,
            [NotNull] WurmUnlimitedLogsDirChecker wurmUnlimitedLogsDirChecker)
        {
            this.userNotifier = userNotifier ?? throw new ArgumentNullException(nameof(userNotifier));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsViewModelFactory = newsViewModelFactory ?? throw new ArgumentNullException(nameof(newsViewModelFactory));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            this.waVersionInfoProvider = waVersionInfoProvider ?? throw new ArgumentNullException(nameof(waVersionInfoProvider));
            this.wurmUnlimitedLogsDirChecker = wurmUnlimitedLogsDirChecker ?? throw new ArgumentNullException(nameof(wurmUnlimitedLogsDirChecker));
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

            try
            {
                wurmUnlimitedLogsDirChecker.HandleOldLogsDirContents();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error at HandleOldLogsDirContents");
                userNotifier.NotifyWithMessageBox("Error at HandleOldLogsDirContents, see logs for details.", NotifyKind.Warning);
            }

            var version = waVersionInfoProvider.Get();
            telemetry.TrackEvent($"Started: " + version);
        }
    }
}
