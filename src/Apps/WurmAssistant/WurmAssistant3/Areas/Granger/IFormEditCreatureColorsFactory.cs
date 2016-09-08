namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface IFormEditCreatureColorsFactory
    {
        FormEditCreatureColors CreateFormEditCreatureColors();
    }
}