var AsistenciaEmpleados = (function () {
    'use strict';

    var obj       = {};
    var urlList, urlCreate;

    var allEmpleados      = [];   // dropdown de empleados: [{ id, texto }]
    var allData           = [];   // registros del rango (todos los empleados)
    var selectedEmpId     = null; // empleado seleccionado en el panel derecho
    var calendarMonth     = null; // Date primer día del mes que muestra el calendario
    var rangoDesde        = null; // string yyyy-mm-dd
    var rangoHasta        = null; // string yyyy-mm-dd

    var MESES = ['Enero','Febrero','Marzo','Abril','Mayo','Junio',
                 'Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];
    var DIAS_CORTOS = ['Lun','Mar','Mié','Jue','Vie','Sáb','Dom'];

    /* ------------------------------------------------------------------ */
    /*  INIT                                                                */
    /* ------------------------------------------------------------------ */
    obj.init = function (ul, uc, urlDropdowns) {
        urlList   = ul;
        urlCreate = uc;

        var hoy = new Date();
        var hoyStr = toYmd(hoy);
        var primerDiaMesAnterior = new Date(hoy.getFullYear(), hoy.getMonth() - 1, 1);
        var desdeStr = toYmd(primerDiaMesAnterior);

        $('#filtro-desde').val('').val(desdeStr).trigger('change');
        $('#filtro-hasta').val('').val(hoyStr).trigger('change');
        $('input[name="Fecha"]').val(hoyStr);

        if (urlDropdowns) {
            $.getJSON(urlDropdowns, function (data) {
                allEmpleados = data.empleados || [];
                var selFiltro = $('#filtro-empleado');
                var selModal  = $('select[name="Emp_Id"]');
                $.each(allEmpleados, function (_, e) {
                    var opt = '<option value="' + e.id + '">' + escHtml(e.texto) + '</option>';
                    selFiltro.append(opt);
                    selModal.append(opt);
                });
                cargarDatos();
            });
        } else {
            cargarDatos();
        }

        $('#btn-buscar').on('click', cargarDatos);
        $('#filtro-empleado').on('change', function () {
            selectedEmpId = null;
            cargarDatos();
        });
        $('#filtro-desde, #filtro-hasta').on('keypress', function (e) {
            if (e.which === 13) { cargarDatos(); }
        });

        $('#asi-buscar-empleado').on('input', function () {
            filtrarLista($(this).val());
        });

        $('#btn-registrar-asistencia').on('click', function () {
            $('#modal-nueva').modal('show');
        });

        $('#modal-nueva').on('show.bs.modal', function () {
            var hoyModal = new Date().toISOString().split('T')[0];
            $('input[name="Fecha"]').val(hoyModal);
        });

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

        // Delegación: click en un empleado de la lista
        $(document).on('click', '.asi-emp-item', function () {
            var empId = parseInt($(this).attr('data-emp-id'), 10);
            seleccionarEmpleado(empId);
        });

        // Delegación: navegación del calendario
        $(document).on('click', '.asi-cal-prev', function () {
            if (!calendarMonth) { return; }
            calendarMonth = new Date(calendarMonth.getFullYear(), calendarMonth.getMonth() - 1, 1);
            renderCalendar();
        });
        $(document).on('click', '.asi-cal-next', function () {
            if (!calendarMonth) { return; }
            calendarMonth = new Date(calendarMonth.getFullYear(), calendarMonth.getMonth() + 1, 1);
            renderCalendar();
        });
    };

    /* ------------------------------------------------------------------ */
    /*  CARGAR DATOS                                                        */
    /* ------------------------------------------------------------------ */
    function cargarDatos() {
        var filtroEmpId = $('#filtro-empleado').val();
        var desde       = $('#filtro-desde').val();
        var hasta       = $('#filtro-hasta').val();

        if (desde && hasta && desde > hasta) {
            var tmp = desde; desde = hasta; hasta = tmp;
            $('#filtro-desde').val(desde);
            $('#filtro-hasta').val(hasta);
        }

        rangoDesde = desde;
        rangoHasta = hasta;

        var params = [];
        if (filtroEmpId) { params.push('Emp_Id='     + encodeURIComponent(filtroEmpId)); }
        if (desde)       { params.push('FechaDesde=' + encodeURIComponent(desde)); }
        if (hasta)       { params.push('FechaHasta=' + encodeURIComponent(hasta)); }
        var qs = params.length > 0 ? '?' + params.join('&') : '';

        actualizarRangoLabel(desde, hasta);
        mostrarLoading();

        $.getJSON(urlList + qs, function (res) {
            allData = (res && res.data) ? res.data
                    : (Array.isArray(res) ? res : []);
            renderKPIs(allData);
            renderEmpleadosList();

            // Si el filtro apunta a un empleado específico, autoseleccionarlo
            if (filtroEmpId) {
                selectedEmpId = parseInt(filtroEmpId, 10);
            }

            if (selectedEmpId) {
                calendarMonth = mesInicialCalendario(selectedEmpId);
                renderCalendar();
                marcarSeleccionado();
            } else {
                renderCalendarEmpty();
            }
        }).fail(function () {
            allData = [];
            $('#asi-kpis').html('');
            $('#asi-empleados-list').html(
                '<div class="asi-empty-state">' +
                '  <div class="asi-empty-icon"><i class="fa-solid fa-triangle-exclamation"></i></div>' +
                '  <p>Error al obtener los datos. Intente nuevamente.</p>' +
                '</div>'
            );
            renderCalendarEmpty();
        });
    }

    function mesInicialCalendario(empId) {
        // Si el empleado tiene registros, saltar al mes del más reciente.
        if (empId) {
            var registrosEmp = allData.filter(function (r) { return r.Emp_Id === empId; });
            if (registrosEmp.length > 0) {
                var maxYmd = '';
                $.each(registrosEmp, function (_, r) {
                    var ymd = ymdFromApi(r.AsiEmp_Fecha);
                    if (ymd > maxYmd) { maxYmd = ymd; }
                });
                if (maxYmd) {
                    var partes = maxYmd.split('-');
                    return new Date(parseInt(partes[0], 10), parseInt(partes[1], 10) - 1, 1);
                }
            }
        }
        // Fallback: mes del "hasta", o mes actual.
        var d = rangoHasta ? fromYmd(rangoHasta) : new Date();
        return new Date(d.getFullYear(), d.getMonth(), 1);
    }

    function actualizarRangoLabel(desde, hasta) {
        var $lbl = $('#asi-rango-label');
        if (!$lbl.length) { return; }
        if (!desde && !hasta) { $lbl.text('mes anterior y mes actual'); return; }
        $lbl.text(formatFecha(desde) + '  →  ' + formatFecha(hasta));
    }

    /* ------------------------------------------------------------------ */
    /*  LOADING                                                             */
    /* ------------------------------------------------------------------ */
    function mostrarLoading() {
        $('#asi-kpis').html('');
        $('#asi-empleados-list').html(
            '<div class="asi-loading">' +
            '  <i class="fa-solid fa-circle-notch fa-spin mr-2"></i>Cargando...' +
            '</div>'
        );
    }

    /* ------------------------------------------------------------------ */
    /*  KPIs                                                                */
    /* ------------------------------------------------------------------ */
    function renderKPIs(data) {
        var conteos = { Presente: 0, Ausente: 0, Tardanza: 0, Permiso: 0 };
        $.each(data, function (_, r) {
            if (conteos.hasOwnProperty(r.AsiEmp_Tipo)) {
                conteos[r.AsiEmp_Tipo]++;
            }
        });

        var kpis = [
            { clase: 'kpi-presente', icono: 'fa-circle-check',      valor: conteos.Presente, label: 'Presentes' },
            { clase: 'kpi-ausente',  icono: 'fa-circle-xmark',      valor: conteos.Ausente,  label: 'Ausentes'  },
            { clase: 'kpi-tardanza', icono: 'fa-clock',             valor: conteos.Tardanza, label: 'Tardanzas' },
            { clase: 'kpi-permiso',  icono: 'fa-file-circle-check', valor: conteos.Permiso,  label: 'Permisos'  }
        ];

        var html = '';
        $.each(kpis, function (_, k) {
            html +=
                '<div class="col-xl-3 col-md-6">' +
                '  <div class="asi-kpi-card ' + k.clase + '">' +
                '    <div class="asi-kpi-icon"><i class="fa-solid ' + k.icono + '"></i></div>' +
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
    /*  LISTA DE EMPLEADOS                                                  */
    /* ------------------------------------------------------------------ */
    function renderEmpleadosList() {
        // Agrupar registros por Emp_Id
        var porEmp = {};
        $.each(allData, function (_, r) {
            var id = r.Emp_Id;
            if (!porEmp[id]) {
                porEmp[id] = {
                    empId:    id,
                    nombre:   r.NombreEmpleado || '',
                    imagen:   r.Per_Imagen || '',
                    codigo:   r.Emp_Codigo || '',
                    registros: []
                };
            }
            porEmp[id].registros.push(r);
        });

        // Base: dropdown completo → así aparecen aun los que no tienen registros
        var lista = [];
        if (allEmpleados.length) {
            $.each(allEmpleados, function (_, e) {
                var g = porEmp[e.id] || {
                    empId:    e.id,
                    nombre:   e.texto,
                    imagen:   '',
                    codigo:   '',
                    registros: []
                };
                if (!g.nombre) { g.nombre = e.texto; }
                lista.push(g);
            });
        } else {
            // Sin dropdown cargado: caer al grupo real
            $.each(porEmp, function (_, g) { lista.push(g); });
        }

        // Orden alfabético por nombre
        lista.sort(function (a, b) {
            return (a.nombre || '').localeCompare(b.nombre || '', 'es');
        });

        $('#asi-empleados-count').text(lista.length);

        if (lista.length === 0) {
            $('#asi-empleados-list').html(
                '<div class="asi-empty-state asi-empty-inline">' +
                '  <div class="asi-empty-icon"><i class="fa-solid fa-user-slash"></i></div>' +
                '  <p>No hay empleados para mostrar.</p>' +
                '</div>'
            );
            return;
        }

        var html = '';
        $.each(lista, function (_, g) {
            var conteos = contarPorTipo(g.registros);
            var total   = g.registros.length;
            var pctPres = total > 0 ? Math.round((conteos.Presente / total) * 100) : null;
            var estado  = calcularEstado(pctPres, total);

            var avatarInner;
            if (g.imagen) {
                avatarInner = '<img src="' + g.imagen + '" alt="' + escHtml(g.nombre) + '">';
            } else {
                avatarInner = iniciales(g.nombre);
            }

            html +=
                '<div class="asi-emp-item" data-emp-id="' + g.empId + '" data-nombre="' + escHtml(g.nombre).toLowerCase() + '">' +
                '  <span class="asi-emp-dot ' + estado.claseDot + '" title="' + estado.titulo + '"></span>' +
                '  <div class="asi-avatar asi-avatar-sm">' + avatarInner + '</div>' +
                '  <div class="asi-emp-body">' +
                '    <div class="asi-emp-nombre">' + escHtml(g.nombre) + '</div>' +
                '    <div class="asi-emp-meta">' +
                '      <span class="asi-mini-badge mini-presente" title="Presentes"><i class="fa-solid fa-circle-check"></i> ' + conteos.Presente + '</span>' +
                '      <span class="asi-mini-badge mini-ausente"  title="Ausentes"><i class="fa-solid fa-circle-xmark"></i> ' + conteos.Ausente + '</span>' +
                '      <span class="asi-mini-badge mini-tardanza" title="Tardanzas"><i class="fa-solid fa-clock"></i> ' + conteos.Tardanza + '</span>' +
                '      <span class="asi-mini-badge mini-permiso"  title="Permisos"><i class="fa-solid fa-file-circle-check"></i> ' + conteos.Permiso + '</span>' +
                '    </div>' +
                '  </div>' +
                '  <div class="asi-emp-estado">' +
                (pctPres === null
                    ? '<span class="asi-emp-sinreg">Sin registros</span>'
                    : '<span class="asi-emp-pct ' + estado.claseDot + '">' + pctPres + '%</span>') +
                '  </div>' +
                '</div>';
        });

        $('#asi-empleados-list').html(html);
        marcarSeleccionado();
        filtrarLista($('#asi-buscar-empleado').val());
    }

    function contarPorTipo(registros) {
        var c = { Presente: 0, Ausente: 0, Tardanza: 0, Permiso: 0 };
        $.each(registros, function (_, r) {
            if (c.hasOwnProperty(r.AsiEmp_Tipo)) { c[r.AsiEmp_Tipo]++; }
        });
        return c;
    }

    function calcularEstado(pctPres, total) {
        if (total === 0 || pctPres === null) {
            return { claseDot: 'estado-sinreg', titulo: 'Sin registros' };
        }
        if (pctPres >= 90) { return { claseDot: 'estado-ok',    titulo: 'Buena asistencia'    }; }
        if (pctPres >= 70) { return { claseDot: 'estado-medio', titulo: 'Asistencia regular'  }; }
        return               { claseDot: 'estado-bajo',  titulo: 'Asistencia baja'     };
    }

    function marcarSeleccionado() {
        $('.asi-emp-item').removeClass('is-active');
        if (selectedEmpId) {
            $('.asi-emp-item[data-emp-id="' + selectedEmpId + '"]').addClass('is-active');
        }
    }

    function filtrarLista(q) {
        q = (q || '').toLowerCase().trim();
        if (!q) {
            $('.asi-emp-item').show();
            return;
        }
        $('.asi-emp-item').each(function () {
            var n = $(this).attr('data-nombre') || '';
            $(this).toggle(n.indexOf(q) !== -1);
        });
    }

    function seleccionarEmpleado(empId) {
        selectedEmpId = empId;
        calendarMonth = mesInicialCalendario(empId);
        marcarSeleccionado();
        renderCalendar();
    }

    /* ------------------------------------------------------------------ */
    /*  CALENDARIO                                                          */
    /* ------------------------------------------------------------------ */
    function renderCalendarEmpty() {
        $('#asi-calendar-panel').html(
            '<div class="asi-empty-state">' +
            '  <div class="asi-empty-icon"><i class="fa-regular fa-calendar"></i></div>' +
            '  <p>Selecciona un empleado de la lista para ver su calendario de asistencia.</p>' +
            '</div>'
        );
    }

    function renderCalendar() {
        if (!selectedEmpId || !calendarMonth) {
            renderCalendarEmpty();
            return;
        }

        // Datos del empleado
        var emp = allEmpleados.filter(function (e) { return e.id === selectedEmpId; })[0];
        var registrosEmp = allData.filter(function (r) { return r.Emp_Id === selectedEmpId; });
        var nombre = emp ? emp.texto : (registrosEmp[0] ? registrosEmp[0].NombreEmpleado : 'Empleado');
        var imagen = registrosEmp[0] ? registrosEmp[0].Per_Imagen : '';
        var codigo = registrosEmp[0] ? registrosEmp[0].Emp_Codigo : '';

        // Map de registros por fecha yyyy-mm-dd
        var porFecha = {};
        $.each(registrosEmp, function (_, r) {
            var ymd = ymdFromApi(r.AsiEmp_Fecha);
            if (ymd) { porFecha[ymd] = r; }
        });

        var y = calendarMonth.getFullYear();
        var m = calendarMonth.getMonth();
        var mesLabel = MESES[m] + ' ' + y;

        // Primer día del mes → offset a lunes (getDay: dom=0 ... sab=6)
        var primerDia = new Date(y, m, 1);
        var dowPrimer = primerDia.getDay();          // 0..6 (dom..sáb)
        var offsetLun = (dowPrimer + 6) % 7;         // días vacíos antes del 1
        var diasEnMes = new Date(y, m + 1, 0).getDate();

        // Conteos del mes (solo para el sub-header)
        var totalMes = { Presente: 0, Ausente: 0, Tardanza: 0, Permiso: 0 };
        for (var d = 1; d <= diasEnMes; d++) {
            var ymd = y + '-' + pad2(m + 1) + '-' + pad2(d);
            var r = porFecha[ymd];
            if (r && totalMes.hasOwnProperty(r.AsiEmp_Tipo)) { totalMes[r.AsiEmp_Tipo]++; }
        }

        var avatarInner = imagen
            ? '<img src="' + imagen + '" alt="' + escHtml(nombre) + '">'
            : iniciales(nombre);

        var hoyYmd = toYmd(new Date());

        var html =
            '<div class="asi-cal-header">' +
            '  <div class="asi-cal-emp">' +
            '    <div class="asi-avatar asi-avatar-md">' + avatarInner + '</div>' +
            '    <div class="asi-cal-emp-info">' +
            '      <div class="asi-cal-emp-nombre">' + escHtml(nombre) + '</div>' +
            (codigo ? '      <div class="asi-cal-emp-codigo">Código: ' + escHtml(codigo) + '</div>' : '') +
            '    </div>' +
            '  </div>' +
            '  <div class="asi-cal-nav">' +
            '    <button type="button" class="asi-cal-btn asi-cal-prev" title="Mes anterior"><i class="fa-solid fa-chevron-left"></i></button>' +
            '    <div class="asi-cal-mes">' + mesLabel + '</div>' +
            '    <button type="button" class="asi-cal-btn asi-cal-next" title="Mes siguiente"><i class="fa-solid fa-chevron-right"></i></button>' +
            '  </div>' +
            '</div>' +
            '<div class="asi-cal-sub">' +
            '  <span class="asi-mini-badge mini-presente"><i class="fa-solid fa-circle-check"></i> ' + totalMes.Presente + ' presentes</span>' +
            '  <span class="asi-mini-badge mini-ausente"><i class="fa-solid fa-circle-xmark"></i> ' + totalMes.Ausente + ' ausentes</span>' +
            '  <span class="asi-mini-badge mini-tardanza"><i class="fa-solid fa-clock"></i> ' + totalMes.Tardanza + ' tardanzas</span>' +
            '  <span class="asi-mini-badge mini-permiso"><i class="fa-solid fa-file-circle-check"></i> ' + totalMes.Permiso + ' permisos</span>' +
            '</div>' +
            '<div class="asi-cal-grid">';

        // Cabecera de días
        $.each(DIAS_CORTOS, function (i, dia) {
            var esFinde = (i >= 5);
            html += '<div class="asi-cal-dow' + (esFinde ? ' es-finde' : '') + '">' + dia + '</div>';
        });

        // Días vacíos antes del 1
        for (var i = 0; i < offsetLun; i++) {
            html += '<div class="asi-cal-cell asi-cal-empty"></div>';
        }

        // Días del mes
        for (var day = 1; day <= diasEnMes; day++) {
            var ymd2 = y + '-' + pad2(m + 1) + '-' + pad2(day);
            var reg  = porFecha[ymd2];
            var dow  = (new Date(y, m, day)).getDay();  // 0..6
            var esFinde2 = (dow === 0 || dow === 6);
            var esHoy = (ymd2 === hoyYmd);

            var claseTipo = 'dia-sinreg';
            var iconoTipo = '';
            var titulo    = 'Sin registro';
            if (reg) {
                var tipoKey = (reg.AsiEmp_Tipo || '').toLowerCase();
                claseTipo = 'dia-' + tipoKey;
                titulo    = reg.AsiEmp_Tipo || 'Sin tipo';
                var iconoMap = {
                    'presente': 'fa-circle-check',
                    'ausente':  'fa-circle-xmark',
                    'tardanza': 'fa-clock',
                    'permiso':  'fa-file-circle-check'
                };
                if (iconoMap[tipoKey]) {
                    iconoTipo = '<i class="fa-solid ' + iconoMap[tipoKey] + '"></i>';
                }
                if (reg.AsiEmp_Observacion) {
                    titulo += ' — ' + reg.AsiEmp_Observacion;
                }
            }

            html +=
                '<div class="asi-cal-cell ' + claseTipo +
                    (esFinde2 ? ' es-finde' : '') +
                    (esHoy ? ' es-hoy' : '') +
                '" title="' + escHtml(titulo) + '">' +
                '  <span class="asi-cal-day-num">' + day + '</span>' +
                '  <span class="asi-cal-day-icon">' + iconoTipo + '</span>' +
                '</div>';
        }

        html += '</div>';

        // Leyenda
        html +=
            '<div class="asi-cal-legend">' +
            '  <span class="legend-item"><span class="legend-swatch swatch-presente"></span> Presente</span>' +
            '  <span class="legend-item"><span class="legend-swatch swatch-ausente"></span> Ausente</span>' +
            '  <span class="legend-item"><span class="legend-swatch swatch-tardanza"></span> Tardanza</span>' +
            '  <span class="legend-item"><span class="legend-swatch swatch-permiso"></span> Permiso</span>' +
            '  <span class="legend-item"><span class="legend-swatch swatch-sinreg"></span> Sin registro</span>' +
            '</div>';

        $('#asi-calendar-panel').html(html);
    }

    /* ------------------------------------------------------------------ */
    /*  UTILIDADES                                                          */
    /* ------------------------------------------------------------------ */
    function formatFecha(valor) {
        if (!valor) { return '—'; }
        var d = new Date(valor);
        if (isNaN(d.getTime())) { return valor; }
        return pad2(d.getDate()) + '/' + pad2(d.getMonth() + 1) + '/' + d.getFullYear();
    }

    function toYmd(d) {
        return d.getFullYear() + '-' + pad2(d.getMonth() + 1) + '-' + pad2(d.getDate());
    }

    // Extrae YYYY-MM-DD del valor devuelto por el API sin aplicar TZ.
    // El backend serializa DateTime como "2026-07-01T00:00:00" (o similar). Usar
    // new Date(...) puede correrlo un día en Honduras (UTC-6).
    function ymdFromApi(v) {
        if (v == null) { return ''; }
        if (typeof v === 'string') {
            var m = v.match(/^(\d{4})-(\d{2})-(\d{2})/);
            if (m) { return m[1] + '-' + m[2] + '-' + m[3]; }
        }
        var d = new Date(v);
        return isNaN(d.getTime()) ? '' : toYmd(d);
    }

    function fromYmd(s) {
        var parts = (s || '').split('-');
        if (parts.length !== 3) { return new Date(); }
        return new Date(parseInt(parts[0], 10), parseInt(parts[1], 10) - 1, parseInt(parts[2], 10));
    }

    function pad2(n) { return String(n).padStart(2, '0'); }

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
