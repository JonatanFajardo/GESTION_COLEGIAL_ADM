
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class SemestreViewModel
    {

        [Key]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Sem_Id { get; set; }
        [StringLength(100)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Sem_Descripcion { get; set; }
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Sem_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Sem_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Sem_UsuarioRegistra { get; set; }
        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime Sem_FechaRegistra { get; set; }
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int? Sem_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Sem_UsuarioModifica { get; set; }
        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime? Sem_FechaModifica { get; set; }

    }
}