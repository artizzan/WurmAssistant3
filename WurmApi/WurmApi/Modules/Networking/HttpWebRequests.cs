using System.Net;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Networking
{
    public class HttpWebRequests : IHttpWebRequests
    {
        public virtual async Task<HttpWebResponse> GetResponseAsync(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 15000;
            var response = (System.Net.HttpWebResponse)await req.GetResponseAsync().ConfigureAwait(false);
            return new HttpWebResponse(response);
        }

        public HttpWebResponse GetResponse(string url)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetResponseAsync(url).Result);
        }
    }
}