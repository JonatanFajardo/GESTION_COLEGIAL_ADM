var AsistenciaEmpleados = (function () {
    'use strict';

    var obj       = {};
    var urlList, urlCreate;

    /* ------------------------------------------------------------------ */
    /*  INIT                                                                */
    /* ------------------------------------------------------------------ */
    obj.init = function (ul, uc, urlDropdowns) {
        urlList   = ul;
        urlCreate = uc;

        // Defaults del rango: primer día del mes anterior → hoy
        // Limpiamos primero por si el navegador autorrellenó valores antiguos.
        var hoy = new Date();
        var hoyStr = toYmd(hoy);
        var primerDiaMesAnterior = new Date(hoy.getFullYear(), hoy.getMonth() - 1, 1);
        var desdeStr = toYmd(primerDiaMesAnterior);

        $('#filtro-desde').val('').val(desdeStr).trigger('change');
        $('#filtro-hasta').val('').val(hoyStr).trigger('change');

        // Fecha del modal: hoy
        $('input[name="Fecha"]').val(hoyStr);

        // Cargar dropdowns de empleados
        if (urlDropdowns) {
            $.getJSON(urlDropdowns, function (data) {
                var selFiltro = $('#filtro-empleado');
                var selModal  = $('select[name="Emp_Id"]');
                $.each(data.empleados || [], function (_, e) {
                    var opt = '<option value="' + e.id + '">' + e.texto + '</option>';
                    selFiltro.append(opt);
                    selModal.append(opt);
                });
            });
        }

        // Carga inicial
        cargarDatos();

        // Botón buscar
        $('#btn-buscar').on('click', function () {
            cargarDatos();
        });

        // Permitir buscar con Enter en los filtros
        $('#filtro-desde, #filtro-hasta, #filtro-empleado').on('keypress', function (e) {
            if (e.which === 13) { cargarDatos(); }
        });

        // Botón registrar asistencia del header
        $('#btn-registrar-asistencia').on('click', function () {
            $('#modal-nueva').modal('show');
        });

        // Resetear fecha del modal al abrirse
        $('#modal-nueva').on('show.bs.modal', function () {
            var hoyModal = new Date().toISOString().split('T')[0];
            $('input[name="Fecha"]').val(hoyModal);
        });

        // Submit del formulario de registro
        $('#form-nueva').on('submit', function (e) {
            e.preventDefault();
            var $btn = $(this).find('button[type="submit"]');
            $btn.prop('disabled', true).text('Guardando...');

            $.post(urlCreate, $(this).serialize(), function (res) {
                if (res && res.success) {
                    $('#modal-nueva').modal('hide');
                    $('#form-nueva')[0].reset();
                    cargarDatos();
                } else {
                    alert(res && res.message ? res.message : 'Error al guardar.');
                }
            }).fail(function () {
                alert('Error de conexión al guardar la asistencia.');
            }).always(function () {
                $btn.prop('disabled', false).text('Guardar');
            });
        });
    };

    /* ------------------------------------------------------------------ */
    /*  CARGAR DATOS                                                        */
    /* ------------------------------------------------------------------ */
    function cargarDatos() {
        var empId = $('#filtro-empleado').val();
        var desde = $('#filtro-desde').val();
        var hasta = $('#filtro-hasta').val();

        // Sanidad: si Desde > Hasta, intercambiar para evitar consulta vacía
        if (desde && hasta && desde > hasta) {
            var tmp = desde; desde = hasta; hasta = tmp;
            $('#filtro-desde').val(desde);
            $('#filtro-hasta').val(hasta);
        }

        var params = [];
        if (empId) { params.push('Emp_Id='     + encodeURIComponent(empId)); }
        if (desde) { params.push('FechaDesde=' + encodeURIComponent(desde)); }
        if (hasta) { params.push('FechaHasta=' + encodeURIComponent(hasta)); }
        var qs = params.length > 0 ? '?' + params.join('&') : '';

        actualizarRangoLabel(desde, hasta);
        mostrarLoading();

        $.getJSON(urlList + qs, function (res) {
            var data = (res && res.data) ? res.data
                     : (Array.isArray(res) ? res : []);
            renderKPIs(data);
            renderCards(data);
        }).fail(function () {
            $('#asi-kpis').html('');
            $('#asi-cards-container').html(
                '<div class="col-12"><div class="asi-empty-state">' +
                '<div class="asi-empty-icon"><i class="fa-solid fa-triangle-exclamation"></i></div>' +
                '<p>Error al obtener los datos. Intente nuevamente.</p>' +
                '</div></div>'
            );
        });
    }

    function actualizarRangoLabel(desde, hasta) {
        var $lbl = $('#asi-rango-label');
        if (!$lbl.length) { return; }
        if (!desde && !hasta) { $lbl.text('mes anterior y mes actual'); return; }
        $lbl.text(formatFecha(desde) + '  →  ' + formatFecha(hasta));
    }

    /* ------------------------------------------------------------------ */
    /*  LOADING STATE                                                       */
    /* ------------------------------------------------------------------ */
    function mostrarLoading() {
        $('#asi-kpis').html('');
        $('#asi-cards-container').html(
            '<div class="col-12"><div class="asi-loading">' +
            '<i class="fa-solid fa-circle-notch fa-spin mr-2"></i>Cargando registros...' +
            '</div></div>'
        );
    }

    /* ------------------------------------------------------------------ */
    /*  RENDER KPIs                                                         */
    /* ------------------------------------------------------------------ */
    function renderKPIs(data) {
        var conteos = { Presente: 0, Ausente: 0, Tardanza: 0, Permiso: 0 };
        $.each(data, function (_, r) {
            if (conteos.hasOwnProperty(r.AsiEmp_Tipo)) {
                conteos[r.AsiEmp_Tipo]++;
            }
        });

        var kpis = [
            {
                clase:  'kpi-presente',
                icono:  'fa-circle-check',
                valor:  conteos.Presente,
                label:  'Presentes'
            },
            {
                clase:  'kpi-ausente',
                icono:  'fa-circle-xmark',
                valor:  conteos.Ausente,
                label:  'Ausentes'
            },
            {
                clase:  'kpi-tardanza',
                icono:  'fa-clock',
                valor:  conteos.Tardanza,
                label:  'Tardanzas'
            },
            {
                clase:  'kpi-permiso',
                icono:  'fa-file-circle-check',
                valor:  conteos.Permiso,
                label:  'Permisos'
            }
        ];

        var html = '';
        $.each(kpis, function (_, k) {
            html +=
                '<div class="col-xl-3 col-md-6">' +
                '  <div class="asi-kpi-card ' + k.clase + '">' +
                '    <div class="asi-kpi-icon">' +
                '      <i class="fa-solid ' + k.icono + '"></i>' +
                '    </div>' +
                '    <div class="asi-kpi-body">' +
                '      <p class="asi-kpi-value">' + k.valor + '</p>' +
                '      <p class="asi-kpi-label">' + k.label + '</p>' +
                '    </div>' +
                '  </div>' +
                '</div>';
        });

        $('#asi-kpis').html(html);
    }

    /* ------------------------------------------------------------------ */
    /*  RENDER CARDS                                                        */
    /* ------------------------------------------------------------------ */
    function renderCards(data) {
        if (!data || data.length === 0) {
            $('#asi-cards-container').html(
                '<div class="col-12">' +
                '  <div class="asi-empty-state">' +
                '    <div class="asi-empty-icon"><i class="fa-solid fa-calendar-xmark"></i></div>' +
                '    <p>No hay registros para el rango seleccionado.</p>' +
                '  </div>' +
                '</div>'
            );
            return;
        }

        var html = '';
        $.each(data, function (_, r) {
            var tipoKey  = (r.AsiEmp_Tipo || '').toLowerCase();   // presente|ausente|tardanza|permiso
            var badgeCls = 'badge-' + tipoKey;
            var cardCls  = 'tipo-' + tipoKey;
            var fecha    = formatFecha(r.AsiEmp_Fecha);
            var nombre   = r.NombreEmpleado || '—';
            var obs      = r.AsiEmp_Observacion || '';

            // Avatar: imagen si existe, iniciales si no
            var avatarInner;
            if (r.Per_Imagen) {
                avatarInner = '<img src="' + r.Per_Imagen + '" alt="' + escHtml(nombre) + '">';
            } else {
                avatarInner = iniciales(nombre);
            }

            var obsHtml = obs
                ? '<div class="asi-rec-obs"><i class="fa-solid fa-comment-dots mr-1"></i>' + escHtml(obs) + '</div>'
                : '';

            html +=
                '<div class="col-xl-4 col-lg-6 col-md-12">' +
                '  <div class="asi-rec-card ' + cardCls + '">' +
                '    <div class="asi-avatar">' + avatarInner + '</div>' +
                '    <div class="asi-rec-body">' +
                '      <div class="asi-rec-nombre">' + escHtml(nombre) + '</div>' +
                '      <div class="asi-rec-meta">' +
                '        <span class="asi-rec-fecha">' +
                '          <i class="fa-regular fa-calendar"></i> ' + fecha +
                '        </span>' +
                '        <span class="asi-badge ' + badgeCls + '">' + escHtml(r.AsiEmp_Tipo) + '</span>' +
                '      </div>' +
                obsHtml +
                '    </div>' +
                '  </div>' +
                '</div>';
        });

        $('#asi-cards-container').html(html);
    }

    /* ------------------------------------------------------------------ */
    /*  UTILIDADES                                                          */
    /* ------------------------------------------------------------------ */
    function formatFecha(valor) {
        if (!valor) { return '—'; }
        var d = new Date(valor);
        if (isNaN(d.getTime())) { return valor; }
        var dd = String(d.getDate()).padStart(2, '0');
        var mm = String(d.getMonth() + 1).padStart(2, '0');
        var yyyy = d.getFullYear();
        return dd + '/' + mm + '/' + yyyy;
    }

    function toYmd(d) {
        var yyyy = d.getFullYear();
        var mm = String(d.getMonth() + 1).padStart(2, '0');
        var dd = String(d.getDate()).padStart(2, '0');
        return yyyy + '-' + mm + '-' + dd;
    }

    function iniciales(nombre) {
        var partes = (nombre || '').trim().split(/\s+/);
        var ini = '';
        if (partes[0]) { ini += partes[0][0]; }
        if (partes[1]) { ini += partes[1][0]; }
        return ini.toUpperCase() || '?';
    }

    function escHtml(str) {
        return String(str || '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    return obj;
}());
