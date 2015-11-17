using System.Threading.Tasks;
using AldursLab.WurmAssistant.Launcher.Modules;

namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    public interface IUpdateService : IWurmAssistantService
    {
        Task<IStagedPackage> GetPackageAsync(IProgressReporter progressReporter, string buildCode, string buildNumber);
    }
}
