
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class NivelEducativoViewModel : BaseViewModel
    {
        [Key]
        public int Niv_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "NivelesEducativos", HttpMethod = "POST", AdditionalFields = nameof(Niv_Id) + "," + nameof(Niv_Descripcion))]
        public string Niv_Descripcion { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool Niv_EsActivo { get; set; }

        public string EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Niv_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Niv_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Niv_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Niv_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Niv_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Niv_FechaModifica { get; set; }

        //public bool IsSelected { get; set; }
    }
}