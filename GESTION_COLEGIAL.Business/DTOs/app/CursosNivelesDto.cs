#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Administra el nivel educativo que tendrá un curso.
    /// </summary>
    public partial class tbCursosNiveles
    {
        public tbCursosNiveles()
        {
            tbAlumnos = new HashSet<tbAlumnos>();
            tbHorarioAlumnos = new HashSet<tbHorarioAlumnos>();
            tbHorarioProfesores = new HashSet<tbHorarioProfesores>();
            Cur = new HashSet<tbCursos>();
            Mda = new HashSet<tbModalidades>();
        }

        /// <summary>
        /// Identificador único de la tabla tbCursosNiveles.
        /// </summary>
        public int Cun_Id { get; set; }

        public int? Cun_Orden { get; set; }

        /// <summary>
        /// Información del curso nivel.
        /// </summary>
        public string Cun_Descripcion { get; set; }

        /// <summary>
        /// Identificador del nivel del curso.
        /// </summary>
        public int Niv_Id { get; set; }

        public bool Cun_EsEliminado { get; set; }
        public int Cun_UsuarioRegistra { get; set; }
        public DateTime Cun_FechaRegistra { get; set; }
        public int? Cun_UsuarioModifica { get; set; }
        public DateTime? Cun_FechaModifica { get; set; }

        public virtual tbNivelesEducativos Niv { get; set; }
        public virtual ICollection<tbAlumnos> tbAlumnos { get; set; }
        public virtual ICollection<tbHorarioAlumnos> tbHorarioAlumnos { get; set; }
        public virtual ICollection<tbHorarioProfesores> tbHorarioProfesores { get; set; }

        public virtual ICollection<tbCursos> Cur { get; set; }
        public virtual ICollection<tbModalidades> Mda { get; set; }
    }
}