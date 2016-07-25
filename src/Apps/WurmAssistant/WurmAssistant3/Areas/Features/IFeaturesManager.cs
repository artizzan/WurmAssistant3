using System.Collections.Generic;

namespace AldursLab.WurmAssistant3.Areas.Features
{
    public interface IFeaturesManager
    {
        IEnumerable<IFeature> Features { get; }
        void InitFeaturesAsync();
    }
}
