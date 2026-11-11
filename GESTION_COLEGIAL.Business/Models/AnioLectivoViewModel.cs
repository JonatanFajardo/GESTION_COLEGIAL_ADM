using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista del Año Lectivo (entidad de la capa multi-año).
    /// </summary>
    public class AnioLectivoViewModel : BaseViewModel
    {
        /// <summary>Identificador único del año lectivo.</summary>
        [Key]
        public int Ani_Id { get; set; }

        /// <summary>Año base (ej. 2026).</summary>
        [Display(Name = "Año")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Range(1900, 2100, ErrorMessage = "Ingrese un año válido")]
        public int Ani_Anio { get; set; }

        /// <summary>Nombre visible (ej. "2026" o "2026-2027").</summary>
        [Display(Name = "Nombre")]
        [StringLength(20)]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Ani_Nombre { get; set; }

        /// <summary>Fecha de inicio del ciclo.</summary>
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime? Ani_FechaInicio { get; set; }

        /// <summary>Fecha de fin del ciclo.</summary>
        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)]
        public DateTime? Ani_FechaFin { get; set; }

        /// <summary>Estado del ciclo de vida: 1 Borrador · 2 Programado · 3 Abierto · 4 Cerrando · 5 Cerrado · 6 Archivado.</summary>
        [Display(Name = "Estado")]
        public byte Ani_Estado { get; set; }

        /// <summary>Descripción del estado (solo lectura, viene del SP List).</summary>
        public string Ani_EstadoDescripcion { get; set; }

        /// <summary>Indica si el año está activo (habilitado).</summary>
        public bool Ani_EsActivo { get; set; }

        /// <summary>Indica si es el año de trabajo actual (seleccionado como activo).</summary>
        public bool Ani_EsActual { get; set; }
    }
}
