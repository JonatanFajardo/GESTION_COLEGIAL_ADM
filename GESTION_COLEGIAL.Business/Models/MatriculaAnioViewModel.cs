using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista de la Matrícula por Año (expediente/transcript + promociones).
    /// Espejo de academic_year_student_record de Skuul. Cubre tanto el listado por
    /// sección-año como el historial del alumno (campos de solo lectura por JOIN).
    /// </summary>
    public class MatriculaAnioViewModel : BaseViewModel
    {
        /// <summary>Identificador único de la matrícula-año.</summary>
        [Key]
        public int Mta_Id { get; set; }

        /// <summary>Alumno matriculado.</summary>
        [Display(Name = "Alumno")]
        public int Alu_Id { get; set; }

        /// <summary>Persona asociada al alumno (solo lectura).</summary>
        public int Per_Id { get; set; }

        /// <summary>Nombre completo del alumno (solo lectura, viene del SP).</summary>
        [Display(Name = "Alumno")]
        public string Alu_Nombre { get; set; }

        /// <summary>Año lectivo de la matrícula.</summary>
        [Display(Name = "Año lectivo")]
        public int Ani_Id { get; set; }

        /// <summary>Año base (solo lectura, expediente).</summary>
        public int Ani_Anio { get; set; }

        /// <summary>Nombre del año lectivo (solo lectura, expediente).</summary>
        [Display(Name = "Año")]
        public string Ani_Nombre { get; set; }

        /// <summary>Sección-año donde cae el alumno (opcional).</summary>
        [Display(Name = "Sección")]
        public int? Sea_Id { get; set; }

        /// <summary>Etiqueta de la sección-año (solo lectura, expediente).</summary>
        public string Sea_Nombre { get; set; }

        /// <summary>Estado: 1 Activa · 2 Promovido · 3 Repite · 4 Retirado · 5 Graduado.</summary>
        [Display(Name = "Estado")]
        public byte Mta_Estado { get; set; }

        /// <summary>Descripción del estado (solo lectura, viene del SP).</summary>
        public string Mta_EstadoDescripcion { get; set; }
    }
}
