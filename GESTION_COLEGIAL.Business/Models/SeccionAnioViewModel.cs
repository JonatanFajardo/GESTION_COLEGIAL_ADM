using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista de una Sección por Año + profesor guía (homeroom).
    /// Fase 4 — Clases y profesores (espejo de academic_cycle_sections de Skuul).
    /// </summary>
    public class SeccionAnioViewModel : BaseViewModel
    {
        /// <summary>Identificador único de la sección del año.</summary>
        [Key]
        public int Sea_Id { get; set; }

        /// <summary>Año lectivo al que pertenece.</summary>
        [Display(Name = "Año lectivo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Ani_Id { get; set; }

        /// <summary>Nivel educativo.</summary>
        [Display(Name = "Nivel")]
        [Required(ErrorMessage = "Seleccione un nivel")]
        public int Niv_Id { get; set; }

        /// <summary>Descripción del nivel (solo lectura, viene del SP List).</summary>
        public string Niv_Descripcion { get; set; }

        /// <summary>Grado académico (curso nivel).</summary>
        [Display(Name = "Grado")]
        [Required(ErrorMessage = "Seleccione un grado")]
        public int Cun_Id { get; set; }

        /// <summary>Descripción del grado (solo lectura).</summary>
        public string Cun_Descripcion { get; set; }

        /// <summary>Sección A/B/C.</summary>
        [Display(Name = "Sección")]
        [Required(ErrorMessage = "Seleccione una sección")]
        public int Sec_Id { get; set; }

        /// <summary>Descripción de la sección (solo lectura).</summary>
        public string Sec_Descripcion { get; set; }

        /// <summary>Modalidad (opcional).</summary>
        [Display(Name = "Modalidad")]
        public int? Mda_Id { get; set; }

        /// <summary>Descripción de la modalidad (solo lectura).</summary>
        public string Mda_Descripcion { get; set; }

        /// <summary>Profesor guía / homeroom (opcional).</summary>
        [Display(Name = "Profesor guía")]
        public int? Emp_Id { get; set; }

        /// <summary>Nombre del profesor guía (solo lectura).</summary>
        public string Emp_Nombre { get; set; }

        /// <summary>Etiqueta visible, ej. "7mo A".</summary>
        [Display(Name = "Nombre")]
        [StringLength(60)]
        public string Sea_Nombre { get; set; }

        /// <summary>Cupo máximo de alumnos.</summary>
        [Display(Name = "Cupo")]
        public int? Sea_Cupo { get; set; }

        /// <summary>Estado: 1 Borrador · 2 Planificada · 3 Activa · 4 Suspendida · 5 Cerrada · 6 Archivada.</summary>
        [Display(Name = "Estado")]
        public byte Sea_Estado { get; set; }

        /// <summary>Descripción del estado (solo lectura, viene del SP List).</summary>
        public string Sea_EstadoDescripcion { get; set; }
    }
}
