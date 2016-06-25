using System;
using System.Diagnostics;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Parts.TriggersManager
{
    public partial class RegexTriggerConfig : UserControl, ITriggerConfig
    {
        private readonly RegexTrigger regexTrigger;
        readonly ILogger logger;

        private bool initComplete = false;
        public RegexTriggerConfig(RegexTrigger regexTrigger, [NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.regexTrigger = regexTrigger;
            this.logger = logger;
            InitializeComponent();
            initComplete = true;
        }

        public UserControl ControlHandle { get { return this; } }

        private void WhatsRegularExprLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(@"http://searchsoftwarequality.techtarget.com/definition/regular-expression");
            }
            catch (Exception exception)
            {
                logger.Error(exception, "");
            }
        }

        private void RegexGuideLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(@"http://www.codeproject.com/Articles/9099/The-30-Minute-Regex-Tutorial");
            }
            catch (Exception exception)
            {
                logger.Error(exception, "");
            }
        }
    }
}
