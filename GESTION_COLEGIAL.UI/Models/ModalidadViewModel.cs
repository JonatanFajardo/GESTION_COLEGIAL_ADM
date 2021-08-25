using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class ModalidadViewModel : BaseViewModel
    {
        [Key]
        public int? Mda_Id { get; set; }

        [Display(Name = "Descripción")]
        //[DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText ="esta nulito")]
        [Required(ErrorMessage = "El campo es requerido")] 
        [Remote(action: "Exist", controller: "Modalidades", HttpMethod ="POST", AdditionalFields = nameof(Mda_Id) + "," + nameof(Mda_Descripcion) )]
        //[custom2()]
        public string Mda_Descripcion { get; set; }



    }
}