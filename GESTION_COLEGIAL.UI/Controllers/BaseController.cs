using GESTION_COLEGIAL.UI.Extensions;
using System.Web.Mvc;
using static GESTION_COLEGIAL.UI.Extensions.AlertMessage;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Clase base para los controladores.
    /// </summary>
    public class BaseController : Controller
    {
        protected string msjExist = $"El registro ya está en uso.";

        #region Result

        /// <summary>
        /// Returns an Ajax result.
        /// </summary>
        /// <param name="item">The result item.</param>
        /// <param name="success">A boolean indicating the success status.</param>
        /// <returns>The Ajax result.</returns>
        public ActionResult AjaxResult(dynamic item, bool success)
        {
            return Json(new { item = item, success = success }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns an Ajax result.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <returns>The Ajax result.</returns>
        public ActionResult AjaxResult(dynamic response)
        {
            // Handle null responses
            if (response == null)
            {
                return Json(new { data = (object)null }, JsonRequestBehavior.AllowGet);
            }

            // If response is boolean, it means this is a POST request.
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

        /// <summary>
        /// Returns a custom Ajax result with an alert message.
        /// </summary>
        /// <param name="item">The result item.</param>
        /// <param name="success">A boolean indicating the success status.</param>
        /// <param name="type">The alert message type.</param>
        /// <returns>The Ajax result with the alert message.</returns>
        public ActionResult AjaxResult(dynamic item, bool success, AlertMessageCustomType type)
        {
            return Json(new { item = item, success = success }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns a custom Ajax result with an alert message.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="type">The alert message type.</param>
        /// <returns>The Ajax result with the alert message.</returns>
        public ActionResult AjaxResult(dynamic response, AlertMessageCustomType type)
        {
            // If response is boolean, it means this is a POST request.
            AlertMessageEntity _mensaje = ShowCustom(type);

            // Handle null responses
            if (response == null)
            {
                return Json(new { data = (object)null, type = _mensaje.type, message = _mensaje.message }, JsonRequestBehavior.AllowGet);
            }

            if (response.GetType() == typeof(bool))
            {
                return Json(new { success = response, type = _mensaje.type, message = _mensaje.message });
            }
            else
            {
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Returns a custom Ajax result with an alert message.
        /// </summary>
        /// <param name="item">The result item.</param>
        /// <param name="success">A boolean indicating the success status.</param>
        /// <param name="type">The alert message type.</param>
        /// <param name="mensaje">The custom alert message.</param>
        /// <returns>The Ajax result with the alert message.</returns>
        public ActionResult AjaxResult(dynamic item, bool success, AlertMessageType type, string mensaje)
        {
            AlertMessageEntity _mensaje = Show(type, mensaje);
            return Json(new { item = item, success = success, type = _mensaje.type, message = _mensaje.message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns a custom Ajax result with an alert message.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="type">The alert message type.</param>
        /// <param name="mensaje">The custom alert message.</param>
        /// <returns>The Ajax result with the alert message.</returns>
        public ActionResult AjaxResult(dynamic response, AlertMessageType type, string mensaje)
        {
            // If response is boolean, it means this is a POST request.
            AlertMessageEntity _mensaje = Show(type, mensaje);

            // Handle null responses
            if (response == null)
            {
                return Json(new { data = (object)null, type = _mensaje.type, message = _mensaje.message }, JsonRequestBehavior.AllowGet);
            }

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
    }
}
