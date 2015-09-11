using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using AldursLab.WurmAssistantWebService.Model.Services;

namespace AldursLab.WurmAssistantWebService.Filters
{
    public class LoggingExceptionFilterAttribute : ExceptionFilterAttribute
    {
        readonly Lazy<Logs> logs = new Lazy<Logs>(() => new Logs());

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            logs.Value.Add(
                string.Format("Controller: {0}, Action: {1}",
                    GetControllerName(actionExecutedContext),
                    GetActionName(actionExecutedContext)),
                actionExecutedContext.Exception.ToString());
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
            return string.Format("error getting {0}: {1}", action, exception.Message);
        }
    }
}