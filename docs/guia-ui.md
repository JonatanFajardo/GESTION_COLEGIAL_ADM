# Guía de interfaz — usar la plantilla CORK

La interfaz del ADM está montada sobre **CORK Admin Template v1** (Bootstrap **4**).
Regla: **primero se usa lo que trae la plantilla**. Solo se escribe CSS propio cuando CORK
no tiene el componente, y va en `wwwroot/Content/css/custom.css` (no un archivo por pantalla).

- Demo de referencia (misma versión): https://designreset.com/cork-admin-v1/ltr/demo4/
- Copia local de las páginas de ejemplo: `C:\gcref\cork\demo4\*.html`
- Los estilos de CORK ya están cargados en `_Layout.cshtml` (`main.css`, `structure.css`,
  `widgets/modules-widgets.css`, `components/cards/card.css`, `components/custom-modal.css`,
  `components/tabs-accordian/custom-tabs.css`, `elements/alert.css`, `forms/*`).
- Colores: los de la plantilla (`primary` = naranja del colegio, `secondary`, `success`, `info`,
  `warning`, `danger`, `dark`). Fuente de los colores: `wwwroot/Content/sass/base/_color_variables.scss`.
- Bootstrap 4: `ml-* / mr-*` (no `ms/me`), `data-toggle` / `data-dismiss` (no `data-bs-*`),
  `custom-select` (no `form-select`), `badge badge-*` (no `bg-*`).

## Prohibido en vistas nuevas o tocadas

- Clases de botón propias (`btn-nuevo-catalogo`, `btn-save-catalogo`, `btn-cancel-catalogo`,
  `btn-modern-*`, `btn-detail-*`, `gc-btn`, `ras-btn`, …).
- Encabezados tipo banner hechos a mano (`page-header-catalogo`, `ras-encabezado`, …).
- Tarjetas KPI hechas a mano (`kpi-card`, `*-kpi`, …).
- Bloques `<style>` dentro de la vista y `style="…"` cuando existe una utilidad de Bootstrap
  (`mb-3`, `d-flex`, `text-muted`, `font-weight-bold`, `w-100`, …). Se permite `style` solo con
  valores calculados (ej. `style="width:@(pct)%"` de una barra).
- Prefijos CSS nuevos por pantalla (`ras-`, `dis-`, `cst-`, …) salvo componente sin equivalente.

## Piezas y su equivalente CORK

### Encabezado de página
Migas de pan (ya existe) + título con Bootstrap. Sin banner.

```html
<div class="breadcrumb-wrapper-content">
    <nav class="breadcrumb-style-one" aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="/">Inicio</a></li>
            <li class="breadcrumb-item">Académico</li>
            <li class="breadcrumb-item active" aria-current="page">Asistencia de alumnos</li>
        </ol>
    </nav>
</div>

<div class="d-flex flex-wrap align-items-center justify-content-between layout-top-spacing mb-4">
    <div class="mr-3">
        <h4 class="mb-1">Asistencia de alumnos</h4>
        <p class="text-muted mb-0">Cómo va la asistencia de cada sección.</p>
    </div>
    <div class="mt-2 mt-md-0">
        <button type="button" class="btn btn-primary"><i class="bi bi-plus-lg mr-1"></i> Nuevo</button>
    </div>
</div>
```

### Botones (`element_buttons.html`)
| Uso | Clase |
|---|---|
| Acción principal (Guardar, Nuevo, Ver reporte) — una por zona | `btn btn-primary` |
| Acción secundaria (Exportar, Ver detalle) | `btn btn-outline-primary` |
| Cancelar / Cerrar | `btn` (botón neutro de CORK) o `btn btn-outline-secondary` |
| Eliminar | `btn btn-danger` / `btn btn-outline-danger` |
| Confirmar positivo (Cobrar, Aprobar) | `btn btn-success` |
| Acción menor tipo enlace (Limpiar filtros) | `btn btn-link` |
| Pequeño (filas de tabla) | añadir `btn-sm` |
| Píldora / atajo seleccionable | `btn btn-sm btn-rounded btn-outline-primary`; activo: `btn btn-sm btn-rounded btn-primary` |
| Grupo | `btn-group` (`element_buttons_group.html`) |

Íconos: solo `bi bi-*` o `fas fa-*`, con `mr-1` antes del texto.

### Tarjeta de contenido / tabla (`form_layouts.html`, `table_dt_basic.html`)
```html
<div class="statbox widget box box-shadow mb-4">
    <div class="widget-header">
        <div class="row"><div class="col-12"><h4>Título de la tarjeta</h4></div></div>
    </div>
    <div class="widget-content widget-content-area">
        … tabla (`table table-hover`), formulario, etc. …
    </div>
</div>
```

### Indicador / KPI (`widgets.html` → `widget-card-four`)
```html
<div class="col-xl-3 col-md-6 col-12 layout-spacing">
    <div class="widget widget-card-four">
        <div class="widget-content">
            <div class="w-content">
                <div class="w-info">
                    <h6 class="value">94.1 %</h6>
                    <p>Asistencia general</p>
                </div>
                <div><div class="w-icon"><i class="bi bi-percent"></i></div></div>
            </div>
            <p class="text-muted small mb-0 mt-2">Presentes y tardanzas sobre todos los registros</p>
        </div>
    </div>
</div>
```
Barra opcional: `<div class="progress"><div class="progress-bar bg-gradient-primary" style="width:57%"></div></div>`.

### Insignias de estado (`element_badges.html`)
`badge badge-success` (activo/pagado), `badge badge-warning` (pendiente), `badge badge-danger`
(vencido/riesgo), `badge badge-info`, `badge badge-secondary` (inactivo).
Versión suave: `badge outline-badge-success`, etc.

### Ventana emergente (`component_modal.html`)
```html
<div class="modal fade" id="edit-modal" tabindex="-1" role="dialog" aria-hidden="true">
  <div class="modal-dialog modal-lg" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Nuevo cargo</h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Cerrar"><i class="bi bi-x-lg"></i></button>
      </div>
      <div class="modal-body"> … formulario … </div>
      <div class="modal-footer">
        <button type="button" class="btn" data-dismiss="modal">Cancelar</button>
        <button type="submit" class="btn btn-primary">Guardar</button>
      </div>
    </div>
  </div>
</div>
```

### Formularios (`form_layouts.html`, `form_validation.html`)
`form-group` + `<label>` + `form-control` / `custom-select`; filas con `form-row` y `col-md-*`;
ayuda con `small.form-text.text-muted`; errores con `invalid-feedback` / `text-danger`.
Casillas: `custom-control custom-checkbox` (`form_checkbox_radio.html`); interruptores:
`<label class="switch s-outline s-outline-primary"><input type="checkbox"><span class="slider round"></span></label>`
(`forms/switches.css`, `form_switches.html`).

### Pestañas (`component_tabs.html`)
`ul.nav.nav-tabs` + `.tab-content` / `.tab-pane`. Variante de píldoras: `nav nav-pills`.

### Alertas (`element_alerts.html`)
`alert alert-info|warning|danger|success` (+ `alert-light-*` para la versión suave).

## Lo que sí puede seguir siendo propio
Componentes que CORK no trae: grilla de horarios, planilla de asistencia día por día, organigrama,
calendario escolar, mapa de aulas. Su CSS va en `custom.css` (si se reusa) o en el archivo de
esa pantalla (si es exclusivo de ella), con un prefijo descriptivo y **sin** redefinir botones,
tarjetas, insignias, encabezados ni formularios.

## Temas (amarillo / azul / oscuro)
`theme.css` solo ajusta componentes de CORK (botones, insignias, widgets, tablas, ventanas
emergentes) para el tema azul y el modo oscuro. No lleva reglas para pantallas concretas.
