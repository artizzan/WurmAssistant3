using System;
using System.Net;
using System.Threading.Tasks;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Networking
{
    class HttpWebRequests : IHttpWebRequests
    {
        public async Task<HttpWebResponse> GetResponseAsync(string url)
        {
            Exception requestException = null;
            System.Net.HttpWebResponse response = null;
            try
            {
                response = await ExecuteRequest(url).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                requestException = exception;
            }
            if (requestException != null)
            {
                try
                {
                    response = await ExecuteRequest(url).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    throw new HttpWebRequestFailedException("Web request failed for url: " + url,
                        exception,
                        new[] {requestException});
                }
            }
            return new HttpWebResponse(response);
        }

        static async Task<System.Net.HttpWebResponse> ExecuteRequest(string url)
        {
            var req = (HttpWebRequest) WebRequest.Create(url);
            if (url.StartsWith("https:"))
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            }
            req.Timeout = 15000;
            var response = (System.Net.HttpWebResponse) await req.GetResponseAsync().ConfigureAwait(false);
            return response;
        }

        public HttpWebResponse GetResponse(string url)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetResponseAsync(url).Result);
        }
    }
}