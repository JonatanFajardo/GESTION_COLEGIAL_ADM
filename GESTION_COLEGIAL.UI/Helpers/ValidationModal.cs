using System.Text.RegularExpressions;

namespace GESTION_COLEGIAL.UI.Helpers
{
    public class ValidationModal
    {
        public ValidationModal()
        {
        }

        private string sendMessage;

        /// <summary>
        /// Contenido a validar.
        /// </summary>
        public string SendMessage
        {
            get { return sendMessage; }
            set { sendMessage = value; }
        }

        private string requestMessage;

        /// <summary>
        /// Resultado con el mensaje de respuesta.
        /// </summary>
        public string RequestMessage
        {
            get { return requestMessage; }
            set { requestMessage = value; }
        }

        /// <summary>
        /// Valida los espacios en blanco
        /// </summary>
        /// <returns></returns>
        public string BlankSpaces()
        {
            if (string.IsNullOrEmpty(sendMessage.Trim()))
            {
                return requestMessage = "No se permiten nulos";
            }
            return requestMessage = requestMessage;
        }

        /// <summary>
        /// Valida los caracteres especiales
        /// </summary>
        /// <returns></returns>
        public string SpecialCharacters()
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (!regexItem.IsMatch(sendMessage.Trim()))
            {
                return requestMessage = "No se permiten caracteres especiales";
            }
            return requestMessage = requestMessage;
        }
    }
}