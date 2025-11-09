using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class UsuarioViewModel : BaseViewModel
    {
        [Key]
        public int UsuarioId { get; set; }

        [Display(Name = "Empleado")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int EmpleadoId { get; set; }

        [StringLength(50)]
        [Display(Name = "Usuario nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string NombreUsuario { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int? ContrasenaId { get; set; }

        [MaxLength(8000)]
        [Display(Name = "Contraseña recuperación")]
        [Required(ErrorMessage = "El campo es requerido")]
        public byte[] ContrasenaRecuperacionUsuario { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int RolId { get; set; }

        [StringLength(16)]
        [Display(Name = "Usuario Ip")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string IpUsuario { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool? EsActivo { get; set; }

        public string EsActivoUsuario { get; set; }

        [Display(Name = "Suspendido")]
        public bool? UsuarioSuspendido { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime? FechaRegistroUsuario { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionUsuario { get; set; }
    }
}
