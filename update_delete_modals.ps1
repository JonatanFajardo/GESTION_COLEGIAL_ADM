# Script para actualizar los modales de eliminación en los catálogos restantes

$catalogos = @(
    @{Nombre="Estados"; Entidad="estado"; Articulo="el"},
    @{Nombre="EstadosPago"; Entidad="estado de pago"; Articulo="el"},
    @{Nombre="FormasPago"; Entidad="forma de pago"; Articulo="la"},
    @{Nombre="Materias"; Entidad="materia"; Articulo="la"},
    @{Nombre="NivelesEducativos"; Entidad="nivel educativo"; Articulo="el"},
    @{Nombre="Parentescos"; Entidad="parentesco"; Articulo="el"},
    @{Nombre="Secciones"; Entidad="sección"; Articulo="la"},
    @{Nombre="Titulos"; Entidad="título"; Articulo="el"},
    @{Nombre="Parciales"; Entidad="parcial"; Articulo="el"},
    @{Nombre="Semestres"; Entidad="semestre"; Articulo="el"}
)

$modalTemplate = @'
<div class="modal fade" id="delete-modal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="deleteModalTitle" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document" style="max-width: 440px;">
        @using (Ajax.BeginForm("DeleteAsync", "{CONTROLLER}", new AjaxOptions
        {
            HttpMethod = "Post",
            OnBegin = "Catalogs.begin",
            OnComplete = "Catalogs.complete",
            OnFailure = "Catalogs.failure",
            OnSuccess = "Catalogs.success"
        }))
        {
            <div class="modal-content" style="border-radius: 12px; border: none;">
                <!-- Header sin título, solo botón cerrar -->
                <div class="modal-header" style="border-bottom: none; padding: 20px 24px 0;">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" style="padding: 0; margin: 0; opacity: 0.5;">
                        <span aria-hidden="true" style="font-size: 24px;">&times;</span>
                    </button>
                </div>

                <div class="modal-body text-center" style="padding: 0 40px 32px;">
                    @Html.HiddenFor(model => model.{ID_FIELD}, new { @id = "delete-item-id" })

                    <!-- Ícono de advertencia grande -->
                    <div class="d-flex justify-content-center mb-3">
                        <div style="width: 80px; height: 80px; background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%); border-radius: 50%; display: flex; align-items: center; justify-content: center;">
                            <div style="width: 56px; height: 56px; background-color: #dc2626; border-radius: 50%; display: flex; align-items: center; justify-content: center;">
                                <i class="fas fa-exclamation-triangle" style="color: white; font-size: 28px;"></i>
                            </div>
                        </div>
                    </div>

                    <!-- Título -->
                    <h5 class="mb-3" style="color: #1f2937; font-weight: 600; font-size: 20px;">Eliminar {ENTIDAD}</h5>

                    <!-- Mensaje de confirmación -->
                    <p class="mb-4" style="color: #6b7280; font-size: 14px; line-height: 1.5;">
                        ¿Está seguro de que desea eliminar {ARTICULO} {ENTIDAD} <span id="delete-item-name" style="font-weight: 600;"></span>?
                    </p>

                    <!-- Área de advertencia -->
                    <div class="alert d-flex align-items-start text-left" style="background-color: #fef2f2; border: 1px solid #fecaca; border-radius: 8px; padding: 16px; margin-bottom: 0;">
                        <div style="flex-shrink: 0; margin-right: 12px;">
                            <i class="fas fa-exclamation-triangle" style="color: #dc2626; font-size: 18px;"></i>
                        </div>
                        <div style="flex: 1;">
                            <p class="mb-1" style="color: #991b1b; font-weight: 600; font-size: 13px; margin: 0;">Esta acción no se puede deshacer</p>
                            <p class="mb-0" style="color: #991b1b; font-size: 13px; margin: 0;">El registro será eliminado permanentemente del sistema.</p>
                        </div>
                    </div>
                </div>

                <div class="modal-footer" style="border-top: none; padding: 0 40px 32px; gap: 12px;">
                    <button type="button" class="btn" data-dismiss="modal" style="flex: 1; padding: 10px 24px; border-radius: 8px; font-weight: 500; background-color: #f3f4f6; border: 1px solid #d1d5db; color: #374151 !important; font-size: 14px;">
                        Cancelar
                    </button>
                    <button type="submit" class="btn btn-danger" style="flex: 1; padding: 10px 24px; border-radius: 8px; font-weight: 500; background-color: #dc2626; border: none; font-size: 14px;">
                        Eliminar
                    </button>
                </div>
            </div>
        }
'@

Write-Host "Catálogos a actualizar:" -ForegroundColor Cyan
foreach ($cat in $catalogos) {
    Write-Host "  - $($cat.Nombre)" -ForegroundColor Yellow
}

Write-Host "`nEste script reemplazará los modales de eliminación en todos estos catálogos."
Write-Host "Presione Enter para continuar o Ctrl+C para cancelar..." -ForegroundColor Green
Read-Host

foreach ($cat in $catalogos) {
    Write-Host "`nProcesando $($cat.Nombre)..." -ForegroundColor Cyan
    Write-Host "Entidad: $($cat.Entidad), Artículo: $($cat.Articulo)"
}
