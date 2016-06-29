using AldursLab.WurmAssistant3.Areas.Main.ViewModels;

namespace AldursLab.WurmAssistant3.Areas.Main
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface INewsViewModelFactory
    {
        NewsViewModel CreateNewsViewModel();
    }
}
