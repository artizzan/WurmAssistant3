using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.CraftingAssistant.Parts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.CraftingAssistant.Services
{
    [KernelHint(BindingHint.Singleton)]
    public partial class CraftingAssistantForm : ExtendedForm
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        public CraftingAssistantForm(IWurmApi wurmApi, ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmApi = wurmApi;
            this.logger = logger;
            InitializeComponent();

            gameCharactersCmb.Items.AddRange(
                wurmApi.Characters.All.Select(character => character.Name.Capitalized).Cast<object>().ToArray());
        }

        private void createWidgetBtn_Click(object sender, EventArgs e)
        {
            var character = gameCharactersCmb.Text;
            if (!string.IsNullOrWhiteSpace(character))
            {
                try
                {
                    var widget = new Widget(character, wurmApi, logger);
                    widget.ShowCenteredOnForm(this);
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Widget creation failed");
                }
            }
        }

        private void CraftingAssistantView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }
    }
}
