using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssist.Views
{
    public partial class CombatAssistFeatureView : ExtendedForm
    {
        readonly IWurmApi wurmApi;

        public CombatAssistFeatureView(IWurmApi wurmApi)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.wurmApi = wurmApi;

            InitializeComponent();

            wurmCharacterCbox.Items.AddRange(
                wurmApi.Characters.All.Select(character => character.Name.Capitalized).Cast<object>().ToArray());
        }

        private void CombatAssistFeatureView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void createLiveSessionBtn_Click(object sender, EventArgs e)
        {

        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            logEntriesBlock.Text = "";
        }

        private void parseBtn_Click(object sender, EventArgs e)
        {
            string entriesBlock = logEntriesBlock.Text.Trim();
            //var parsedEvents = 
        }
    }
}
