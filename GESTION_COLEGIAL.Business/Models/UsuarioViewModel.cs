using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class UsuarioViewModel : BaseViewModel
    {
        [Key]
        public int Usu_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Nombre de usuario")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Usuarios", HttpMethod = "POST", AdditionalFields = nameof(Usu_Id) + "," + nameof(Usu_Name))]
        public string Usu_Name { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Rol_Id { get; set; }

        [Display(Name = "Rol")]
        public string Rol_Descripcion { get; set; }

        [Display(Name = "Empleado")]
        public int Emp_Id { get; set; }

        [Display(Name = "Activo")]
        public bool Usu_EsActivo { get; set; }
    }
}
