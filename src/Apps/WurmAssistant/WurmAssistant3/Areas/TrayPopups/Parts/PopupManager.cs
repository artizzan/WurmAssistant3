using System;
using System.Threading;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.TrayPopups.Parts
{
    class PopupManager
    {
        readonly ILogger logger;
        FormPopupContainer popupContainer;
        Thread popupThread;

        internal PopupManager([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
            BuildPopupThread();
        }

        void BuildPopupThread()
        {
            popupThread = new Thread(PopupThreadStart);
            popupThread.Priority = ThreadPriority.BelowNormal;
            popupThread.IsBackground = true;
            popupThread.Start();
        }

        void PopupThreadStart()
        {
            popupContainer = new FormPopupContainer();
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
