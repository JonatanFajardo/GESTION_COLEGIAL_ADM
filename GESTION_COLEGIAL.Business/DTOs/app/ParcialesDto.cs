#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Indica los parciales que tendrá un semestre
    /// </summary>
    public partial class tbParciales
    {
        public tbParciales()
        {
            tbNotas = new HashSet<tbNotas>();
        }

        /// <summary>
        /// Identificador único del parcial.
        /// </summary>
        public int Pac_Id { get; set; }

        /// <summary>
        /// Información por el cual se conocera el parcial.
        /// </summary>
        public string Pac_Descripcion { get; set; }

        public bool Pac_EsEliminado { get; set; }
        public int Pac_UsuarioRegistra { get; set; }
        public DateTime Pac_FechaRegistra { get; set; }
        public int? Pac_UsuarioModifica { get; set; }
        public DateTime? Pac_FechaModifica { get; set; }

        public virtual tbUsuarios Pac_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Pac_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbNotas> tbNotas { get; set; }
    }
}