#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra los días laborales.
    /// </summary>
    public partial class tbDias
    {
        public tbDias()
        {
            tbHorarioAlumnos = new HashSet<tbHorarioAlumnos>();
            tbHorarioProfesores = new HashSet<tbHorarioProfesores>();
            tbModalidad_tbDias = new HashSet<tbModalidad_tbDias>();
        }

        /// <summary>
        /// Identificador único de los dias.
        /// </summary>
        public int Dia_Id { get; set; }

        /// <summary>
        /// Nombre del dia.
        /// </summary>
        public string Dia_Descripcion { get; set; }

        public bool Dia_EsEliminado { get; set; }
        public int Dia_UsuarioRegistra { get; set; }
        public DateTime Dia_FechaRegistra { get; set; }
        public int? Dia_UsuarioModifica { get; set; }
        public DateTime? Dia_FechaModifica { get; set; }

        public virtual tbUsuarios Dia_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Dia_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbHorarioAlumnos> tbHorarioAlumnos { get; set; }
        public virtual ICollection<tbHorarioProfesores> tbHorarioProfesores { get; set; }
        public virtual ICollection<tbModalidad_tbDias> tbModalidad_tbDias { get; set; }
    }
}