using System;
using System.Diagnostics;

namespace AldursLab.WurmAssistant.PublishRobot
{
    class Program
    {
        static void Main(string[] args)
        {
            var sampleOut = "args: " + string.Join(", ", args);
            Console.WriteLine("Console: " + sampleOut);
            Trace.WriteLine("Trace: " + sampleOut);
        }
    }
}
