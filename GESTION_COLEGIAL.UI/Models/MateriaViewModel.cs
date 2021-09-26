
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class Materia : BaseViewModel
    {
        [Key]
        public int Mat_Id { get; set; }

        [StringLength(150)]
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo  es requerido")]
        [Remote(action: "Exist", controller: "Materias", HttpMethod = "POST", AdditionalFields = nameof(Mat_Id) + "," + nameof(Mat_Nombre))]
        public string Mat_Nombre { get; set; }

        [Display(Name = "Duración")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Dur_Id { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? EsActivo { get; set; }
        public string Mat_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Mat_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Mat_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Mat_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Mat_UsuarioModifica { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Mat_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Mat_FechaModifica { get; set; }

    }
}