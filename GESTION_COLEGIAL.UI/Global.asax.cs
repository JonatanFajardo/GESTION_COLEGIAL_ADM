using GESTION_COLEGIAL.UI.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GESTION_COLEGIAL.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.DefaultBinder = new BaseViewModel();
        }
    }
}
