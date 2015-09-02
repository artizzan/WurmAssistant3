using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant.PublishRobot.Parts
{
    class SlackService : RestServiceBase
    {
        readonly IOutput output;
        readonly string webHookSubUrl;

        public SlackService([NotNull] IOutput output, [NotNull] string webHookSubUrl)
        {
            if (output == null) throw new ArgumentNullException("output");
            if (webHookSubUrl == null) throw new ArgumentNullException("webHookSubUrl");
            this.output = output;
            this.webHookSubUrl = webHookSubUrl;
        }

        public void SendMessage(string text)
        {
            output.Write("Sending Slack message: " + text);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://hooks.slack.com");
                var message = new SlackMessage()
                {
                    Text = text
                };
                using (var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"))
                {
                    var response =
                        client.PostAsync(webHookSubUrl, content)
                              .Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Sending Slack message failed: " + FormatHttpError(response));
                    }
                    output.Write("Slack message sent");
                }
            }
        }
    }

    public class SlackMessage
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
