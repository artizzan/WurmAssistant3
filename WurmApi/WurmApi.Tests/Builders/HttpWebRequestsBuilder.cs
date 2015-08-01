using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Networking;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.Builders
{
    public static class HttpWebRequestsBuilder
    {
        public static IHttpWebRequests SetupUrl(this IHttpWebRequests req, string url, Func<HttpWebResponse> response)
        {
            req.Arrange(requests => requests.GetResponseAsync(url)).Returns(async () => response());
            return req;
        }
    }
}
