
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class DiaViewModel : BaseViewModel
    {

        [Key]
        public int Dia_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "Exist", controller: "Dias", HttpMethod = "POST", AdditionalFields = nameof(Dia_Id) + "," + nameof(Dia_Descripcion))]
        public string Dia_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Dia_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Dia_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Dia_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Dia_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Dia_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Dia_FechaModifica { get; set; }

    }
}