var DashboardRRHH = (function () {
    'use strict';

    var _baseUrl = '';
    var _charts  = {};

    var MESES_CORTOS = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];
    var MESES_LARGOS = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = (value !== null && value !== undefined) ? value : '—';
    }

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-GT');
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

    function esHoy(dia, mes) {
        var h = new Date();
        return h.getDate() === dia && (h.getMonth() + 1) === mes;
    }

    function poblarSelectores() {
        var anioActual = new Date().getFullYear();
        var mesActual  = new Date().getMonth() + 1;
        var selAnio = document.getElementById('sel-anio');
        var selMes  = document.getElementById('sel-mes');
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
        var anio = (document.getElementById('sel-anio') || {}).value || new Date().getFullYear();
        var mes  = (document.getElementById('sel-mes')  || {}).value || (new Date().getMonth() + 1);
        $.getJSON(_baseUrl + '?Anio=' + anio + '&Mes=' + mes, function (res) {
            var d = res || {};
            renderKpis(d.Kpis || {});
            renderAsistenciaHoy(d.Kpis || {});
            renderChartAsistenciaDiaria(d.AsistenciaDiaria || []);
            renderChartDepartamento(d.PorDepartamento || []);
            renderChartCargo(d.PorCargo || []);
            renderChartGenero(d.PorGenero || []);
            renderChartEdad(d.PorEdad || []);
            renderChartNuevos(d.NuevosPorMes || []);
            renderCumpleanos(d.Cumpleanos || []);
            renderTablaSolicitudes(d.SolicitudesPendientes || []);
            renderAntiguedad(d.TopAntiguedad || []);
            renderAlertas(d.Alertas || []);
        }).fail(function (xhr, status, err) {
            console.warn('DashboardRRHH: error al cargar.', status, err);
        });
    }

    function renderKpis(k) {
        setText('kpi-activos',       fmtNum(k.TotalEmpleadosActivos));
        setText('kpi-departamentos', fmtNum(k.TotalDepartamentos));
        setText('kpi-cargos',        fmtNum(k.TotalCargos));
        setText('kpi-nuevos',        fmtNum(k.NuevosEmpleadosAnio));
        setText('kpi-vac-pend',      fmtNum(k.VacacionesPendientes));
        setText('kpi-per-pend',      fmtNum(k.PermisosPendientes));
        setText('kpi-inca-pend',     fmtNum(k.IncapacidadesPendientes));
        setText('kpi-cumple-mes',    fmtNum(k.CumpleanosMes));
    }

    function renderAsistenciaHoy(k) {
        var pres = k.PresentesHoy || 0, ause = k.AusentesHoy || 0, tard = k.TardanzasHoy || 0;
        var total = pres + ause + tard;
        setText('kpi-presentes', fmtNum(pres));
        setText('kpi-ausentes',  fmtNum(ause));
        setText('kpi-tardanzas', fmtNum(tard));
        setText('kpi-pct-hoy',   total > 0 ? ((pres / total) * 100).toFixed(1) + '%' : '—');

        destroyChart('asist-hoy');
        var ctx = document.getElementById('chart-asistencia-hoy');
        if (!ctx || total === 0) return;
        _charts['asist-hoy'] = new Chart(ctx, {
            type: 'doughnut',
            data: { labels: ['Presentes','Ausentes','Tardanzas'], datasets: [{ data: [pres,ause,tard], backgroundColor: ['#22c55e','#ef4444','#f59e0b'], borderWidth: 2, borderColor: '#fff' }] },
            options: {
                responsive: true, maintainAspectRatio: true, cutout: '68%',
                plugins: {
                    legend: { position: 'right', labels: { font: { size: 11 }, boxWidth: 12, padding: 10 } },
                    tooltip: { callbacks: { label: function (c) { var t = c.dataset.data.reduce(function(a,b){return a+b;},0); return ' '+c.label+': '+c.parsed+' ('+((c.parsed/t)*100).toFixed(1)+'%)'; } } }
                }
            }
        });
    }

    function renderChartAsistenciaDiaria(data) {
        destroyChart('asist-diaria');
        var ctx = document.getElementById('chart-asistencia-diaria');
        if (!ctx) return;
        var labels = [], pres = [], ause = [], tard = [];
        for (var i = 0; i < data.length; i++) {
            labels.push(fmtFecha(data[i].Fecha));
            pres.push(data[i].Presentes || 0);
            ause.push(data[i].Ausentes  || 0);
            tard.push(data[i].Tardanzas || 0);
        }
        _charts['asist-diaria'] = new Chart(ctx, {
            type: 'line',
            data: { labels: labels, datasets: [
                { label:'Presentes', data:pres, borderColor:'#22c55e', backgroundColor:'rgba(34,197,94,.1)', fill:true, tension:0.4, pointBackgroundColor:'#22c55e', pointBorderColor:'#fff', pointBorderWidth:2, pointRadius:4, pointHoverRadius:6, borderWidth:2 },
                { label:'Ausentes',  data:ause, borderColor:'#ef4444', backgroundColor:'transparent', fill:false, tension:0.4, pointBackgroundColor:'#ef4444', pointBorderColor:'#fff', pointBorderWidth:2, pointRadius:4, pointHoverRadius:6, borderWidth:2 },
                { label:'Tardanzas', data:tard, borderColor:'#f59e0b', backgroundColor:'transparent', fill:false, tension:0.4, pointBackgroundColor:'#f59e0b', pointBorderColor:'#fff', pointBorderWidth:2, pointRadius:4, pointHoverRadius:6, borderWidth:2 }
            ]},
            options: {
                responsive:true, maintainAspectRatio:true, interaction:{mode:'index',intersect:false},
                plugins: { legend:{display:true,position:'top',align:'end',labels:{usePointStyle:true,pointStyle:'circle',boxWidth:8,boxHeight:8,padding:12,font:{size:11,weight:'600'},color:'#475569'}} },
                scales: { x:{grid:{display:false},ticks:{font:{size:10},color:'#64748b',maxRotation:45}}, y:{beginAtZero:true,grid:{color:'#f1f5f9'},ticks:{font:{size:11},color:'#64748b',precision:0}} }
            }
        });
    }

    function renderChartDepartamento(data) {
        destroyChart('departamento');
        var ctx = document.getElementById('chart-departamento');
        if (!ctx) return;
        var labels = [], valores = [], total = 0;
        var palette = ['#6366f1','#3b82f6','#14b8a6','#22c55e','#f59e0b','#f97316'];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].Departamento||'—'); valores.push(data[i].TotalEmpleados||0); total+=data[i].TotalEmpleados||0; }
        var el = document.getElementById('stat-total-depto');
        if (el) el.textContent = total + ' empleados';
        _charts['departamento'] = new Chart(ctx, {
            type: 'doughnut',
            data: { labels:labels, datasets:[{data:valores, backgroundColor:palette.slice(0,labels.length), borderWidth:2, borderColor:'#fff'}] },
            options: {
                responsive:true, maintainAspectRatio:true, cutout:'60%',
                plugins: { legend:{position:'bottom',labels:{font:{size:11},boxWidth:12,padding:10}}, tooltip:{callbacks:{label:function(c){var t=c.dataset.data.reduce(function(a,b){return a+b;},0);return ' '+c.label+': '+c.parsed+' ('+((c.parsed/t)*100).toFixed(1)+'%)';}}} }
            }
        });
    }

    function renderChartCargo(data) {
        destroyChart('cargo');
        var ctx = document.getElementById('chart-cargo');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].Cargo||'—'); valores.push(data[i].TotalEmpleados||0); }
        _charts['cargo'] = new Chart(ctx, {
            type:'bar',
            data:{labels:labels,datasets:[{label:'Empleados',data:valores,backgroundColor:'#14b8a6',borderRadius:5,borderSkipped:false}]},
            options:{indexAxis:'y',responsive:true,maintainAspectRatio:true,plugins:{legend:{display:false},tooltip:{callbacks:{label:function(c){return ' '+c.parsed.x+' empleados';}}}},scales:{x:{beginAtZero:true,grid:{color:'#f1f5f9'},ticks:{font:{size:11},color:'#64748b',precision:0}},y:{grid:{display:false},ticks:{font:{size:11},color:'#334155'}}}}
        });
    }

    function renderChartGenero(data) {
        destroyChart('genero');
        var ctx = document.getElementById('chart-genero');
        if (!ctx) return;
        var labels = [], valores = [];
        var colors = ['#6366f1','#ec4899','#94a3b8'];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].Genero||'—'); valores.push(data[i].Total||0); }
        _charts['genero'] = new Chart(ctx, {
            type:'pie',
            data:{labels:labels,datasets:[{data:valores,backgroundColor:colors.slice(0,labels.length),borderWidth:2,borderColor:'#fff'}]},
            options:{responsive:true,maintainAspectRatio:true,plugins:{legend:{position:'bottom',labels:{font:{size:11},boxWidth:12,padding:10}},tooltip:{callbacks:{label:function(c){var t=c.dataset.data.reduce(function(a,b){return a+b;},0);return ' '+c.label+': '+c.parsed+' ('+((c.parsed/t)*100).toFixed(1)+'%)';}}}}}
        });
    }

    function renderChartEdad(data) {
        destroyChart('edad');
        var ctx = document.getElementById('chart-edad');
        if (!ctx) return;
        var labels = [], valores = [];
        var colors = ['#22c55e','#3b82f6','#6366f1','#a855f7','#f59e0b'];
        for (var i = 0; i < data.length; i++) { labels.push(data[i].RangoEdad||'—'); valores.push(data[i].Total||0); }
        _charts['edad'] = new Chart(ctx, {
            type:'bar',
            data:{labels:labels,datasets:[{label:'Empleados',data:valores,backgroundColor:colors.slice(0,labels.length),borderRadius:5,borderSkipped:false}]},
            options:{responsive:true,maintainAspectRatio:true,plugins:{legend:{display:false},tooltip:{callbacks:{label:function(c){return ' '+c.parsed.y+' empleados';}}}},scales:{x:{grid:{display:false},ticks:{font:{size:10},color:'#64748b'}},y:{beginAtZero:true,grid:{color:'#f1f5f9'},ticks:{font:{size:11},color:'#64748b',precision:0}}}}
        });
    }

    function renderChartNuevos(data) {
        destroyChart('nuevos');
        var ctx = document.getElementById('chart-nuevos');
        if (!ctx) return;
        var labels = [], valores = [];
        for (var i = 0; i < data.length; i++) { labels.push(MESES_CORTOS[(data[i].Mes||1)-1]+' '+String(data[i].Anio||'').slice(-2)); valores.push(data[i].NuevosEmpleados||0); }
        _charts['nuevos'] = new Chart(ctx, {
            type:'bar',
            data:{labels:labels,datasets:[{label:'Nuevos',data:valores,backgroundColor:'#22c55e',borderRadius:5,borderSkipped:false}]},
            options:{responsive:true,maintainAspectRatio:true,plugins:{legend:{display:false},tooltip:{callbacks:{label:function(c){return ' '+c.parsed.y+' empleados';}}}},scales:{x:{grid:{display:false},ticks:{font:{size:10},color:'#64748b'}},y:{beginAtZero:true,grid:{color:'#f1f5f9'},ticks:{font:{size:11},color:'#64748b',precision:0}}}}
        });
    }

    function renderCumpleanos(data) {
        var cont = document.getElementById('lista-cumpleanos');
        if (!cont) return;
        if (!data || data.length === 0) { cont.innerHTML = '<p style="color:#94a3b8;font-size:.82rem;margin:0;">Sin cumpleaños este mes.</p>'; return; }
        var mesActual = new Date().getMonth() + 1;
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var c = data[i], hoyFlag = esHoy(c.Dia, mesActual), ini = (c.NombreCompleto||'?').charAt(0).toUpperCase();
            var av = c.Per_Imagen && c.Per_Imagen.trim()
                ? '<img class="cumple-avatar" src="'+c.Per_Imagen+'" alt="" onerror="this.style.display=\'none\';this.nextElementSibling.style.display=\'flex\';"><div class="cumple-avatar-ph" style="display:none;">'+ini+'</div>'
                : '<div class="cumple-avatar-ph">'+ini+'</div>';
            var diaLabel = hoyFlag ? '&#x1F382; ¡Hoy!' : 'Día ' + c.Dia;
            html += '<div class="cumple-card'+(hoyFlag?' es-hoy':'')+'">'+av+'<div class="cumple-nombre">'+(c.NombreCompleto||'—')+'</div><div class="cumple-cargo">'+(c.Cargo||'')+'</div><div class="cumple-dia">'+diaLabel+'</div></div>';
        }
        cont.innerHTML = html;
    }

    function renderTablaSolicitudes(data) {
        var tbody = document.getElementById('tabla-solicitudes');
        var el    = document.getElementById('stat-total-sol');
        if (!tbody) return;
        if (el) el.textContent = data.length > 0 ? data.length + ' pendientes' : '';
        if (!data || data.length === 0) { tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;color:#94a3b8;padding:16px;">Sin solicitudes pendientes.</td></tr>'; return; }
        var tipoMap = { 'Vacaciones':'V','Permiso':'P','Incapacidad':'I' };
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var row = data[i], tipo = row.Tipo||'—', cls = 'tipo-'+(tipoMap[tipo]||'V');
            html += '<tr>'
                  + '<td style="font-weight:600;">'+(row.NombreEmpleado||'—')+'</td>'
                  + '<td style="font-size:.75rem;color:#64748b;">'+(row.Cargo||'—')+'<br><span style="color:#94a3b8;">'+(row.Departamento||'')+'</span></td>'
                  + '<td style="text-align:center;"><span class="tipo-badge '+cls+'">'+tipo+'</span></td>'
                  + '<td style="font-size:.78rem;">'+fmtFecha(row.Vac_FechaInicio)+' – '+fmtFecha(row.Vac_FechaFin)+'</td>'
                  + '<td style="text-align:center;font-weight:800;color:#0f172a;">'+(row.Vac_DiasTotal||0)+'</td>'
                  + '</tr>';
        }
        tbody.innerHTML = html;
    }

    function renderAntiguedad(data) {
        var cont = document.getElementById('lista-antiguedad');
        if (!cont) return;
        if (!data || data.length === 0) { cont.innerHTML = '<p style="color:#94a3b8;font-size:.82rem;margin:0;">Sin datos.</p>'; return; }
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var e = data[i], ini = (e.NombreEmpleado||'?').charAt(0).toUpperCase();
            var av = e.Per_Imagen && e.Per_Imagen.trim()
                ? '<img class="antig-avatar" src="'+e.Per_Imagen+'" alt="" onerror="this.style.display=\'none\';this.nextElementSibling.style.display=\'flex\';"><div class="antig-avatar-ph" style="display:none;">'+ini+'</div>'
                : '<div class="antig-avatar-ph">'+ini+'</div>';
            var anos = e.AnosServicio || 0;
            html += '<div class="antig-item">'+av+'<div class="antig-info"><div class="antig-nombre">'+(e.NombreEmpleado||'—')+'</div><div class="antig-cargo">'+(e.Cargo||'')+' · '+(e.Departamento||'')+'</div></div><div class="antig-badge">'+anos+(anos===1?' año':' años')+'</div></div>';
        }
        cont.innerHTML = html;
    }

    function renderAlertas(data) {
        var cont = document.getElementById('lista-alertas');
        if (!cont) return;
        if (!data || data.length === 0) { cont.innerHTML = '<p style="color:#94a3b8;font-size:.82rem;margin:0;">Sin alertas activas.</p>'; return; }
        var iconos = { 'solicitudes':'&#x1F4CB;','cumpleanos':'&#x1F382;','contacto':'&#x1F4DE;' };
        var html = '';
        for (var i = 0; i < data.length; i++) {
            var a = data[i], urg = (a.Urgencia||'baja').toLowerCase(), tipo = (a.Tipo||'').toLowerCase();
            var icono = iconos[tipo] || '&#x2139;&#xFE0F;';
            var bClass = urg==='alta'?'badge-alta':(urg==='media'?'badge-media':'badge-baja');
            html += '<div class="alerta-item '+urg+'"><span class="alerta-icon">'+icono+'</span><div><div class="alerta-msg">'+(a.Mensaje||'')+'</div><span class="alerta-badge '+bClass+'">'+urg.charAt(0).toUpperCase()+urg.slice(1)+'</span></div></div>';
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
    };
    return obj;
}());
