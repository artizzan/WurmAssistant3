namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    [KernelBind(BindingHint.FactoryProxy)]
    interface IExporterFactory
    {
        Importer CreateExporter();
    }
}