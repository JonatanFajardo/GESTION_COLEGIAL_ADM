/* Analítica Académica — hub con tabs:
   1. Resumen institucional (KPIs generales, distribución, matrícula, alumnos en riesgo)
   2. Rendimiento por sección (cards por sección con materias y promedios)
   Absorbe la lógica de rendimientoporseccion.js. Los dos tabs comparten
   los filtros Año + Parcial y el mismo endpoint de rendimiento (con caché). */
var KPIsAcademicos = (function () {
    var obj = {};
    var urls = {};
    var chartInstances = {};

    // Nota mínima de aprobación (configurable vía GraduacionAlumnos/NotaMinimaAsync)
    var NOTA_MIN_DEFAULT = 70;
    var notaMin = NOTA_MIN_DEFAULT;

    // Caché del rendimiento compartido entre tabs: una sola petición por (año, parcial)
    var rendCache = { key: null, data: null };

    // Estado de tabs
    var tabActiva = 'resumen';
    var tabCargada = { resumen: false, rendimiento: false };

    // ─── Helpers ────────────────────────────────────────────────────────────────

    function escapeHtml(str) {
        if (str == null) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    /* Clasificación ÚNICA de promedios para TODO el hub (colores, doughnut, badges):
       aprobado  >= notaMin
       riesgo    >= notaMin - 10 y < notaMin
       reprobado <  notaMin - 10 */
    function clasificar(prom) {
        if (prom == null || isNaN(prom)) return 'nodata';
        if (prom >= notaMin) return 'aprobado';
        if (prom >= notaMin - 10) return 'riesgo';
        return 'reprobado';
    }

    // Mapea la clasificación al color contextual (badge-*, bg-*, text-*)
    function claseScore(prom) {
        switch (clasificar(prom)) {
            case 'aprobado': return 'success';
            case 'riesgo':   return 'warning';
            case 'nodata':   return 'secondary';
            default:         return 'danger';
        }
    }

    // Color por clasificación; los aprobados >= 85 se destacan como sobresalientes
    function colorPorRango(prom) {
        switch (clasificar(prom)) {
            case 'aprobado': return prom >= 85 ? '#3b82f6' : '#22c55e';
            case 'riesgo':   return '#f59e0b';
            case 'reprobado': return '#ef4444';
            default:         return '#94a3b8';
        }
    }

    function destroyChart(key) {
        if (chartInstances[key]) {
            chartInstances[key].destroy();
            chartInstances[key] = null;
        }
    }

    function filtrosActuales() {
        return {
            anio:  $("#filtro-anio").val(),
            parId: $("#filtro-parcial").val()
        };
    }

    // ─── Init ────────────────────────────────────────────────────────────────────

    obj.init = function (urlsObj) {
        urls = urlsObj || {};

        $("#filtro-anio").val(new Date().getFullYear());

        // Nota mínima primero: alimenta la clasificación de ambos tabs
        cargarNotaMinima(function () {
            cargarParciales(function () {
                cargarTabActiva(true);
            });
        });

        $("#btn-filtrar").on("click", function () {
            // Al consultar se invalida el caché y se recarga solo el tab activo;
            // el otro tab se recargará al entrar (lazy).
            rendCache = { key: null, data: null };
            tabCargada = { resumen: false, rendimiento: false };
            cargarTabActiva(true);
        });

        $("#aa-tabs").on("click", ".nav-link", function (e) {
            e.preventDefault();
            var tab = $(this).data("tab");
            if (!tab || tab === tabActiva) return;
            activarTab(tab);
        });
    };

    function activarTab(tab) {
        tabActiva = tab;
        $("#aa-tabs .nav-link").removeClass("active");
        $("#aa-tabs .nav-link[data-tab='" + tab + "']").addClass("active");
        $(".aa-tab-panel").hide();
        $("#tab-" + tab).show();
        cargarTabActiva(false);
    }

    function cargarTabActiva(forzar) {
        if (!forzar && tabCargada[tabActiva]) return;
        tabCargada[tabActiva] = true;

        var f = filtrosActuales();
        if (tabActiva === 'resumen') {
            cargarResumen(f.anio, f.parId);
        } else {
            cargarTabRendimiento(f.anio, f.parId);
        }
    }

    // ─── Nota mínima configurable (fallback 70) ──────────────────────────────────

    function cargarNotaMinima(callback) {
        if (!urls.urlNotaMinima) {
            aplicarNotaMinima();
            if (typeof callback === 'function') callback();
            return;
        }

        $.ajax({
            url: urls.urlNotaMinima,
            type: 'GET',
            success: function (resp) {
                var v = resp && resp.valor != null ? parseFloat(resp.valor) : NaN;
                notaMin = (!isNaN(v) && v > 0 && v <= 100) ? v : NOTA_MIN_DEFAULT;
                aplicarNotaMinima();
                if (typeof callback === 'function') callback();
            },
            error: function () {
                notaMin = NOTA_MIN_DEFAULT;
                aplicarNotaMinima();
                if (typeof callback === 'function') callback();
            }
        });
    }

    function aplicarNotaMinima() {
        $("#sub-en-riesgo").text("Promedio menor a " + notaMin + " — Top 15");
    }

    // ─── Cargar parciales en select compartido ────────────────────────────────────

    function cargarParciales(callback) {
        if (!urls.urlDropdowns) {
            if (typeof callback === 'function') callback();
            return;
        }

        $.ajax({
            url: urls.urlDropdowns,
            type: 'GET',
            success: function (resp) {
                var $select = $("#filtro-parcial");
                $select.empty();

                var parciales = (resp && resp.parciales) ? resp.parciales : [];
                $.each(parciales, function (i, p) {
                    $select.append(
                        '<option value="' + escapeHtml(p.id) + '">' + escapeHtml(p.texto) + '</option>'
                    );
                });

                if (parciales.length > 0) {
                    $select.val(parciales[0].id);
                } else {
                    $select.append('<option value="">Parcial 1</option>');
                }

                if (typeof callback === 'function') callback();
            },
            error: function () {
                $("#filtro-parcial").html('<option value="">Parcial 1</option>');
                if (typeof callback === 'function') callback();
            }
        });
    }

    // ─── Rendimiento compartido (una sola petición por año+parcial) ───────────────

    function getRendimiento(anio, parId, callback) {
        if (!urls.urlRendimiento) return;

        var key = anio + '|' + parId;
        if (rendCache.key === key && rendCache.data) {
            callback(rendCache.data);
            return;
        }

        $.ajax({
            url: urls.urlRendimiento,
            type: 'GET',
            data: { Anio: anio, Par_Id: parId },
            success: function (resp) {
                var filas = (resp && resp.data) ? resp.data : [];
                rendCache = { key: key, data: filas };
                callback(filas);
            },
            error: function (xhr) {
                console.error("Analítica Académica — rendimiento error:", xhr.status, xhr.responseText);
                callback(null);
            }
        });
    }

    // ══════════ TAB 1: Resumen institucional ══════════

    function cargarResumen(anio, parId) {
        cargarKPIsGenerales(anio);
        getRendimiento(anio, parId, renderResumenRendimiento);
        cargarEnRiesgo(anio);
        cargarPiramide(anio);
    }

    // ─── 1. KPIs generales ───────────────────────────────────────────────────────

    function cargarKPIsGenerales(anio) {
        if (!urls.urlFind) return;

        $.ajax({
            url: urls.urlFind,
            type: 'GET',
            data: { Anio: anio },
            success: function (resp) {
                var d = (resp && resp.data) ? resp.data : {};

                var totalAlumnos  = d.TotalAlumnosActivos != null ? d.TotalAlumnosActivos : 0;
                var totalDocentes = d.TotalDocentes != null ? d.TotalDocentes : 0;
                var promedio      = d.PromedioGeneralInstitucional != null ? d.PromedioGeneralInstitucional : 0;
                var asistencia    = d.PorcentajeAsistencia != null ? d.PorcentajeAsistencia : 0;

                // Semáforo promedio — usa la clasificación compartida
                var promVal = parseFloat(promedio);
                var promTexto = promedio != null ? promVal.toFixed(2) : "—";
                var promClase = promedio != null ? "text-" + claseScore(promVal) : "";
                $("#kpi-total-alumnos").text(totalAlumnos);
                $("#kpi-total-docentes").text(totalDocentes);
                $("#kpi-promedio-general").text(promTexto).removeClass("text-success text-warning text-danger text-secondary").addClass(promClase);

                // Semáforo asistencia
                var asistVal = parseFloat(asistencia);
                var asistTexto = asistencia != null ? asistVal.toFixed(1) + "%" : "—";
                var asistClase = asistencia == null ? "" : (asistVal >= 90 ? "text-success" : asistVal >= 75 ? "text-warning" : "text-danger");
                $("#kpi-porcentaje-asistencia").text(asistTexto).removeClass("text-success text-warning text-danger").addClass(asistClase);

                // Barra de asistencia
                if (asistencia != null) {
                    var pct = Math.min(Math.max(asistVal, 0), 100);
                    var barraClase = pct >= 90 ? "bg-success" : pct >= 75 ? "bg-warning" : "bg-danger";
                    $("#barra-asistencia")
                        .css("width", pct + "%")
                        .attr("aria-valuenow", pct)
                        .removeClass("bg-success bg-warning bg-danger")
                        .addClass(barraClase);
                    $("#barra-asistencia-texto").text(pct.toFixed(1) + "%");
                    $("#card-asistencia-barra").removeClass("d-none");
                } else {
                    $("#card-asistencia-barra").addClass("d-none");
                }
            },
            error: function () {
                $("#kpi-total-alumnos, #kpi-total-docentes, #kpi-promedio-general, #kpi-porcentaje-asistencia").text("—");
            }
        });
    }

    // ─── 2. Distribución de rendimiento (tab Resumen) ─────────────────────────────

    function renderResumenRendimiento(filas) {
        if (!filas) return;

        var total         = filas.length;
        var aprobados     = 0;
        var enRiesgo      = 0;
        var reprobados    = 0;
        var sobresaliente = 0;

        // Agrupar materias únicas: mayor promedio por Cur_Descripcion
        var materiasMap = {};

        $.each(filas, function (i, f) {
            var prom = parseFloat(f.PromedioGeneral) || 0;

            switch (clasificar(prom)) {
                case 'aprobado':
                    if (prom >= 85) sobresaliente++;
                    else            aprobados++;
                    break;
                case 'riesgo':   enRiesgo++;   break;
                default:         reprobados++; break;
            }

            var key = f.Cur_Descripcion || '';
            if (!materiasMap.hasOwnProperty(key) || prom > materiasMap[key]) {
                materiasMap[key] = prom;
            }
        });

        var totalMaterias = Object.keys(materiasMap).length;

        // KPI cards de distribución
        $("#kpi-aprobados").text(aprobados + sobresaliente);
        $("#kpi-en-riesgo").text(enRiesgo);
        $("#kpi-reprobados").text(reprobados);
        $("#kpi-materias").text(totalMaterias);

        if (total > 0) {
            $("#kpi-aprobados-pct").text(((aprobados + sobresaliente) / total * 100).toFixed(0) + '% del total');
            $("#kpi-en-riesgo-pct").text((enRiesgo / total * 100).toFixed(0) + '% del total');
            $("#kpi-reprobados-pct").text((reprobados / total * 100).toFixed(0) + '% del total');
        } else {
            $("#kpi-aprobados-pct, #kpi-en-riesgo-pct, #kpi-reprobados-pct").text('0%');
        }
        $("#kpi-secciones-sub").text(totalMaterias + ' materias únicas');

        renderDistribucion(reprobados, enRiesgo, aprobados, sobresaliente);
        renderTopMaterias(materiasMap);
    }

    function renderDistribucion(reprobados, enRiesgo, aprobados, sobresaliente) {
        destroyChart('distribucion');

        var ctx = document.getElementById('chart-distribucion');
        if (!ctx) return;

        var limReprobado = notaMin - 10;

        chartInstances['distribucion'] = new Chart(ctx.getContext('2d'), {
            type: 'doughnut',
            data: {
                labels: [
                    'Reprobado (<' + limReprobado + ')',
                    'En Riesgo (' + limReprobado + ' a <' + notaMin + ')',
                    'Aprobado (' + notaMin + ' a <85)',
                    'Sobresaliente (≥85)'
                ],
                datasets: [{
                    data: [reprobados, enRiesgo, aprobados, sobresaliente],
                    backgroundColor: ['#ef4444', '#f59e0b', '#22c55e', '#3b82f6'],
                    borderWidth: 2,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                cutout: '65%',
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            font: { size: 11 },
                            color: '#64748b',
                            padding: 12
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (ctx) {
                                var total = ctx.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                                var pct = total > 0 ? (ctx.raw / total * 100).toFixed(1) : 0;
                                return ' ' + ctx.raw + ' (' + pct + '%)';
                            }
                        }
                    }
                }
            }
        });
    }

    function renderTopMaterias(materiasMap) {
        destroyChart('topMaterias');

        var ctx = document.getElementById('chart-top-materias');
        if (!ctx) return;

        // Convertir a array, ordenar descendente, top 10
        var arr = [];
        $.each(materiasMap, function (nombre, prom) {
            arr.push({ nombre: nombre, prom: prom });
        });
        arr.sort(function (a, b) { return b.prom - a.prom; });
        arr = arr.slice(0, 10);

        var labels = arr.map(function (x) { return x.nombre; });
        var data   = arr.map(function (x) { return parseFloat(x.prom.toFixed(1)); });
        var colors = arr.map(function (x) { return colorPorRango(x.prom); });

        chartInstances['topMaterias'] = new Chart(ctx.getContext('2d'), {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Promedio',
                    data: data,
                    backgroundColor: colors,
                    borderRadius: 4,
                    borderSkipped: false
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    x: {
                        min: 0,
                        max: 100,
                        grid: { color: '#f1f5f9' },
                        ticks: { font: { size: 11 }, color: '#64748b' }
                    },
                    y: {
                        grid: { display: false },
                        ticks: { font: { size: 11 }, color: '#64748b' }
                    }
                }
            }
        });
    }

    // ─── 3. Alumnos en riesgo ─────────────────────────────────────────────────────

    function cargarEnRiesgo(anio) {
        if (!urls.urlEnRiesgo) return;

        $.ajax({
            url: urls.urlEnRiesgo,
            type: 'GET',
            data: { Anio: anio },
            success: function (resp) {
                var filas = (resp && resp.data) ? resp.data : [];
                var $tbody = $("#tabla-en-riesgo");
                $tbody.empty();

                if (!filas || filas.length === 0) {
                    $tbody.html(
                        '<tr><td colspan="3" class="text-center text-muted py-3">Sin datos.</td></tr>'
                    );
                    return;
                }

                var max = Math.min(filas.length, 15);
                for (var i = 0; i < max; i++) {
                    var f = filas[i];

                    // El API expone NombreAlumno / PromedioAnual (se dejan los nombres
                    // antiguos como fallback por compatibilidad).
                    var nombre  = f.NombreAlumno != null ? f.NombreAlumno : f.Alu_NombreCompleto;
                    var promRaw = f.PromedioAnual != null ? f.PromedioAnual : f.Promedio;
                    var prom    = parseFloat(promRaw) || 0;
                    var cls     = 'text-' + claseScore(prom) + ' font-weight-bold';
                    var aluId   = f.Alu_Id != null ? parseInt(f.Alu_Id, 10) : null;

                    var clickable = aluId && urls.urlDetalleAlumno;
                    var trAttrs = clickable
                        ? ' class="kpi-riesgo-row" data-alu-id="' + aluId + '" title="Ver ficha del alumno" tabindex="0"'
                        : '';

                    $tbody.append(
                        '<tr' + trAttrs + '>' +
                        '<td>' + escapeHtml(nombre) + '</td>' +
                        '<td>' + escapeHtml(f.Sec_Descripcion) + '</td>' +
                        '<td class="text-right"><span class="' + cls + '">' + prom.toFixed(1) + '</span></td>' +
                        '</tr>'
                    );
                }

                // Fila clickeable → ficha 360 del alumno
                $tbody.off("click.riesgo").on("click.riesgo", ".kpi-riesgo-row", function () {
                    var id = $(this).data("alu-id");
                    if (id && urls.urlDetalleAlumno) {
                        window.location.href = urls.urlDetalleAlumno + '/' + id;
                    }
                });
            },
            error: function () {
                $("#tabla-en-riesgo").html(
                    '<tr><td colspan="3" class="text-center text-muted py-3">Sin datos.</td></tr>'
                );
            }
        });
    }

    // ─── 4. Pirámide de género por grado ─────────────────────────────────────────

    function cargarPiramide(anio) {
        if (!urls.urlPiramide) return;

        $.ajax({
            url: urls.urlPiramide,
            type: 'GET',
            data: { Anio: anio },
            success: function (resp) {
                var filas = (resp && resp.data) ? resp.data : [];
                renderPiramide(filas);
            }
        });
    }

    function renderPiramide(filas) {
        destroyChart('piramide');

        var ctx = document.getElementById('chart-piramide');
        if (!ctx) return;

        var labels    = filas.map(function (f) { return f.Cur_Nombre || ''; });
        var masculino = filas.map(function (f) { return f.TotalMasculino || 0; });
        var femenino  = filas.map(function (f) { return f.TotalFemenino  || 0; });

        chartInstances['piramide'] = new Chart(ctx.getContext('2d'), {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Masculino',
                        data: masculino,
                        backgroundColor: '#3b82f6',
                        borderRadius: 4,
                        borderSkipped: false
                    },
                    {
                        label: 'Femenino',
                        data: femenino,
                        backgroundColor: '#ec4899',
                        borderRadius: 4,
                        borderSkipped: false
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            font: { size: 11 },
                            color: '#64748b',
                            padding: 12
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { color: '#f1f5f9' },
                        ticks: { font: { size: 11 }, color: '#64748b' }
                    },
                    y: {
                        grid: { color: '#f1f5f9' },
                        ticks: { font: { size: 11 }, color: '#64748b' }
                    }
                }
            }
        });
    }

    // ══════════ TAB 2: Rendimiento por sección ══════════
    // (portado de rendimientoporseccion.js, usando la clasificación compartida)

    function cargarTabRendimiento(anio, parId) {
        var container = $("#rendimiento-container");
        container.html('<div class="text-center text-muted py-5"><i class="fa-solid fa-spinner fa-spin fa-2x mb-2"></i><p class="mb-0">Cargando datos...</p></div>');
        $("#rps-kpis").hide();

        getRendimiento(anio, parId, function (datos) {
            if (datos == null) {
                container.html('<div class="alert alert-light-danger" role="alert"><i class="fa-solid fa-triangle-exclamation mr-1"></i> Error al cargar los datos. Verifique que la API esté en ejecución.</div>');
                return;
            }
            renderSecciones(datos);
        });
    }

    function renderSecciones(datos) {
        var container = $("#rendimiento-container");
        var kpis      = $("#rps-kpis");

        if (!datos || datos.length === 0) {
            kpis.hide();
            container.html('<div class="statbox widget box box-shadow"><div class="widget-content widget-content-area text-center text-muted py-5"><i class="fa-solid fa-folder-open fa-3x mb-3"></i><h5>Sin datos para los filtros seleccionados</h5><p class="mb-0">Probá con otro año o parcial.</p></div></div>');
            return;
        }

        // Agrupar primero por sección
        var grupos = {};
        datos.forEach(function (d) {
            var sec = d.Sec_Descripcion || "Sin sección";
            if (!grupos[sec]) grupos[sec] = [];
            grupos[sec].push(d);
        });

        // KPIs agregados
        var promedios   = datos.filter(function (d) { return d.PromedioGeneral != null; }).map(function (d) { return parseFloat(d.PromedioGeneral); });
        var promGeneral = promedios.length > 0 ? (promedios.reduce(function (a, b) { return a + b; }, 0) / promedios.length) : null;

        // Total alumnos: máximo por sección (no sumar curso a curso, generaría duplicados)
        var totAlumnos = 0;
        Object.keys(grupos).forEach(function (sec) {
            var max = grupos[sec].reduce(function (m, d) { return Math.max(m, d.TotalAlumnos || 0); }, 0);
            totAlumnos += max;
        });

        var cursos = {};
        datos.forEach(function (d) { cursos[d.Cur_Descripcion] = true; });

        $("#kpi-promedio").html(promGeneral != null
            ? '<span class="text-' + claseScore(promGeneral) + '">' + promGeneral.toFixed(2) + '</span>'
            : '—');
        $("#kpi-alumnos").text(totAlumnos);
        $("#kpi-secciones").text(Object.keys(grupos).length);
        $("#kpi-cursos").text(Object.keys(cursos).length);
        kpis.show();

        // Render cards
        var html = '<div class="row">';
        Object.keys(grupos).sort().forEach(function (seccion) {
            var materias = grupos[seccion];

            var promsSec = materias.filter(function (m) { return m.PromedioGeneral != null; }).map(function (m) { return parseFloat(m.PromedioGeneral); });
            var promSec  = promsSec.length > 0 ? (promsSec.reduce(function (a, b) { return a + b; }, 0) / promsSec.length) : null;
            var aluSec   = materias.reduce(function (max, m) { return Math.max(max, m.TotalAlumnos || 0); }, 0);

            html += '<div class="col-xl-4 col-md-6 col-12 layout-spacing">';
            html += '<div class="statbox widget box box-shadow h-100">';
            html +=   '<div class="widget-header"><div class="row"><div class="col-12 d-flex flex-wrap align-items-center justify-content-between">';
            html +=     '<h4><i class="fa-solid fa-users mr-1"></i> Sección ' + escapeHtml(seccion) + '</h4>';
            html +=     '<div>';
            html +=       '<span class="badge outline-badge-primary mr-1"><i class="fa-solid fa-user-graduate mr-1"></i>' + aluSec + ' alumnos</span>';
            if (promSec != null) {
                html +=   '<span class="badge outline-badge-' + claseScore(promSec) + '"><i class="fa-solid fa-chart-line mr-1"></i>' + promSec.toFixed(2) + '</span>';
            }
            html +=     '</div>';
            html +=   '</div></div></div>';
            html +=   '<div class="widget-content widget-content-area">';

            materias.forEach(function (m) {
                var prom    = m.PromedioGeneral != null ? parseFloat(m.PromedioGeneral) : null;
                var cls     = claseScore(prom);
                var width   = prom != null ? Math.max(0, Math.min(100, prom)) : 0;
                var promTxt = prom != null ? prom.toFixed(2) : 'Sin notas';

                html += '<div class="mb-3">';
                html +=   '<div class="d-flex justify-content-between align-items-center mb-1">';
                html +=     '<span class="font-weight-bold">' + escapeHtml(m.Cur_Descripcion) + '</span>';
                html +=     '<span><span class="badge badge-' + cls + ' mr-1">' + promTxt + '</span>';
                html +=     '<span class="text-muted small"><i class="fa-solid fa-user-graduate mr-1"></i>' + (m.TotalAlumnos || 0) + '</span></span>';
                html +=   '</div>';
                html +=   '<div class="progress mb-0">';
                html +=     '<div class="progress-bar bg-' + cls + '" role="progressbar" style="width:' + width.toFixed(1) + '%"></div>';
                html +=   '</div>';
                html += '</div>';
            });

            html +=   '</div>';
            html += '</div>';
            html += '</div>';
        });
        html += '</div>';

        container.html(html);
    }

    return obj;
}());
