var RendimientoPorSeccion = (function () {
    var obj = {};
    var urlList, urlDropdowns;

    obj.init = function (urlL, urlD) {
        urlList      = urlL;
        urlDropdowns = urlD;

        $("#filtro-anio").val(new Date().getFullYear());
        cargarDropdowns();

        $("#btn-filtrar").on("click", cargarDatos);
    };

    function cargarDropdowns() {
        $.ajax({
            url: urlDropdowns,
            dataType: "json",
            success: function (response) {
                var parciales = response.parciales || [];
                var html = '<option value="">-- Seleccione --</option>';
                parciales.forEach(function (p) {
                    html += '<option value="' + p.id + '">' + p.texto + '</option>';
                });
                $("#filtro-parcial").html(html);
                if (parciales.length > 0) {
                    $("#filtro-parcial").val(parciales[0].id);
                    cargarDatos();
                }
            }
        });
    }

    function cargarDatos() {
        var anio  = $("#filtro-anio").val();
        var parId = $("#filtro-parcial").val();
        if (!parId) return;

        var container = $("#rendimiento-container");
        container.html('<div class="rps-loading"><i class="fa-solid fa-spinner fa-spin"></i><p>Cargando datos...</p></div>');
        $("#rps-kpis").hide();

        $.ajax({
            url: urlList,
            data: { Anio: anio, Par_Id: parId },
            dataType: "json",
            success: function (response) {
                var datos = (response && response.data) ? response.data : [];
                render(datos);
            },
            error: function (xhr) {
                container.html('<div class="rps-error"><i class="fa-solid fa-triangle-exclamation"></i><p>Error al cargar los datos. Verifique que la API esté en ejecución.</p></div>');
                console.error("RendimientoPorSeccion ListAsync error:", xhr.status, xhr.responseText);
            }
        });
    }

    function render(datos) {
        var container = $("#rendimiento-container");
        var kpis      = $("#rps-kpis");

        if (datos.length === 0) {
            kpis.hide();
            container.html('<div class="rps-empty-state"><i class="fa-solid fa-folder-open"></i><h3>Sin datos para los filtros seleccionados</h3><p>Probá con otro año o parcial.</p></div>');
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
            ? '<span class="' + claseScore(promGeneral) + '">' + promGeneral.toFixed(2) + '</span>'
            : '—');
        $("#kpi-alumnos").text(totAlumnos);
        $("#kpi-secciones").text(Object.keys(grupos).length);
        $("#kpi-cursos").text(Object.keys(cursos).length);
        kpis.show();

        // Render cards
        var html = '<div class="rps-secciones-grid">';
        Object.keys(grupos).sort().forEach(function (seccion) {
            var materias = grupos[seccion];

            var promsSec = materias.filter(function (m) { return m.PromedioGeneral != null; }).map(function (m) { return parseFloat(m.PromedioGeneral); });
            var promSec  = promsSec.length > 0 ? (promsSec.reduce(function (a, b) { return a + b; }, 0) / promsSec.length) : null;
            var aluSec   = materias.reduce(function (max, m) { return Math.max(max, m.TotalAlumnos || 0); }, 0);

            html += '<div class="rps-seccion">';
            html +=   '<div class="rps-seccion-header">';
            html +=     '<div class="rps-seccion-title"><i class="fa-solid fa-users"></i> Sección ' + escapeHtml(seccion) + '</div>';
            html +=     '<div class="rps-seccion-summary">';
            html +=       '<span class="rps-summary-pill"><i class="fa-solid fa-user-graduate"></i> ' + aluSec + ' alumnos</span>';
            if (promSec != null) {
                html +=   '<span class="rps-summary-pill"><i class="fa-solid fa-chart-line"></i> ' + promSec.toFixed(2) + '</span>';
            }
            html +=     '</div>';
            html +=   '</div>';

            materias.forEach(function (m) {
                var prom    = m.PromedioGeneral != null ? parseFloat(m.PromedioGeneral) : null;
                var cls     = claseScore(prom);
                var width   = prom != null ? Math.max(0, Math.min(100, prom)) : 0;
                var promTxt = prom != null ? prom.toFixed(2) : 'Sin notas';

                html += '<div class="rps-materia">';
                html +=   '<div class="rps-materia-nombre">' + escapeHtml(m.Cur_Descripcion) + '</div>';
                html +=   '<div class="rps-materia-meta">';
                html +=     '<span class="rps-promedio ' + cls + '">' + promTxt + '</span>';
                html +=     '<span class="rps-alumnos"><i class="fa-solid fa-user-graduate"></i> ' + (m.TotalAlumnos || 0) + '</span>';
                html +=   '</div>';
                html +=   '<div class="rps-progress-wrap">';
                html +=     '<div class="rps-progress-bar ' + cls + '" style="width:' + width.toFixed(1) + '%"></div>';
                html +=   '</div>';
                html += '</div>';
            });

            html += '</div>';
        });
        html += '</div>';

        container.html(html);
    }

    function claseScore(prom) {
        if (prom == null) return 'nodata';
        if (prom >= 70)   return 'alto';
        if (prom >= 50)   return 'medio';
        return 'bajo';
    }

    function escapeHtml(str) {
        if (str == null) return "";
        return String(str)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;")
            .replace(/>/g, "&gt;").replace(/"/g, "&quot;");
    }

    return obj;
}());
