#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Almacena la hora a la que el alumno recibirá sus clases y recesos.
    /// </summary>
    public partial class tbHorarioAlumnos
    {
        /// <summary>
        /// Identificador único de la tabla.
        /// </summary>
        public int HoAl_Id { get; set; }

        /// <summary>
        /// Identificador del curso.
        /// </summary>
        public int Cur_Id { get; set; }

        /// <summary>
        /// Identificador de curso niveles.
        /// </summary>
        public int Cun_Id { get; set; }

        /// <summary>
        /// Identificador de la materia.
        /// </summary>
        public int Mat_Id { get; set; }

        /// <summary>
        /// Fecha de inicio de las clases.
        /// </summary>
        public int HoAl_HoraInicio { get; set; }

        /// <summary>
        /// Fecha de finalizacion de las clases.
        /// </summary>
        public int HoAl_HoraFinaliza { get; set; }

        /// <summary>
        /// Identificador del dia.
        /// </summary>
        public int Dia_Id { get; set; }

        public bool HoAl_EsEliminado { get; set; }
        public int HoAl_UsuarioRegistra { get; set; }
        public DateTime HoAl_FechaRegistra { get; set; }
        public int? HoAl_UsuarioModifica { get; set; }
        public DateTime? HoAl_FechaModifica { get; set; }

        public virtual tbCursosNiveles Cun { get; set; }
        public virtual tbDias Dia { get; set; }
        public virtual tbHoras HoAl_HoraFinalizaNavigation { get; set; }
        public virtual tbHoras HoAl_HoraInicioNavigation { get; set; }
        public virtual tbUsuarios HoAl_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios HoAl_UsuarioRegistraNavigation { get; set; }
        public virtual tbMaterias Mat { get; set; }
    }
}