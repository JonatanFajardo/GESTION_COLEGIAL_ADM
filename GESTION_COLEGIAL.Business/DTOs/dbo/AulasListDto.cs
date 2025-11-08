using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class AulaListDto
    {
        public int AulaId { get; set; }
        public string DescripcionAula { get; set; }
    }
}
