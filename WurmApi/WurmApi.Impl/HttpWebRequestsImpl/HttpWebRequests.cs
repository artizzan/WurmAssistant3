using System.Net;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Impl.HttpWebRequestsImpl
{
    public class HttpWebRequests : IHttpWebRequests
    {
        public virtual async Task<HttpWebResponse> GetResponse(string serverStatsLink)
        {
            var req = (HttpWebRequest)WebRequest.Create(serverStatsLink);
            req.Timeout = 15000;
            var response = (System.Net.HttpWebResponse)await req.GetResponseAsync();
            return new HttpWebResponse(response);
        }
    }
}