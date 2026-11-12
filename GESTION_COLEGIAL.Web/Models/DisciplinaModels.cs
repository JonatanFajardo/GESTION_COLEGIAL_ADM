using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Formatos de fecha de Disciplina (como Skuul: "12 nov 2026" y "14:30").</summary>
    public static class FormatoCaso
    {
        public static readonly CultureInfo Cultura = new CultureInfo("es-HN");
        public static string Dia(DateTime d) => d.ToString("d MMM yyyy", Cultura).Replace(".", "");
        public static string Hora(DateTime d) => d.ToString("HH:mm", Cultura);
        public static string DiaHora(DateTime d) => Dia(d) + ", " + Hora(d);
    }

    /// <summary>Pantalla "Casos": filtros, contador de abiertos y una página de casos.</summary>
    public class CasosIndexViewModel
    {
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public bool SoloAbiertos { get; set; }
        public int Pagina { get; set; } = 1;
        public int Total { get; set; }
        public int TamanoPagina { get; set; } = 20;
        public int Abiertos { get; set; }
        public bool PuedeRegistrar { get; set; }
        public string Aviso { get; set; }
        public List<CasoDisciplinaListaViewModel> Casos { get; set; } = new List<CasoDisciplinaListaViewModel>();

        public bool HayFiltros => !string.IsNullOrEmpty(Estado) || !string.IsNullOrEmpty(Tipo) || SoloAbiertos;
        public int Paginas => Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
        public int Desde => Total == 0 ? 0 : (Pagina - 1) * TamanoPagina + 1;
        public int Hasta => Math.Min(Total, Pagina * TamanoPagina);
    }

    /// <summary>Pantalla "Registrar un caso".</summary>
    public class CasoRegistrarPaginaViewModel
    {
        public DateTime Ahora { get; set; }
        public List<CasoAlumnoViewModel> Alumnos { get; set; } = new List<CasoAlumnoViewModel>();
        public List<CasoEmpleadoViewModel> Empleados { get; set; } = new List<CasoEmpleadoViewModel>();
        public int MaxPersonas { get; set; } = 20;
    }

    /// <summary>Pantalla del caso: resumen, personas, acciones, notas e historial.</summary>
    public class CasoPaginaViewModel
    {
        public CasoDisciplinaViewModel Caso { get; set; }
        public bool PuedeAtender { get; set; }
        public string Aviso { get; set; }
        public List<CasoPersonaViewModel> Personas { get; set; } = new List<CasoPersonaViewModel>();
        public List<CasoAccionViewModel> Acciones { get; set; } = new List<CasoAccionViewModel>();
        public List<CasoNotaViewModel> Notas { get; set; } = new List<CasoNotaViewModel>();
        public List<CasoEstadoViewModel> Historial { get; set; } = new List<CasoEstadoViewModel>();
        public List<CasoEmpleadoViewModel> Empleados { get; set; } = new List<CasoEmpleadoViewModel>();
        public List<OpcionCaso> Siguientes { get; set; } = new List<OpcionCaso>();

        public bool EstaAbierto => CatalogoCasos.EstaAbierto(Caso?.Cas_Estado);
    }
}
