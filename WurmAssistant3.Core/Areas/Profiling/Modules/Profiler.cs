using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Areas.Profiling.Modules
{
    public static class Profiler
    {
        [Conditional("DEBUG")]
        public static void Start(string id)
        {
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss:fff") + " start: " + id);
        }

        [Conditional("DEBUG")]
        public static void End(string id)
        {
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss:fff") + " end: " + id);
        }
    }
}
