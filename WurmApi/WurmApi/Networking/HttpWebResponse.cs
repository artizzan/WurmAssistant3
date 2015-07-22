using System;
using System.IO;

namespace AldurSoft.WurmApi.Networking
{
    public class HttpWebResponse
    {
        private readonly System.Net.HttpWebResponse webResponse;

        public HttpWebResponse(System.Net.HttpWebResponse webResponse)
        {
            if (webResponse == null)
            {
                throw new ArgumentNullException("webResponse");
            }
            this.webResponse = webResponse;
        }

        public virtual Stream GetResponseStream()
        {
            return this.webResponse.GetResponseStream();
        }

        public virtual DateTime LastModified
        {
            get { return this.webResponse.LastModified; }
        }
    }
}