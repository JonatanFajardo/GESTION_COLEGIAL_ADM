using System.ComponentModel.DataAnnotations;

namespace Gestion.Colegial.Entities.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contrasena es requerida")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}