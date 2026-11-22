using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Formatos de la ficha de salud: teléfonos de Honduras (9999-9999) y fechas cortas.</summary>
    public static class FormatoSalud
    {
        public static readonly CultureInfo Cultura = new CultureInfo("es-HN");

        public static readonly string[] TiposSangre = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

        /// <summary>Sugerencias (el campo es libre).</summary>
        public static readonly string[] Seguros = { "IHSS", "Seguro privado", "Ninguno" };
        public static readonly string[] Parentescos = { "Madre", "Padre", "Abuela", "Abuelo", "Tía", "Tío", "Hermana", "Hermano", "Madrina", "Padrino", "Vecina", "Vecino" };

        /// <summary>"99998888" → "9999-8888"; otro texto se deja tal cual; vacío → null.</summary>
        public static string Telefono(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            string d = new string(valor.Where(char.IsDigit).ToArray());
            return d.Length == 8 ? d.Substring(0, 4) + "-" + d.Substring(4) : valor.Trim();
        }

        /// <summary>Para el enlace tel: solo dígitos.</summary>
        public static string TelefonoEnlace(string valor) =>
            string.IsNullOrWhiteSpace(valor) ? null : "tel:" + new string(valor.Where(char.IsDigit).ToArray());

        /// <summary>¿8 dígitos, con guion o espacio opcional en medio?</summary>
        public static bool EsTelefono(string valor) =>
            string.IsNullOrWhiteSpace(valor) || Regex.IsMatch(valor.Trim(), @"^\d{4}[-\s]?\d{4}$");

        public static string Dia(DateTime? d) => d.HasValue ? d.Value.ToString("d MMM yyyy", Cultura).Replace(".", "") : "—";
        public static string DiaHora(DateTime d) => d.ToString("d MMM yyyy", Cultura).Replace(".", "") + ", " + d.ToString("HH:mm", Cultura);

        /// <summary>Texto o null si viene vacío (para decidir si mostrar una fila).</summary>
        public static string Texto(string valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    /// <summary>Pantalla "Ficha de salud" de un alumno.</summary>
    public class FichaSaludPaginaViewModel
    {
        public FichaSaludEmergenciaViewModel Emergencia { get; set; }
        public List<FichaSaludBitacoraViewModel> Historial { get; set; } = new List<FichaSaludBitacoraViewModel>();
        public bool PuedeEditar { get; set; }
        public string Aviso { get; set; }

        public FichaSaludViewModel Ficha => Emergencia?.Ficha;
    }
}
