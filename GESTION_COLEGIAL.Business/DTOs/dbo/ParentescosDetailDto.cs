using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class ParentescoDetailDto
    {
        public int? ParentescoId { get; set; }
        public string DescripcionParentesco { get; set; }
        public string NombreUsuarioRegistraParentesco { get; set; }
        public DateTime? FechaRegistroParentesco { get; set; }
        public string NombreUsuarioModificaParentesco { get; set; }
        public DateTime? FechaModificacionParentesco { get; set; }
    }
}