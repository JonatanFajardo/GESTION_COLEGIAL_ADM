using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista de la Oferta de asignatura por año (Fase 5).
    /// Espejo de course_offerings + teaching_assignments: materia del año,
    /// en un grado/sección, con un docente asignado.
    /// </summary>
    public class OfertaAnioViewModel : BaseViewModel
    {
        /// <summary>Identificador único de la oferta.</summary>
        [Key]
        public int Ofa_Id { get; set; }

        /// <summary>Año lectivo (FK).</summary>
        [Display(Name = "Año lectivo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Ani_Id { get; set; }

        /// <summary>Materia ofertada (FK).</summary>
        [Display(Name = "Materia")]
        [Required(ErrorMessage = "Seleccione una materia")]
        public int Mat_Id { get; set; }

        /// <summary>Nombre de la materia (solo lectura, viene del SP List).</summary>
        public string Mat_Nombre { get; set; }

        /// <summary>Grado académico (FK, opcional).</summary>
        [Display(Name = "Grado")]
        public int? Cun_Id { get; set; }

        /// <summary>Descripción del grado (solo lectura, viene del SP List).</summary>
        public string Cun_Descripcion { get; set; }

        /// <summary>Sección-año (opcional; tabla creada en paralelo).</summary>
        [Display(Name = "Sección")]
        public int? Sea_Id { get; set; }

        /// <summary>Docente = empleado (FK, opcional).</summary>
        [Display(Name = "Docente")]
        public int? Emp_Id { get; set; }

        /// <summary>Nombre del docente (solo lectura, viene del SP List).</summary>
        public string Emp_Nombre { get; set; }

        /// <summary>Período (opcional; tabla creada en paralelo).</summary>
        [Display(Name = "Período")]
        public int? Pan_Id { get; set; }

        /// <summary>Horas semanales (opcional).</summary>
        [Display(Name = "Horas por semana")]
        public int? Ofa_HorasSemana { get; set; }

        /// <summary>Estado: 1 Borrador · 2 Programada · 3 Activa · 4 Suspendida · 5 Cerrada · 6 Archivada.</summary>
        [Display(Name = "Estado")]
        public byte Ofa_Estado { get; set; }

        /// <summary>Descripción del estado (solo lectura, viene del SP List).</summary>
        public string Ofa_EstadoDescripcion { get; set; }
    }
}
