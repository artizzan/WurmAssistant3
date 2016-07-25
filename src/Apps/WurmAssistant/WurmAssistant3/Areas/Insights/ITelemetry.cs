using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Areas.Insights
{
    public interface ITelemetry
    {
        void TrackEvent(string eventName);
    }
}
