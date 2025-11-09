using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class HorarioProfesorViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador del horario del profesor.
        /// </summary>
        [Key]
        public int HorarioProfesorId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del curso al que está asociado el horario del profesor.
        /// </summary>
        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int CursoId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del nivel del curso al que está asociado el horario del profesor.
        /// </summary>
        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int CursoNivelId { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de inicio del horario del profesor.
        /// </summary>
        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int HoraInicioHorarioProfesor { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de finalización del horario del profesor.
        /// </summary>
        [Display(Name = "Hora finaliza")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int HoraFinHorarioProfesor { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del día al que está asociado el horario del profesor.
        /// </summary>
        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int DiaId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del usuario que registra el horario del profesor.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraHorarioProfesorId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registra el horario del profesor.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraHorarioProfesor { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del horario del profesor.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroHorarioProfesor { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del usuario que modifica el horario del profesor.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaHorarioProfesorId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el horario del profesor.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaHorarioProfesor { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del horario del profesor.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionHorarioProfesor { get; set; }
    }
}

