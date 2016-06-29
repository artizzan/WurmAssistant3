using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Features;

namespace AldursLab.WurmAssistant3.Areas.TestArea1.Services
{
    [KernelBind(BindingHint.Singleton)]
    public class SampleFeature : IFeature
    {
        public void Show()
        {
        }

        public void Hide()
        {
        }

        public string Name { get { return "SampleFeature"; } }
        public Image Icon { get; }
        public Task InitAsync()
        {
            return Task.FromResult(true);
        }
    }
}
