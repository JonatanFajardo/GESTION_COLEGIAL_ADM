var DashboardConsulta = (function () {
    'use strict';

    var _baseUrl = '';
    var _charts  = {};

    var PALETTE_INDIGO = ['#4338ca','#7c3aed','#a855f7','#ec4899','#f59e0b','#22c55e','#14b8a6','#0ea5e9','#3b82f6','#f43f5e'];

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = (value !== null && value !== undefined && value !== '') ? value : '—';
    }
    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-HN');
    }
    function fmtPct(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toFixed(2) + '%';
    }
    function destroyChart(key) {
        if (_charts[key] && typeof _charts[key].destroy === 'function') _charts[key].destroy();
        _charts[key] = null;
    }

    function poblarSelectores() {
        var anioActual = new Date().getFullYear();
        var selAnio = document.getElementById('sel-anio');
        if (!selAnio) return;
        selAnio.innerHTML = '';
        for (var a = anioActual - 2; a <= anioActual + 1; a++) {
            var opt = document.createElement('option');
            opt.value = a; opt.textContent = a;
            if (a === anioActual) opt.selected = true;
            selAnio.appendChild(opt);
        }
    }

    function cargar() {
        var anio = (document.getElementById('sel-anio') || {}).value || new Date().getFullYear();
        $.getJSON(_baseUrl + '?Anio=' + anio, function (res) {
            var d = res || {};
            renderKpis(d.Kpis || {});
            renderEvolucion(d.EvolucionMatricula || []);
            renderNivel(d.MatriculaPorNivel || []);
            renderJornada(d.MatriculaPorJornada || []);
            renderGenero(d.AlumnosPorGenero || []);
            renderCursos(d.MatriculaPorCurso || []);
            renderEdad(d.AlumnosPorEdad || []);
            renderDepartamento(d.EmpleadosPorDepartamento || []);
            renderCargo(d.EmpleadosPorCargo || []);
            renderIndicadores(d.IndicadoresInstitucionales || []);
        }).fail(function (xhr, status, err) {
            console.warn('DashboardConsulta: error al cargar.', status, err);
        });
    }

    function renderKpis(k) {
        setText('kpi-alumnos',          fmtNum(k.TotalAlumnos));
        setText('kpi-alumnos-activos',  fmtNum(k.TotalAlumnosActivos) + ' activos');
        setText('kpi-empleados',        fmtNum(k.TotalEmpleados));
        setText('kpi-docentes',         fmtNum(k.TotalDocentes));
        setText('kpi-cursos',           fmtNum(k.TotalCursos));
        setText('kpi-clases',           fmtNum(k.TotalClasesHorario));
        setText('kpi-niveles',          fmtNum(k.TotalNiveles));
        setText('kpi-jornadas',         fmtNum(k.TotalJornadas));
        setText('kpi-cobrado',          fmtPct(k.PorcentajeCobrado));
    }

    function renderEvolucion(data) {
        destroyChart('evolucion');
        var ctx = document.getElementById('chart-evolucion');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) {
            labels.push(String(data[i].Anio || ''));
            valores.push(data[i].TotalAlumnos || 0);
        }
        var el = document.getElementById('stat-evolucion');
        if (el && valores.length > 1) {
            var ult = valores[valores.length - 1], pen = valores[valores.length - 2];
            var diff = pen > 0 ? (((ult - pen) / pen) * 100).toFixed(1) : '0';
            el.textContent = (Number(diff) >= 0 ? '+' : '') + diff + '% vs año anterior';
        }
        var grad = ctx.getContext('2d').createLinearGradient(0, 0, 0, 230);
        grad.addColorStop(0, 'rgba(124,58,237,.35)');
        grad.addColorStop(1, 'rgba(124,58,237,0)');
        _charts['evolucion'] = new Chart(ctx, {
            type: 'line',
            data: { labels: labels, datasets: [{
                label: 'Matrícula',
                data: valores,
                borderColor: '#7c3aed',
                backgroundColor: grad,
                fill: true, tension: 0.4,
                pointBackgroundColor: '#7c3aed',
                pointBorderColor: '#fff', pointBorderWidth: 2,
                pointRadius: 5, pointHoverRadius: 7, borderWidth: 3
            }]},
            options: {
                responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' ' + fmtNum(c.parsed.y) + ' alumnos'; } } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 11, weight: '600' }, color: '#64748b' } },
                    y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 }, color: '#64748b', precision: 0 } }
                }
            }
        });
    }

    function renderNivel(data) {
        destroyChart('nivel');
        var ctx = document.getElementById('chart-nivel');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].NivelNombre || '—'); valores.push(data[i].TotalAlumnos || 0); }
        _charts['nivel'] = new Chart(ctx, {
            type: 'doughnut',
            data: { labels: labels, datasets: [{ data: valores, backgroundColor: PALETTE_INDIGO.slice(0, labels.length), borderWidth: 2, borderColor: '#fff' }] },
            options: {
                responsive: true, maintainAspectRatio: true, cutout: '62%',
                plugins: {
                    legend: { position: 'bottom', labels: { font: { size: 11 }, boxWidth: 12, padding: 10 } },
                    tooltip: { callbacks: { label: function (c) { var t = c.dataset.data.reduce(function(a,b){return a+b;},0); return ' ' + c.label + ': ' + fmtNum(c.parsed) + ' (' + ((c.parsed/t)*100).toFixed(1) + '%)'; } } }
                }
            }
        });
    }

    function renderJornada(data) {
        destroyChart('jornada');
        var ctx = document.getElementById('chart-jornada');
        if (!ctx) return;
        var labels = [], valores = [];
        var colors = ['#f43f5e','#4338ca','#0ea5e9','#f59e0b'];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].JornadaNombre || '—'); valores.push(data[i].TotalAlumnos || 0); }
        _charts['jornada'] = new Chart(ctx, {
            type: 'pie',
            data: { labels: labels, datasets: [{ data: valores, backgroundColor: colors.slice(0, labels.length), borderWidth: 2, borderColor: '#fff' }] },
            options: {
                responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { position: 'bottom', labels: { font: { size: 11 }, boxWidth: 12, padding: 10 } },
                    tooltip: { callbacks: { label: function (c) { var t = c.dataset.data.reduce(function(a,b){return a+b;},0); return ' ' + c.label + ': ' + fmtNum(c.parsed) + ' (' + ((c.parsed/t)*100).toFixed(1) + '%)'; } } }
                }
            }
        });
    }

    function renderGenero(data) {
        destroyChart('genero');
        var ctx = document.getElementById('chart-genero');
        if (!ctx) return;
        var labels = [], valores = [];
        var colors = ['#3b82f6','#ec4899','#94a3b8'];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].Genero || '—'); valores.push(data[i].Total || 0); }
        _charts['genero'] = new Chart(ctx, {
            type: 'pie',
            data: { labels: labels, datasets: [{ data: valores, backgroundColor: colors.slice(0, labels.length), borderWidth: 2, borderColor: '#fff' }] },
            options: {
                responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { position: 'bottom', labels: { font: { size: 11 }, boxWidth: 12, padding: 10 } },
                    tooltip: { callbacks: { label: function (c) { var t = c.dataset.data.reduce(function(a,b){return a+b;},0); return ' ' + c.label + ': ' + fmtNum(c.parsed) + ' (' + ((c.parsed/t)*100).toFixed(1) + '%)'; } } }
                }
            }
        });
    }

    function renderCursos(data) {
        destroyChart('cursos');
        var ctx = document.getElementById('chart-cursos');
        if (!ctx) return;
        var labels = [], valores = [], total = 0;
        for (var i = 0; i < data.length; i++) {
            var nom = (data[i].CursoNombre || '—').replace(/\s+/g, ' ').trim();
            labels.push(nom);
            valores.push(data[i].TotalAlumnos || 0);
            total += data[i].TotalAlumnos || 0;
        }
        var el = document.getElementById('stat-cursos');
        if (el) el.textContent = total > 0 ? fmtNum(total) + ' alumnos' : '';
        _charts['cursos'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Alumnos', data: valores, backgroundColor: '#0ea5e9', borderRadius: 5, borderSkipped: false }] },
            options: {
                indexAxis: 'y', responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' ' + fmtNum(c.parsed.x) + ' alumnos'; } } }
                },
                scales: {
                    x: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 }, color: '#64748b', precision: 0 } },
                    y: { grid: { display: false }, ticks: { font: { size: 11 }, color: '#334155' } }
                }
            }
        });
    }

    function renderEdad(data) {
        destroyChart('edad');
        var ctx = document.getElementById('chart-edad');
        if (!ctx) return;
        var labels = [], valores = [];
        var colors = ['#22c55e','#3b82f6','#6366f1','#a855f7','#f59e0b'];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].RangoEdad || '—'); valores.push(data[i].Total || 0); }
        _charts['edad'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Alumnos', data: valores, backgroundColor: colors.slice(0, labels.length), borderRadius: 5, borderSkipped: false }] },
            options: {
                responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' ' + fmtNum(c.parsed.y) + ' alumnos'; } } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 11 }, color: '#64748b' } },
                    y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 }, color: '#64748b', precision: 0 } }
                }
            }
        });
    }

    function renderDepartamento(data) {
        destroyChart('departamento');
        var ctx = document.getElementById('chart-departamento');
        if (!ctx) return;
        var labels = [], valores = [], total = 0;
        for (var i = 0; i < data.length; i++) { labels.push(data[i].DepartamentoNombre || '—'); valores.push(data[i].TotalEmpleados || 0); total += data[i].TotalEmpleados || 0; }
        var el = document.getElementById('stat-deptos');
        if (el) el.textContent = fmtNum(total) + ' empleados';
        _charts['departamento'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Empleados', data: valores, backgroundColor: '#14b8a6', borderRadius: 5, borderSkipped: false }] },
            options: {
                indexAxis: 'y', responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' ' + fmtNum(c.parsed.x) + ' empleados'; } } }
                },
                scales: {
                    x: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 }, color: '#64748b', precision: 0 } },
                    y: { grid: { display: false }, ticks: { font: { size: 11 }, color: '#334155' } }
                }
            }
        });
    }

    function renderCargo(data) {
        destroyChart('cargo');
        var ctx = document.getElementById('chart-cargo');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].CargoNombre || '—'); valores.push(data[i].TotalEmpleados || 0); }
        _charts['cargo'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Empleados', data: valores, backgroundColor: '#22c55e', borderRadius: 5, borderSkipped: false }] },
            options: {
                responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' ' + fmtNum(c.parsed.y) + ' empleados'; } } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 10 }, color: '#64748b', maxRotation: 30 } },
                    y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 }, color: '#64748b', precision: 0 } }
                }
            }
        });
    }

    function renderIndicadores(data) {
        var cont = document.getElementById('lista-indicadores');
        if (!cont) return;
        if (!data || data.length === 0) {
            cont.innerHTML = '<p style="color:#94a3b8;font-size:.82rem;margin:0;">Sin indicadores disponibles.</p>';
            return;
        }
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var ind = data[i];
            var tipo = (ind.Tipo || 'neutral').toLowerCase();
            var icono = ind.Icono || 'fa-chart-simple';
            html += '<div class="ind-card ' + tipo + '">'
                  + '<div class="ind-icon"><i class="fa-solid ' + icono + '"></i></div>'
                  + '<div class="ind-info">'
                  + '<div class="ind-label">' + (ind.Indicador || '') + '</div>'
                  + '<div class="ind-value">' + (ind.Valor || '—') + '</div>'
                  + '</div></div>';
        }
        cont.innerHTML = html;
    }

    var obj = {};
    obj.init = function (url) {
        _baseUrl = url || '';
        poblarSelectores();
        cargar();
        var btn = document.getElementById('btn-actualizar');
        if (btn) btn.addEventListener('click', function () { cargar(); });
        var sel = document.getElementById('sel-anio');
        if (sel) sel.addEventListener('change', function () { cargar(); });
    };
    return obj;
}());
