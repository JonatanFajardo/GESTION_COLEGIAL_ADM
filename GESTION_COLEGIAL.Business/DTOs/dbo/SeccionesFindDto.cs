using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class SeccionFindDto
    {
        public int SeccionId { get; set; }
        public string DescripcionSeccion { get; set; }
    }
}