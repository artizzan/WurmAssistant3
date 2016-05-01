using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Networking
{
    public class ExtendedWebClient : WebClient
    {
        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        public int Timeout { get; private set; }

        public ExtendedWebClient() : this(30000) { }

        public ExtendedWebClient(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }
}
