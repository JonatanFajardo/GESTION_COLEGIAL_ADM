using GESTION_COLEGIAL.Business.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Formatos de fecha del plan de apoyo ("12 nov 2026", "12 nov 2026, 14:30").</summary>
    public static class FormatoPlan
    {
        public static readonly CultureInfo Cultura = new CultureInfo("es-HN");
        public static string Dia(DateTime d) => d.ToString("d MMM yyyy", Cultura).Replace(".", "");
        public static string Dia(DateTime? d) => d.HasValue ? Dia(d.Value) : "—";
        public static string DiaHora(DateTime d) => Dia(d) + ", " + d.ToString("HH:mm", Cultura);
        public static string Iso(DateTime? d) => d.HasValue ? d.Value.ToString("yyyy-MM-dd") : "";

        /// <summary>Grupo del selector de responsables según el rol del usuario.</summary>
        public static string GrupoRol(string rol) => (rol ?? "").Trim().ToLowerInvariant() switch
        {
            "director" => "Dirección",
            "docente" => "Docentes",
            "admin" => "Administración",
            _ => "Otros"
        };

        /// <summary>Iniciales para el avatar ("Francisco Pineda" → "FP").</summary>
        public static string Iniciales(string nombre) =>
            string.Concat((nombre ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpperInvariant(p[0])));
    }

    /// <summary>Directorio "Planes de apoyo".</summary>
    public class PlanesApoyoIndexViewModel
    {
        public bool PuedeGestionar { get; set; }
        public IEnumerable<SelectListItem> Niveles { get; set; } = Enumerable.Empty<SelectListItem>();
        /// <summary>Filtros de entrada desde un enlace (?estado=activo&amp;vencidos=true).</summary>
        public string Estado { get; set; }
        public string Categoria { get; set; }
        public bool SoloVencidos { get; set; }
        public bool SoloMios { get; set; }
    }

    /// <summary>Pantalla "Nuevo plan de apoyo".</summary>
    public class PlanApoyoNuevoPaginaViewModel
    {
        public List<PlanApoyoUsuarioViewModel> Usuarios { get; set; } = new List<PlanApoyoUsuarioViewModel>();
        public int? AluId { get; set; }
        public string Categoria { get; set; }
        public int? UsuarioActual { get; set; }
        public string Hoy { get; set; }
        public string RevisionSugerida { get; set; }
    }

    /// <summary>Detalle del plan: datos, pasos, notas, historial y lo que la persona puede hacer.</summary>
    public class PlanApoyoPaginaViewModel
    {
        public PlanApoyoViewModel Plan { get; set; }
        public List<PlanApoyoPasoViewModel> Pasos { get; set; } = new List<PlanApoyoPasoViewModel>();
        public List<PlanApoyoNotaViewModel> Notas { get; set; } = new List<PlanApoyoNotaViewModel>();
        public List<PlanApoyoEstadoViewModel> Historial { get; set; } = new List<PlanApoyoEstadoViewModel>();
        public List<PlanApoyoUsuarioViewModel> Usuarios { get; set; } = new List<PlanApoyoUsuarioViewModel>();
        public List<OpcionPlan> Siguientes { get; set; } = new List<OpcionPlan>();
        public bool PuedeGestionar { get; set; }
        public string Aviso { get; set; }
        public string Hoy { get; set; }

        public bool EstaAbierto => Plan?.EstaAbierto ?? false;
        /// <summary>Gestiona y el plan sigue abierto: editar datos y agregar o quitar pasos.</summary>
        public bool PuedeEditar => PuedeGestionar && EstaAbierto;
        /// <summary>Cualquiera que lee el plan escribe notas mientras siga abierto.</summary>
        public bool PuedeAnotar => EstaAbierto;
        public bool PuedeMarcar(PlanApoyoPasoViewModel p) => EstaAbierto && !p.Hecho && (PuedeGestionar || p.EsMio);
        public int Pendientes => Pasos.Count(p => !p.Hecho);
        public int Atrasados => Pasos.Count(p => p.Atrasado);
    }

    /// <summary>Hoja tamaño carta del plan.</summary>
    public class PlanApoyoImprimirViewModel
    {
        public PlanApoyoViewModel Plan { get; set; }
        public List<PlanApoyoPasoViewModel> Pasos { get; set; } = new List<PlanApoyoPasoViewModel>();
        public List<PlanApoyoNotaViewModel> Notas { get; set; } = new List<PlanApoyoNotaViewModel>();
        public List<PlanApoyoEstadoViewModel> Historial { get; set; } = new List<PlanApoyoEstadoViewModel>();
        public string Institucion { get; set; }
        public string Direccion { get; set; }
        public DateTime Impreso { get; set; }
    }
}
