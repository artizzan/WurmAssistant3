using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;
using NLog;

namespace AldursLab.WurmAssistant3.Areas.Logging.Views
{
    public partial class LogView : UserControl
    {
        readonly ILogMessageFlow logMessageFlow;
        readonly ILoggingConfig loggingConfig;
        readonly IProcessStarter processStarter;
        readonly IUserNotifier userNotifier;
        readonly ISendBugReportViewFactory sendBugReportViewFactory;

        private List<string> messages = new List<string>();

        bool logChanged = false;

        public LogView([NotNull] ILogMessageFlow logMessageFlow, [NotNull] ILoggingConfig loggingConfig,
            [NotNull] IProcessStarter processStarter, [NotNull] IUserNotifier userNotifier,
            [NotNull] ISendBugReportViewFactory sendBugReportViewFactory) 
        {
            if (logMessageFlow == null)
                throw new ArgumentNullException("logMessageFlow");
            if (loggingConfig == null)
                throw new ArgumentNullException("loggingConfig");
            if (processStarter == null)
                throw new ArgumentNullException("processStarter");
            if (userNotifier == null)
                throw new ArgumentNullException("userNotifier");
            if (sendBugReportViewFactory == null) throw new ArgumentNullException("sendBugReportViewFactory");

            this.logMessageFlow = logMessageFlow;
            this.loggingConfig = loggingConfig;
            this.processStarter = processStarter;
            this.userNotifier = userNotifier;
            this.sendBugReportViewFactory = sendBugReportViewFactory;

            logMessageFlow.EventLogged += LogMessagePublisherOnEventLogged;
            logMessageFlow.ErrorCountChanged += LogMessagePublisherOnErrorCountChanged;

            InitializeComponent();

            timer.Enabled = true;
        }

        void LogMessagePublisherOnErrorCountChanged(object sender, EventArgs eventArgs)
        {
            errorCounter.Text = string.Format("{0}{1}{2}",
                "Warnings: " + logMessageFlow.WarnCount,
                Environment.NewLine,
                "Errors: " + logMessageFlow.ErrorCount);
        }

        void LogMessagePublisherOnEventLogged(object sender, LogMessageEventArgs logMessageEventArgs)
        {
            Handle(logMessageEventArgs.Level,
                logMessageEventArgs.Message,
                logMessageEventArgs.Exception,
                logMessageEventArgs.Category);
        }

        public void AddMessage(string message)
        {
            messages.Add(message);
            TrimMessages();
            logChanged = true;
        }

        public void Handle(LogLevel level, string message, Exception exception, string category)
        {
            var formattedCategory = ShortenCategory(FilterCategory(category));
            AddMessage(string.Format("{0} > {3}{1}{2}",
                level,
                message ?? "NULL",
                FormatException(exception),
                formattedCategory != string.Empty ? formattedCategory + " > " : string.Empty));
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
                logOutput.Text = string.Join(Environment.NewLine, messages);
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

        private void buyBeerBtn_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=aldurcraft%40gmail%2ecom&lc=GB&item_name=Wurm%20Assistant&item_number=funding&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted");
        }
    }
}
