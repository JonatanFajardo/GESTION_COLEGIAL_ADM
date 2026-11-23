using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Biblioteca de temarios (/temarios).</summary>
    public class TemariosIndexViewModel
    {
        public bool PuedeEditar { get; set; }
        public bool PuedeSupervisar { get; set; }
        public int AnioActual { get; set; }
        public List<TemarioOpcionViewModel> Niveles { get; set; } = new List<TemarioOpcionViewModel>();
        public List<TemarioOpcionViewModel> Semestres { get; set; } = new List<TemarioOpcionViewModel>();
        public List<TemarioOpcionViewModel> Anios { get; set; } = new List<TemarioOpcionViewModel>();
    }

    /// <summary>Un temario: temas por semana y avance por sección (/temarios/temario/5).</summary>
    public class TemarioPaginaViewModel
    {
        public TemarioViewModel Temario { get; set; }
        /// <summary>"Editar temarios" en la sesión (crear / copiar a un temario nuevo).</summary>
        public bool PuedeCrear { get; set; }
        public string Tab { get; set; }
        public string Aviso { get; set; }
        public string Hoy { get; set; }
        public List<TemarioOpcionViewModel> Semestres { get; set; } = new List<TemarioOpcionViewModel>();
        public List<TemarioOpcionViewModel> Anios { get; set; } = new List<TemarioOpcionViewModel>();
    }

    /// <summary>Hoja tamaño carta del temario (/temarios/imprimir/5).</summary>
    public class TemarioImprimirViewModel
    {
        public TemarioViewModel Temario { get; set; }
        public List<TemarioTemaViewModel> Temas { get; set; } = new List<TemarioTemaViewModel>();
        public string Institucion { get; set; }
        public string Direccion { get; set; }
        public DateTime Impreso { get; set; }
    }

    /// <summary>Resumen de Dirección (/temarios/resumen).</summary>
    public class TemariosResumenPaginaViewModel
    {
        public bool PuedeEditar { get; set; }
        public int AnioActual { get; set; }
        public List<TemarioOpcionViewModel> Niveles { get; set; } = new List<TemarioOpcionViewModel>();
        public List<TemarioOpcionViewModel> Semestres { get; set; } = new List<TemarioOpcionViewModel>();
        public List<TemarioOpcionViewModel> Anios { get; set; } = new List<TemarioOpcionViewModel>();
    }

    /// <summary>Ventana "Copiar temario" (biblioteca y ficha). Con OrigenId el origen queda fijo.</summary>
    public class TemarioCopiarModalViewModel
    {
        public int AnioActual { get; set; }
        public int? OrigenId { get; set; }
        public string OrigenTexto { get; set; }
        public List<TemarioOpcionViewModel> Semestres { get; set; } = new List<TemarioOpcionViewModel>();
        public List<TemarioOpcionViewModel> Anios { get; set; } = new List<TemarioOpcionViewModel>();
    }
}
