#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Administra las horas de labor diaria.
    /// </summary>
    public partial class tbHoras
    {
        public tbHoras()
        {
            tbHorarioAlumnosHoAl_HoraFinalizaNavigation = new HashSet<tbHorarioAlumnos>();
            tbHorarioAlumnosHoAl_HoraInicioNavigation = new HashSet<tbHorarioAlumnos>();
            tbHorarioProfesoresHoPr_HoraFinalizaNavigation = new HashSet<tbHorarioProfesores>();
            tbHorarioProfesoresHoPr_HoraInicioNavigation = new HashSet<tbHorarioProfesores>();
        }

        /// <summary>
        /// Identificador único de la hora.
        /// </summary>
        public int Hor_Id { get; set; }

        /// <summary>
        /// Descripción de la hora indicada.
        /// </summary>
        public string Hor_Hora { get; set; }

        public bool Hor_EsEliminado { get; set; }
        public int Hor_UsuarioRegistra { get; set; }
        public DateTime Hor_FechaRegistra { get; set; }
        public int? Hor_UsuarioModifica { get; set; }
        public DateTime? Hor_FechaModifica { get; set; }

        public virtual ICollection<tbHorarioAlumnos> tbHorarioAlumnosHoAl_HoraFinalizaNavigation { get; set; }
        public virtual ICollection<tbHorarioAlumnos> tbHorarioAlumnosHoAl_HoraInicioNavigation { get; set; }
        public virtual ICollection<tbHorarioProfesores> tbHorarioProfesoresHoPr_HoraFinalizaNavigation { get; set; }
        public virtual ICollection<tbHorarioProfesores> tbHorarioProfesoresHoPr_HoraInicioNavigation { get; set; }
    }
}