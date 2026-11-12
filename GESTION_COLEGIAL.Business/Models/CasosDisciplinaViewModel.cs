using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Una opción con código y etiqueta (tipos, estados y papeles de un caso).</summary>
    public class OpcionCaso
    {
        public string Codigo { get; set; }
        public string Etiqueta { get; set; }
    }

    /// <summary>
    /// Catálogos de Disciplina (espejo de Skuul IncidentCategory / IncidentStatus / IncidentParticipantRole).
    /// Mismas transiciones de estado que Skuul; el primer estado de cada lista es la sugerencia.
    /// </summary>
    public static class CatalogoCasos
    {
        public static readonly IReadOnlyList<OpcionCaso> Tipos = new List<OpcionCaso>
        {
            new OpcionCaso { Codigo = "conducta",   Etiqueta = "Conducta" },
            new OpcionCaso { Codigo = "proteccion", Etiqueta = "Protección del menor" },
        };

        public static readonly IReadOnlyList<OpcionCaso> Estados = new List<OpcionCaso>
        {
            new OpcionCaso { Codigo = "reportado",   Etiqueta = "Reportado" },
            new OpcionCaso { Codigo = "en_revision", Etiqueta = "En revisión" },
            new OpcionCaso { Codigo = "derivado",    Etiqueta = "Derivado" },
            new OpcionCaso { Codigo = "con_medidas", Etiqueta = "Con medidas" },
            new OpcionCaso { Codigo = "resuelto",    Etiqueta = "Resuelto" },
            new OpcionCaso { Codigo = "cerrado",     Etiqueta = "Cerrado" },
        };

        public static readonly IReadOnlyList<OpcionCaso> Papeles = new List<OpcionCaso>
        {
            new OpcionCaso { Codigo = "implicado", Etiqueta = "Implicado en el caso" },
            new OpcionCaso { Codigo = "afectado",  Etiqueta = "Afectado" },
            new OpcionCaso { Codigo = "testigo",   Etiqueta = "Testigo" },
            new OpcionCaso { Codigo = "reporta",   Etiqueta = "Quien reporta" },
            new OpcionCaso { Codigo = "atiende",   Etiqueta = "Quien atiende" },
        };

        private static readonly Dictionary<string, string[]> Siguientes = new Dictionary<string, string[]>
        {
            ["reportado"]   = new[] { "en_revision", "derivado", "con_medidas", "resuelto", "cerrado" },
            ["en_revision"] = new[] { "derivado", "con_medidas", "resuelto", "cerrado" },
            ["derivado"]    = new[] { "en_revision", "con_medidas", "resuelto", "cerrado" },
            ["con_medidas"] = new[] { "en_revision", "resuelto", "cerrado" },
            ["resuelto"]    = new[] { "cerrado", "en_revision" },
            ["cerrado"]     = new string[0],
        };

        private static string Etiqueta(IReadOnlyList<OpcionCaso> lista, string codigo) =>
            lista.FirstOrDefault(x => x.Codigo == codigo)?.Etiqueta ?? codigo ?? "";

        public static string Tipo(string codigo) => Etiqueta(Tipos, codigo);
        public static string Estado(string codigo) => Etiqueta(Estados, codigo);
        public static string Papel(string codigo) => Etiqueta(Papeles, codigo);

        public static bool ExisteTipo(string codigo) => Tipos.Any(x => x.Codigo == codigo);
        public static bool ExisteEstado(string codigo) => Estados.Any(x => x.Codigo == codigo);

        /// <summary>¿El caso todavía acepta trabajo? (resuelto y cerrado, no)</summary>
        public static bool EstaAbierto(string estado) => estado != "resuelto" && estado != "cerrado";

        public static List<OpcionCaso> SiguientesDe(string estado) =>
            (Siguientes.TryGetValue(estado ?? "", out string[] s) ? s : new string[0])
                .Select(c => new OpcionCaso { Codigo = c, Etiqueta = Estado(c) }).ToList();
    }

    public class CasoDisciplinaListaViewModel
    {
        public int Cas_Id { get; set; }
        public string Cas_Referencia { get; set; }
        public string Cas_Tipo { get; set; }
        public string Cas_Estado { get; set; }
        public bool Cas_EsReservado { get; set; }
        public string Cas_Resumen { get; set; }
        public string Cas_Lugar { get; set; }
        public DateTime Cas_FechaOcurrio { get; set; }
        public int? Emp_IdAtiende { get; set; }
        public string Atiende { get; set; }
        public int Personas { get; set; }
        public int TotalFilas { get; set; }
    }

    public class CasosAbiertosViewModel
    {
        public int Abiertos { get; set; }
    }

    public class CasoDisciplinaViewModel
    {
        public int Cas_Id { get; set; }
        public string Cas_Referencia { get; set; }
        public string Cas_Tipo { get; set; }
        public string Cas_Estado { get; set; }
        public bool Cas_EsReservado { get; set; }
        public string Cas_Resumen { get; set; }
        public string Cas_Descripcion { get; set; }
        public string Cas_Lugar { get; set; }
        public DateTime Cas_FechaOcurrio { get; set; }
        public DateTime Cas_FechaRegistra { get; set; }
        public int? Ani_Id { get; set; }
        public string Ani_Nombre { get; set; }
        public int? Pan_Id { get; set; }
        public string Pan_Nombre { get; set; }
        public int? Cas_UsuarioReporta { get; set; }
        public string Reporta { get; set; }
        public int? Emp_IdAtiende { get; set; }
        public string Atiende { get; set; }
        public int Pendientes { get; set; }
        public bool EsDeLaPersona { get; set; }
    }

    public class CasoPersonaViewModel
    {
        public int Cpe_Id { get; set; }
        public int? Alu_Id { get; set; }
        public int? Emp_Id { get; set; }
        public string Cpe_Papel { get; set; }
        public string Cpe_Nota { get; set; }
        public string Nombre { get; set; }
        public string Identidad { get; set; }
        public string Ubicacion { get; set; }
    }

    public class CasoAccionViewModel
    {
        public int Cac_Id { get; set; }
        public string Cac_Tipo { get; set; }
        public string Cac_Descripcion { get; set; }
        public DateTime? Cac_FechaLimite { get; set; }
        public DateTime? Cac_FechaHecha { get; set; }
        public int? Emp_IdResponsable { get; set; }
        public string Responsable { get; set; }
        public bool Pendiente => !Cac_FechaHecha.HasValue;
    }

    public class CasoEstadoViewModel
    {
        public int Ces_Id { get; set; }
        public string Ces_De { get; set; }
        public string Ces_A { get; set; }
        public string Ces_Motivo { get; set; }
        public DateTime Ces_Fecha { get; set; }
        public string Usuario { get; set; }
    }

    public class CasoNotaViewModel
    {
        public int Cno_Id { get; set; }
        public string Cno_Texto { get; set; }
        public bool Cno_EsPrivada { get; set; }
        public DateTime Cno_Fecha { get; set; }
        public string Usuario { get; set; }
    }

    public class CasoAlumnoViewModel
    {
        public int Alu_Id { get; set; }
        public string Nombre { get; set; }
        public string Identidad { get; set; }
        public string Ubicacion { get; set; }
    }

    public class CasoEmpleadoViewModel
    {
        public int Emp_Id { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
    }

    // ───────────── Lo que se manda a la API ─────────────

    public class CasoRegistrarViewModel
    {
        public string Cas_Resumen { get; set; }
        public string Cas_Tipo { get; set; }
        public string Cas_Descripcion { get; set; }
        public string Cas_Lugar { get; set; }
        public DateTime Cas_FechaOcurrio { get; set; }
        public int? Emp_IdAtiende { get; set; }
        public List<CasoPersonaRegistrarViewModel> Personas { get; set; } = new List<CasoPersonaRegistrarViewModel>();
        public int? Usuario { get; set; }
    }

    public class CasoPersonaRegistrarViewModel
    {
        public int? Alu_Id { get; set; }
        public string Papel { get; set; }
        public string Nota { get; set; }
    }

    public class CasoCambiarEstadoViewModel
    {
        public int Cas_Id { get; set; }
        public string Estado { get; set; }
        public string Motivo { get; set; }
        public int? Usuario { get; set; }
    }

    public class CasoAgregarAccionViewModel
    {
        public int Cas_Id { get; set; }
        public string Cac_Tipo { get; set; }
        public string Cac_Descripcion { get; set; }
        public DateTime? Cac_FechaLimite { get; set; }
        public int? Emp_IdResponsable { get; set; }
        public int? Usuario { get; set; }
    }

    public class CasoAgregarNotaViewModel
    {
        public int Cas_Id { get; set; }
        public string Cno_Texto { get; set; }
        public bool Cno_EsPrivada { get; set; } = true;
        public int? Usuario { get; set; }
    }

    /// <summary>Respuesta de toda escritura de la API: { Ok, Id, Referencia, Estado, Mensaje }.</summary>
    public class CasoResultadoViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public string Referencia { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
    }
}
