using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json;
using static GESTION_COLEGIAL.UI.Extensions.AlertMessage;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Clase base para los controladores.
    /// </summary>
    public class BaseController : Controller
    {
        protected string msjExist = $"El registro ya está en uso.";

        /// <summary>
        /// Misma sintaxis que MVC 5: Session["UserId"], Session["pantallas"], etc.
        /// </summary>
        protected SesionUsuario Session => new SesionUsuario(HttpContext);

        /// <summary>
        /// Obtiene el Id del usuario logueado.
        /// </summary>
        protected int GetUsuarioId()
        {
            return Session[SesionUsuario.UserId] != null ? (int)Session[SesionUsuario.UserId] : 0;
        }

        /// <summary>
        /// Verifica si el usuario tiene acceso a una pantalla.
        /// </summary>
        protected bool TienePantalla(string pantallaNombre)
        {
            string pantallas = Session?[SesionUsuario.Pantallas] as string;
            return SessionManager.TienePantalla(pantallas, pantallaNombre);
        }

        #region Result

        /// <summary>
        /// Retorna el resultado de las solicitudes por Ajax.
        /// </summary>
        public ActionResult AjaxResult(dynamic item, bool success)
        {
            return Json(new { item = item, success = success });
        }

        /// <summary>
        /// Retorna el resultado de las solicitudes por Ajax.
        /// </summary>
        public ActionResult AjaxResult(dynamic response)
        {
            if (response == null)
                return Content(JsonConvert.SerializeObject(new { data = (object)null }), "application/json");

            if (response is bool || response.GetType() == typeof(bool))
                return Content(JsonConvert.SerializeObject(new { success = (bool)response }), "application/json");

            return Content(JsonConvert.SerializeObject(new { data = response }), "application/json");
        }

        #endregion Result

        #region CustomResult

        public ActionResult AjaxResult(dynamic item, bool success, AlertMessageCustomType type)
        {
            return Json(new { item = item, success = success });
        }

        public ActionResult AjaxResult(dynamic response, AlertMessageCustomType type)
        {
            // Si la respuesta es booleana, es un POST.
            AlertMessageEntity _mensaje = ShowCustom(type);

            if (response == null)
            {
                return Json(new { data = (object)null, type = _mensaje.type, message = _mensaje.message });
            }

            if (response.GetType() == typeof(bool))
            {
                return Json(new { success = response, type = _mensaje.type, message = _mensaje.message });
            }
            else
            {
                return Json(new { data = response });
            }
        }

        public ActionResult AjaxResult(dynamic item, bool success, AlertMessageType type, string mensaje)
        {
            AlertMessageEntity _mensaje = Show(type, mensaje);
            return Json(new { item = item, success = success, type = _mensaje.type, message = _mensaje.message });
        }

        public ActionResult AjaxResult(dynamic response, AlertMessageType type, string mensaje)
        {
            // Si la respuesta es booleana, es un POST.
            AlertMessageEntity _mensaje = Show(type, mensaje);

            if (response == null)
            {
                return Json(new { data = (object)null, type = _mensaje.type, message = _mensaje.message });
            }

            if (response.GetType() == typeof(bool))
            {
                return Json(new { success = response, type = _mensaje.type, message = _mensaje.message });
            }
            else
            {
                return Json(new { data = response, type = _mensaje.type, message = _mensaje.message });
            }
        }

        #endregion CustomResult
    }
}
