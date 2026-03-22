using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class RolViewModel : BaseViewModel
    {
        [Key]
        public int Rol_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Roles", HttpMethod = "POST", AdditionalFields = nameof(Rol_Id) + "," + nameof(Rol_Descripcion))]
        public string Rol_Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool Rol_Estado { get; set; }

        public int CantidadPantallas { get; set; }
    }
}
