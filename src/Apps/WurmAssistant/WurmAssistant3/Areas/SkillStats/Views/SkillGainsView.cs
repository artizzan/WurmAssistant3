using System.Collections.Generic;
using AldursLab.WurmAssistant3.Areas.SkillStats.Data;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.SkillStats.Views
{
    public partial class SkillGainsView : ExtendedForm
    {
        public SkillGainsView(QueryParams queryParams, IEnumerable<SkillGainReportItem> reportItems)
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
