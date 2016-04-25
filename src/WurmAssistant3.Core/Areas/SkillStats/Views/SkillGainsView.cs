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
