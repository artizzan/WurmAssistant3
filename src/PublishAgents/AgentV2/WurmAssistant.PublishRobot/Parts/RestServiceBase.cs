using System.Net.Http;

namespace AldursLab.WurmAssistant.PublishRobot.Parts
{
    abstract class RestServiceBase
    {
        protected string FormatHttpError(HttpResponseMessage response)
        {
            return string.Format("{0}:{1}:{2}", (int)response.StatusCode, response.StatusCode, response.ReasonPhrase);
        }
    }
}