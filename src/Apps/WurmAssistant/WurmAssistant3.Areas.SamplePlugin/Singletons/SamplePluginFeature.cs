using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.SamplePlugin.CustomViews;
using AldursLab.WurmAssistant3.Areas.SamplePlugin.Properties;

namespace AldursLab.WurmAssistant3.Areas.SamplePlugin.Singletons
{
    public class SamplePluginFeature : IFeature
    {
        readonly SamplePluginForm mainForm = new SamplePluginForm();

        public void Show()
        {
            mainForm.ShowAndBringToFront();
        }

        public void Hide()
        {
            mainForm.Hide();
        }

        public string Name => "Sample Plugin";
        public Image Icon => Resources.smile1_md;

        public Task InitAsync()
        {
            return Task.FromResult(true);
        }
    }
}
