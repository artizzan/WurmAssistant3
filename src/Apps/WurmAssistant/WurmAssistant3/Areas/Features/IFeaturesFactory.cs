using System.Collections.Generic;

namespace AldursLab.WurmAssistant3.Areas.Features
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface IFeaturesFactory
    {
        IEnumerable<IFeature> CreateFeatures();
    }
}
