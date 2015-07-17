using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Object must be periodically refreshed to keep its internal data in sync.
    /// </summary>
    public interface IRequireRefresh
    {
        void Refresh();
    }
}
