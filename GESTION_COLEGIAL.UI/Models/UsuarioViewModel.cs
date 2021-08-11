﻿
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class UsuarioViewModel
    {
        [Key]
        public int Usu_Id { get; set; }

        [Display(Name = "Empleado")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Emp_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Usuario nombre")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Usu_Name { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int? Con_Id { get; set; }

        [MaxLength(8000)]
        [Display(Name = "Contraseña recuperación")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public byte[] Usu_ContrasenaRecuperacion { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Rol_Id { get; set; }

        [StringLength(16)]
        [Display(Name = "Usuario Ip")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Usu_Ip { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Usu_EsActivo { get; set; }

        [Display(Name = "Suspendido")]
        public bool? Usu_Suspendido { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime? Usu_FechaRegistra { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Usu_FechaModifica { get; set; }
    }
}