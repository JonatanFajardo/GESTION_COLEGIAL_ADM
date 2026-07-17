/* ============================================================
   Ficha del Alumno — JS
   Orquesta el detalle 360° del alumno:
     - Navegación por tabs (con hash routing #tab-nombre)
     - Lazy AJAX loading de contenido por tab (partial views)
     - Cache in-memory para no repetir fetches
     - Botón recargar (.af-tab-reload) fuerza refetch
     - KPIs del hero cargados vía /Alumnos/GetKPIsAsync
   Uso desde la vista (Detail.cshtml):
     <script>AlumnoFicha.init(@Model.Alu_Id);</script>
   ============================================================ */
window.AlumnoFicha = (function () {
    'use strict';

    // --------------------------------------------------
    // Endpoints del API interno (controlador MVC)
    // --------------------------------------------------
    var ENDPOINTS = {
        kpis:        '/Alumnos/GetKPIsAsync',
        resumen:     '/Alumnos/GetResumenAsync',
        // datos: no requiere AJAX, viene renderizado desde server-side
        academico:   '/Alumnos/GetAcademicoAsync',
        asistencia:  '/Alumnos/GetAsistenciaAsync',
        pagos:       '/Alumnos/GetPagosAsync',
        familia:     '/Alumnos/GetFamiliaAsync',
        documentos:  '/Alumnos/GetDocumentosAsync',
        historial:   '/Alumnos/GetHistorialAsync'
    };

    // Tabs que se cargan de forma perezosa (excepto 'datos' que ya está
    // en el HTML server-side, y 'resumen' que se carga como default).
    var LAZY_TABS = ['resumen', 'academico', 'asistencia', 'pagos', 'familia', 'documentos', 'historial'];

    // --------------------------------------------------
    // Estado del módulo
    // --------------------------------------------------
    var state = {
        alumnoId: 0,
        loaded: {},           // { tabName: true } — tracking del cache
        activeTab: 'resumen'
    };

    // --------------------------------------------------
    // Utils
    // --------------------------------------------------

    // Escape básico de HTML para inyección segura
    function esc(s) {
        if (s === null || s === undefined) return '';
        return String(s)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    // Formatea un número decimal (promedio) o devuelve guión si no aplica
    function fmtNumber(v, decimals) {
        if (v === null || v === undefined || v === '' || isNaN(v)) return '—';
        var d = (typeof decimals === 'number') ? decimals : 1;
        return Number(v).toFixed(d);
    }

    // Formatea un porcentaje
    function fmtPct(v) {
        if (v === null || v === undefined || v === '' || isNaN(v)) return '—';
        return Number(v).toFixed(0) + '%';
    }

    // Formatea una fecha (soporta "/Date(...)/", ISO o Date)
    function fmtDate(v) {
        if (!v) return '—';
        try {
            var m = /\/Date\((\d+)/.exec(v);
            var d = m ? new Date(parseInt(m[1], 10)) : new Date(v);
            if (isNaN(d.getTime())) return String(v);
            var pad = function (n) { return n < 10 ? '0' + n : n; };
            return pad(d.getDate()) + '/' + pad(d.getMonth() + 1) + '/' + d.getFullYear();
        } catch (e) {
            return String(v);
        }
    }

    // Devuelve el marcador HTML para la tendencia (trend) de una KPI
    function trendBadge(trend) {
        if (trend === null || trend === undefined || trend === '' || isNaN(trend)) return '';
        var n = Number(trend);
        if (n > 0) {
            return '<span class="af-kpi-trend up"><i class="bi bi-arrow-up-short"></i> ' + n + '</span>';
        }
        if (n < 0) {
            return '<span class="af-kpi-trend down"><i class="bi bi-arrow-down-short"></i> ' + Math.abs(n) + '</span>';
        }
        return '<span class="af-kpi-trend flat"><i class="bi bi-dash"></i> 0</span>';
    }

    // Notificación con fallback a console
    function notify(type, msg) {
        try {
            if (window.toastr) {
                (toastr[type] || toastr.info)(msg);
                return;
            }
            if (window.Swal) {
                Swal.fire({
                    icon: type === 'success' ? 'success' : type === 'error' ? 'error' : 'info',
                    title: msg,
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 2600
                });
                return;
            }
        } catch (e) {}
        console.log('[' + type + '] ' + msg);
    }

    // --------------------------------------------------
    // KPIs del hero
    // --------------------------------------------------
    function renderKPIs(data) {
        data = data || {};

        // Promedio general
        $('#af-kpi-promedio').html(fmtNumber(data.promedio, 1));
        $('#af-kpi-promedio-trend').html(trendBadge(data.promedioTrend));

        // Asistencia %
        $('#af-kpi-asistencia').html(fmtPct(data.asistencia));
        $('#af-kpi-asistencia-trend').html(trendBadge(data.asistenciaTrend));

        // Estado de cuenta + próximo cargo
        var estado = data.cuentaEstado || '—';
        $('#af-kpi-cuenta').html(esc(estado));
        var prox = data.proximoCargo ? ('Próx: ' + fmtDate(data.proximoCargo)) : '';
        $('#af-kpi-cuenta-trend').html(prox ? '<span class="af-kpi-sub">' + esc(prox) + '</span>' : '');

        // Tareas entregadas / total
        var entregadas = data.tareasEntregadas;
        var total = data.tareasTotal;
        if (entregadas === null || entregadas === undefined || total === null || total === undefined) {
            $('#af-kpi-tareas').html('—');
            $('#af-kpi-tareas-trend').html('');
        } else {
            $('#af-kpi-tareas').html(esc(entregadas) + '<span class="af-kpi-sub">/' + esc(total) + '</span>');
            var pct = total > 0 ? Math.round((entregadas / total) * 100) : 0;
            $('#af-kpi-tareas-trend').html('<span class="af-kpi-sub">' + pct + '% completado</span>');
        }
    }

    function fetchKPIs() {
        $('.af-kpi-value').html('<span class="af-loading-dot"></span>');
        $.ajax({
            url: ENDPOINTS.kpis,
            type: 'GET',
            data: { id: state.alumnoId },
            dataType: 'json'
        }).done(function (resp) {
            var data = (resp && resp.item) ? resp.item
                     : (resp && resp.data) ? resp.data
                     : resp;
            renderKPIs(data);
        }).fail(function () {
            renderKPIs({}); // pinta guiones
            notify('error', 'No se pudieron cargar los indicadores del alumno.');
        });
    }

    // --------------------------------------------------
    // Tabs — lazy loading de contenido
    // --------------------------------------------------

    // Muestra el panel del tab activo y oculta el resto
    function showPanel(tabName) {
        $('.af-tab-panel').attr('hidden', true).removeClass('active');
        $('#af-tab-content-' + tabName).removeAttr('hidden').addClass('active');
    }

    // Marca la pestaña activa en la barra superior
    function markActiveTab(tabName) {
        $('.af-tab').removeClass('active').attr('aria-selected', 'false');
        $('.af-tab[data-tab="' + tabName + '"]').addClass('active').attr('aria-selected', 'true');
    }

    // Renderiza el spinner de carga dentro del panel del tab
    function renderLoading($panel) {
        $panel.html(
            '<div class="af-loading" role="status" aria-live="polite">' +
                '<div class="af-loading-spinner"><i class="bi bi-arrow-repeat"></i></div>' +
                '<div class="af-loading-text">Cargando…</div>' +
            '</div>'
        );
    }

    // Renderiza un mensaje de error con botón de reintento
    function renderError($panel, tabName) {
        $panel.html(
            '<div class="af-tab-error">' +
                '<i class="bi bi-exclamation-triangle"></i>' +
                '<h4>No se pudo cargar esta sección</h4>' +
                '<p>Revisá tu conexión y volvé a intentar.</p>' +
                '<button type="button" class="af-tab-reload" data-tab="' + esc(tabName) + '">' +
                    '<i class="bi bi-arrow-clockwise"></i> Reintentar' +
                '</button>' +
            '</div>'
        );
    }

    // Carga el HTML parcial del tab desde el server
    function fetchTab(tabName, force) {
        // 'datos' se sirve pre-renderizado; no hay AJAX
        if (tabName === 'datos') return;

        // Cache: no repetir a menos que se fuerce
        if (!force && state.loaded[tabName]) return;

        var endpoint = ENDPOINTS[tabName];
        if (!endpoint) return;

        var $panel = $('#af-tab-content-' + tabName);
        if (!$panel.length) return;

        renderLoading($panel);

        $.ajax({
            url: endpoint,
            type: 'GET',
            data: { id: state.alumnoId },
            dataType: 'html'
        }).done(function (html) {
            $panel.html(html);
            state.loaded[tabName] = true;
        }).fail(function () {
            state.loaded[tabName] = false;
            renderError($panel, tabName);
        });
    }

    // Activa un tab: marca UI, muestra panel, dispara lazy load si aplica
    function activateTab(tabName, opts) {
        opts = opts || {};
        if (!tabName) tabName = 'resumen';

        // Validar que el tab exista realmente en el DOM
        if (!$('.af-tab[data-tab="' + tabName + '"]').length) {
            tabName = 'resumen';
        }

        state.activeTab = tabName;
        markActiveTab(tabName);
        showPanel(tabName);

        // Actualiza el hash sin scrollear
        if (opts.updateHash !== false) {
            var newHash = '#tab-' + tabName;
            if (window.location.hash !== newHash) {
                // history.replaceState evita agregar entradas al historial en cada click
                if (window.history && window.history.replaceState) {
                    window.history.replaceState(null, '', newHash);
                } else {
                    window.location.hash = newHash;
                }
            }
        }

        // Lazy fetch
        fetchTab(tabName, opts.force === true);
    }

    // Lee el tab inicial desde el hash de la URL (#tab-academico -> 'academico')
    function readTabFromHash() {
        var h = (window.location.hash || '').replace(/^#/, '');
        if (h.indexOf('tab-') === 0) {
            return h.substring(4);
        }
        return null;
    }

    // --------------------------------------------------
    // Event bindings
    // --------------------------------------------------
    function bindEvents() {
        // Click en cualquier pestaña (delegado)
        $(document).on('click', '.af-tab', function (e) {
            e.preventDefault();
            var tabName = $(this).data('tab');
            if (!tabName || tabName === state.activeTab) return;
            activateTab(tabName);
        });

        // Botón recargar dentro de un panel: fuerza refetch
        $(document).on('click', '.af-tab-reload', function (e) {
            e.preventDefault();
            var tabName = $(this).data('tab') || state.activeTab;
            state.loaded[tabName] = false;
            activateTab(tabName, { force: true });
            notify('info', 'Recargando sección…');
        });

        // Navegación con teclado: flechas izquierda/derecha entre tabs
        $(document).on('keydown', '.af-tab', function (e) {
            if (e.key !== 'ArrowRight' && e.key !== 'ArrowLeft') return;
            e.preventDefault();
            var $tabs = $('.af-tab');
            var idx = $tabs.index(this);
            var next = e.key === 'ArrowRight' ? idx + 1 : idx - 1;
            if (next < 0) next = $tabs.length - 1;
            if (next >= $tabs.length) next = 0;
            $tabs.eq(next).trigger('focus').trigger('click');
        });

        // Cambios de hash externos (usuario pega URL con #tab-...)
        $(window).on('hashchange', function () {
            var t = readTabFromHash();
            if (t && t !== state.activeTab) {
                activateTab(t, { updateHash: false });
            }
        });
    }

    // --------------------------------------------------
    // Init — punto de entrada público
    // --------------------------------------------------
    function init(alumnoId) {
        state.alumnoId = parseInt(alumnoId, 10) || 0;
        state.loaded = {};
        state.activeTab = 'resumen';

        if (!state.alumnoId) {
            console.warn('[AlumnoFicha] init llamado sin alumnoId válido.');
            return;
        }

        bindEvents();

        // KPIs del hero se piden inmediatamente
        fetchKPIs();

        // Determinar tab inicial: hash > 'resumen' por default
        var initial = readTabFromHash();
        if (!initial || LAZY_TABS.indexOf(initial) < 0 && initial !== 'datos') {
            initial = 'resumen';
        }

        activateTab(initial, { updateHash: false });
    }

    // Interfaz pública
    return {
        init: init
    };
})();
