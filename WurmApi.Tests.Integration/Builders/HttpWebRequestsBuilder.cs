using System;
using AldursLab.WurmApi.Modules.Networking;
using Telerik.JustMock.Helpers;

namespace AldursLab.WurmApi.Tests.Builders
{
    static class HttpWebRequestsBuilder
    {
        public static IHttpWebRequests SetupUrl(this IHttpWebRequests req, string url, Func<HttpWebResponse> response)
        {
            req.Arrange(requests => requests.GetResponseAsync(url)).Returns(async () => response());
            return req;
        }
    }
}
