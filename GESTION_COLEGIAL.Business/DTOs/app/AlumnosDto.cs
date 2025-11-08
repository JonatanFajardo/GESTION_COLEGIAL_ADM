#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Contiene la información de los alumnos.
    /// </summary>
    public partial class tbAlumnos
    {
        public tbAlumnos()
        {
            tbEncargados_tbAlumnos = new HashSet<tbEncargados_tbAlumnos>();
        }

        /// <summary>
        /// Identificador unico del tbAlumnos.
        /// </summary>
        public int Alu_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de la tbPersonas.
        /// </summary>
        public int Per_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbNivelesEducativos.
        /// </summary>
        public int? Niv_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbCursoNiveles.
        /// </summary>
        public int? Cun_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de la tbModalidades.
        /// </summary>
        public int? Mda_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbCursos.
        /// </summary>
        public int Cur_Id { get; set; }

        /// <summary>
        /// Identificador unico de tbSecciones.
        /// </summary>
        public int? Sec_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbEstados.
        /// </summary>
        public int Est_Id { get; set; }

        public int? AnioCursado { get; set; }
        public decimal? PromedioAnual { get; set; }

        public virtual tbCursosNiveles Cun { get; set; }
        public virtual tbCursos Cur { get; set; }
        public virtual tbEstados Est { get; set; }
        public virtual tbModalidades Mda { get; set; }
        public virtual tbNivelesEducativos Niv { get; set; }
        public virtual tbPersonas Per { get; set; }
        public virtual tbSecciones Sec { get; set; }
        public virtual ICollection<tbEncargados_tbAlumnos> tbEncargados_tbAlumnos { get; set; }
    }
}