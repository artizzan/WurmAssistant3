using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    public interface IThreadGuard
    {
        /// <summary>
        /// Ensure, that current thread is the same, as the 
        /// thread that created this ThreadGuard instance.
        /// </summary>
        void ValidateCurrentThread();
    }
}
