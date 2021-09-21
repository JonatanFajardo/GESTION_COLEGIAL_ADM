using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class BaseController : Controller
    {
        protected string msjExist = $"El registro ya está en uso.";


        /// <summary>
        /// Muestra una alerta en pantalla.
        /// </summary>
        /// <param name="type">Tipo de mensaje.</param>
        /// <param name="message">Descripción a mostrar.</param>
        protected void Show(AlertMessageType type, string message = null)
        {
            ViewBag.JavaScriptFunction = string.Format($"alertConfig.alert('{message}', '{type}');");
        }

        public ActionResult AjaxResult(dynamic item, bool success)
        {
            return Json(new { item = item, success = success }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxResult(dynamic response)
        {
            //if: si response es boleano significa que este es un POST.
            if (response.GetType() == typeof(bool))
            {
                return Json(new { success = response, type = "success", message = "este es el mensaje." });
            }
            else
            {
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Mensajes predefinidos para el controlador.
        /// </summary>
        /// <param name="type">Tipo de alerta.</param>
        protected void ShowController(AlertMessageCustomType type)
        {
            switch (type)
            {
                case AlertMessageCustomType.SuccessInsert:
                    Show(AlertMessageType.Success, "Registro guardado exitosamente.");//satifactoriamente
                    break;
                case AlertMessageCustomType.SuccessUpdate:
                    Show(AlertMessageType.Success, "Registro editado exitosamente.");//satifactoriamente
                    break;
                case AlertMessageCustomType.SuccessDelete:
                    Show(AlertMessageType.Success, "Registro eliminado exitosamente.");//satifactoriamente
                    break;
                case AlertMessageCustomType.Error:
                    Show(AlertMessageType.Error, "Se produjo un error inesperado.");
                    break;
            }
        }
        //case AlertMessageType.Success:
        //    Show(AlertMessageType.Success, "Registro guardado exitosamente.");//satifactoriamente
        //    break;
        //case AlertMessageType.Warning:
        //    Show(AlertMessageType.Warning, "Ha ocurrido un problema.");
        //    break;
        //case AlertMessageType.Error:
        //    Show(AlertMessageType.Error, "Se produjo un error inesperado.");
        //    break;

        public enum AlertMessageType
        {
            Info = 0,
            Warning = 1,
            Success = 2,
            Error = 3
        }

        public enum AlertMessageCustomType
        {
            SuccessInsert = 0,
            SuccessUpdate = 1,
            SuccessDelete = 2,
            Error = 3
        }
    }
}