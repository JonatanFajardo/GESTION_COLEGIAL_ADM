# NivelEducativoDetailViewModel
$content1 = @"
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class NivelEducativoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Niv_Id { get; set; }
        
        [Display(Name = "Descripción")]
        public string Niv_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public string Niv_EsActivo { get; set; }
        
        [Display(Name = "Creado por")]
        public string Niv_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime? Niv_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Niv_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? Niv_FechaModifica { get; set; }
    }
}
"@
$content1 | Out-File -FilePath "GESTION_COLEGIAL.Business\Models\NivelEducativoDetailViewModel.cs" -Encoding UTF8

# MateriaDetailViewModel
$content2 = @"
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class MateriaDetailViewModel : BaseViewModel
    {
        [Key]
        public int Mat_Id { get; set; }
        
        [Display(Name = "Nombre")]
        public string Mat_Nombre { get; set; }
        
        [Display(Name = "Duración")]
        public string Dur_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public string Mat_EsActivo { get; set; }
        
        [Display(Name = "Creado por")]
        public string Mat_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime? Mat_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Mat_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? Mat_FechaModifica { get; set; }
    }
}
"@
$content2 | Out-File -FilePath "GESTION_COLEGIAL.Business\Models\MateriaDetailViewModel.cs" -Encoding UTF8

Write-Output "Creados 2 DetailViewModels complejos (NivelEducativo, Materia)"
