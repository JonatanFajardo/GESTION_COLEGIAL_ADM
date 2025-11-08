#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Almacena la hora a la que el profesor dará sus clases y recesos.
    /// </summary>
    public partial class tbHorarioProfesores
    {
        /// <summary>
        /// Identificador único de cada hora.
        /// </summary>
        public int HoPr_Id { get; set; }

        /// <summary>
        /// Identificador del curso.
        /// </summary>
        public int Cur_Id { get; set; }

        /// <summary>
        /// Identificador del curso.
        /// </summary>
        public int Cun_Id { get; set; }

        /// <summary>
        /// Hora a la que inicia.
        /// </summary>
        public int HoPr_HoraInicio { get; set; }

        /// <summary>
        /// Hora a la que finaliza.
        /// </summary>
        public int HoPr_HoraFinaliza { get; set; }

        /// <summary>
        /// Identificador único.
        /// </summary>
        public int Dia_Id { get; set; }

        public bool HoPr_EsEliminado { get; set; }
        public int HoPr_UsuarioRegistra { get; set; }
        public DateTime HoPr_FechaRegistra { get; set; }
        public int? HoPr_UsuarioModifica { get; set; }
        public DateTime? HoPr_FechaModifica { get; set; }

        public virtual tbCursosNiveles Cun { get; set; }
        public virtual tbCursos Cur { get; set; }
        public virtual tbDias Dia { get; set; }
        public virtual tbHoras HoPr_HoraFinalizaNavigation { get; set; }
        public virtual tbHoras HoPr_HoraInicioNavigation { get; set; }
    }
}