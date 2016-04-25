using System;
using System.IO;

namespace AldursLab.WurmApi.Modules.Networking
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    class HttpWebResponse
    {
        private readonly System.Net.HttpWebResponse webResponse;

        public HttpWebResponse(System.Net.HttpWebResponse webResponse)
        {
            if (webResponse == null)
            {
                throw new ArgumentNullException(nameof(webResponse));
            }
            this.webResponse = webResponse;
        }

        public virtual Stream GetResponseStream()
        {
            return webResponse.GetResponseStream();
        }

        public virtual DateTime LastModified => webResponse.LastModified;
    }
}