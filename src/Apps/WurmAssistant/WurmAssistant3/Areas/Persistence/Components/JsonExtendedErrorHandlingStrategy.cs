using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Persistence.Views;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;

namespace AldursLab.WurmAssistant3.Areas.Persistence.Components
{
    class JsonExtendedErrorHandlingStrategy : JsonDefaultErrorHandlingStrategy
    {
        readonly ILogger logger;

        public JsonExtendedErrorHandlingStrategy([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
        }

        public override void HandleErrorOnDeserialize(object o, ErrorEventArgs args)
        {
            base.HandleErrorOnDeserialize(o, args);

            logger.Error(args.ErrorContext.Error,
                string.Format((string) "Deserialization error. Member = {0} ; Path = {1}",
                    args.ErrorContext.Member,
                    args.ErrorContext.Path));
            var view = new DeserializationErrorResolverForm(o, args);
            if (view.ShowDialog() == DialogResult.No)
            {
                System.Windows.Application.Current.Shutdown();
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
