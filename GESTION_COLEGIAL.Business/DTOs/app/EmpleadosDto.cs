#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Guarda la información relacionada con el personal que labora en la empresa.
    /// </summary>
    public partial class tbEmpleados
    {
        public tbEmpleados()
        {
            Mat = new HashSet<tbMaterias>();
        }

        /// <summary>
        /// Identificador único de la tabla tbEmpleados.
        /// </summary>
        public int Emp_Id { get; set; }

        /// <summary>
        /// Codigo numerico que identifica a tbEmpleados.
        /// </summary>
        public string Emp_Codigo { get; set; }

        /// <summary>
        /// Identificador de tbPersonas.
        /// </summary>
        public int Per_Id { get; set; }

        /// <summary>
        /// Identificador de la tabla tbTitulos.
        /// </summary>
        public int Tit_Id { get; set; }

        /// <summary>
        /// Identificador de la tabla tbCargos.
        /// </summary>
        public int Car_Id { get; set; }

        public int? Dep_Id { get; set; }

        public virtual tbCargos Car { get; set; }
        public virtual tbDepartamentoEmpleados Dep { get; set; }
        public virtual tbPersonas Per { get; set; }
        public virtual tbTitulos Tit { get; set; }

        public virtual ICollection<tbMaterias> Mat { get; set; }
    }
}