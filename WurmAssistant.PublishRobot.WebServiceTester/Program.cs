using System;
using System.IO;
using AldursLab.WurmAssistant.PublishRobot.Parts;

namespace AldursLab.WurmAssistant.PublishRobot.WebServiceTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestSlacker();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                Console.ReadKey();
            }
        }

        static void TestSlacker()
        {
            var client = new SlackService(new ConsoleOutput(), "placeholder");
            client.SendMessage("test message");
        }

        static void TestPublisher()
        {
            var client = new PublishingWebService(new ConsoleOutput(),
                "http://wurmassistant.aldurslab.net",
                "api/WurmAssistant3/",
                "wurmassistant@gmail.com",
                "placeholder");

            client.Authenticate();
            client.GetLatestVersion("Stable");
            var info = new FileInfo("pak.dat");
            File.WriteAllText(info.FullName, "test");
            client.Publish(info, new Version(0, 0, 0, 1), "Stable");
        }
    }
}
