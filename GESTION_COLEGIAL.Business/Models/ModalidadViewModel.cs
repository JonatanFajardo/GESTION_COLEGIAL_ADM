﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ModalidadViewModel : BaseViewModel
    {
        [Key]
        public int Mda_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Modalidades", HttpMethod = "POST", AdditionalFields = nameof(Mda_Id) + "," + nameof(Mda_Descripcion))]
        public string Mda_Descripcion { get; set; }

        public bool IsSelected { get; set; }
    }
}