using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Modo de inserción del resultado (igual que System.Web.Mvc.Ajax.InsertionMode).
    /// </summary>
    public enum InsertionMode
    {
        Replace = 0,
        InsertBefore = 1,
        InsertAfter = 2,
        ReplaceWith = 3
    }

    /// <summary>
    /// Opciones de un formulario o link Ajax (igual que System.Web.Mvc.Ajax.AjaxOptions).
    /// Se traducen a atributos data-ajax-* que procesa jquery.unobtrusive-ajax.js.
    /// </summary>
    public class AjaxOptions
    {
        public string Confirm { get; set; }
        public string HttpMethod { get; set; }
        public InsertionMode InsertionMode { get; set; } = InsertionMode.Replace;
        public int LoadingElementDuration { get; set; }
        public string LoadingElementId { get; set; }
        public string OnBegin { get; set; }
        public string OnComplete { get; set; }
        public string OnFailure { get; set; }
        public string OnSuccess { get; set; }
        public string UpdateTargetId { get; set; }
        public string Url { get; set; }
        public bool AllowCache { get; set; }

        public IDictionary<string, object> ToUnobtrusiveHtmlAttributes()
        {
            var atributos = new Dictionary<string, object> { ["data-ajax"] = "true" };

            AgregarSiTieneValor(atributos, "data-ajax-url", Url);
            AgregarSiTieneValor(atributos, "data-ajax-method", HttpMethod);
            AgregarSiTieneValor(atributos, "data-ajax-confirm", Confirm);
            AgregarSiTieneValor(atributos, "data-ajax-begin", OnBegin);
            AgregarSiTieneValor(atributos, "data-ajax-complete", OnComplete);
            AgregarSiTieneValor(atributos, "data-ajax-failure", OnFailure);
            AgregarSiTieneValor(atributos, "data-ajax-success", OnSuccess);

            if (AllowCache)
            {
                atributos["data-ajax-cache"] = "true";
            }

            if (!string.IsNullOrWhiteSpace(LoadingElementId))
            {
                atributos["data-ajax-loading"] = "#" + LoadingElementId;
                if (LoadingElementDuration > 0)
                {
                    atributos["data-ajax-loading-duration"] = LoadingElementDuration;
                }
            }

            if (!string.IsNullOrWhiteSpace(UpdateTargetId))
            {
                atributos["data-ajax-update"] = "#" + UpdateTargetId;
                atributos["data-ajax-mode"] = InsertionMode switch
                {
                    InsertionMode.InsertBefore => "before",
                    InsertionMode.InsertAfter => "after",
                    InsertionMode.ReplaceWith => "replace-with",
                    _ => "replace"
                };
            }

            return atributos;
        }

        private static void AgregarSiTieneValor(IDictionary<string, object> atributos, string clave, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                atributos[clave] = valor;
            }
        }
    }

    /// <summary>
    /// Reemplazo de Ajax.BeginForm / Ajax.ActionLink de MVC 5, que no existen en ASP.NET Core.
    /// Genera el mismo HTML (form con data-ajax-*), así las vistas portadas no cambian.
    /// Se expone en las vistas como la propiedad Ajax (ver PaginaBase).
    /// </summary>
    public class AjaxHelperCompat
    {
        private const string ClaveContadorFormularios = "__ContadorFormulariosAjax";
        private readonly ViewContext _viewContext;

        public AjaxHelperCompat(ViewContext viewContext)
        {
            _viewContext = viewContext;
        }

        public MvcForm BeginForm(AjaxOptions ajaxOptions) =>
            Formulario(ajaxOptions?.Url ?? _viewContext.HttpContext.Request.Path.ToString(), ajaxOptions, null);

        public MvcForm BeginForm(string actionName, AjaxOptions ajaxOptions) =>
            BeginForm(actionName, null, null, ajaxOptions, null);

        public MvcForm BeginForm(string actionName, object routeValues, AjaxOptions ajaxOptions) =>
            BeginForm(actionName, null, routeValues, ajaxOptions, null);

        public MvcForm BeginForm(string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes) =>
            BeginForm(actionName, null, routeValues, ajaxOptions, htmlAttributes);

        public MvcForm BeginForm(string actionName, string controllerName, AjaxOptions ajaxOptions) =>
            BeginForm(actionName, controllerName, null, ajaxOptions, null);

        public MvcForm BeginForm(string actionName, string controllerName, AjaxOptions ajaxOptions, object htmlAttributes) =>
            BeginForm(actionName, controllerName, null, ajaxOptions, htmlAttributes);

        public MvcForm BeginForm(string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions) =>
            BeginForm(actionName, controllerName, routeValues, ajaxOptions, null);

        public MvcForm BeginForm(string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes) =>
            Formulario(Url().Action(actionName, controllerName, routeValues), ajaxOptions, htmlAttributes);

        public IHtmlContent ActionLink(string linkText, string actionName, AjaxOptions ajaxOptions) =>
            ActionLink(linkText, actionName, null, null, ajaxOptions, null);

        public IHtmlContent ActionLink(string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions) =>
            ActionLink(linkText, actionName, null, routeValues, ajaxOptions, null);

        public IHtmlContent ActionLink(string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes) =>
            ActionLink(linkText, actionName, null, routeValues, ajaxOptions, htmlAttributes);

        public IHtmlContent ActionLink(string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions) =>
            ActionLink(linkText, actionName, controllerName, null, ajaxOptions, null);

        public IHtmlContent ActionLink(string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions) =>
            ActionLink(linkText, actionName, controllerName, routeValues, ajaxOptions, null);

        public IHtmlContent ActionLink(string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var link = new TagBuilder("a");
            link.InnerHtml.Append(linkText ?? string.Empty);
            link.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            link.MergeAttribute("href", Url().Action(actionName, controllerName, routeValues));
            link.MergeAttributes((ajaxOptions ?? new AjaxOptions()).ToUnobtrusiveHtmlAttributes());
            return link;
        }

        private MvcForm Formulario(string accion, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            // Mismo orden que MVC 5: los htmlAttributes de la vista mandan sobre action/method/data-ajax.
            var form = new TagBuilder("form");
            form.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            form.MergeAttribute("action", accion);
            form.MergeAttribute("method", "post");
            form.MergeAttributes((ajaxOptions ?? new AjaxOptions()).ToUnobtrusiveHtmlAttributes());

            // MVC 5 numeraba los formularios sin id (form0, form1…) cuando había validación en cliente.
            if (!form.Attributes.ContainsKey("id"))
            {
                form.Attributes["id"] = "form" + SiguienteNumeroFormulario();
            }

            var encoder = _viewContext.HttpContext.RequestServices.GetRequiredService<HtmlEncoder>();
            form.TagRenderMode = TagRenderMode.StartTag;
            form.WriteTo(_viewContext.Writer, encoder);

            // Igual que Html.BeginForm: contexto propio para la validación en cliente de los campos.
            _viewContext.FormContext = new FormContext { CanRenderAtEndOfForm = true };
            return new MvcForm(_viewContext, encoder);
        }

        private int SiguienteNumeroFormulario()
        {
            var items = _viewContext.HttpContext.Items;
            var numero = items.TryGetValue(ClaveContadorFormularios, out var actual) ? (int)actual : 0;
            items[ClaveContadorFormularios] = numero + 1;
            return numero;
        }

        private IUrlHelper Url() =>
            _viewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(_viewContext);
    }
}
