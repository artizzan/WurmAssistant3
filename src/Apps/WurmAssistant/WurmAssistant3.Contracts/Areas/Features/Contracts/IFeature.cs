using System.Drawing;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Areas.Features.Contracts
{
    public interface IFeature
    {
        void Show();

        void Hide();

        string Name { get; }

        Image Icon { get; }

        Task InitAsync();
    }
}
