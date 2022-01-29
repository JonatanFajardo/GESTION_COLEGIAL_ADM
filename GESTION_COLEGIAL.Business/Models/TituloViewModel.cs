
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class TituloViewModel : BaseViewModel
    {

        [Key]
        public int Tit_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "Exist", controller: "Titulos", HttpMethod = "POST", AdditionalFields = nameof(Tit_Id) + "," + nameof(Tit_Descripcion))]
        public string Tit_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Tit_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Tit_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Tit_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Tit_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Tit_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Tit_FechaModifica { get; set; }

    }
}