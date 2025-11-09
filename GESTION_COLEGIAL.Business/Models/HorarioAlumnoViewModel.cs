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
        public int HorarioAlumnoId { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del curso.
        /// </summary>
        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int CursoId { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del nivel del curso.
        /// </summary>
        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int CursoNivelId { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la materia.
        /// </summary>
        [Display(Name = "Materia")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int MateriaId { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de inicio del horario.
        /// </summary>
        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int HoraInicioHorarioAlumno { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de finalización del horario.
        /// </summary>
        [Display(Name = "Hora finaliza")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int HoraFinHorarioAlumno { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del día.
        /// </summary>
        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int DiaId { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int UsuarioRegistraHorarioAlumnoId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string NombreUsuarioRegistraHorarioAlumno { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del horario del alumno.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistroHorarioAlumno { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? UsuarioModificaHorarioAlumnoId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el horario del alumno.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string NombreUsuarioModificaHorarioAlumno { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del horario del alumno.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? FechaModificacionHorarioAlumno { get; set; }
    }
}
