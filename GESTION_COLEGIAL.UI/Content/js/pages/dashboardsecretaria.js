var DashboardSecretaria = (function () {
    'use strict';

    // Chart.js instances (destroy before recreate)
    var _chartMatriculaMes     = null;
    var _chartMatriculaSeccion = null;

    var _baseUrl = '';

    var MESES_CORTOS = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];

    // Utilidades

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-HN');
    }

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = (value !== null && value !== undefined) ? value : '—';
    }

    function destroyChart(chart) {
        if (chart && typeof chart.destroy === 'function') {
            chart.destroy();
        }
        return null;
    }

    function cleanText(txt) {
        if (txt === null || txt === undefined) return '';
        return String(txt).replace(/\s+/g, ' ').trim();
    }

    function escapeHtml(s) {
        if (s === null || s === undefined) return '';
        return String(s)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function initial(name) {
        var n = cleanText(name);
        if (!n) return '?';
        return n.charAt(0).toUpperCase();
    }

    function fmtFechaCorta(iso) {
        if (!iso) return '—';
        try {
            var d = new Date(iso);
            if (isNaN(d.getTime())) return '—';
            return d.toLocaleDateString('es-HN', { day: '2-digit', month: 'short' });
        } catch (e) { return '—'; }
    }

    // Poblar selector de Año

    function poblarSelectorAnio() {
        var anioActual = new Date().getFullYear();
        var selAnio = document.getElementById('sel-anio');
        if (!selAnio) return;

        selAnio.innerHTML = '';
        for (var a = anioActual - 2; a <= anioActual + 2; a++) {
            var opt = document.createElement('option');
            opt.value = a;
            opt.textContent = a;
            if (a === anioActual) opt.selected = true;
            selAnio.appendChild(opt);
        }
    }

    // Fetch y render

    function cargar() {
        var selAnio = document.getElementById('sel-anio');
        var anio = selAnio ? selAnio.value : new Date().getFullYear();
        var url = _baseUrl + '?Anio=' + anio;

        $.getJSON(url, function (res) {
            var d = res || {};
            renderKpis(d.Kpis                                || {});
            renderTramites(d.TramitesHoy                     || []);
            renderChartMatriculaMes(d.MatriculaPorMes        || []);
            renderChartMatriculaSeccion(d.MatriculaPorSeccion || []);
            renderDocsPorTipo(d.DocumentosPorTipo            || []);
            renderAlumnosDocs(d.AlumnosConDocsPendientes     || []);
            renderEncargados(d.EncargadosIncompletos         || []);
            renderCumpleanos(d.CumpleanosSemana              || []);
        }).fail(function (xhr, status, err) {
            console.warn('DashboardSecretaria: no se pudo cargar el resumen.', status, err);
        });
    }

    // KPI Cards

    function renderKpis(k) {
        setText('kpi-matriculados',     fmtNum(k.TotalMatriculados));
        setText('kpi-nuevas-mes',       fmtNum(k.NuevasMatriculasMes));
        setText('kpi-docs-pendientes',  fmtNum(k.DocumentosPendientes));
        setText('kpi-sin-foto',         fmtNum(k.AlumnosSinFoto));
        setText('kpi-enc-incompletos',  fmtNum(k.EncargadosIncompletos));
        setText('kpi-cumple-semana',    fmtNum(k.CumpleanosSemana));
    }

    // Bandeja Tramites de Hoy

    function renderTramites(data) {
        var cont = document.getElementById('lista-tramites');
        if (!cont) return;

        if (!data || data.length === 0) {
            cont.innerHTML = '<p class="dd-empty">No hay trámites pendientes hoy.</p>';
            return;
        }

        // Ordenar por Orden ascendente si existe
        data.sort(function (a, b) {
            var oa = (a.Orden !== undefined && a.Orden !== null) ? a.Orden : 999;
            var ob = (b.Orden !== undefined && b.Orden !== null) ? b.Orden : 999;
            return oa - ob;
        });

        var iconos = {
            'cumple':    '&#x1F382;',   // cake
            'documento': '&#x1F4C4;',   // page
            'foto':      '&#x1F4F7;'    // camera
        };

        var html = '';
        for (var i = 0; i < data.length; i++) {
            var t = data[i];
            var tipo = (t.Tipo || '').toLowerCase();
            var urgencia = (t.Urgencia || 'baja').toLowerCase();
            var icono = iconos[tipo] || '&#x2139;&#xFE0F;';

            var badgeClass = 'badge-baja';
            if (urgencia === 'alta')  badgeClass = 'badge-alta';
            if (urgencia === 'media') badgeClass = 'badge-media';

            var urgLabel = urgencia.charAt(0).toUpperCase() + urgencia.slice(1);
            var alumno  = escapeHtml(cleanText(t.Alumno));
            var detalle = escapeHtml(cleanText(t.Detalle));

            html += '<div class="tramite-item ' + urgencia + '">'
                +   '<span class="tramite-icon">' + icono + '</span>'
                +   '<div class="tramite-body">'
                +     '<div class="tramite-alumno">' + (alumno || '—') + '</div>'
                +     (detalle ? '<div class="tramite-detalle">' + detalle + '</div>' : '')
                +     '<span class="tramite-badge ' + badgeClass + '">' + urgLabel + '</span>'
                +   '</div>'
                + '</div>';
        }
        cont.innerHTML = html;
    }

    // Chart Matricula por Mes (line, 2 series)

    function renderChartMatriculaMes(data) {
        _chartMatriculaMes = destroyChart(_chartMatriculaMes);
        var ctx = document.getElementById('chart-matricula-mes');
        if (!ctx) return;

        // Asegurar orden por Mes ascendente
        data.sort(function (a, b) { return (a.Mes || 0) - (b.Mes || 0); });

        var labels       = [];
        var nuevasActual = [];
        var nuevasAnter  = [];
        var totalActual  = 0;

        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var mesIdx = (row.Mes || 1) - 1;
            if (mesIdx < 0) mesIdx = 0;
            if (mesIdx > 11) mesIdx = 11;
            labels.push(MESES_CORTOS[mesIdx]);
            nuevasActual.push(row.NuevasActual   || 0);
            nuevasAnter.push(row.NuevasAnterior || 0);
            totalActual += (row.NuevasActual || 0);
        }

        var statEl = document.getElementById('stat-matricula-mes');
        if (statEl) statEl.textContent = totalActual > 0 ? totalActual.toLocaleString('es-HN') + ' nuevas YTD' : '';

        if (labels.length === 0) return;

        _chartMatriculaMes = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Año Actual',
                        data: nuevasActual,
                        borderColor: '#3b82f6',
                        backgroundColor: 'rgba(59,130,246,.12)',
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: '#3b82f6',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2,
                        pointRadius: 5,
                        pointHoverRadius: 7,
                        borderWidth: 2
                    },
                    {
                        label: 'Año Anterior',
                        data: nuevasAnter,
                        borderColor: '#94a3b8',
                        backgroundColor: 'transparent',
                        fill: false,
                        tension: 0.4,
                        pointBackgroundColor: '#94a3b8',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2,
                        pointRadius: 5,
                        pointHoverRadius: 7,
                        borderWidth: 2,
                        borderDash: [5, 4]
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                interaction: { mode: 'index', intersect: false },
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        align: 'end',
                        labels: {
                            usePointStyle: true,
                            pointStyle: 'rectRounded',
                            boxWidth: 10,
                            boxHeight: 10,
                            padding: 14,
                            font: { size: 12, weight: '600' },
                            color: '#475569'
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return ' ' + c.dataset.label + ': ' + Number(c.parsed.y).toLocaleString('es-HN') + ' alumnos';
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { font: { size: 11 }, color: '#64748b' }
                    },
                    y: {
                        beginAtZero: true,
                        grid: { color: '#f1f5f9' },
                        ticks: { font: { size: 11 }, color: '#64748b' }
                    }
                }
            }
        });
    }

    // Chart Matricula por Curso (bar horizontal, agrupado por Cur_Nombre, top 8)

    function renderChartMatriculaSeccion(data) {
        _chartMatriculaSeccion = destroyChart(_chartMatriculaSeccion);
        var ctx = document.getElementById('chart-matricula-seccion');
        if (!ctx) return;

        // Agrupar por Cur_Nombre limpio
        var map = {};
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var cur = cleanText(row.Cur_Nombre) || 'Sin nombre';
            if (!map[cur]) map[cur] = 0;
            map[cur] += (row.TotalAlumnos || 0);
        }

        // Convertir a array y top 8
        var arr = [];
        for (var k in map) {
            if (Object.prototype.hasOwnProperty.call(map, k)) {
                arr.push({ curso: k, total: map[k] });
            }
        }
        arr.sort(function (a, b) { return b.total - a.total; });
        arr = arr.slice(0, 8);

        var labels = arr.map(function (x) { return x.curso; });
        var valores = arr.map(function (x) { return x.total; });
        var totalGral = valores.reduce(function (a, b) { return a + b; }, 0);

        var statEl = document.getElementById('stat-matricula-seccion');
        if (statEl) statEl.textContent = totalGral > 0 ? totalGral.toLocaleString('es-HN') + ' alumnos' : '';

        if (labels.length === 0) return;

        _chartMatriculaSeccion = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Alumnos',
                    data: valores,
                    backgroundColor: '#6366f1',
                    borderRadius: 4,
                    borderSkipped: false
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return ' ' + c.parsed.x + ' alumnos';
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        grid: { color: '#f1f5f9' },
                        ticks: { font: { size: 11 }, color: '#64748b' }
                    },
                    y: {
                        grid: { display: false },
                        ticks: { font: { size: 11 }, color: '#334155' }
                    }
                }
            }
        });
    }

    // Tabla Documentos por Tipo (barra de progreso proporcional al máximo)

    function renderDocsPorTipo(data) {
        var tbody = document.getElementById('tabla-docs-tipo');
        if (!tbody) return;

        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="3" style="text-align:center;color:#94a3b8;padding:16px;">Sin datos disponibles.</td></tr>';
            return;
        }

        var max = 0;
        for (var j = 0; j < data.length; j++) {
            if ((data[j].Pendientes || 0) > max) max = data[j].Pendientes || 0;
        }
        if (max <= 0) max = 1;

        var html = '';
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var pend = row.Pendientes || 0;
            var pct = Math.round((pend / max) * 100);
            var tipo = escapeHtml(cleanText(row.TipoDocumento) || '—');
            html += '<tr>'
                + '<td style="font-weight:600;">' + tipo + '</td>'
                + '<td style="text-align:center;font-weight:800;color:#dc2626;">' + fmtNum(pend) + '</td>'
                + '<td>'
                +   '<div class="dd-progress">'
                +     '<div class="dd-progress-fill" style="width:' + pct + '%;background:#ef4444;"></div>'
                +   '</div>'
                + '</td>'
                + '</tr>';
        }
        tbody.innerHTML = html;
    }

    // Tabla Top alumnos con docs pendientes

    function renderAlumnosDocs(data) {
        var tbody = document.getElementById('tabla-alumnos-docs');
        if (!tbody) return;

        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" style="text-align:center;color:#94a3b8;padding:16px;">Sin datos disponibles.</td></tr>';
            return;
        }

        var rankClasses = ['rank-1', 'rank-2', 'rank-3'];
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var rClass = i < 3 ? rankClasses[i] : 'rank-n';
            var alumno = escapeHtml(cleanText(row.Alumno) || '—');
            var curso  = escapeHtml(cleanText(row.Curso)  || '—');
            html += '<tr>'
                + '<td><span class="rank-badge ' + rClass + '">' + (i + 1) + '</span></td>'
                + '<td style="font-weight:600;">' + alumno + '</td>'
                + '<td style="color:#64748b;">' + curso + '</td>'
                + '<td style="text-align:center;font-weight:800;color:#dc2626;">' + fmtNum(row.DocsPendientes) + '</td>'
                + '</tr>';
        }
        tbody.innerHTML = html;
    }

    // Tabla Encargados Incompletos

    function renderEncargados(data) {
        var tbody = document.getElementById('tabla-encargados');
        if (!tbody) return;

        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="3" style="text-align:center;color:#94a3b8;padding:16px;">Sin datos disponibles.</td></tr>';
            return;
        }

        var html = '';
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var nombre   = escapeHtml(cleanText(row.Nombre) || '—');
            var faltante = escapeHtml(cleanText(row.Faltante) || '—');
            html += '<tr>'
                + '<td style="font-weight:600;">' + nombre + '</td>'
                + '<td><span class="faltante-badge">' + faltante + '</span></td>'
                + '<td style="text-align:center;font-weight:700;color:#0f172a;">' + fmtNum(row.AlumnosACargo) + '</td>'
                + '</tr>';
        }
        tbody.innerHTML = html;
    }

    // Lista Cumpleaños de la Semana

    function renderCumpleanos(data) {
        var cont = document.getElementById('lista-cumple');
        if (!cont) return;

        if (!data || data.length === 0) {
            cont.innerHTML = '<p class="dd-empty">Sin cumpleaños esta semana.</p>';
            return;
        }

        var html = '';
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var nombre = cleanText(row.Alumno) || '—';
            var curso  = escapeHtml(cleanText(row.Curso) || '—');
            var fecha  = fmtFechaCorta(row.Fecha);
            var edad   = (row.Edad !== null && row.Edad !== undefined) ? row.Edad + ' años' : '';

            html += '<div class="cumple-item">'
                +   '<div class="cumple-avatar">' + escapeHtml(initial(nombre)) + '</div>'
                +   '<div class="cumple-body">'
                +     '<div class="cumple-nombre">' + escapeHtml(nombre) + '</div>'
                +     '<div class="cumple-meta">' + curso + '</div>'
                +   '</div>'
                +   '<div class="cumple-right">'
                +     '<div class="cumple-fecha">' + fecha + '</div>'
                +     '<div class="cumple-edad">' + edad + '</div>'
                +   '</div>'
                + '</div>';
        }
        cont.innerHTML = html;
    }

    // API pública

    var obj = {};

    obj.init = function (url) {
        _baseUrl = url || '';

        poblarSelectorAnio();
        cargar();

        var btn = document.getElementById('btn-actualizar');
        if (btn) {
            btn.addEventListener('click', function () {
                cargar();
            });
        }

        var selAnio = document.getElementById('sel-anio');
        if (selAnio) {
            selAnio.addEventListener('change', function () {
                cargar();
            });
        }
    };

    return obj;
}());
