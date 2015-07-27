using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Networking;

namespace AldurSoft.WurmApi
{
    public interface IHttpWebRequests
    {
        Task<HttpWebResponse> GetResponse(string url);
    }
}