using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista de un Período de reporte del año lectivo (Fase 2).
    /// Subdivide un año lectivo en parciales/semestres/trimestres con fechas.
    /// </summary>
    public class PeriodoAnioViewModel : BaseViewModel
    {
        /// <summary>Identificador único del período.</summary>
        [Key]
        public int Pan_Id { get; set; }

        /// <summary>Año lectivo al que pertenece (FK a Ani_Id).</summary>
        [Display(Name = "Año lectivo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Ani_Id { get; set; }

        /// <summary>Nombre visible (ej. "I Parcial", "Primer Semestre").</summary>
        [Display(Name = "Nombre")]
        [StringLength(40)]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Pan_Nombre { get; set; }

        /// <summary>Tipo: parcial / semestre / trimestre / bimestre / otro.</summary>
        [Display(Name = "Tipo")]
        [StringLength(20)]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Pan_Tipo { get; set; }

        /// <summary>Orden de aparición dentro del año.</summary>
        [Display(Name = "Orden")]
        public short Pan_Orden { get; set; }

        /// <summary>Fecha de inicio del período.</summary>
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime? Pan_FechaInicio { get; set; }

        /// <summary>Fecha de fin del período.</summary>
        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)]
        public DateTime? Pan_FechaFin { get; set; }

        /// <summary>Estado del ciclo de vida: 1 Borrador · 2 Programado · 3 Abierto · 4 Cerrando · 5 Cerrado · 6 Archivado.</summary>
        [Display(Name = "Estado")]
        public byte Pan_Estado { get; set; }

        /// <summary>Descripción del estado (solo lectura, viene del SP List).</summary>
        public string Pan_EstadoDescripcion { get; set; }

        /// <summary>Indica si el período está activo (habilitado).</summary>
        public bool Pan_EsActivo { get; set; }
    }
}
