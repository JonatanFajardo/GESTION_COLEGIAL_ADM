
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class HoraViewModel : BaseViewModel
    {

        [Key]
        public int Hor_Id { get; set; }

        [StringLength(11)]
        [Display(Name = "Hora")]
        [Required(ErrorMessage = "El campo  es requerido")]
        [Remote(action: "Exist", controller: "Horas", HttpMethod = "POST", AdditionalFields = nameof(Hor_Id) + "," + nameof(Hor_Hora))]
        public string Hor_Hora { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Hor_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Hor_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Hor_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Hor_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Hor_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Hor_FechaModifica { get; set; }
    }

}