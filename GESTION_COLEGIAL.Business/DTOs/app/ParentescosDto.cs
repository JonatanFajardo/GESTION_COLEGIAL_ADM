#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Relación entre el encargado legal y el alumno.
    /// </summary>
    public partial class tbParentescos
    {
        public tbParentescos()
        {
            tbEncargados_tbAlumnos = new HashSet<tbEncargados_tbAlumnos>();
        }

        /// <summary>
        /// Identificador único de un parentesco.
        /// </summary>
        public int Par_Id { get; set; }

        /// <summary>
        /// Descripción sobre el parentesco sangíneo o no sangíneo del encargado.
        /// </summary>
        public string Par_Descripcion { get; set; }

        public bool Par_EsEliminado { get; set; }
        public int Par_UsuarioRegistra { get; set; }
        public DateTime Par_FechaRegistra { get; set; }
        public int? Par_UsuarioModifica { get; set; }
        public DateTime? Par_FechaModifica { get; set; }

        public virtual tbUsuarios Par_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Par_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbEncargados_tbAlumnos> tbEncargados_tbAlumnos { get; set; }
    }
}