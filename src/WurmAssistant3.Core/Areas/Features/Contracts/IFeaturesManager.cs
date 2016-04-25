using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Areas.Features.Contracts
{
    public interface IFeaturesManager
    {
        IEnumerable<IFeature> Features { get; }
        void InitFeatures();
    }
}
