using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class HorarioAlumnoViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del horario del alumno.
        /// </summary>
        [Key]
        public int HoAl_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del curso.
        /// </summary>
        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cur_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del nivel del curso.
        /// </summary>
        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cun_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la materia.
        /// </summary>
        [Display(Name = "Materia")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Mat_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de inicio del horario.
        /// </summary>
        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int HoAl_HoraInicio { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de finalización del horario.
        /// </summary>
        [Display(Name = "Hora finaliza")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int HoAl_HoraFinaliza { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del día.
        /// </summary>
        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Dia_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la sección.
        /// </summary>
        [Display(Name = "Sección")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sec_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del aula.
        /// </summary>
        [Display(Name = "Aula")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Aul_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del empleado (profesor).
        /// </summary>
        [Display(Name = "Profesor")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Emp_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del semestre.
        /// </summary>
        [Display(Name = "Semestre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sem_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la modalidad.
        /// </summary>
        [Display(Name = "Modalidad")]
        public int? Mda_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el año académico.
        /// </summary>
        [Display(Name = "Año Académico")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int HoAl_Año { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int HoAl_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string HoAl_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del horario del alumno.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime HoAl_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? HoAl_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string HoAl_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del horario del alumno.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? HoAl_FechaModifica { get; set; }
    }
}