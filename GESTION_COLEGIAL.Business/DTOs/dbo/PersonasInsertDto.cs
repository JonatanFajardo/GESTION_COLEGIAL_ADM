using System.ComponentModel.DataAnnotations.Schema;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class PersonaInsertDto
    {
        [Column("ScopeIdentity", TypeName = "decimal(38,0)")]
        public decimal? ScopeIdentity { get; set; }
    }
}