# Script para actualizar modales de edición de catálogos
# Este script actualiza el diseño de los modales de edición siguiendo el nuevo diseño moderno

$viewsPath = "GESTION_COLEGIAL.UI\Views"

# Catálogos pendientes de actualizar
$catalogs = @(
    @{ Name="Materias"; Property="Mat_Descripcion"; IdProperty="Mat_Id"; Placeholder="Ingrese la descripción de la materia" },
    @{ Name="Modalidades"; Property="Mda_Descripcion"; IdProperty="Mda_Id"; Placeholder="Ingrese la descripción de la modalidad" },
    @{ Name="NivelesEducativos"; Property="Niv_Descripcion"; IdProperty="Niv_Id"; Placeholder="Ingrese la descripción del nivel educativo" },
    @{ Name="Parciales"; Property="Par_Descripcion"; IdProperty="Par_Id"; Placeholder="Ingrese la descripción del parcial" },
    @{ Name="Parentescos"; Property="Pre_Descripcion"; IdProperty="Pre_Id"; Placeholder="Ingrese la descripción del parentesco" },
    @{ Name="Secciones"; Property="Sec_Descripcion"; IdProperty="Sec_Id"; Placeholder="Ingrese la descripción de la sección" },
    @{ Name="Semestres"; Property="Sem_Descripcion"; IdProperty="Sem_Id"; Placeholder="Ingrese la descripción del semestre" },
    @{ Name="Titulos"; Property="Tit_Descripcion"; IdProperty="Tit_Id"; Placeholder="Ingrese la descripción del título" }
)

foreach ($catalog in $catalogs) {
    $filePath = Join-Path $viewsPath "$($catalog.Name)\Index.cshtml"

    Write-Host "Procesando $($catalog.Name)..." -ForegroundColor Cyan

    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw -Encoding UTF8

        # Reemplazar el modal de edición
        $oldPattern = @"
<!-- Modal -->
<div class="modal" id="edit-modal"
"@

        $newPattern = @"
<!-- Modal -->
<div class="modal fade" id="edit-modal"
"@

        # Actualizar clase modal
        $content = $content -replace '<div class="modal" id="edit-modal"', '<div class="modal fade" id="edit-modal"'

        # Actualizar header
        $content = $content -replace '(<div class="modal-header">)\s*<h5 class="modal-title" id="exampleModalLabel"></h5>', @"
<!-- Header with Orange Gradient -->
                `$1
                    <h5 class="modal-title" id="exampleModalLabel">
                        <i class="mdi mdi-pencil"></i>
                    </h5>
"@

        # Actualizar modal-body para agregar form-card
        $content = $content -replace '(<div class="modal-body">)\s*(<div class="form-row">)', @"
`$1
                            <!-- Form Card -->
                            <div class="form-card">
                                `$2
"@

        # Actualizar labels y controles
        $content = $content -replace '@class = "control-label"', '@class = "form-label"'
        $content = $content -replace '@class = "form-control"', '@class = "form-control form-control-modern", placeholder = "' + $catalog.Placeholder + '"'

        # Cerrar form-card
        $content = $content -replace '(</div>\s*</div>\s*</div>\s*<div class="modal-footer">)', @"
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
"@

        # Actualizar botones
        $content = $content -replace '<button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>', @"
<button type="button" class="btn btn-secondary btn-modern-cancel" data-dismiss="modal">
                                <i class="mdi mdi-close mr-1"></i>Cancelar
                            </button>
"@

        $content = $content -replace '<button type="submit" class="btn btn-primary"><i class="mdi mdi-content-save"></i>Guardar</button>', @"
<button type="submit" class="btn btn-primary btn-modern-save">
                                <i class="mdi mdi-content-save mr-1"></i>Guardar
                            </button>
"@

        # Guardar el archivo
        $content | Set-Content $filePath -Encoding UTF8 -NoNewline
        Write-Host "  ✓ Actualizado correctamente" -ForegroundColor Green
    }
    else {
        Write-Host "  ✗ Archivo no encontrado: $filePath" -ForegroundColor Red
    }
}

Write-Host "`nProceso completado!" -ForegroundColor Green
