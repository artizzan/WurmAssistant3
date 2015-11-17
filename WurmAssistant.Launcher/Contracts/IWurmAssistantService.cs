using System.Threading.Tasks;
using AldursLab.WurmAssistant.Shared.Dtos;

namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    public interface IWurmAssistantService
    {
        Task<string> GetLatestVersionAsync(IProgressReporter progressReporter, string buildCode);
        Task<Package[]> GetAllPackages();
    }
}