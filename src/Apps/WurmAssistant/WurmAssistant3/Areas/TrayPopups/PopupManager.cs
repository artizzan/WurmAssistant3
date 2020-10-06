using System;
using System.Threading;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.TrayPopups
{
    class PopupManager
    {
        readonly ILogger logger;
        readonly IWurmAssistantConfig config;
        FormPopupContainer popupContainer;
        Thread popupThread;

        ManualResetEvent mre = new ManualResetEvent(false);

        internal PopupManager([NotNull] ILogger logger, [NotNull] IWurmAssistantConfig config)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            BuildPopupThread();
        }

        void BuildPopupThread()
        {
            popupThread = new Thread(PopupThreadStart);
            popupThread.Priority = ThreadPriority.BelowNormal;
            popupThread.IsBackground = true;
            popupThread.Start();
            if (!mre.WaitOne(TimeSpan.FromSeconds(5)))
            {
                logger.Error("Timeout at ManualResetEvent.WaitOne");
            }
        }

        void PopupThreadStart()
        {
            popupContainer = new FormPopupContainer(config);
            popupContainer.Load += (sender, args) => mre.Set();
            Application.Run(popupContainer);
        }

        internal void ScheduleCustomPopupNotify(string content, string title, int timeToShowMillis = 3000)
        {
            try
            {
                popupContainer.BeginInvoke(new Action<string, string, int>(popupContainer.ScheduleCustomPopupNotify), title, content, timeToShowMillis);
            }
            catch (Exception exception)
            {
                if (exception is NullReferenceException || exception is InvalidOperationException)
                {
                    logger.Error(exception, "! Invoke exception at ScheduleCustomPopupNotify:");
                    try
                    {
                        if (exception is InvalidOperationException)
                        {
                            try
                            {
                                popupContainer.BeginInvoke(new Action(popupContainer.CloseThisContainer));
                            }
                            catch (Exception)
                            {
                                logger.Error(exception, "! Invoke exception at ScheduleCustomPopupNotify:");
                            };
                        }
                        BuildPopupThread();
                        popupContainer.BeginInvoke(new Action<string, string, int>(popupContainer.ScheduleCustomPopupNotify), title, content, timeToShowMillis);
                    }
                    catch (Exception exception2)
                    {
                        logger.Error(exception2, "! Fix failed");
                    }
                }
                else
                {
                    logger.Error(exception, "! Unknown Invoke exception at ScheduleCustomPopupNotify");
                }
            }
        }

        internal void SetDefaultTitle(string title)
        {
            try
            {
                popupContainer.BeginInvoke(
                    new Action<string>(popupContainer.SetDefaultTitle),
                    title);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "! Invoke exception at ScheduleCustomPopupNotify:");
            }
        }

        ~PopupManager()
        {
            try
            {
                popupContainer.BeginInvoke(new Action(popupContainer.CloseThisContainer));
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("PopupManager finalizer exception: " + exception);
            }
        }
    }
}
