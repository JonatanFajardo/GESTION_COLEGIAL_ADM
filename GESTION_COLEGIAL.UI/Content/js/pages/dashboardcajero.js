var DashboardCajero = (function () {
    'use strict';

    var _baseUrl = '';
    var _usuId   = 1;
    var _charts  = {};

    var MESES_CORTOS = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = (value !== null && value !== undefined && value !== '') ? value : '—';
    }

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        var n = Number(val);
        if (isNaN(n)) return '—';
        return n.toLocaleString('es-GT');
    }

    function fmtMoney(val) {
        if (val === null || val === undefined || val === '') return '—';
        var n = Number(val);
        if (isNaN(n)) return '—';
        return window.GC ? window.GC.fmtMoney(n) : ('Q ' + n.toLocaleString('es-GT', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    }

    function fmtFechaCorta(val) {
        if (!val) return '—';
        var d = new Date(val);
        if (isNaN(d)) return '—';
        return d.getDate() + ' ' + MESES_CORTOS[d.getMonth()];
    }

    function fmtHoraMin(val) {
        if (!val) return '—';
        var d = new Date(val);
        if (isNaN(d)) return '—';
        var hh = String(d.getHours()).padStart(2, '0');
        var mm = String(d.getMinutes()).padStart(2, '0');
        return hh + ':' + mm;
    }

    function hoyISO() {
        var d = new Date();
        var m = String(d.getMonth() + 1).padStart(2, '0');
        var dd = String(d.getDate()).padStart(2, '0');
        return d.getFullYear() + '-' + m + '-' + dd;
    }

    function destroyChart(key) {
        if (_charts[key] && typeof _charts[key].destroy === 'function') _charts[key].destroy();
        _charts[key] = null;
    }

    function fpaClass(desc) {
        if (!desc) return 'fpa-Otro';
        var d = String(desc).trim();
        if (d === 'Efectivo' || d === 'Tarjeta' || d === 'Transferencia' || d === 'Cheque') return 'fpa-' + d;
        return 'fpa-Otro';
    }

    function cargar() {
        var fecha = (document.getElementById('caj-fecha') || {}).value || hoyISO();
        $.getJSON(_baseUrl + '?Usu_Id=' + _usuId + '&Fecha=' + fecha, function (res) {
            var d = res || {};
            renderKpis(d.Kpis || {});
            renderResumen(d.Kpis || {}, d.PagosPorHora || []);
            renderChartPagosHora(d.PagosPorHora || []);
            renderChartFormaPago(d.CobrosPorFormaPago || []);
            renderChartConcepto(d.CobrosPorConcepto || []);
            renderChartSemana(d.ResumenSemanal || []);
            renderTimeline(d.TimelinePagos || []);
            renderTopAlumnos(d.TopAlumnosDelDia || []);
            renderVencidas(d.CuentasVencidas || []);
            renderArqueo(d.UltimoArqueo);
            renderAlertas(d.Alertas || []);
        }).fail(function (xhr, status, err) {
            console.warn('DashboardCajero: error al cargar.', status, err);
        });
    }

    function renderKpis(k) {
        setText('kpi-cobrado',    fmtMoney(k.TotalCobradoHoy));
        setText('kpi-cobros',     fmtNum(k.CantidadCobros));
        setText('kpi-ticket',     fmtMoney(k.TicketPromedio));
        setText('kpi-mayor',      fmtMoney(k.MayorPago));
        setText('kpi-alumnos',    fmtNum(k.AlumnosAtendidos));
        setText('kpi-pend-cnt',   fmtNum(k.CuentasPendientesHoy));
        setText('kpi-pend-monto', fmtMoney(k.MontoPendienteHoy));
        setText('kpi-mora',       fmtMoney(k.MontoMoraHoy));
    }

    function renderResumen(k, horas) {
        var cobrado = Number(k.TotalCobradoHoy || 0);
        var pend    = Number(k.MontoPendienteHoy || 0);
        var denom = cobrado + pend;
        var efi = denom > 0 ? ((cobrado / denom) * 100).toFixed(1) + '%' : '—';
        setText('stat-eficiencia', efi);

        var horaPico = '—', maxC = -1, horasActivas = 0, totalCobros = 0;
        for (var i = 0; i < horas.length; i++) {
            var c = horas[i].Cantidad || 0;
            if (c > 0) { horasActivas++; totalCobros += c; }
            if (c > maxC) { maxC = c; horaPico = horas[i].Hora; }
        }
        setText('stat-hora-pico', (maxC > 0 && horaPico !== '—') ? (String(horaPico).padStart(2,'0') + ':00') : '—');
        var prom = horasActivas > 0 ? (totalCobros / horasActivas).toFixed(1) : '—';
        setText('stat-pagos-prom', prom);
    }

    function renderChartPagosHora(data) {
        destroyChart('pagos-hora');
        var ctx = document.getElementById('chart-pagos-hora');
        if (!ctx) return;
        var labels = [], cantidades = [], montos = [], totalMonto = 0;
        for (var i = 0; i < data.length; i++) {
            labels.push(String(data[i].Hora).padStart(2,'0') + 'h');
            cantidades.push(data[i].Cantidad || 0);
            montos.push(Number(data[i].MontoTotal || 0));
            totalMonto += Number(data[i].MontoTotal || 0);
        }
        var el = document.getElementById('stat-monto-hora');
        if (el) el.textContent = totalMonto > 0 ? fmtMoney(totalMonto) : '';

        _charts['pagos-hora'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        type: 'line',
                        label: 'Monto',
                        data: montos,
                        borderColor: '#15803d',
                        backgroundColor: 'rgba(21,128,61,.12)',
                        fill: true, tension: 0.4,
                        pointBackgroundColor: '#15803d', pointBorderColor: '#fff',
                        pointBorderWidth: 2, pointRadius: 3, pointHoverRadius: 5,
                        borderWidth: 2, yAxisID: 'y1'
                    },
                    {
                        label: '# Cobros',
                        data: cantidades,
                        backgroundColor: '#f59e0b',
                        borderRadius: 4, borderSkipped: false,
                        yAxisID: 'y'
                    }
                ]
            },
            options: {
                responsive: true, maintainAspectRatio: true,
                interaction: { mode: 'index', intersect: false },
                plugins: {
                    legend: { display: true, position: 'top', align: 'end', labels: { usePointStyle: true, pointStyle: 'circle', boxWidth: 8, padding: 12, font: { size: 11, weight: '600' }, color: '#475569' } },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                if (c.dataset.label === 'Monto') return ' ' + c.dataset.label + ': ' + fmtMoney(c.parsed.y);
                                return ' ' + c.dataset.label + ': ' + c.parsed.y;
                            }
                        }
                    }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 10 }, color: '#64748b' } },
                    y: { beginAtZero: true, position: 'left', grid: { color: '#f1f5f9' }, ticks: { font: { size: 10 }, color: '#64748b', precision: 0 }, title: { display: true, text: '# Cobros', font: { size: 10 }, color: '#94a3b8' } },
                    y1:{ beginAtZero: true, position: 'right', grid: { display: false }, ticks: { font: { size: 10 }, color: '#64748b', callback: function (v) { return (window.GC ? window.GC.currencySymbol() : 'Q') + v; } }, title: { display: true, text: 'Monto', font: { size: 10 }, color: '#94a3b8' } }
                }
            }
        });
    }

    function renderChartFormaPago(data) {
        destroyChart('forma-pago');
        var ctx = document.getElementById('chart-forma-pago');
        if (!ctx) return;
        var labels = [], valores = [], total = 0;
        var palette = ['#22c55e','#3b82f6','#a855f7','#f59e0b','#ef4444','#14b8a6'];
        for (var i = 0; i < data.length; i++) {
            labels.push(data[i].Fpa_Descripcion || '—');
            valores.push(Number(data[i].Total || 0));
            total += Number(data[i].Total || 0);
        }
        var el = document.getElementById('stat-formas');
        if (el) el.textContent = total > 0 ? fmtMoney(total) : '';

        if (labels.length === 0) {
            ctx.style.display = 'none';
            if (!ctx.parentNode.querySelector('.empty-fpa')) {
                var p = document.createElement('p');
                p.className = 'empty-state empty-fpa';
                p.textContent = 'Sin cobros para mostrar.';
                ctx.parentNode.appendChild(p);
            }
            return;
        }
        ctx.style.display = '';
        var prev = ctx.parentNode.querySelector('.empty-fpa');
        if (prev) prev.remove();

        _charts['forma-pago'] = new Chart(ctx, {
            type: 'doughnut',
            data: { labels: labels, datasets: [{ data: valores, backgroundColor: palette.slice(0, labels.length), borderWidth: 2, borderColor: '#fff' }] },
            options: {
                responsive: true, maintainAspectRatio: true, cutout: '62%',
                plugins: {
                    legend: { position: 'bottom', labels: { font: { size: 11 }, boxWidth: 12, padding: 10 } },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                var t = c.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                                return ' ' + c.label + ': ' + fmtMoney(c.parsed) + ' (' + ((c.parsed / t) * 100).toFixed(1) + '%)';
                            }
                        }
                    }
                }
            }
        });
    }

    function renderChartConcepto(data) {
        destroyChart('concepto');
        var ctx = document.getElementById('chart-concepto');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].Cpa_Descripcion || '—'); valores.push(Number(data[i].Total || 0)); }

        if (labels.length === 0) {
            ctx.style.display = 'none';
            if (!ctx.parentNode.querySelector('.empty-cpa')) {
                var p = document.createElement('p');
                p.className = 'empty-state empty-cpa';
                p.textContent = 'Sin cobros por concepto.';
                ctx.parentNode.appendChild(p);
            }
            return;
        }
        ctx.style.display = '';
        var prev = ctx.parentNode.querySelector('.empty-cpa');
        if (prev) prev.remove();

        _charts['concepto'] = new Chart(ctx, {
            type: 'bar',
            data: { labels: labels, datasets: [{ label: 'Monto', data: valores, backgroundColor: '#14b8a6', borderRadius: 5, borderSkipped: false }] },
            options: {
                indexAxis: 'y', responsive: true, maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: function (c) { return ' ' + fmtMoney(c.parsed.x); } } }
                },
                scales: {
                    x: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { size: 10 }, color: '#64748b', callback: function (v) { return (window.GC ? window.GC.currencySymbol() : 'Q') + v; } } },
                    y: { grid: { display: false }, ticks: { font: { size: 11 }, color: '#334155' } }
                }
            }
        });
    }

    function renderChartSemana(data) {
        destroyChart('semana');
        var ctx = document.getElementById('chart-semana');
        if (!ctx) return;
        var labels = [], montos = [], cant = [], total = 0;
        for (var i = 0; i < data.length; i++) {
            labels.push(fmtFechaCorta(data[i].Fecha));
            montos.push(Number(data[i].MontoTotal || 0));
            cant.push(data[i].CantidadCobros || 0);
            total += Number(data[i].MontoTotal || 0);
        }
        var el = document.getElementById('stat-semana');
        if (el) el.textContent = total > 0 ? fmtMoney(total) + ' total' : '';

        _charts['semana'] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    { label: 'Monto', data: montos, borderColor: '#3b82f6', backgroundColor: 'rgba(59,130,246,.12)', fill: true, tension: 0.4, pointBackgroundColor: '#3b82f6', pointBorderColor: '#fff', pointBorderWidth: 2, pointRadius: 5, pointHoverRadius: 7, borderWidth: 3, yAxisID: 'y1' },
                    { label: '# Cobros', data: cant, borderColor: '#f59e0b', backgroundColor: 'transparent', fill: false, tension: 0.4, pointBackgroundColor: '#f59e0b', pointBorderColor: '#fff', pointBorderWidth: 2, pointRadius: 4, pointHoverRadius: 6, borderWidth: 2, yAxisID: 'y' }
                ]
            },
            options: {
                responsive: true, maintainAspectRatio: true, interaction: { mode: 'index', intersect: false },
                plugins: {
                    legend: { display: true, position: 'top', align: 'end', labels: { usePointStyle: true, pointStyle: 'circle', boxWidth: 8, padding: 12, font: { size: 11, weight: '600' }, color: '#475569' } },
                    tooltip: { callbacks: { label: function (c) { if (c.dataset.label === 'Monto') return ' ' + c.dataset.label + ': ' + fmtMoney(c.parsed.y); return ' ' + c.dataset.label + ': ' + c.parsed.y; } } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 10 }, color: '#64748b' } },
                    y: { beginAtZero: true, position: 'left', grid: { color: '#f1f5f9' }, ticks: { font: { size: 10 }, color: '#64748b', precision: 0 } },
                    y1:{ beginAtZero: true, position: 'right', grid: { display: false }, ticks: { font: { size: 10 }, color: '#64748b', callback: function (v) { return (window.GC ? window.GC.currencySymbol() : 'Q') + v; } } }
                }
            }
        });
    }

    function renderTimeline(data) {
        var tbody = document.getElementById('tabla-timeline');
        var stat  = document.getElementById('stat-timeline');
        if (!tbody) return;
        if (stat) stat.textContent = data.length > 0 ? data.length + ' pagos' : '';
        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="empty-state">Sin pagos registrados para esta fecha.</td></tr>';
            return;
        }
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var r = data[i], fp = r.Fpa_Descripcion || '—';
            html += '<tr>'
                  + '<td style="font-weight:700;color:#15803d;white-space:nowrap;">' + fmtHoraMin(r.Pag_FechaPago) + '</td>'
                  + '<td style="font-weight:600;">' + (r.NombreAlumno || '—') + '</td>'
                  + '<td style="text-align:center;"><span class="fpa-badge ' + fpaClass(fp) + '">' + fp + '</span></td>'
                  + '<td style="font-size:.75rem;color:#64748b;">' + (r.Pag_NumeroReferencia || '—') + '</td>'
                  + '<td style="text-align:right;font-weight:800;color:#0f172a;white-space:nowrap;">' + fmtMoney(r.Pag_MontoTotal) + '</td>'
                  + '</tr>';
        }
        tbody.innerHTML = html;
    }

    function renderTopAlumnos(data) {
        var cont = document.getElementById('lista-top-alumnos');
        if (!cont) return;
        if (!data || data.length === 0) { cont.innerHTML = '<p class="empty-state">Sin alumnos atendidos hoy.</p>'; return; }
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var a = data[i], ini = (a.NombreAlumno || '?').charAt(0).toUpperCase();
            var av = a.Per_Imagen && String(a.Per_Imagen).trim()
                ? '<img class="topal-avatar" src="' + a.Per_Imagen + '" alt="" onerror="this.style.display=\'none\';this.nextElementSibling.style.display=\'flex\';"><div class="topal-avatar-ph" style="display:none;">' + ini + '</div>'
                : '<div class="topal-avatar-ph">' + ini + '</div>';
            html += '<div class="topal-item">' + av
                  + '<div class="topal-info">'
                  +   '<div class="topal-nombre">' + (a.NombreAlumno || '—') + '</div>'
                  +   '<div class="topal-meta">' + (a.CantidadPagos || 0) + ' ' + ((a.CantidadPagos || 0) === 1 ? 'pago' : 'pagos') + '</div>'
                  + '</div>'
                  + '<div class="topal-badge">' + fmtMoney(a.MontoTotal) + '</div>'
                  + '</div>';
        }
        cont.innerHTML = html;
    }

    function renderVencidas(data) {
        var cont = document.getElementById('lista-vencidas');
        var stat = document.getElementById('stat-vencidas');
        if (!cont) return;
        if (stat) stat.textContent = data.length > 0 ? data.length + ' cuentas' : '';
        if (!data || data.length === 0) { cont.innerHTML = '<p class="empty-state">Sin cuentas vencidas.</p>'; return; }
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var v = data[i], dias = v.DiasVencido || 0;
            var diasClass = dias > 30 ? '' : 'med';
            html += '<div class="venc-item">'
                  + '<div class="venc-dias ' + diasClass + '">' + dias + '<span class="venc-dias-lbl">días</span></div>'
                  + '<div class="venc-info">'
                  +   '<div class="venc-nombre">' + (v.NombreAlumno || '—') + '</div>'
                  +   '<div class="venc-concepto">' + (v.Cpa_Descripcion || '') + ' · Venc. ' + fmtFechaCorta(v.Cco_FechaVencimiento) + '</div>'
                  + '</div>'
                  + '<div class="venc-monto">' + fmtMoney(v.Cco_MontoPendiente) + '</div>'
                  + '</div>';
        }
        cont.innerHTML = html;
    }

    function renderArqueo(a) {
        var cont = document.getElementById('arqueo-info');
        if (!cont) return;
        if (!a || !a.Arq_Id) {
            cont.innerHTML = '<div class="arqueo-empty"><i class="fa-solid fa-box-open" style="font-size:1.8rem;display:block;margin-bottom:8px;opacity:.6;"></i>Sin arqueo previo registrado.</div>';
            return;
        }
        cont.innerHTML = ''
            + '<div class="arqueo-card">'
            +   '<div class="arqueo-row"><span class="arqueo-lbl">Fecha</span><span class="arqueo-val">' + fmtFechaCorta(a.Arq_Fecha) + '</span></div>'
            +   '<div class="arqueo-row"><span class="arqueo-lbl">Total</span><span class="arqueo-total">' + fmtMoney(a.Arq_TotalGeneral) + '</span></div>'
            +   '<div class="arqueo-row" style="border-bottom:none;display:block;">'
            +     '<div class="arqueo-lbl" style="margin-bottom:4px;">Observaciones</div>'
            +     '<div style="font-size:.8rem;color:#334155;line-height:1.4;">' + (a.Arq_Observaciones || '—') + '</div>'
            +   '</div>'
            + '</div>';
    }

    function renderAlertas(data) {
        var cont = document.getElementById('lista-alertas');
        if (!cont) return;
        if (!data || data.length === 0) { cont.innerHTML = '<p class="empty-state" style="grid-column:1/-1;">Sin alertas activas.</p>'; return; }
        var iconos = {
            'vencidas':   '&#x23F1;&#xFE0F;',
            'mora':       '&#x1F525;',
            'arqueo':     '&#x1F4E6;',
            'sin_cobros': '&#x1F4ED;',
            'cobros_ok':  '&#x2705;'
        };
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var a = data[i], urg = (a.Urgencia || 'baja').toLowerCase(), tipo = (a.Tipo || '').toLowerCase();
            var icono = iconos[tipo] || '&#x2139;&#xFE0F;';
            var bClass = urg === 'alta' ? 'badge-alta' : (urg === 'media' ? 'badge-media' : 'badge-baja');
            html += '<div class="alerta-item ' + urg + '">'
                  + '<span class="alerta-icon">' + icono + '</span>'
                  + '<div>'
                  +   '<div class="alerta-msg">' + (a.Mensaje || '') + '</div>'
                  +   '<span class="alerta-badge ' + bClass + '">' + urg.charAt(0).toUpperCase() + urg.slice(1) + '</span>'
                  + '</div>'
                  + '</div>';
        }
        cont.innerHTML = html;
    }

    var obj = {};
    obj.init = function (url, usuId) {
        _baseUrl = url || '';
        _usuId = usuId || 1;
        var fechaInput = document.getElementById('caj-fecha');
        if (fechaInput && !fechaInput.value) fechaInput.value = hoyISO();
        cargar();
        var btn = document.getElementById('btn-actualizar');
        if (btn) btn.addEventListener('click', function () { cargar(); });
        if (fechaInput) fechaInput.addEventListener('change', function () { cargar(); });
        document.addEventListener('gc:moneda', function () { cargar(); });
    };
    return obj;
}());
