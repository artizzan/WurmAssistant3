namespace AldursLab.WurmAssistant3.Areas.TestArea1.Contracts.Nested
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface ISampleNestedViewModelFactory
    {
        ISampleNestedViewModel CreateSampleNestedViewModel();
    }
}
