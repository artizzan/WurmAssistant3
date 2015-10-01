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
                RetryTest();
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

        static void RetryTest()
        {
            int count = 0;
            Action action = () =>
            {
                count++;
                throw new Exception("123");
            };

            RetryManager.AutoRetry(action);
        }

        static void TestSlacker()
        {
            var client = new SlackService(new ConsoleOutput(), "placeholder");
            client.SendMessage("test message");
        }

        static void TestPublisher()
        {
            var client = new PublishingWebService(new ConsoleOutput(),
                "http://localhost:54793/",
                "api/WurmAssistant3/",
                "wurmassistant@gmail.com",
                "placeholder");

            client.Authenticate();
            var info = new FileInfo("pak.dat");
            File.WriteAllText(info.FullName, "test");
            client.Publish(info, "publish-test", "2");

            var version = client.GetLatestVersion("publish-test");
            Console.WriteLine("latest version: " + version);
        }
    }
}
