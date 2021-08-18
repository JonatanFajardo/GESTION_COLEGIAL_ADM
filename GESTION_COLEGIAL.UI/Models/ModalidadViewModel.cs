using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class ModalidadViewModel
    {
        [Key]
        public int Mda_Id { get; set; }
 
        public string Mda_Descripcion { get; set; }

    }
}