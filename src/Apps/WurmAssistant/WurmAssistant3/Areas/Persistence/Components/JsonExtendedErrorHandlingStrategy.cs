using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Persistence.Views;
using AldursLab.WurmAssistant3.Root.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;

namespace AldursLab.WurmAssistant3.Areas.Persistence.Components
{
    class JsonExtendedErrorHandlingStrategy : JsonDefaultErrorHandlingStrategy
    {
        readonly ILogger logger;
        readonly IHostEnvironment host;

        public JsonExtendedErrorHandlingStrategy([NotNull] ILogger logger, [NotNull] IHostEnvironment host)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (host == null) throw new ArgumentNullException("host");
            this.logger = logger;
            this.host = host;
        }

        public override void HandleErrorOnDeserialize(object o, ErrorEventArgs args)
        {
            base.HandleErrorOnDeserialize(o, args);

            logger.Error(args.ErrorContext.Error,
                string.Format((string) "Deserialization error. Member = {0} ; Path = {1}",
                    args.ErrorContext.Member,
                    args.ErrorContext.Path));
            var view = new DeserializationErrorResolverView(o, args);
            if (view.ShowDialog() == DialogResult.No)
            {
                host.Shutdown();
            }
        }

        public override void HandleErrorOnSerialize(object o, ErrorEventArgs args)
        {
            base.HandleErrorOnSerialize(o, args);
            logger.Error(args.ErrorContext.Error,
                string.Format((string) "Serialization error. Member = {0} ; Path = {1}",
                    args.ErrorContext.Member,
                    args.ErrorContext.Path));
        }

        public override PreviewResult PreviewJsonStringOnPopulate(string rawJson, object populatedObject)
        {
            var result = base.PreviewJsonStringOnPopulate(rawJson, populatedObject);

            if (string.IsNullOrEmpty(rawJson))
            {
                LogError("an empty string was found in the data source", populatedObject);
                result.BreakPopulating = true;
            }
            else if (rawJson.Trim().All(c => c == '\0'))
            {
                LogError("an all-NUL string content was found in the data source", populatedObject);
                result.BreakPopulating = true;
            }

            return result;
        }

        private void LogError(string reason, object obj)
        {
            logger.Error(string.Format(
                "While restoring state for object {0}, {1}. Skipping population.",
                obj != null ? obj.GetType().ToString() : "NULL",
                reason));
        }
    }
}
