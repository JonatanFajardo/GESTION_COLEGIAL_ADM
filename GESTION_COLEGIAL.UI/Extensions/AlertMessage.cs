using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Extensions
{
    public class AlertMessage : Controller
    {
        //public string Show()
        //{

        //    return ViewBag.JavaScriptFunction = string.Format("alertConfig.alert('{0}', '{1}');", "Ingresado :p", "success"); ;
        //}
        public enum AlertMessageType
        {
            Success = 0,
            Info = 1,
            Warning = 2,
            Error = 3

        }

    }
}