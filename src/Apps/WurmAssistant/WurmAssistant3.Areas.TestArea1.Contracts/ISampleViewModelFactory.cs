namespace AldursLab.WurmAssistant3.Areas.TestArea1.Contracts
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface ISampleViewModelFactory
    {
        ISampleViewModel CreateSampleViewModel();
    }
}
