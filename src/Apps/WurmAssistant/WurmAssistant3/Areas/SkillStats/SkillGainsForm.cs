using System.Collections.Generic;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.SkillStats
{
    public partial class SkillGainsForm : ExtendedForm
    {
        public SkillGainsForm(QueryParams queryParams, IEnumerable<SkillGainReportItem> reportItems)
        {
            InitializeComponent();
            descLbl.Text = string.Format("From {0} to {1} on {2} for {3}",
                queryParams.From,
                queryParams.To,
                queryParams.ServerGroupId,
                string.Join(", ", queryParams.GameCharacters));
            objectListView.SetObjects(reportItems);
        }
    }
}
