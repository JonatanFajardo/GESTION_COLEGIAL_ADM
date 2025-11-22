using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista para el detalle de la entidad "Titulo" con campos de auditoría
    /// </summary>
    public class TituloDetailViewModel : BaseViewModel
    {
        [Key]
        public int Tit_Id { get; set; }

        [Display(Name = "Descripción")]
        public string Tit_Descripcion { get; set; }

        [Display(Name = "Creado por")]
        public string Tit_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime? Tit_FechaRegistra { get; set; }

        [Display(Name = "Modificado por")]
        public string Tit_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha de modificación")]
        public DateTime? Tit_FechaModifica { get; set; }
    }
}
