
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ParcialViewModel : BaseViewModel
    {
        [Key]
        public int Pac_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Parciales", HttpMethod = "POST", AdditionalFields = nameof(Pac_Id) + "," + nameof(Pac_Descripcion))]
        public string Pac_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Pac_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Pac_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Pac_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Pac_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Pac_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Pac_FechaModifica { get; set; }

    }
}