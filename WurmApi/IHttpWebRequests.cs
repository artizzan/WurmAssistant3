using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Networking;

namespace AldursLab.WurmApi
{
    interface IHttpWebRequests
    {
        Task<HttpWebResponse> GetResponseAsync(string url);
        HttpWebResponse GetResponse(string url);
    }
}