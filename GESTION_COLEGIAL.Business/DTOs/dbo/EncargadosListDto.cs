using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class EncargadoListDto
    {
        public int EncargadoId { get; set; }
        public string NumeroIdentidad { get; set; }
        public string NombreEncargado { get; set; }
        public string Telefono { get; set; }
        public string OcupacionEncargado { get; set; }
        public string EsActivo { get; set; }
    }
}