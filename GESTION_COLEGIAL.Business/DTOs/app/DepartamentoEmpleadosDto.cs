#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbDepartamentoEmpleados
    {
        public tbDepartamentoEmpleados()
        {
            tbEmpleados = new HashSet<tbEmpleados>();
        }

        public int Dep_Id { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<tbEmpleados> tbEmpleados { get; set; }
    }
}