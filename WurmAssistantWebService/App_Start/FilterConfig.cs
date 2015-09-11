using System.Web.Mvc;
using AldursLab.WurmAssistantWebService.Filters;

namespace AldursLab.WurmAssistantWebService
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
