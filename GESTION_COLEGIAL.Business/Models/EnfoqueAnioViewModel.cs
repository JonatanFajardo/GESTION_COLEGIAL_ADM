using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista del Enfoque de enseñanza por año lectivo (Fase 3).
    /// 1 registro por año (Ani_Id). Espejo del Instructional Model de Skuul.
    /// </summary>
    public class EnfoqueAnioViewModel : BaseViewModel
    {
        /// <summary>Identificador único del enfoque.</summary>
        [Key]
        public int Enf_Id { get; set; }

        /// <summary>Año lectivo al que pertenece.</summary>
        [Required(ErrorMessage = "El campo es requerido")]
        public int Ani_Id { get; set; }

        /// <summary>Modelo de agrupación: grupo_fijo · hibrido · horario_materias.</summary>
        [Display(Name = "Enfoque de enseñanza")]
        [Required(ErrorMessage = "Seleccione un enfoque de enseñanza")]
        [StringLength(30)]
        public string Enf_Modelo { get; set; }

        /// <summary>Razón/justificación opcional de la elección.</summary>
        [Display(Name = "Razón")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string Enf_Razon { get; set; }
    }
}
