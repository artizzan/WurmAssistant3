using System;
using System.Windows;
using System.Windows.Threading;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Model
{
    public class WpfGuiThreadEventMarshaller : IEventMarshaller
    {
        readonly IEnvironment environmentStatus;

        public WpfGuiThreadEventMarshaller([NotNull] IEnvironment environmentStatus)
        {
            if (environmentStatus == null) throw new ArgumentNullException("environmentStatus");
            this.environmentStatus = environmentStatus;
        }

        public void Marshal(Action action)
        {
            var currentApplication = Application.Current;
            if (currentApplication != null)
            {
                currentApplication.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);

            }
            else
            {
                if (!environmentStatus.Closing)
                {
                    throw new NullReferenceException("Application.Current is null, but the enviroment is not closing!");
                }
                // else ignore
            }
        }
    }
}
