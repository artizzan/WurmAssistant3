using System;
using System.Diagnostics;
using System.IO;
using AldursLab.Essentials.Configs;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant.PublishRobot.Actions;
using AldursLab.WurmAssistant.PublishRobot.Parts;

namespace AldursLab.WurmAssistant.PublishRobot
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintArgs(args);

            var workDir = Directory.GetCurrentDirectory();
            var tempDir = Path.Combine(workDir, "temp");
            ClearDir(tempDir);

            ValidateArgs(args);
            var command = args[0];
            var configPath = Path.Combine(workDir, command);
            if (!File.Exists(configPath))
            {
                throw new ArgumentException("config file does not exist, path: " + configPath);
            }
            IConfig config = new FileSimpleConfig(configPath);
            IOutput output = new ConsoleOutput();

            if (command == "publish-package")
            {
                var action = new PublishPackage(config, tempDir, output);
                action.Execute();
            }
            // 

            Console.ReadKey();
        }

        static void ValidateArgs(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("argument for action was not supplied");
            }
        }

        static void PrintArgs(string[] args)
        {
            var formattedArgs = "args: " + string.Join("\r\n", args);
            Console.WriteLine("Console: " + formattedArgs);
            Trace.WriteLine("Trace: " + formattedArgs);
        }

        static void ClearDir(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, recursive:true);
            }
            Directory.CreateDirectory(dirPath);
        }
    }

    class OperationHandler
    {
        public OperationHandler()
        {
            
        }
    }
}
