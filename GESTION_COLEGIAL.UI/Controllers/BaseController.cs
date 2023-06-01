using GESTION_COLEGIAL.UI.Extensions;
using System.Web.Mvc;
using static GESTION_COLEGIAL.UI.Extensions.AlertMessage;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class BaseController : Controller
    {
        protected string msjExist = $"El registro ya está en uso.";

        ///// <summary>
        ///// Muestra una alerta en pantalla.
        ///// </summary>
        ///// <param name="type">Tipo de mensaje.</param>
        ///// <param name="message">Descripción a mostrar.</param>
        //protected void Show(AlertMessageType type, string message = null)
        //{
        //    ViewBag.JavaScriptFunction = string.Format($"alertConfig.alert('{message}', '{type}');");
        //}

        #region Result

        public ActionResult AjaxResult(dynamic item, bool success)
        {
            return Json(new { item = item, success = success }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxResult(dynamic response)
        {
            //if: si response es boleano significa que este es un POST.
            if (response.GetType() == typeof(bool))
            {
                return Json(new { success = response });
            }
            else
            {
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Result

        #region CustomResult

        public ActionResult AjaxResult(dynamic item, bool success, AlertMessageCustomType type)
        {
            return Json(new { item = item, success = success }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxResult(dynamic response, AlertMessageCustomType type)
        {
            //if: si response es boleano significa que este es un POST.
            AlertMessageEntity _mensaje = ShowCustom(type);
            if (response.GetType() == typeof(bool))
            {
                return Json(new { success = response, type = _mensaje.type, message = _mensaje.message });
            }
            else
            {
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AjaxResult(dynamic item, bool success, AlertMessageType type, string mensaje)
        {
            AlertMessageEntity _mensaje = Show(type, mensaje);
            return Json(new { item = item, success = success, type = _mensaje.type, message = _mensaje.message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxResult(dynamic response, AlertMessageType type, string mensaje)
        {
            //if: si response es boleano significa que este es un POST.
            AlertMessageEntity _mensaje = Show(type, mensaje);
            if (response.GetType() == typeof(bool))
            {
                return Json(new { success = response, type = _mensaje.type, message = _mensaje.message });
            }
            else
            {
                return Json(new { data = response, type = _mensaje.type, message = _mensaje.message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion CustomResult

        //public ActionResult PageError404()
        //{
        //}

        //case AlertMessageType.Success:
        //    Show(AlertMessageType.Success, "Registro guardado exitosamente.");//satifactoriamente
        //    break;
        //case AlertMessageType.Warning:
        //    Show(AlertMessageType.Warning, "Ha ocurrido un problema.");
        //    break;
        //case AlertMessageType.Error:
        //    Show(AlertMessageType.Error, "Se produjo un error inesperado.");
        //    break;
    }
}