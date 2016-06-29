using System;
using AldursLab.WurmAssistant3.Areas.Core;
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

        public AppRuntimeManager(
            [NotNull] IUserNotifier userNotifier,
            [NotNull] ILogger logger,
            [NotNull] INewsViewModelFactory newsViewModelFactory)
        {
            if (userNotifier == null) throw new ArgumentNullException(nameof(userNotifier));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (newsViewModelFactory == null) throw new ArgumentNullException(nameof(newsViewModelFactory));
            this.userNotifier = userNotifier;
            this.logger = logger;
            this.newsViewModelFactory = newsViewModelFactory;
            
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
        }
    }
}
