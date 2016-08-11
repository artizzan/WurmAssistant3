namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface IImporterFactory
    {
        Importer CreateImporter();
    }
}