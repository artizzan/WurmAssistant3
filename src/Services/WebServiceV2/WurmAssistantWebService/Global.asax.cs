using System;
using System.Data.Entity;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AldursLab.WurmAssistantWebService.Migrations;
using AldursLab.WurmAssistantWebService.Model;
using AldursLab.WurmAssistantWebService.Model.Services;
using Microsoft.ApplicationInsights.Extensibility;

namespace AldursLab.WurmAssistantWebService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey =
                WebConfigurationManager.AppSettings["InsightsInstrumentationKey"];

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }
    }
}
