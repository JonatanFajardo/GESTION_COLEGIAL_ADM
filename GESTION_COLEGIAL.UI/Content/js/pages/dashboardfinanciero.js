var DashboardFinanciero = (function () {
    'use strict';

    var _baseUrl = '';
    var _charts = {};

    var MESES_CORTOS = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];
    var MESES_LARGOS = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = (value !== null && value !== undefined && value !== '') ? value : '—';
    }

    function fmtMoney(val) {
        if (val === null || val === undefined || val === '') return 'Q 0.00';
        var n = Number(val);
        if (isNaN(n)) return 'Q 0.00';
        return 'Q ' + n.toLocaleString('es-GT', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function fmtMoneyShort(val) {
        var n = Number(val) || 0;
        if (Math.abs(n) >= 1000000) return 'Q ' + (n / 1000000).toFixed(1) + 'M';
        if (Math.abs(n) >= 1000)    return 'Q ' + (n / 1000).toFixed(1) + 'K';
        return 'Q ' + n.toFixed(0);
    }

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-GT');
    }

    function fmtPct(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toFixed(1) + '%';
    }

    function fmtFecha(val) {
        if (!val) return '—';
        var d = new Date(val);
        if (isNaN(d)) return '—';
        return d.getDate() + ' ' + MESES_CORTOS[d.getMonth()];
    }

    function destroyChart(key) {
        if (_charts[key] && typeof _charts[key].destroy === 'function') _charts[key].destroy();
        _charts[key] = null;
    }

    function poblarSelectores() {
        var anioActual = new Date().getFullYear();
        var mesActual  = new Date().getMonth() + 1;
        var selAnio = document.getElementById('sel-anio-fin');
        var selMes  = document.getElementById('sel-mes-fin');
        if (!selAnio || !selMes) return;
        selAnio.innerHTML = ''; selMes.innerHTML = '';
        for (var a = anioActual - 2; a <= anioActual + 1; a++) {
            var optA = document.createElement('option');
            optA.value = a; optA.textContent = a;
            if (a === anioActual) optA.selected = true;
            selAnio.appendChild(optA);
        }
        for (var m = 1; m <= 12; m++) {
            var optM = document.createElement('option');
            optM.value = m; optM.textContent = MESES_LARGOS[m - 1];
            if (m === mesActual) optM.selected = true;
            selMes.appendChild(optM);
        }
    }

    function cargar() {
        var anio = (document.getElementById('sel-anio-fin') || {}).value || new Date().getFullYear();
        var mes  = (document.getElementById('sel-mes-fin')  || {}).value || (new Date().getMonth() + 1);
        $.getJSON(_baseUrl + '?Anio=' + anio + '&Mes=' + mes, function (res) {
            var d = res || {};
            renderKpis(d.KpisFinancieros || {});
            renderChartIngresosMensuales(d.IngresosMensuales || []);
            renderChartEstadoCartera(d.EstadoCartera || []);
            renderChartFormaPago(d.CobrosPorFormaPago || []);
            renderChartCobrosConcepto(d.CobrosPorConcepto || []);
            renderChartMorosidadConcepto(d.MorosidadPorConcepto || []);
            renderTopMorosos(d.TopAlumnosMorosos || []);
            renderChartCobrosDia(d.CobrosPorDia || []);
            renderTablaPorVencer(d.CuentasPorVencer || []);
            renderAlertas(d.Alertas || []);
        }).fail(function (xhr, status, err) {
            console.warn('DashboardFinanciero: error al cargar.', status, err);
        });
    }

    function renderKpis(k) {
        setText('kpi-facturado',       fmtMoney(k.TotalFacturadoMes));
        setText('kpi-cobrado',         fmtMoney(k.TotalCobradoMes));
        setText('kpi-pendiente',       fmtMoney(k.TotalPendiente));
        setText('kpi-mora',            fmtMoney(k.TotalMora));
        setText('kpi-pct-cobranza',    fmtPct(k.PorcentajeCobranza));
        setText('kpi-cantidad-pagos',  fmtNum(k.CantidadPagosMes));
        setText('kpi-ticket',          fmtMoney(k.TicketPromedioMes));
        setText('kpi-morosos',         fmtNum(k.AlumnosMorosos));
    }

    function renderChartIngresosMensuales(data) {
        destroyChart('evol');
        var ctx = document.getElementById('chart-ingresos-mensuales');
        if (!ctx) return;
        var labels = [], facturado = [], cobrado = [], pendiente = [];
        var totalFact = 0, totalCob = 0;
        for (var i = 0; i < data.length; i++) {
            labels.push(data[i].EtiquetaMes || '—');
            facturado.push(Number(data[i].Facturado) || 0);
            cobrado.push(Number(data[i].Cobrado) || 0);
            pendiente.push(Number(data[i].Pendiente) || 0);
            totalFact += Number(data[i].Facturado) || 0;
            totalCob  += Number(data[i].Cobrado) || 0;
        }
        var stat = document.getElementById('stat-evol');
        if (stat) stat.textContent = 'Facturado: ' + fmtMoneyShort(totalFact) + '  ·  Cobrado: ' + fmtMoneyShort(totalCob);

        _charts['evol'] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    { label: 'Facturado', data: facturado, borderColor: '#0ea5e9', backgroundColor: 'rgba(14,165,233,.12)', fill: true, tension: 0.4, pointBackgroundColor: '#0ea5e9', pointBorderColor: '#fff', pointBorderWidth: 2, pointRadius: 4, pointHoverRadius: 6, borderWidth: 2 },
                    { label: 'Cobrado',   data: cobrado,   borderColor: '#14b8a6', backgroundColor: 'rgba(20,184,166,.10)', fill: true, tension: 0.4, pointBackgroundColor: '#14b8a6', pointBorderColor: '#fff', pointBorderWidth: 2, pointRadius: 4, pointHoverRadius: 6, borderWidth: 2 },
                    { label: 'Pendiente', data: pendiente, borderColor: '#f59e0b', backgroundColor: 'transparent', fill: false, tension: 0.4, pointBackgroundColor: '#f59e0b', pointBorderColor: '#fff', pointBorderWidth: 2, pointRadius: 3, pointHoverRadius: 5, borderWidth: 2, borderDash: [5, 4] }
                ]
            },
            options: {
                responsive: true, maintainAspectRatio: true,
                interaction: { mode: 'index', intersect: false },
                plugins: {
                    legend: { display: true, position: 'top', align: 'end', labels: { usePointStyle: true, pointStyle: 'circle', boxWidth: 8, boxHeight: 8, padding: 12, font: { size: 11, weight: '600' }, color: '#475569' } },
                    tooltip: { callbacks: { label: function (c) { return ' ' + c.dataset.label + ': ' + fmtMoney(c.parsed.y); } } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 10 }, color: '#64748b', maxRotation: 0 } },
                    y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 }, color: '#64748b', callback: function (v) { return fmtMoneyShort(v); } } }
                }
            }
        });
    }

    function renderChartEstadoCartera(data) {
        destroyChart('estado');
        var ctx = document.getElementById('chart-estado-cartera');
        if (!ctx) return;
        var labels = [], valores = [];
        var colorMap = { 'Cobrado': '#22c55e', 'Pendiente': '#f59e0b', 'Mora': '#ef4444' };
        var colors = [];
        for (var i = 0; i < data.length; i++) {
            labels.push(data[i].Estado || '—');
            valores.push(Number(data[i].Total) || 0);
            colors.push(colorMap[data[i].Estado] || '#94a3b8');
        }
        _charts['estado'] = new Chart(ctx, {
            type: 'pie',
            data: { labels: labels, datasets: [{ data: valores, backgroundColor: colors, borderWidth: 2, borderColor: '#fff' }] },
            options: {
                responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { position: 'bottom', labels: { font: { size: 11 }, boxWidth: 12, padding: 10 } },
                    tooltip: { callbacks: { label: function (c) { var t = c.dataset.data.reduce(function (a, b) { return a + b; }, 0); return ' ' + c.label + ': ' + fmtMoney(c.parsed) + ' (' + (t > 0 ? ((c.parsed / t) * 100).toFixed(1) : '0') + '%)'; } } }
                }
            }
        });
    }

    function renderChartFormaPago(data) {
        destroyChart('fpa');
        var ctx = document.getElementById('chart-forma-pago');
        if (!ctx) return;
        var labels = [], valores = [], total = 0;
        var palette = ['#14b8a6','#0ea5e9','#6366f1','#a855f7','#f59e0b','#ef4444'];
        for (var i = 0; i < data.length; i++) {
            labels.push(data[i].Fpa_Descripcion || '—');
            valores.push(Number(data[i].Total) || 0);
            total += Number(data[i].Total) || 0;
        }
        var stat = document.getElementById('stat-fpa');
        if (stat) stat.textContent = fmtMoneyShort(total);
        _charts['fpa'] = new Chart(ctx, {
            type: 'doughnut',
            data: { labels: labels, datasets: [{ data: valores, backgroundColor: palette.slice(0, labels.length), borderWidth: 2, borderColor: '#fff' }] },
            options: {
                responsive: true, maintainAspectRatio: true, cutout: '60%',
                plugins: {
                    legend: { position: 'bottom', labels: { font: { size: 11 }, boxWidth: 12, padding: 10 } },
                    tooltip: { callbacks: { label: function (c) { var t = c.dataset.data.reduce(function (a, b) { return a + b; }, 0); return ' ' + c.label + ': ' + fmtMoney(c.parsed) + ' (' + (t > 0 ? ((c.parsed / t) * 100).toFixed(1) : '0') + '%)'; } } }
                }
            }
        });
    }

    function renderChartCobrosConcepto(data) {
        destroyChart('cobros-cpa');
        var ctx = document.getElementById('chart-cobros-concepto');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) {
            labels.push(data[i].Cpa_Descripcion || '—');
            valores.push(Number(data[i].Total) || 0);
        }
        _charts['cobros-cpa'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Cobrado', data: valores, backgroundColor: '#0ea5e9', borderRadius: 5, borderSkipped: false }] },
            options: {
                indexAxis: 'y', responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' ' + fmtMoney(c.parsed.x); } } }
                },
                scales: {
                    x: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 10 }, color: '#64748b', callback: function (v) { return fmtMoneyShort(v); } } },
                    y: { grid: { display: false }, ticks: { font: { size: 11 }, color: '#334155' } }
                }
            }
        });
    }

    function renderChartMorosidadConcepto(data) {
        destroyChart('mor-cpa');
        var ctx = document.getElementById('chart-morosidad-concepto');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) {
            labels.push(data[i].Cpa_Descripcion || '—');
            valores.push(Number(data[i].MontoMoroso) || 0);
        }
        _charts['mor-cpa'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Mora', data: valores, backgroundColor: '#ef4444', borderRadius: 5, borderSkipped: false }] },
            options: {
                indexAxis: 'y', responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' Mora: ' + fmtMoney(c.parsed.x); } } }
                },
                scales: {
                    x: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 10 }, color: '#64748b', callback: function (v) { return fmtMoneyShort(v); } } },
                    y: { grid: { display: false }, ticks: { font: { size: 11 }, color: '#334155' } }
                }
            }
        });
    }

    function renderTopMorosos(data) {
        var cont = document.getElementById('lista-morosos');
        var el   = document.getElementById('stat-morosos');
        if (!cont) return;
        if (el) el.textContent = data.length > 0 ? data.length + ' alumnos' : '';
        if (!data || data.length === 0) { cont.innerHTML = '<p style="color:#94a3b8;font-size:.82rem;margin:0;padding:14px 0;">Sin alumnos morosos.</p>'; return; }
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var m = data[i];
            var ini = (m.NombreAlumno || '?').charAt(0).toUpperCase();
            var av = (m.Per_Imagen && String(m.Per_Imagen).trim())
                ? '<img class="moroso-avatar" src="' + m.Per_Imagen + '" alt="" onerror="this.style.display=\'none\';this.nextElementSibling.style.display=\'flex\';"><div class="moroso-avatar-ph" style="display:none;">' + ini + '</div>'
                : '<div class="moroso-avatar-ph">' + ini + '</div>';
            html += '<div class="moroso-item">'
                  + av
                  + '<div class="moroso-info">'
                  +   '<div class="moroso-nombre">' + (m.NombreAlumno || '—') + '</div>'
                  +   '<div class="moroso-curso">' + (m.Curso || '—') + ' · ' + (m.CuentasVencidas || 0) + ' cuenta(s)</div>'
                  + '</div>'
                  + '<div>'
                  +   '<div class="moroso-monto">' + fmtMoney(m.MontoMoroso) + '</div>'
                  +   '<div class="moroso-dias">' + (m.DiasVencido || 0) + ' días vencido</div>'
                  + '</div>'
                  + '</div>';
        }
        cont.innerHTML = html;
    }

    function renderChartCobrosDia(data) {
        destroyChart('dia');
        var ctx = document.getElementById('chart-cobros-dia');
        if (!ctx) return;
        var labels = [], valores = [], total = 0;
        for (var i = 0; i < data.length; i++) {
            labels.push(String(data[i].Dia));
            valores.push(Number(data[i].Total) || 0);
            total += Number(data[i].Total) || 0;
        }
        var stat = document.getElementById('stat-cobros-dia');
        if (stat) stat.textContent = 'Total mes: ' + fmtMoney(total);
        _charts['dia'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Cobrado', data: valores, backgroundColor: 'rgba(34,197,94,.75)', borderRadius: 4, borderSkipped: false, borderColor: '#16a34a', borderWidth: 1 }] },
            options: {
                responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { title: function (c) { return 'Día ' + c[0].label; }, label: function (c) { return ' ' + fmtMoney(c.parsed.y); } } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 10 }, color: '#64748b', maxRotation: 0, autoSkip: true, maxTicksLimit: 16 } },
                    y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 }, color: '#64748b', callback: function (v) { return fmtMoneyShort(v); } } }
                }
            }
        });
    }

    function renderTablaPorVencer(data) {
        var tbody = document.getElementById('tabla-vencer');
        var el    = document.getElementById('stat-vencer');
        if (!tbody) return;
        if (el) el.textContent = data.length > 0 ? data.length + ' cuentas' : '';
        if (!data || data.length === 0) { tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;color:#94a3b8;padding:16px;">Sin cuentas por vencer en los próximos 7 días.</td></tr>'; return; }
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var c = data[i];
            var dias = Number(c.DiasParaVencer) || 0;
            var cls = dias <= 2 ? 'urgente' : (dias <= 5 ? 'proximo' : 'normal');
            html += '<tr>'
                  + '<td style="font-weight:600;">' + (c.NombreAlumno || '—') + '</td>'
                  + '<td style="font-size:.78rem;color:#64748b;">' + (c.Cpa_Descripcion || '—') + '</td>'
                  + '<td style="font-size:.78rem;">' + fmtFecha(c.Cco_FechaVencimiento) + '</td>'
                  + '<td style="text-align:right;font-weight:700;color:#0f172a;">' + fmtMoney(c.Cco_MontoPendiente) + '</td>'
                  + '<td style="text-align:center;"><span class="dias-badge ' + cls + '">' + dias + ' d</span></td>'
                  + '</tr>';
        }
        tbody.innerHTML = html;
    }

    function renderAlertas(data) {
        var cont = document.getElementById('lista-alertas-fin');
        if (!cont) return;
        if (!data || data.length === 0) { cont.innerHTML = '<p style="color:#94a3b8;font-size:.82rem;margin:0;">Sin alertas activas.</p>'; return; }
        var iconos = { 'mora':'&#x26A0;&#xFE0F;', 'cobranza':'&#x1F4B0;', 'morosos':'&#x1F465;', 'vencer':'&#x23F0;', 'pago':'&#x1F4B5;' };
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var a = data[i];
            var urg = (a.Urgencia || 'baja').toLowerCase();
            var tipo = (a.Tipo || '').toLowerCase();
            var icono = iconos[tipo] || '&#x2139;&#xFE0F;';
            var bClass = urg === 'alta' ? 'badge-alta-fin' : (urg === 'media' ? 'badge-media-fin' : 'badge-baja-fin');
            html += '<div class="alerta-item-fin ' + urg + '">'
                  +   '<span class="alerta-icon-fin">' + icono + '</span>'
                  +   '<div>'
                  +     '<div class="alerta-msg-fin">' + (a.Mensaje || '') + '</div>'
                  +     '<span class="alerta-badge-fin ' + bClass + '">' + urg.charAt(0).toUpperCase() + urg.slice(1) + '</span>'
                  +   '</div>'
                  + '</div>';
        }
        cont.innerHTML = html;
    }

    var obj = {};
    obj.init = function (url) {
        _baseUrl = url || '';
        poblarSelectores();
        cargar();
        var btn = document.getElementById('btn-actualizar-fin');
        if (btn) btn.addEventListener('click', function () { cargar(); });
    };
    return obj;
}());
