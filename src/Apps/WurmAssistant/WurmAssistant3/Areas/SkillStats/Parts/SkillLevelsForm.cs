using System.Collections.Generic;
using AldursLab.WurmAssistant3.Areas.SkillStats.Data;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.SkillStats.Parts
{
    public partial class SkillLevelsForm : ExtendedForm
    {
        public SkillLevelsForm(QueryParams queryParams, IEnumerable<SkillLevelReportItem> reportItems)
        {
            InitializeComponent();
            descLbl.Text = string.Format("{0} on {1} for {2}",
                queryParams.QueryKind == QueryKind.BestSkill ? "Best skills" : "Total skills",
                queryParams.ServerGroupId,
                string.Join(", ", queryParams.GameCharacters));
            objectListView.SetObjects(reportItems);
        }
    }
}
