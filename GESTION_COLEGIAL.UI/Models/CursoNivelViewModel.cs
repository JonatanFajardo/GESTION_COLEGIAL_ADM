
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class CursoNivelViewModel : BaseViewModel
    {
        [Key]
        public int Cun_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        [Remote(action: "Exist", controller: "CursosNiveles", HttpMethod = "POST", AdditionalFields = nameof(Cun_Id) + "," + nameof(Cun_Descripcion))]
        public string Cun_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Cun_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Cun_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Cun_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Cun_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Cun_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Cun_FechaModifica { get; set; }

    }
}