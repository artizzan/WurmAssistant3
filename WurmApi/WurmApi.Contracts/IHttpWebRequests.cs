using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    public interface IHttpWebRequests
    {
        Task<HttpWebResponse> GetResponse(string serverStatsLink);
    }
}