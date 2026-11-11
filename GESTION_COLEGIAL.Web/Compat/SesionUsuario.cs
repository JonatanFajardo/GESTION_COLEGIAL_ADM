using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Permite seguir usando Session["UserId"], Session["pantallas"], etc. como en MVC 5.
    /// - Los datos del usuario logueado se leen de la cookie de login (claims).
    /// - Lo que se asigne (Session["x"] = valor) se guarda en la sesión y tiene prioridad;
    ///   así funciona "ver como rol" de DevTools, que reemplaza las pantallas temporalmente.
    /// Solo guarda textos y números enteros, que es lo único que el sistema pone en sesión.
    /// </summary>
    public sealed class SesionUsuario
    {
        // Nombres de los datos que vienen del login (mismas claves que usaba MVC 5).
        public const string UserId = "UserId";
        public const string EmpId = "EmpId";
        public const string RolId = "RolId";
        public const string RoleName = "RoleName";
        public const string Username = "Username";
        public const string Token = "Token";
        public const string Pantallas = "pantallas";

        private static readonly HashSet<string> DatosEnteros =
            new HashSet<string>(StringComparer.Ordinal) { UserId, EmpId, RolId };

        private static readonly HashSet<string> DatosDelLogin =
            new HashSet<string>(StringComparer.Ordinal) { UserId, EmpId, RolId, RoleName, Username, Token, Pantallas };

        private const string PrefijoEntero = "i:";
        private const string PrefijoTexto = "s:";

        private readonly HttpContext _contexto;

        public SesionUsuario(HttpContext contexto)
        {
            _contexto = contexto;
        }

        public object this[string clave]
        {
            get
            {
                var guardado = _contexto.Session.GetString(clave);
                if (guardado != null)
                {
                    return Leer(guardado);
                }

                if (!DatosDelLogin.Contains(clave))
                {
                    return null;
                }

                var valor = _contexto.User?.FindFirst(clave)?.Value;
                if (valor == null)
                {
                    return null;
                }

                if (DatosEnteros.Contains(clave))
                {
                    return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numero)
                        ? numero
                        : (object)null;
                }

                return valor;
            }
            set
            {
                if (value == null)
                {
                    _contexto.Session.Remove(clave);
                    return;
                }

                var texto = value is int numero
                    ? PrefijoEntero + numero.ToString(CultureInfo.InvariantCulture)
                    : PrefijoTexto + value;
                _contexto.Session.SetString(clave, texto);
            }
        }

        /// <summary>Borra lo guardado en la sesión (los datos del login siguen en la cookie).</summary>
        public void Clear() => _contexto.Session.Clear();

        private static object Leer(string guardado)
        {
            if (guardado.StartsWith(PrefijoEntero, StringComparison.Ordinal)
                && int.TryParse(guardado.Substring(PrefijoEntero.Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out var numero))
            {
                return numero;
            }

            return guardado.StartsWith(PrefijoTexto, StringComparison.Ordinal)
                ? guardado.Substring(PrefijoTexto.Length)
                : guardado;
        }
    }
}
