using System.Collections.Generic;

namespace AldursLab.WurmAssistant3.Areas.Features.Contracts
{
    public interface IFeaturesManager
    {
        IEnumerable<IFeature> Features { get; }
        void InitFeaturesAsync();
    }
}
