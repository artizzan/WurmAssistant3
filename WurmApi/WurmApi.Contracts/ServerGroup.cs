using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    public abstract class ServerGroup
    {
        public abstract ServerGroupId ServerGroupId { get; }
    }

    public enum ServerGroupId
    {
        Unknown = 0,
        Freedom = 1,
        Epic = 2,
        Challenge = 3
    }
}
