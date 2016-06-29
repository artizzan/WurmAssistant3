using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.SamplePlugin.CustomViews;
using AldursLab.WurmAssistant3.Areas.SamplePlugin.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.SamplePlugin.Services
{
    [KernelBind(BindingHint.Singleton)]
    public class SamplePluginFeature : IFeature
    {
        readonly SamplePluginForm mainForm;

        public SamplePluginFeature([NotNull] SamplePluginForm mainForm)
        {
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            this.mainForm = mainForm;
        }

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
