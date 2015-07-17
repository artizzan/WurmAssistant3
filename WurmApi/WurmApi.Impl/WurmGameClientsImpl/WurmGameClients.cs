using System;
using System.Diagnostics;
using System.Linq;

namespace AldurSoft.WurmApi.Impl.WurmGameClientsImpl
{
    public class WurmGameClients : IWurmGameClients
    {
        // they changed something and the usual way of detecting this is gone.

        //public virtual bool AnyRunning
        //{
        //    get
        //    {
        //        Process[] allActiveProcesses = Process.GetProcessesByName("javaw");
        //        return
        //            allActiveProcesses.Any(
        //                process => process.MainWindowTitle.StartsWith("Wurm Online", StringComparison.Ordinal));
        //    }
        //}
    }
}