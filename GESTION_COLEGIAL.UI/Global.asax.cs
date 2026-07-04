using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

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
            ModelBinders.Binders.DefaultBinder = new Business.Models.BaseViewModel();
        }

        protected void Application_PostAcquireRequestState()
        {
            var ctx = HttpContext.Current;
            if (ctx?.Session == null) return;
            if (ctx.Session["UserId"] != null) return;
            if (!(ctx.User?.Identity?.IsAuthenticated ?? false)) return;

            var cookie = ctx.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value)) return;

            try
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket == null || string.IsNullOrEmpty(ticket.UserData)) return;

                var data = JObject.Parse(ticket.UserData);
                ctx.Session["UserId"] = (int)data["UserId"];
                ctx.Session["Username"] = (string)data["Username"];
                ctx.Session["RolId"] = (int)data["RolId"];
                ctx.Session["RoleName"] = (string)data["RoleName"];
                ctx.Session["Token"] = (string)data["Token"];
                ctx.Session["pantallas"] = (string)data["Pantallas"];
            }
            catch (Exception)
            {
                // Ticket inválido/corrupto: dejar que el pipeline redirija a Login
            }
        }
    }
}
