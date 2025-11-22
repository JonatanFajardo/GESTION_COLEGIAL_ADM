$services = @(
    @{File='SemestresService.cs'; OldType='SemestreViewModel'; NewType='SemestreDetailViewModel'},
    @{File='SeccionesService.cs'; OldType='SeccionViewModel'; NewType='SeccionDetailViewModel'},
    @{File='ParentescosService.cs'; OldType='ParentescoViewModel'; NewType='ParentescoDetailViewModel'},
    @{File='ParcialesService.cs'; OldType='ParcialViewModel'; NewType='ParcialDetailViewModel'},
    @{File='NivelesEducativosService.cs'; OldType='NivelEducativoViewModel'; NewType='NivelEducativoDetailViewModel'},
    @{File='MateriasService.cs'; OldType='MateriaViewModel'; NewType='MateriaDetailViewModel'},
    @{File='DuracionesService.cs'; OldType='DuracionViewModel'; NewType='DuracionDetailViewModel'},
    @{File='CursosNivelesService.cs'; OldType='CursoNivelViewModel'; NewType='CursoNivelDetailViewModel'},
    @{File='CargosService.cs'; OldType='CargoViewModel'; NewType='CargoDetailViewModel'},
    @{File='EstadosService.cs'; OldType='EstadoViewModel'; NewType='EstadoDetailViewModel'},
    @{File='FormasPagoService.cs'; OldType='FormaPagoViewModel'; NewType='FormaPagoDetailViewModel'},
    @{File='EstadosPagoService.cs'; OldType='EstadoPagoViewModel'; NewType='EstadoPagoDetailViewModel'},
    @{File='DescuentosService.cs'; OldType='DescuentoViewModel'; NewType='DescuentoDetailViewModel'},
    @{File='ConceptosPagoService.cs'; OldType='ConceptoPagoViewModel'; NewType='ConceptoPagoDetailViewModel'}
)

$basePath = "GESTION_COLEGIAL.Business\Services"
$updatedCount = 0

foreach ($svc in $services) {
    $filePath = Join-Path $basePath $svc.File
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Patron para encontrar el metodo Detail
        $pattern = "public async Task<$($svc.OldType)> Detail\(int id\)\s*\{\s*string url = "[^""]+"";\s*$($svc.OldType) apiUrl = await ApiRequests\.FindAsync<$($svc.OldType)>\(url, id\);\s*return apiUrl;\s*\}"
        $replacement = "public async Task<$($svc.NewType)> Detail(int id)`n        {`n            string url = `"`"PLACEHOLDER/DetailAsync`"`";`n            $($svc.NewType) apiUrl = await ApiRequests.FindAsync<$($svc.NewType)>(url, id);`n            return apiUrl;`n        }"
        
        # Extraer el nombre del controlador del archivo
        $controllerName = $svc.File -replace 'Service\.cs',''
        $replacement = $replacement -replace 'PLACEHOLDER',$controllerName
        
        if ($content -match $pattern) {
            $content = $content -replace $pattern,$replacement
            Set-Content -Path $filePath -Value $content -Encoding UTF8 -NoNewline
            $updatedCount++
            Write-Output "Actualizado: $($svc.File)"
        } else {
            Write-Output "No se encontro patron en: $($svc.File)"
        }
    }
}

Write-Output "Actualizados $updatedCount servicios"
