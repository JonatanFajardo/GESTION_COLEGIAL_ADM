#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Especifica las diferentes modalidades en que pueden impartirse clases, estos son horas determinadas en la semana para que los alumnos asistan a clases.
    /// </summary>
    public partial class tbModalidades
    {
        public tbModalidades()
        {
            tbAlumnos = new HashSet<tbAlumnos>();
            tbModalidad_tbDias = new HashSet<tbModalidad_tbDias>();
            Cun = new HashSet<tbCursosNiveles>();
            Cur = new HashSet<tbCursos>();
        }

        /// <summary>
        /// Identificador único de la modalidad.
        /// </summary>
        public int Mda_Id { get; set; }

        /// <summary>
        /// Descripción por el cual se conoce la modalidad.
        /// </summary>
        public string Mda_Descripcion { get; set; }

        public bool Mda_EsEliminado { get; set; }
        public int Mda_UsuarioRegistra { get; set; }
        public DateTime Mda_FechaRegistra { get; set; }
        public int? Mda_UsuarioModifica { get; set; }
        public DateTime? Mda_FechaModifica { get; set; }

        public virtual tbUsuarios Mda_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Mda_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbAlumnos> tbAlumnos { get; set; }
        public virtual ICollection<tbModalidad_tbDias> tbModalidad_tbDias { get; set; }

        public virtual ICollection<tbCursosNiveles> Cun { get; set; }
        public virtual ICollection<tbCursos> Cur { get; set; }
    }
}