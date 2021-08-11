
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    [Table("tbModalidades", Schema = "app")]
    public class Modalidades
    {
        public tbModalidades()
        {
            tbCursos = new HashSet<tbCursos>();
        }

        [Key]
        public int Mda_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Mda_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Mda_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Mda_UsuarioRegistra { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Mda_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Mda_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Mda_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Mda_FechaModifica { get; set; }

    }
}