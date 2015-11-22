using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SkillStats.Data;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.SkillStats.Views
{
    public partial class SkillLevelsView : ExtendedForm
    {
        public SkillLevelsView(QueryParams queryParams, IEnumerable<SkillLevelReportItem> reportItems)
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
