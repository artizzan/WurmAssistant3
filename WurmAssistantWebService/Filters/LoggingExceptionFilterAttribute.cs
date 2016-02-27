using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using AldursLab.WurmAssistantWebService.Model.Services;
using Microsoft.ApplicationInsights;

namespace AldursLab.WurmAssistantWebService.Filters
{
    public class LoggingExceptionFilterAttribute : ExceptionFilterAttribute
    {
        readonly Lazy<Logs> logs = new Lazy<Logs>(() => new Logs());
        readonly TelemetryClient telemetryClient = new TelemetryClient();

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var controller = GetControllerName(actionExecutedContext);
            var action = GetActionName(actionExecutedContext);
            logs.Value.Add(
                string.Format("Controller: {0}, Action: {1}", controller, action),
                exception.ToString());
            telemetryClient.TrackException(exception, new Dictionary<string, string>()
            {
                ["Controller"] = controller,
                ["Action"] = action
            });
        }

        string GetActionName(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                return actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            }
            catch (Exception exception)
            {
                return HandleInternalIssue(exception, "getting ActionName");
            }
        }

        string GetControllerName(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                return actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            }
            catch (Exception exception)
            {
                return HandleInternalIssue(exception, "getting ControllerName");
            }
        }

        string HandleInternalIssue(Exception exception, string action)
        {
            logs.Value.Add("LoggingExceptionFilterAttribute", string.Format("error at {0}: {1}", action, exception));
            telemetryClient.TrackException(exception, new Dictionary<string, string>()
            {
                ["Source"] = "Internal Issue at LoggingExceptionFilterAttribute",
            });
            return string.Format("error getting {0}: {1}", action, exception.Message);
        }
    }
}