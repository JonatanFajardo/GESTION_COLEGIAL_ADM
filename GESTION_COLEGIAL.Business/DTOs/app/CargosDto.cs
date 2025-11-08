#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra los puestos que puede tener un empleado en el colegio.
    /// </summary>
    public partial class tbCargos
    {
        public tbCargos()
        {
            tbEmpleados = new HashSet<tbEmpleados>();
        }

        /// <summary>
        /// Identificador único del puesto.
        /// </summary>
        public int Car_Id { get; set; }

        /// <summary>
        /// Dato informativo del cargo.
        /// </summary>
        public string Car_Descripcion { get; set; }

        public bool Car_EsEliminado { get; set; }
        public int Car_UsuarioRegistra { get; set; }
        public DateTime Car_FechaRegistra { get; set; }
        public int? Car_UsuarioModifica { get; set; }
        public DateTime? Car_FechaModifica { get; set; }

        public virtual tbUsuarios Car_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Car_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbEmpleados> tbEmpleados { get; set; }
    }
}