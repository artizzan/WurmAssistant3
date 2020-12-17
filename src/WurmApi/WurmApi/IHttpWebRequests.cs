using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Networking;

namespace AldursLab.WurmApi
{
    public interface IHttpWebRequests
    {
        Task<HttpWebResponse> GetResponseAsync(string url);
        HttpWebResponse GetResponse(string url);
    }
}