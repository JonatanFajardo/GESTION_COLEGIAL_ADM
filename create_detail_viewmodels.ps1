$models = @(
    @{File='SeccionDetailViewModel.cs'; Prefix='Sec'; Name='Seccion'},
    @{File='ParentescoDetailViewModel.cs'; Prefix='Par'; Name='Parentesco'},
    @{File='ParcialDetailViewModel.cs'; Prefix='Pac'; Name='Parcial'},
    @{File='DuracionDetailViewModel.cs'; Prefix='Dur'; Name='Duracion'},
    @{File='CursoNivelDetailViewModel.cs'; Prefix='Cun'; Name='Curso Nivel'},
    @{File='CargoDetailViewModel.cs'; Prefix='Car'; Name='Cargo'},
    @{File='EstadoDetailViewModel.cs'; Prefix='Est'; Name='Estado'}
)

foreach ($m in $models) {
    $content = @"
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class $($m.File.Replace('.cs','')) : BaseViewModel
    {
        [Key]
        public int $($m.Prefix)_Id { get; set; }
        
        [Display(Name = "Descripción")]
        public string $($m.Prefix)_Descripcion { get; set; }
        
        [Display(Name = "Creado por")]
        public string $($m.Prefix)_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime? $($m.Prefix)_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string $($m.Prefix)_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? $($m.Prefix)_FechaModifica { get; set; }
    }
}
"@
    $content | Out-File -FilePath "GESTION_COLEGIAL.Business\Models\$($m.File)" -Encoding UTF8
}
Write-Output "Creados $($models.Count) DetailViewModels"
