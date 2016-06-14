using AldursLab.WurmAssistant3.Areas.TestArea1.Contracts;

namespace AldursLab.WurmAssistant3.Areas.TestArea1.Services
{
    [KernelBind(BindingHint.Singleton)]
    public class SampleSingleton : ISampleSingleton
    {
    }
}
