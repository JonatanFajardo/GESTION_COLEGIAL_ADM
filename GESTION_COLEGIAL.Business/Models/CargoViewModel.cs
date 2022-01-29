
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class CargoViewModel : BaseViewModel
    {

        [Key]
        public int Car_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "Exist", controller: "Cargos", HttpMethod = "POST", AdditionalFields = nameof(Car_Id) + "," + nameof(Car_Descripcion))]
        public string Car_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Car_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Car_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Car_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Car_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Car_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Car_FechaModifica { get; set; }

    }
}