var DashboardDirector = (function () {
    'use strict';

    // ── Referencias a instancias Chart.js (para destruir antes de recrear) ──
    var _chartAsistencia = null;
    var _chartMatricula  = null;
    var _chartCobros     = null;

    // URL base guardada al init
    var _baseUrl = '';

    // Nombres de meses
    var MESES_CORTOS = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];
    var MESES_LARGOS = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];

    // ── Utilidades ──────────────────────────────────────────────────────────

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-GT');
    }

    function fmtMoney(val) {
        if (val === null || val === undefined || val === '') return '—';
        return window.GC ? window.GC.fmtMoney(val) : ('Q ' + Number(val).toLocaleString('es-GT', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    }

    function fmtDecimal(val, decimals) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toFixed(decimals !== undefined ? decimals : 1);
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

    // ── Poblar selectores de Año y Mes ──────────────────────────────────────

    function poblarSelectores() {
        var anioActual = new Date().getFullYear();
        var mesActual  = new Date().getMonth() + 1;

        var selAnio = document.getElementById('sel-anio');
        var selMes  = document.getElementById('sel-mes');
        if (!selAnio || !selMes) return;

        // Limpiar
        selAnio.innerHTML = '';
        selMes.innerHTML  = '';

        // Años: actual -2 hasta actual +2
        for (var a = anioActual - 2; a <= anioActual + 2; a++) {
            var optA = document.createElement('option');
            optA.value = a;
            optA.textContent = a;
            if (a === anioActual) optA.selected = true;
            selAnio.appendChild(optA);
        }

        // Meses: 1–12
        for (var m = 1; m <= 12; m++) {
            var optM = document.createElement('option');
            optM.value = m;
            optM.textContent = MESES_LARGOS[m - 1];
            if (m === mesActual) optM.selected = true;
            selMes.appendChild(optM);
        }
    }

    // ── Fetch y render ───────────────────────────────────────────────────────

    function cargar() {
        var selAnio = document.getElementById('sel-anio');
        var selMes  = document.getElementById('sel-mes');
        var anio = selAnio ? selAnio.value : new Date().getFullYear();
        var mes  = selMes  ? selMes.value  : (new Date().getMonth() + 1);

        var url = _baseUrl + '?Anio=' + anio + '&Mes=' + mes;

        $.getJSON(url, function (res) {
            // El UI controller hace passthrough crudo del API (PascalCase).
            var d = res || {};
            renderKpis(d.Kpis                          || {});
            renderAsistencia(d.AsistenciaHoy           || []);
            renderChartMatricula(d.MatriculaPorCurso   || []);
            renderChartCobros(d.CobrosPorMes           || []);
            renderTablaRendimiento(d.RendimientoPorCurso || []);
            renderTablaDeuda(d.TopDeuda                || []);
            renderAlertas(d.Alertas                    || []);
        }).fail(function (xhr, status, err) {
            console.warn('DashboardDirector: no se pudo cargar el resumen.', status, err);
        });
    }

    // ── KPI Cards ───────────────────────────────────────────────────────────

    function renderKpis(k) {
        setText('kpi-alumnos',   fmtNum(k.TotalAlumnos));
        setText('kpi-empleados', fmtNum(k.TotalEmpleados));
        setText('kpi-cobrado',   fmtMoney(k.TotalCobradoMes));
        setText('kpi-deuda',     fmtMoney(k.TotalDeudaPendiente));
        setText('kpi-promedio',  fmtDecimal(k.PromedioGeneral, 1));
        setText('kpi-riesgo',    fmtNum(k.AlumnosEnRiesgo));

        // Badge de cuentas vencidas en tarjeta de deuda
        var badgeEl = document.getElementById('kpi-cuentas-vencidas-badge');
        if (badgeEl) {
            var cv = k.CuentasVencidas;
            badgeEl.textContent = (cv !== null && cv !== undefined) ? cv + ' vencidas' : '';
        }
    }

    // ── Asistencia Hoy ──────────────────────────────────────────────────────

    function renderAsistencia(data) {
        var totPresentes  = 0;
        var totAusentes   = 0;
        var totTardanzas  = 0;
        var totEsperados  = 0;

        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            totPresentes += (row.Presentes  || 0);
            totAusentes  += (row.Ausentes   || 0);
            totTardanzas += (row.Tardanzas  || 0);
            totEsperados += (row.TotalEsperados || 0);
        }

        setText('kpi-presentes',  fmtNum(totPresentes));
        setText('kpi-ausentes',   fmtNum(totAusentes));
        setText('kpi-tardanzas',  fmtNum(totTardanzas));

        var pct = totEsperados > 0
            ? ((totPresentes / totEsperados) * 100).toFixed(1) + '%'
            : '—';
        setText('kpi-pct-asistencia', pct);

        // Doughnut chart
        _chartAsistencia = destroyChart(_chartAsistencia);
        var ctx = document.getElementById('chart-asistencia');
        if (!ctx) return;

        _chartAsistencia = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Presentes', 'Ausentes', 'Tardanzas'],
                datasets: [{
                    data: [totPresentes, totAusentes, totTardanzas],
                    backgroundColor: ['#22c55e', '#ef4444', '#f59e0b'],
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                cutout: '68%',
                plugins: {
                    legend: {
                        position: 'right',
                        labels: { font: { size: 11 }, boxWidth: 12, padding: 10 }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (ctx) {
                                var total = ctx.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                                var pctTip = total > 0 ? ' (' + ((ctx.parsed / total) * 100).toFixed(1) + '%)' : '';
                                return ' ' + ctx.label + ': ' + ctx.parsed + pctTip;
                            }
                        }
                    }
                }
            }
        });
    }

    // ── Chart Matrícula (barras horizontales) ────────────────────────────────

    function renderChartMatricula(data) {
        _chartMatricula = destroyChart(_chartMatricula);
        var ctx = document.getElementById('chart-matricula');
        if (!ctx) return;

        var labels = [];
        var valores = [];
        var total = 0;
        for (var i = 0; i < data.length; i++) {
            labels.push(data[i].Cur_Nombre  || 'Sin nombre');
            valores.push(data[i].TotalAlumnos || 0);
            total += (data[i].TotalAlumnos || 0);
        }
        setText('stat-total-matricula', total > 0 ? total + ' alumnos' : '');

        _chartMatricula = new Chart(ctx, {
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
                            label: function (ctx) {
                                return ' ' + ctx.parsed.x + ' alumnos';
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

    // ── Chart Cobros por mes (line) ──────────────────────────────────────────

    function renderChartCobros(data) {
        _chartCobros = destroyChart(_chartCobros);
        var ctx = document.getElementById('chart-cobros');
        if (!ctx) return;

        var labels      = [];
        var cobrados    = [];
        var pendientes  = [];
        var totalCobros = 0;
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var mesIdx = (row.Mes || 1) - 1;
            labels.push(MESES_CORTOS[mesIdx]);
            cobrados.push(row.TotalCobrado   || 0);
            pendientes.push(row.TotalPendiente || 0);
            totalCobros += (row.TotalCobrado || 0);
        }
        var statEl = document.getElementById('stat-total-cobros');
        if (statEl) statEl.textContent = totalCobros > 0 ? fmtMoney(totalCobros) + ' acumulado' : '';

        _chartCobros = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Cobrado',
                        data: cobrados,
                        borderColor: '#22c55e',
                        backgroundColor: 'rgba(34,197,94,.12)',
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: '#22c55e',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2,
                        pointRadius: 5,
                        pointHoverRadius: 7,
                        borderWidth: 2
                    },
                    {
                        label: 'Pendiente',
                        data: pendientes,
                        borderColor: '#ef4444',
                        backgroundColor: 'transparent',
                        fill: false,
                        tension: 0.4,
                        pointBackgroundColor: '#ef4444',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2,
                        pointRadius: 5,
                        pointHoverRadius: 7,
                        borderWidth: 2
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
                            label: function (ctx) {
                                return ' ' + ctx.dataset.label + ': ' + fmtMoney(ctx.parsed.y);
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
                        ticks: {
                            font: { size: 11 },
                            color: '#64748b',
                            callback: function (val) {
                                return (window.GC ? window.GC.currencySymbol() : 'Q') + ' ' + (val >= 1000 ? (val / 1000).toFixed(0) + 'k' : val);
                            }
                        }
                    }
                }
            }
        });
    }

    // ── Tabla Rendimiento ───────────────────────────────────────────────────

    function renderTablaRendimiento(data) {
        var tbody = document.getElementById('tabla-rendimiento');
        if (!tbody) return;

        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" style="text-align:center;color:#94a3b8;padding:16px;">Sin datos disponibles.</td></tr>';
            return;
        }

        var html = '';
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var prom = parseFloat(row.Promedio) || 0;
            var pct  = Math.min(Math.max(prom, 0), 100).toFixed(0);
            var enRiesgo = row.EnRiesgo || 0;
            var rowClass = prom >= 70 ? 'rend-ok' : (prom >= 60 ? 'rend-med' : 'rend-bad');
            var barColor = prom >= 70 ? '#22c55e' : (prom >= 60 ? '#f59e0b' : '#ef4444');
            var riesgoColor = enRiesgo > 0 ? '#dc2626' : '#16a34a';

            html += '<tr class="' + rowClass + '">'
                + '<td style="font-weight:600;">' + (row.Cur_Nombre || '—') + '</td>'
                + '<td style="text-align:center;font-weight:800;font-size:.95rem;">' + prom.toFixed(1) + '</td>'
                + '<td style="text-align:center;font-weight:700;color:' + riesgoColor + ';">'
                +   (enRiesgo > 0 ? '<i class="fa-solid fa-circle-exclamation" style="margin-right:3px;font-size:.75rem;"></i>' : '')
                +   enRiesgo
                + '</td>'
                + '<td>'
                +   '<div class="dd-progress">'
                +     '<div class="dd-progress-fill" style="width:' + pct + '%;background:' + barColor + ';"></div>'
                +   '</div>'
                + '</td>'
                + '</tr>';
        }
        tbody.innerHTML = html;
    }

    // ── Tabla Top Deuda ─────────────────────────────────────────────────────

    function renderTablaDeuda(data) {
        var tbody = document.getElementById('tabla-deuda');
        if (!tbody) return;

        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="3" style="text-align:center;color:#94a3b8;padding:16px;">Sin datos disponibles.</td></tr>';
            return;
        }

        var rankClasses = ['rank-1', 'rank-2', 'rank-3'];
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var rClass = i < 3 ? rankClasses[i] : 'rank-n';
            html += '<tr>'
                + '<td><span class="rank-badge ' + rClass + '">' + (i + 1) + '</span></td>'
                + '<td style="font-weight:600;">' + (row.Cur_Nombre || '—') + '</td>'
                + '<td style="text-align:center;font-weight:700;color:#dc2626;">' + (row.AlumnosConDeuda || 0) + '</td>'
                + '<td style="text-align:right;font-weight:700;color:#0f172a;">' + fmtMoney(row.MontoTotal) + '</td>'
                + '</tr>';
        }
        tbody.innerHTML = html;
    }

    // ── Alertas ─────────────────────────────────────────────────────────────

    function renderAlertas(data) {
        var cont = document.getElementById('lista-alertas');
        if (!cont) return;

        if (!data || data.length === 0) {
            cont.innerHTML = '<p style="color:#94a3b8;font-size:.82rem;margin:0;">Sin alertas activas.</p>';
            return;
        }

        var iconos = {
            'financiero': '&#x1F4B0;',
            'academico':  '&#x1F4DA;',
            'asistencia': '&#x1F4CB;'
        };

        var html = '';
        for (var i = 0; i < data.length; i++) {
            var a = data[i];
            var urgencia = (a.Urgencia || 'baja').toLowerCase();
            var tipo     = (a.Tipo    || '').toLowerCase();
            var icono    = iconos[tipo] || '&#x2139;&#xFE0F;';

            var badgeClass = 'badge-baja';
            if (urgencia === 'alta')  badgeClass = 'badge-alta';
            if (urgencia === 'media') badgeClass = 'badge-media';

            var urgLabel = urgencia.charAt(0).toUpperCase() + urgencia.slice(1);
            html += '<div class="alerta-item ' + urgencia + '">'
                +   '<span class="alerta-icon">' + icono + '</span>'
                +   '<div class="alerta-body">'
                +     '<div class="alerta-msg">' + (a.Mensaje || '') + '</div>'
                +     '<span class="alerta-badge ' + badgeClass + '">' + urgLabel + '</span>'
                +   '</div>'
                + '</div>';
        }
        cont.innerHTML = html;
    }

    // ── API pública ─────────────────────────────────────────────────────────

    var obj = {};

    obj.init = function (url) {
        _baseUrl = url || '';

        poblarSelectores();
        cargar();

        var btn = document.getElementById('btn-actualizar');
        if (btn) btn.addEventListener('click', function () { cargar(); });
        document.addEventListener('gc:moneda', function () { cargar(); });
    };

    return obj;
}());
