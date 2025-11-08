using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class EmpleadoInsertDto
    {
        [Column("ScopeIdentity", TypeName = "decimal(38,0)")]
        public decimal? ScopeIdentity { get; set; }
    }
}