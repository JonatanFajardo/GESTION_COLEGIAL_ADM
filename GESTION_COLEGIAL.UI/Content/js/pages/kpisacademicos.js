var KPIsAcademicos = (function () {
    var obj = {};
    var urls = {};
    var chartInstances = {};

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

    function claseScore(prom) {
        if (prom >= 70) return 'alto';
        if (prom >= 60) return 'medio';
        return 'bajo';
    }

    function colorPorRango(prom) {
        if (prom >= 85) return '#3b82f6';
        if (prom >= 70) return '#22c55e';
        if (prom >= 60) return '#f59e0b';
        return '#ef4444';
    }

    function destroyChart(key) {
        if (chartInstances[key]) {
            chartInstances[key].destroy();
            chartInstances[key] = null;
        }
    }

    // ─── Init ────────────────────────────────────────────────────────────────────

    obj.init = function (urlsObj) {
        urls = urlsObj || {};

        $("#filtro-anio").val(new Date().getFullYear());

        cargarParciales(function () {
            cargarTodo();
        });

        $("#btn-filtrar").on("click", function () {
            cargarTodo();
        });
    };

    // ─── Cargar parciales en select ───────────────────────────────────────────────

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
                $select.empty().append('<option value="">Todos</option>');

                var parciales = (resp && resp.parciales) ? resp.parciales : [];
                $.each(parciales, function (i, p) {
                    $select.append(
                        '<option value="' + escapeHtml(p.id) + '">' + escapeHtml(p.texto) + '</option>'
                    );
                });

                if (typeof callback === 'function') callback();
            },
            error: function () {
                if (typeof callback === 'function') callback();
            }
        });
    }

    // ─── Cargar todo en paralelo ──────────────────────────────────────────────────

    function cargarTodo() {
        var anio  = $("#filtro-anio").val();
        var parId = $("#filtro-parcial").val();

        cargarKPIsGenerales(anio);
        cargarRendimiento(anio, parId);
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

                // Semáforo promedio
                var promVal = parseFloat(promedio);
                var promTexto = promedio != null ? promVal.toFixed(2) : "—";
                var promClase = promedio == null ? "" : (promVal >= 70 ? "promedio-alto" : promVal >= 60 ? "promedio-medio" : "promedio-bajo");
                $("#kpi-total-alumnos").text(totalAlumnos);
                $("#kpi-total-docentes").text(totalDocentes);
                $("#kpi-promedio-general").text(promTexto).removeClass("promedio-alto promedio-medio promedio-bajo").addClass(promClase);

                // Semáforo asistencia
                var asistVal = parseFloat(asistencia);
                var asistTexto = asistencia != null ? asistVal.toFixed(1) + "%" : "—";
                var asistClase = asistencia == null ? "" : (asistVal >= 90 ? "asistencia-alta" : asistVal >= 75 ? "asistencia-media" : "asistencia-baja");
                $("#kpi-porcentaje-asistencia").text(asistTexto).removeClass("asistencia-alta asistencia-media asistencia-baja").addClass(asistClase);

                // Barra de asistencia
                if (asistencia != null) {
                    var pct = Math.min(Math.max(asistVal, 0), 100);
                    var barraClase = pct >= 90 ? "" : pct >= 75 ? "bar-warning" : "bar-danger";
                    $("#barra-asistencia")
                        .css("width", pct + "%")
                        .attr("aria-valuenow", pct)
                        .removeClass("bar-warning bar-danger")
                        .addClass(barraClase);
                    $("#barra-asistencia-texto").text(pct.toFixed(1) + "%");
                    $("#card-asistencia-barra").removeClass("kpi-barra-oculta");
                } else {
                    $("#card-asistencia-barra").addClass("kpi-barra-oculta");
                }
            },
            error: function () {
                $("#kpi-total-alumnos, #kpi-total-docentes, #kpi-promedio-general, #kpi-porcentaje-asistencia").text("—");
            }
        });
    }

    // ─── 2. Rendimiento por sección/materia ──────────────────────────────────────

    function cargarRendimiento(anio, parId) {
        if (!urls.urlRendimiento) return;

        $.ajax({
            url: urls.urlRendimiento,
            type: 'GET',
            data: { Anio: anio, Par_Id: parId },
            success: function (resp) {
                var filas = (resp && resp.data) ? resp.data : [];

                var total         = filas.length;
                var aprobados     = 0;
                var enRiesgo      = 0;
                var reprobados    = 0;
                var sobresaliente = 0;

                // Agrupar materias únicas: mayor promedio por Cur_Descripcion
                var materiasMap = {};

                $.each(filas, function (i, f) {
                    var prom = parseFloat(f.PromedioGeneral) || 0;

                    if (prom >= 85)      sobresaliente++;
                    else if (prom >= 70) aprobados++;
                    else if (prom >= 60) enRiesgo++;
                    else                 reprobados++;

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
        });
    }

    function renderDistribucion(reprobados, enRiesgo, aprobados, sobresaliente) {
        destroyChart('distribucion');

        var ctx = document.getElementById('chart-distribucion');
        if (!ctx) return;

        chartInstances['distribucion'] = new Chart(ctx.getContext('2d'), {
            type: 'doughnut',
            data: {
                labels: ['Reprobado (<60)', 'En Riesgo (60-69)', 'Aprobado (70-84)', 'Sobresaliente (≥85)'],
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
                    var f    = filas[i];
                    var prom = parseFloat(f.Promedio) || 0;
                    var cls  = 'rend-' + claseScore(prom);

                    $tbody.append(
                        '<tr>' +
                        '<td>' + escapeHtml(f.Alu_NombreCompleto) + '</td>' +
                        '<td>' + escapeHtml(f.Sec_Descripcion) + '</td>' +
                        '<td><span class="' + cls + '">' + prom.toFixed(1) + '</span></td>' +
                        '</tr>'
                    );
                }
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

    return obj;
}());
