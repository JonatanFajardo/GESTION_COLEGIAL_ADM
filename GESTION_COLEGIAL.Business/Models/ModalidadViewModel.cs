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
        public int ModalidadId { get; set; }

        /// <summary>
        /// Descripción de la modalidad
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Modalidades", HttpMethod = "POST", AdditionalFields = nameof(ModalidadId) + "," + nameof(DescripcionModalidad))]
        public string DescripcionModalidad { get; set; }

        /// <summary>
        /// Indica si la modalidad está seleccionada o no
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
