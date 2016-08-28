namespace AldursLab.WurmAssistant3.Areas.Main.ViewModels
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface IDataBackupsViewModelFactory
    {
        DataBackupsViewModel CreateDataBackupsViewModel();
    }
}