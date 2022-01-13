
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class CursoNombreViewModel : BaseViewModel
    {
        [Key]
        public int Cno_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "Exist", controller: "CursosNombres", HttpMethod = "POST", AdditionalFields = nameof(Cno_Id) + "," + nameof(Cno_Descripcion))]
        public string Cno_Descripcion { get; set; }
        public int Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Cno_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Cno_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Cno_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Cno_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Cno_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Cno_FechaModifica { get; set; }

        public SelectList NivelEducativoList { get; set; }

        public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoViewModels)
        {
            NivelEducativoList = new SelectList(nivelEducativoViewModels, "Niv_Id", "Niv_Descripcion");

        }

    }
}