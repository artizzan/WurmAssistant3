namespace AldursLab.WurmAssistant3.Core.Infrastructure.Modules
{
    public interface IModule
    {
        void ShowGui();

        string Name { get; }
    }
}