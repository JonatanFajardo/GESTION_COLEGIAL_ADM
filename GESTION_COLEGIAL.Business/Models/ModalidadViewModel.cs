using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista para la entidad "Modalidad"
    /// </summary>
    public class ModalidadViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador de la modalidad (Clave primaria)
        /// </summary>
        [Key]
        public int Mda_Id { get; set; }

        /// <summary>
        /// Descripción de la modalidad
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Modalidades", HttpMethod = "POST", AdditionalFields = nameof(Mda_Id) + "," + nameof(Mda_Descripcion))]
        public string Mda_Descripcion { get; set; }

        /// <summary>
        /// Indica si la modalidad está seleccionada o no
        /// </summary>
        public bool IsSelected { get; set; }
    }
}