using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatAssist.Views;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Properties;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssist.Modules
{
    public class CombatAssistFeature : IFeature
    {
        readonly IWurmApi wurmApi;
        readonly CombatAssistFeatureView view;

        public CombatAssistFeature(IWurmApi wurmApi)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.wurmApi = wurmApi;
            view = new CombatAssistFeatureView(wurmApi);
        }

        #region IFeature

        void IFeature.Show()
        {
            view.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
        }

        string IFeature.Name
        {
            get { return "Combat Assistant"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.Improvement; }
        }

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        void IFeature.PopulateDto(WurmAssistantDto dto)
        {
        }

        async Task IFeature.ImportDataFromWa2Async(WurmAssistantDto dto)
        {
            await Task.FromResult(true);
        }

        int IFeature.DataImportOrder
        {
            get { return 0; }
        }

        #endregion
    }
}
