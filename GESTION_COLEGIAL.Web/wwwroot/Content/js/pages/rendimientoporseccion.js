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
        container.html('<div class="text-center text-muted py-5"><i class="fa-solid fa-spinner fa-spin fa-2x mb-2"></i><p class="mb-0">Cargando datos...</p></div>');
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
                container.html('<div class="alert alert-light-danger" role="alert"><i class="fa-solid fa-triangle-exclamation mr-1"></i> Error al cargar los datos. Verifique que la API esté en ejecución.</div>');
                console.error("RendimientoPorSeccion ListAsync error:", xhr.status, xhr.responseText);
            }
        });
    }

    function render(datos) {
        var container = $("#rendimiento-container");
        var kpis      = $("#rps-kpis");

        if (datos.length === 0) {
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

    function claseScore(prom) {
        if (prom == null) return 'secondary';
        if (prom >= 70)   return 'success';
        if (prom >= 50)   return 'warning';
        return 'danger';
    }

    function escapeHtml(str) {
        if (str == null) return "";
        return String(str)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;")
            .replace(/>/g, "&gt;").replace(/"/g, "&quot;");
    }

    return obj;
}());
