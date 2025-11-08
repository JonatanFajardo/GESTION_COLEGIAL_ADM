#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra la cantidad de secciones que puede haber en el centro estudiantil.
    /// </summary>
    public partial class tbSecciones
    {
        public tbSecciones()
        {
            tbAlumnos = new HashSet<tbAlumnos>();
            tbCursos_tbSecciones = new HashSet<tbCursos_tbSecciones>();
        }

        /// <summary>
        /// Identificador único de la sección.
        /// </summary>
        public int Sec_Id { get; set; }

        /// <summary>
        /// Nombre de la sección.
        /// </summary>
        public string Sec_Descripcion { get; set; }

        public bool Sec_EsEliminado { get; set; }
        public int Sec_UsuarioRegistra { get; set; }
        public DateTime Sec_FechaRegistra { get; set; }
        public int? Sec_UsuarioModifica { get; set; }
        public DateTime? Sec_FechaModifica { get; set; }

        public virtual tbUsuarios Sec_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Sec_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbAlumnos> tbAlumnos { get; set; }
        public virtual ICollection<tbCursos_tbSecciones> tbCursos_tbSecciones { get; set; }
    }
}