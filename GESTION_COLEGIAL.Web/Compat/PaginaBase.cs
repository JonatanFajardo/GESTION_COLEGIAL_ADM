using Microsoft.AspNetCore.Mvc.Razor;

namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Base de todas las vistas (se declara en Views/_ViewImports.cshtml).
    /// Expone lo que las vistas de MVC 5 usaban y ASP.NET Core no trae, para portarlas sin cambios:
    /// - Session["x"]  (donde decía HttpContext.Current.Session["x"] ahora va Session["x"])
    /// - Ajax.BeginForm / Ajax.ActionLink
    /// </summary>
    public abstract class PaginaBase<TModel> : RazorPage<TModel>
    {
        public SesionUsuario Session => new SesionUsuario(Context);

        public AjaxHelperCompat Ajax => new AjaxHelperCompat(ViewContext);
    }
}
