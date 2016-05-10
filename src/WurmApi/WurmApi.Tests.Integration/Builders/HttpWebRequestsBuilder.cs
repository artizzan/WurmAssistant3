using System;
using AldursLab.WurmApi.Modules.Networking;
using Telerik.JustMock.Helpers;

namespace AldursLab.WurmApi.Tests.Integration.Builders
{
    static class HttpWebRequestsBuilder
    {
        public static IHttpWebRequests SetupUrl(this IHttpWebRequests req, string url, Func<HttpWebResponse> response)
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            // intended to be async for testing purposes
            req.Arrange(requests => requests.GetResponseAsync(url)).Returns(async () => response());
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            return req;
        }
    }
}
