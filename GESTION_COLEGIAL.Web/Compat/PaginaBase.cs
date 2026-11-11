using Microsoft.AspNetCore.Mvc.Razor;

namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Base de todas las vistas (se declara en Views/_ViewImports.cshtml).
    /// Expone Session["..."] para que las vistas portadas de MVC 5 no cambien;
    /// donde decía HttpContext.Current.Session["x"] ahora va Session["x"].
    /// </summary>
    public abstract class PaginaBase<TModel> : RazorPage<TModel>
    {
        public SesionUsuario Session => new SesionUsuario(Context);
    }
}
