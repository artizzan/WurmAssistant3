using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Networking
{
    public interface IHttpWebRequests
    {
        Task<HttpWebResponse> GetResponse(string url);
    }
}