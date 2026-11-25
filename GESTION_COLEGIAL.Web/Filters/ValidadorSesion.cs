using System.Collections.Concurrent;
using System.Security.Claims;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GESTION_COLEGIAL.UI.Filters
{
    /// <summary>
    /// Cierra las sesiones abiertas cuando la cuenta cambió: se cambió la contraseña, se cambió el rol,
    /// se desactivó o suspendió. La cookie de login guarda el "sello de sesión" que tenía la cuenta al
    /// entrar; cada pocos minutos se compara con el sello actual (consulta a la API con un caché corto,
    /// para no llamarla en cada petición). Si la API no responde, la sesión se deja como está.
    /// </summary>
    public static class ValidadorSesion
    {
        public const string ClaimSello = "SelloSesion";

        // Cada cuánto se vuelve a preguntar a la API por un mismo usuario.
        private static readonly TimeSpan Vigencia = TimeSpan.FromMinutes(2);

        private sealed record Entrada(EstadoSesionViewModel Estado, DateTime Vence);

        // Si la API no respondió, no se vuelve a intentar durante este tiempo (evita esperar en cada petición).
        private static readonly TimeSpan EsperaTrasFallo = TimeSpan.FromSeconds(30);
        private static DateTime sinApiHasta = DateTime.MinValue;

        private static readonly ConcurrentDictionary<int, Entrada> Cache = new();
        private static readonly AccountService Cuenta = new AccountService();

        /// <summary>Se conecta en Program.cs: CookieAuthenticationEvents.OnValidatePrincipal.</summary>
        public static async Task ValidarAsync(CookieValidatePrincipalContext contexto)
        {
            ClaimsPrincipal principal = contexto.Principal;
            if (principal == null || !int.TryParse(principal.FindFirst(SesionUsuario.UserId)?.Value, out int usuId) || usuId <= 0)
            {
                return;
            }

            string sello = principal.FindFirst(ClaimSello)?.Value;
            if (string.IsNullOrEmpty(sello))
            {
                // Cookie de antes de existir el sello: se vuelve a iniciar sesión.
                await Rechazar(contexto);
                return;
            }

            if (!Cache.TryGetValue(usuId, out Entrada entrada) || entrada.Vence < DateTime.UtcNow)
            {
                if (DateTime.UtcNow < sinApiHasta) return; // La API no responde: se deja la sesión como está.
                // La llamada a la API firma con el usuario de HttpContext.User, que aún no está puesto aquí.
                contexto.HttpContext.User = principal;
                EstadoSesionViewModel estado = null;
                try { estado = await Cuenta.EstadoSesionAsync(usuId); } catch { }
                if (estado == null)
                {
                    sinApiHasta = DateTime.UtcNow + EsperaTrasFallo;
                    return; // La API no respondió: no se cierra la sesión por eso.
                }
                entrada = new Entrada(estado, DateTime.UtcNow + Vigencia);
                Cache[usuId] = entrada;
            }

            if (entrada.Estado.Habilitado != 1 || !string.Equals(entrada.Estado.Sello, sello, StringComparison.OrdinalIgnoreCase))
            {
                await Rechazar(contexto);
            }
        }

        /// <summary>Deja el estado conocido de un usuario (lo usa quien acaba de cambiar su propia contraseña).</summary>
        public static void Recordar(int usuId, EstadoSesionViewModel estado)
        {
            Cache[usuId] = new Entrada(estado, DateTime.UtcNow + Vigencia);
        }

        /// <summary>Olvida lo conocido de un usuario: su próxima petición vuelve a preguntar a la API.</summary>
        public static void Olvidar(int usuId)
        {
            Cache.TryRemove(usuId, out _);
        }

        /// <summary>
        /// La sesión que cambió su propia contraseña sigue abierta: se le pone el sello nuevo en la cookie
        /// (las demás sesiones de la cuenta quedan con el sello viejo y se cierran).
        /// </summary>
        public static async Task RenovarAsync(HttpContext contexto)
        {
            if (!int.TryParse(contexto.User?.FindFirst(SesionUsuario.UserId)?.Value, out int usuId) || usuId <= 0) return;

            EstadoSesionViewModel estado = await Cuenta.EstadoSesionAsync(usuId);
            if (estado == null || estado.Habilitado != 1 || string.IsNullOrEmpty(estado.Sello)) return;

            AuthenticateResult actual = await contexto.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!actual.Succeeded) return;

            var claims = actual.Principal.Claims.Where(c => c.Type != ClaimSello).ToList();
            claims.Add(new Claim(ClaimSello, estado.Sello));
            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await contexto.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad), actual.Properties);
            Recordar(usuId, estado);
        }

        private static async Task Rechazar(CookieValidatePrincipalContext contexto)
        {
            contexto.RejectPrincipal();
            // Esta misma petición ya no cuenta como una sesión iniciada (ver "HttpContext.User" arriba).
            contexto.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            await contexto.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
