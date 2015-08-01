using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers.Jobs
{
    abstract class Job
    {
        protected Job(ServerName serverName)
        {
            ServerName = serverName;
        }

        public ServerName ServerName { get; private set; }
    }
}
