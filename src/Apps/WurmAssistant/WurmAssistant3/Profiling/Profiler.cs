using System;
using System.Diagnostics;

namespace AldursLab.WurmAssistant3.Profiling
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
