
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class AulaViewModel : BaseViewModel
    {

        [Key]
        public int Aul_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Aul_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Aul_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Aul_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Aul_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Aul_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Aul_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Aul_FechaModifica { get; set; }


    }
}