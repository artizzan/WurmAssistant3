using System;
using AldursLab.WurmApi.PersistentObjects;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.PersistentCollections
{
    class CustomDeserializationErrorHandler : IObjectDeserializationErrorHandlingStrategy
    {
        readonly Action<ErrorContext> action;

        public CustomDeserializationErrorHandler(Action<ErrorContext> action)
        {
            this.action = action;
        }

        public void Handle(ErrorContext errorContext)
        {
            action(errorContext);
        }
    }
}