using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class UsuarioViewModel : BaseViewModel
    {
        public const string MensajeNombre = "El usuario debe tener entre 4 y 50 caracteres: letras, números, punto, guion o guion bajo, sin espacios.";

        /// <summary>Regla única del nombre de usuario (la misma que aplican la API y los procedimientos).</summary>
        public static bool NombreValido(string nombre) =>
            !string.IsNullOrEmpty(nombre) && nombre.Length >= 4 && nombre.Length <= 50
            && System.Text.RegularExpressions.Regex.IsMatch(nombre, @"^[-a-zA-Z0-9._]+$");

        [Key]
        public int Usu_Id { get; set; }

        [StringLength(50, MinimumLength = 4, ErrorMessage = MensajeNombre)]
        [RegularExpression(@"^[-a-zA-Z0-9._]+$", ErrorMessage = MensajeNombre)]
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

        /// <summary>
        /// Contraseña inicial (obligatoria al crear) o nueva (al editar: vacía = no se cambia).
        /// Viaja a la API como "Usu_Contraseña"; la huella la calcula la API.
        /// </summary>
        [JsonProperty("Usu_Contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 128 caracteres.")]
        public string Usu_Contrasena { get; set; }
    }
}
