using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }

    public class LoginResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UserInfoViewModel User { get; set; }
        public List<string> Pantallas { get; set; }
    }

    public class UserInfoViewModel
    {
        public int Usu_Id { get; set; }
        public string Usu_Name { get; set; }
        public int Emp_Id { get; set; }
        public int Rol_Id { get; set; }
        public string Rol_Nombre { get; set; }
        /// <summary>Sello de sesión: cambia al cambiar la contraseña, desactivar la cuenta o cambiar el rol.</summary>
        public string Sello { get; set; }
    }

    /// <summary>Respuesta de Account/EstadoSesion: si la cuenta sigue habilitada y su sello actual.</summary>
    public class EstadoSesionViewModel
    {
        public int Habilitado { get; set; }
        public string Sello { get; set; }
    }
}
