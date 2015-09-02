using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant.PublishRobot.Parts
{
    class PublishingWebService : RestServiceBase
    {
        readonly IOutput output;
        readonly string webServiceRootPath;
        readonly string webServiceControllerPath;
        readonly string login;
        readonly string password;
        string token = string.Empty;

        public PublishingWebService([NotNull] IOutput output, [NotNull] string webServiceRootPath,
            [NotNull] string webServiceControllerPath, [NotNull] string login, [NotNull] string password)
        {
            if (output == null) throw new ArgumentNullException("output");
            if (webServiceRootPath == null) throw new ArgumentNullException("webServiceRootPath");
            if (webServiceControllerPath == null) throw new ArgumentNullException("webServiceControllerPath");
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");
            this.output = output;
            this.webServiceRootPath = webServiceRootPath;
            this.webServiceControllerPath = webServiceControllerPath;
            this.login = login;
            this.password = password;
        }

        public void Authenticate()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(webServiceRootPath);
                var content = new StringContent(
                    string.Format("grant_type=password&username={0}&password={1}", login.Replace("@", "%40"), password),
                    Encoding.UTF8,
                    "text/plain");
                client.Timeout = TimeSpan.FromSeconds(30);
                var response = client.PostAsync("Token", content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Authentication failed " + FormatHttpError(response));
                }
                string serialized = response.Content.ReadAsStringAsync().Result;
                var model = JsonConvert.DeserializeObject<TokenResponseModel>(serialized);
                if (!"bearer".Equals(model.TokenType, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Returned token is not of bearer type, actual: " + model.TokenType);
                }
                token = model.AccessToken;
            }
        }

        public Version GetLatestVersion(string buildType)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Format("{0}/{1}", webServiceRootPath, webServiceControllerPath));
                client.Timeout = TimeSpan.FromSeconds(30);
                var path = string.Format(@"{0}/{1}",
                    buildType, "LatestVersion");
                var response = client.GetAsync(path).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("GetLatestVersion failed " + FormatHttpError(response));
                }
                var result = response.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
                var version = Version.Parse(result);
                return version;
            }
        }

        public void Publish(FileInfo zipFile, Version latestVersion, string buildType)
        {
            output.Write(string.Format("Publish args: {0}, {1}, {2}",
                zipFile.FullName,
                latestVersion,
                buildType));

            if (token == string.Empty)
            {
                output.Write("No token, authenticating...");
                Authenticate();
                output.Write("Authenticated");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Format("{0}/{1}", webServiceRootPath, webServiceControllerPath));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.Timeout = TimeSpan.FromSeconds(600);
                using (var content = new MultipartFormDataContent())
                {
                    using (var stream = zipFile.OpenRead())
                    {
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = zipFile.Name
                        };
                        content.Add(fileContent);
                        var publishUrl = string.Format(@"{0}/{1}/{2}",
                                    buildType, "Package", latestVersion.ToString().Replace(".", "-"));
                        output.Write("Publishing to: " + client.BaseAddress + "/" + publishUrl);
                        var response =
                            client.PostAsync(publishUrl, content)
                                .Result;
                        output.Write("Publish result: " + FormatHttpError(response));
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Web publish failed");
                        }
                    }
                }
            }
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.Members | ImplicitUseTargetFlags.Itself)]
    class TokenResponseModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty(".issued")]
        public string IssuedAt { get; set; }

        [JsonProperty(".expires")]
        public string ExpiresAt { get; set; }
    }
}
