#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra las diferentes duraciones que puede tener una materia.
    /// </summary>
    public partial class tbDuraciones
    {
        public tbDuraciones()
        {
            tbMaterias = new HashSet<tbMaterias>();
        }

        /// <summary>
        /// Identificador único de la duracion.
        /// </summary>
        public int Dur_Id { get; set; }

        /// <summary>
        /// Información del tiempo designado para una materia.
        /// </summary>
        public string Dur_Descripcion { get; set; }

        public bool Dur_EsEliminado { get; set; }
        public int Dur_UsuarioRegistra { get; set; }
        public DateTime Dur_FechaRegistra { get; set; }
        public int? Dur_UsuarioModifica { get; set; }
        public DateTime? Dur_FechaModifica { get; set; }

        public virtual tbUsuarios Dur_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Dur_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbMaterias> tbMaterias { get; set; }
    }
}