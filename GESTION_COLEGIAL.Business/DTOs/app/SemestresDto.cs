#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Periodo por año de la materia.
    /// </summary>
    public partial class tbSemestres
    {
        public tbSemestres()
        {
            tbNotas = new HashSet<tbNotas>();
        }

        /// <summary>
        /// Identificador único de la tabla.
        /// </summary>
        public int Sem_Id { get; set; }

        /// <summary>
        /// Información sobre cual es el semestre.
        /// </summary>
        public string Sem_Descripcion { get; set; }

        public bool Sem_EsActivo { get; set; }
        public bool Sem_EsEliminado { get; set; }
        public int Sem_UsuarioRegistra { get; set; }
        public DateTime Sem_FechaRegistra { get; set; }
        public int? Sem_UsuarioModifica { get; set; }
        public DateTime? Sem_FechaModifica { get; set; }

        public virtual tbUsuarios Sem_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Sem_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbNotas> tbNotas { get; set; }
    }
}