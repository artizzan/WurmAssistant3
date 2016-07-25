using System;
using System.Threading;
using AldursLab.WurmAssistant3.Areas.Config;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights;

namespace AldursLab.WurmAssistant3.Areas.Insights
{
    [KernelBind(BindingHint.Singleton)]
    class Telemetry : ITelemetry, IDisposable
    {
        readonly IWurmAssistantConfig wurmAssistantConfig;

        readonly TelemetryClient client;
        readonly Guid sessionId = Guid.NewGuid();

        public Telemetry([NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            this.wurmAssistantConfig = wurmAssistantConfig;

            client = new TelemetryClient
            {
                InstrumentationKey = "9813c551-b5b1-48c0-baa5-3e8e47ec9683",
            };
            client.Context.User.Id = wurmAssistantConfig.InstallationId.ToString();
            client.Context.Session.Id = sessionId.ToString();
        }

        public void TrackEvent(string eventName)
        {
            if (wurmAssistantConfig.AllowInsights)
            {
                client.TrackEvent(eventName);
            }
        }

        public void Dispose()
        {
            client.Flush();
            // Allow for the events to be sent. This is currently the only decent way.
            Thread.Sleep(1000);
        }
    }
}