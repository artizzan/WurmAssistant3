using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.MainMenu
{
    [KernelBind(BindingHint.Transient)]
    public partial class OptionsForm : ExtendedForm
    {
        readonly IWurmAssistantConfig wurmAssistantConfig;

        public OptionsForm([NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            this.wurmAssistantConfig = wurmAssistantConfig;
            InitializeComponent();

            checkBoxGatherInsights.Checked = wurmAssistantConfig.AllowInsights;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            wurmAssistantConfig.AllowInsights = checkBoxGatherInsights.Checked;
            this.Close();
        }
    }
}
