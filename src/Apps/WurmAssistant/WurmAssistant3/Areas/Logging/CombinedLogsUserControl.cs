using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Core;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    [KernelBind]
    public partial class CombinedLogsUserControl : UserControl
    {
        readonly ILogMessageSteam logMessageSteam;
        readonly ILoggingConfig loggingConfig;
        readonly IProcessStarter processStarter;
        readonly IUserNotifier userNotifier;
        readonly ISendBugReportViewFactory sendBugReportViewFactory;

        private List<string> messages = new List<string>();

        bool logChanged = false;

        public CombinedLogsUserControl(
            [NotNull] ILogMessageSteam logMessageSteam, 
            [NotNull] ILoggingConfig loggingConfig,
            [NotNull] IProcessStarter processStarter, 
            [NotNull] IUserNotifier userNotifier,
            [NotNull] ISendBugReportViewFactory sendBugReportViewFactory) 
        {
            if (logMessageSteam == null) throw new ArgumentNullException(nameof(logMessageSteam));
            if (loggingConfig == null) throw new ArgumentNullException(nameof(loggingConfig));
            if (processStarter == null) throw new ArgumentNullException(nameof(processStarter));
            if (userNotifier == null) throw new ArgumentNullException(nameof(userNotifier));
            if (sendBugReportViewFactory == null) throw new ArgumentNullException(nameof(sendBugReportViewFactory));

            this.logMessageSteam = logMessageSteam;
            this.loggingConfig = loggingConfig;
            this.processStarter = processStarter;
            this.userNotifier = userNotifier;
            this.sendBugReportViewFactory = sendBugReportViewFactory;

            InitializeComponent();

            logMessageSteam.EventLogged += LogMessagePublisherOnEventLogged;
            logMessageSteam.ErrorCountChanged += LogMessagePublisherOnErrorCountChanged;

            var missedEvents = logMessageSteam.ConsumeMissedMessages();
            foreach (var missedEvent in missedEvents)
            {
                LogMessagePublisherOnEventLogged(logMessageSteam, missedEvent);
            }
            
            timer.Enabled = true;

            RecalculateErrors();
        }

        void LogMessagePublisherOnErrorCountChanged(object sender, EventArgs eventArgs)
        {
            RecalculateErrors();
        }

        void RecalculateErrors()
        {
            errorCounter.Text = string.Format("{0}{1}{2}",
                "Warnings: " + logMessageSteam.WarnCount,
                Environment.NewLine,
                "Errors: " + logMessageSteam.ErrorCount);
        }

        void LogMessagePublisherOnEventLogged(object sender, LogMessageEventArgs args)
        {
            var formattedCategory = ShortenCategory(FilterCategory(args.Category));
            AddMessage(string.Format("[{4}] > {0} > {3}{1}{2}",
                args.Level,
                args.Message ?? "NULL",
                FormatException(args.Exception),
                formattedCategory != string.Empty ? formattedCategory + " > " : string.Empty, args.Timestamp.ToString("HH:mm:ss")));
        }

        public void AddMessage(string message)
        {
            messages.Add(message);
            TrimMessages();
            logChanged = true;
        }

        string FilterCategory(string category)
        {
            if (category == "WurmApi")
            {
                return string.Empty;
            }
            return category;
        }

        string ShortenCategory(string category)
        {
            if (string.IsNullOrEmpty(category)) return string.Empty;

            var lastDotIndex = category.LastIndexOf('.');
            
            if (lastDotIndex > -1)
            {
                var maxIndex = category.Length - 1;
                if (lastDotIndex < maxIndex)
                {
                    var substringLength = maxIndex - lastDotIndex;
                    return category.Substring(lastDotIndex + 1, substringLength);
                }
            }
            
            return category;
        }

        private string FormatException(Exception exception)
        {
            string result = string.Empty;
            if (exception != null)
            {
                result += " > " + exception.Message;
                if (exception.InnerException != null)
                {
                    result += " > " + exception.InnerException.Message;
                }
            }
            return result;
        }

        void TrimMessages()
        {
            const int maxMessagesInDisplay = 1000;
            const int messagesToKeep = 900;

            if (messages.Count > maxMessagesInDisplay)
            {
                var lastIndex = messages.Count - 1;
                var oldestMessageToKeepIndex = lastIndex - messagesToKeep;
                messages = messages.GetRange(oldestMessageToKeepIndex, messagesToKeep).ToList();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (logChanged)
            {
                var str = string.Join(Environment.NewLine, messages);
                logOutput.Text = str;
                logChanged = false;
                logOutput.SelectionStart = logOutput.Text.Length - 1;
                logOutput.SelectionLength = 0;
                logOutput.ScrollToCaret();
            }
        }

        private void showAppLog_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(loggingConfig.GetCurrentReadableLogFileFullPath());
        }

        private void shopAppLogDetailed_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(loggingConfig.GetCurrentVerboseLogFileFullPath());
        }

        private void reportBug_Click(object sender, EventArgs e)
        {
            var view = sendBugReportViewFactory.CreateSendBugReportView();
            var parentForm = FindForm();
            if (parentForm != null)
            {
                view.ShowCenteredOnForm(parentForm);
            }
            else
            {
                view.Show();
            }
        }

        private void openLogsFolderBtn_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(loggingConfig.LogsDirectoryFullPath);
        }
    }
}
