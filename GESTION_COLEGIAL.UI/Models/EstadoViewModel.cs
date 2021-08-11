
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    [Table("tbEstados", Schema = "app")]
    public class EstadoViewModel
    {
        [Key]
        public int Est_Id { get; set; }

        [StringLength(150)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Est_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Est_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Est_UsuarioRegistra { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Est_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Est_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Est_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Est_FechaModifica { get; set; }

    }
}