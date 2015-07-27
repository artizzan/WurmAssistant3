using System.Net;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Modules.Networking
{
    public class HttpWebRequests : IHttpWebRequests
    {
        public virtual async Task<HttpWebResponse> GetResponse(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 15000;
            var response = (System.Net.HttpWebResponse)await req.GetResponseAsync();
            return new HttpWebResponse(response);
        }
    }
}