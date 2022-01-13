
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    [Table("tbEstados", Schema = "app")]
    public class EstadoViewModel : BaseViewModel
    {
        [Key]
        public int Est_Id { get; set; }

        [StringLength(150)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "Exist", controller: "Estados", HttpMethod = "POST", AdditionalFields = nameof(Est_Id) + "," + nameof(Est_Descripcion))]
        public string Est_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Est_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Est_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Est_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Est_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Est_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Est_FechaModifica { get; set; }

    }
}