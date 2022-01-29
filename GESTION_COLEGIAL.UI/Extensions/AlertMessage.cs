namespace GESTION_COLEGIAL.UI.Extensions
{
    public static class AlertMessage
    {
        //public string Show()
        //{

        //    return ViewBag.JavaScriptFunction = string.Format("alertConfig.alert('{0}', '{1}');", "Ingresado :p", "success"); ;
        //}

        /// <summary>
        /// Muestra una alerta en pantalla.
        /// </summary>
        /// <param name="type">Tipo de mensaje.</param>
        /// <param name="message">Descripción a mostrar.</param>
        public static AlertMessageEntity Show(AlertMessageType typeparam, string messageparam = null)
        {
            var result = new AlertMessageEntity();

            switch (typeparam)
            {
                case AlertMessageType.Info:
                    result.type = "Info";
                    result.message = messageparam;
                    return result;
                case AlertMessageType.Warning:
                    result.type = "Warning";
                    result.message = messageparam;
                    return result;
                case AlertMessageType.Success:
                    result.type = "Success";
                    result.message = messageparam;
                    return result;
                case AlertMessageType.Error:
                    result.type = "Error";
                    result.message = messageparam;
                    return result;
            }
            return result;

            //ViewBag.JavaScriptFunction = string.Format($"alertConfig.alert('{message}', '{type}');");
        }

        /// <summary>
        /// Mensajes predefinidos para el controlador.
        /// </summary>
        /// <param name="type">Tipo de alerta.</param>
        public static AlertMessageEntity ShowCustom(AlertMessageCustomType type)
        {

            switch (type)
            {
                case AlertMessageCustomType.SuccessInsert:
                    return Show(AlertMessageType.Success, "Registro guardado exitosamente.");//satifactoriamente
                case AlertMessageCustomType.SuccessUpdate:
                    return Show(AlertMessageType.Success, "Registro editado exitosamente.");//satifactoriamente
                case AlertMessageCustomType.SuccessDelete:
                    return Show(AlertMessageType.Success, "Registro eliminado exitosamente.");//satifactoriamente
                case AlertMessageCustomType.Error:
                    return Show(AlertMessageType.Error, "Se produjo un error inesperado.");
                default:
                    return Show(AlertMessageType.Error, "Se produjo un error inesperado.");

            }
        }

        //public enum AlertMessageType
        //{
        //    Success = 0,
        //    Info = 1,
        //    Warning = 2,
        //    Error = 3

        //}

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

    public class AlertMessageEntity
    {
        public string type;
        public string message;
    }

}

