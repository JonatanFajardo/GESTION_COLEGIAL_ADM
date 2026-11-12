var DashboardAulas = (function () {
    'use strict';

    var aulas = [];
    var niveles = [];
    var jornadas = [];
    var bajoDisponible = false;
    var asistenciaDisponible = false;
    var currentFilter = 'all';
    var currentJornada = 'all';
    var compChart = null;

    function jornadaPorHora() {
        var h = new Date().getHours();
        if (h >= 6 && h < 12) return 'Matutina';
        if (h >= 12 && h < 18) return 'Vespertina';
        if (h >= 18 && h < 23) return 'Nocturna';
        return null;
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

    function pct(a, b) {
        if (!b || b === 0) return 0;
        return Math.round((a / b) * 100);
    }

    function getStatus(a) {
        if (!bajoDisponible) return 'good';
        var p = pct(a.Bajo, a.Alumnos);
        if (p >= 25) return 'danger';
        if (p >= 15) return 'warning';
        return 'good';
    }

    function getBadge(a) {
        if (!bajoDisponible) return '';
        var p = pct(a.Bajo, a.Alumnos);
        if (p >= 25) return '<span class="badge badge-danger">' + p + '% bajo rendimiento</span>';
        if (p >= 15) return '<span class="badge badge-warning">' + p + '% bajo rendimiento</span>';
        return '<span class="badge badge-good">' + p + '% rendimiento OK</span>';
    }

    function buildDots(a) {
        var html = '';
        var i;
        if (!bajoDisponible) {
            var debt = Math.min(a.Deuda, a.Alumnos);
            var ok = a.Alumnos - debt;
            for (i = 0; i < ok; i++) html += '<span class="dot ok" title="Normal"></span>';
            for (i = 0; i < debt; i++) html += '<span class="dot debt" title="Con deuda"></span>';
            return html;
        }
        var okFull = a.Alumnos - a.Bajo;
        var lowOnly = a.Bajo - Math.min(a.Bajo, a.Deuda);
        var debtLow = Math.min(a.Bajo, a.Deuda);
        var debtOk = a.Deuda - debtLow;
        var pureOk = okFull - debtOk;
        for (i = 0; i < pureOk; i++) html += '<span class="dot ok" title="Normal"></span>';
        for (i = 0; i < debtOk; i++) html += '<span class="dot debt" title="Con deuda"></span>';
        for (i = 0; i < debtLow; i++) html += '<span class="dot low" title="Bajo rendimiento + deuda" style="border:2px solid #E74C3C;background:#FDECEA"></span>';
        for (i = 0; i < lowOnly; i++) html += '<span class="dot low" title="Bajo rendimiento"></span>';
        return html;
    }

    function renderFilters() {
        var container = document.getElementById('da-filters');
        if (container) {
            var criticasBtn = container.querySelector('[data-filter="danger"]');
            for (var i = 0; i < niveles.length; i++) {
                var n = niveles[i];
                var btn = document.createElement('button');
                btn.className = 'filter-btn';
                btn.setAttribute('data-filter', n);
                btn.type = 'button';
                btn.textContent = n;
                if (criticasBtn) container.insertBefore(btn, criticasBtn);
                else container.appendChild(btn);
            }
        }

        var jornadaContainer = document.getElementById('da-filters-jornada');
        if (jornadaContainer) {
            if (!jornadas.length) {
                jornadaContainer.style.display = 'none';
            } else {
                for (var k = 0; k < jornadas.length; k++) {
                    var jBtn = document.createElement('button');
                    jBtn.className = 'filter-btn';
                    jBtn.setAttribute('data-jornada', jornadas[k]);
                    jBtn.type = 'button';
                    jBtn.textContent = jornadas[k];
                    jornadaContainer.appendChild(jBtn);
                }
            }
        }
    }

    function renderSummary(list) {
        var totalAlumnos = 0, totalBajo = 0, totalDeuda = 0, sumAsis = 0;
        for (var i = 0; i < list.length; i++) {
            totalAlumnos += list[i].Alumnos;
            totalBajo += list[i].Bajo;
            totalDeuda += list[i].Deuda;
            sumAsis += list[i].Asistencia;
        }
        var avgAsis = list.length ? Math.round(sumAsis / list.length) : 0;
        var criticas = 0;
        for (var j = 0; j < list.length; j++) {
            if (getStatus(list[j]) === 'danger') criticas++;
        }

        function card(color, icon, label, value, pctText) {
            var pctHtml = pctText ? ' <span class="stat-pct">(' + pctText + ')</span>' : '';
            return '<div class="stat-card ' + color + '">'
                +    '<div class="stat-icon"><i class="fa-solid ' + icon + '"></i></div>'
                +    '<div class="stat-body">'
                +      '<div class="stat-label">' + label + '</div>'
                +      '<div class="stat-value">' + value + pctHtml + '</div>'
                +    '</div>'
                +    '<div class="stat-bg-icon"><i class="fa-solid ' + icon + '"></i></div>'
                +  '</div>';
        }

        var html = card('indigo', 'fa-user-graduate', 'Total alumnos', totalAlumnos, '');

        if (bajoDisponible) {
            html += card('red', 'fa-chart-line-down', 'Bajo rendimiento', totalBajo, pct(totalBajo, totalAlumnos) + '%');
        }

        html += card('cyan', 'fa-money-bill-wave', 'Con deuda', totalDeuda, pct(totalDeuda, totalAlumnos) + '%');

        if (asistenciaDisponible) {
            html += card('emerald', 'fa-calendar-check', 'Asistencia promedio', avgAsis + '%', '');
        }

        if (bajoDisponible) {
            html += card(criticas > 0 ? 'amber' : 'emerald', 'fa-triangle-exclamation', 'Aulas críticas', criticas, '');
        }

        var el = document.getElementById('da-summary');
        if (el) el.innerHTML = html;
    }

    function renderCards(list) {
        var grid = document.getElementById('da-grid');
        if (!grid) return;
        if (!list.length) {
            grid.innerHTML = '<div style="padding:1rem;color:var(--color-text-secondary);font-size:13px">No hay aulas para mostrar.</div>';
            return;
        }
        var out = '';
        for (var i = 0; i < list.length; i++) {
            var a = list[i];
            var st = getStatus(a);
            var ocupPct = pct(a.Alumnos, a.Capacidad);
            var bajoPct = pct(a.Bajo, a.Alumnos);
            var deudaPct = pct(a.Deuda, a.Alumnos);

            var card = '<div class="aula-card ' + st + '" data-aula-id="' + a.Id + '">'
                + '<div class="aula-header">'
                +   '<div>'
                +     '<div class="aula-name">' + escapeHtml(a.Nombre) + '</div>'
                +     '<div class="aula-sub">' + escapeHtml(a.Nivel) + ' &middot; ' + escapeHtml(a.Modalidad) + '</div>'
                +   '</div>'
                +   getBadge(a)
                + '</div>'
                + '<div class="bar-row">'
                +   '<div class="bar-label"><span>Ocupacion</span><span>' + a.Alumnos + '/' + a.Capacidad + ' (' + ocupPct + '%)</span></div>'
                +   '<div class="bar-track"><div class="bar-fill fill-orange" style="width:' + ocupPct + '%"></div></div>'
                + '</div>';

            if (bajoDisponible) {
                card += '<div class="bar-row">'
                    +     '<div class="bar-label"><span>Bajo rendimiento</span><span>' + bajoPct + '%</span></div>'
                    +     '<div class="bar-track"><div class="bar-fill fill-red" style="width:' + bajoPct + '%"></div></div>'
                    +   '</div>';
            }

            if (asistenciaDisponible) {
                card += '<div class="bar-row">'
                    +     '<div class="bar-label"><span>Asistencia</span><span>' + a.Asistencia + '%</span></div>'
                    +     '<div class="bar-track"><div class="bar-fill fill-green" style="width:' + a.Asistencia + '%"></div></div>'
                    +   '</div>';
            }

            card += '<div class="bar-row">'
                +     '<div class="bar-label"><span>Deudas</span><span>' + a.Deuda + ' alumnos (' + deudaPct + '%)</span></div>'
                +     '<div class="bar-track"><div class="bar-fill fill-blue" style="width:' + deudaPct + '%"></div></div>'
                +   '</div>'
                + '<div class="alumnos-dots">' + buildDots(a) + '</div>'
                + '<div class="dot-legend" style="margin-top:5px">'
                +   '<span class="dot-leg-item"><span class="dot ok" style="width:9px;height:9px"></span>Normal</span>';
            if (bajoDisponible) {
                card += '<span class="dot-leg-item"><span class="dot low" style="width:9px;height:9px"></span>Bajo rend.</span>';
            }
            card += '<span class="dot-leg-item"><span class="dot debt" style="width:9px;height:9px"></span>Con deuda</span>'
                + '</div>'
                + '</div>';
            out += card;
        }
        grid.innerHTML = out;
    }

    function renderChart(list) {
        var canvas = document.getElementById('da-chart');
        if (!canvas || typeof Chart === 'undefined') return;

        var labels = [], data1 = [], data2 = [], data3 = [], data4 = [];
        for (var i = 0; i < list.length; i++) {
            labels.push(list[i].Nombre);
            data1.push(list[i].Alumnos);
            data2.push(list[i].Bajo);
            data3.push(list[i].Deuda);
            data4.push(list[i].Asistencia);
        }

        var datasets = [
            { label: 'Alumnos', data: data1, backgroundColor: '#E8A445', borderWidth: 0 },
            { label: 'Bajo rend.', data: data2, backgroundColor: '#E74C3C', borderWidth: 0 },
            { label: 'Con deuda', data: data3, backgroundColor: '#2980B9', borderWidth: 0 }
        ];
        if (asistenciaDisponible) {
            datasets.push({
                label: 'Asistencia %', data: data4, backgroundColor: '#27AE60',
                borderColor: '#27AE60', borderWidth: 2,
                type: 'line', yAxisID: 'y2', tension: 0.3, pointRadius: 4
            });
        }

        var scales = {
            y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.05)' }, ticks: { color: '#888' } },
            x: { ticks: { color: '#888', autoSkip: false } }
        };
        if (asistenciaDisponible) {
            scales.y2 = {
                position: 'right', min: 50, max: 100,
                grid: { drawOnChartArea: false },
                ticks: { color: '#27AE60', callback: function (v) { return v + '%'; } }
            };
        }

        if (compChart) compChart.destroy();
        compChart = new Chart(canvas, {
            type: 'bar',
            data: { labels: labels, datasets: datasets },
            options: {
                responsive: true, maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: scales
            }
        });
    }

    function openDetail(id) {
        var a = null;
        for (var i = 0; i < aulas.length; i++) {
            if (aulas[i].Id === id) { a = aulas[i]; break; }
        }
        if (!a) return;

        var ok = a.Alumnos - a.Bajo;
        var bajoCell = bajoDisponible
            ? (a.Bajo + ' (' + pct(a.Bajo, a.Alumnos) + '%)')
            : 'N/D';
        var buenCell = bajoDisponible
            ? (ok + ' (' + pct(ok, a.Alumnos) + '%)')
            : 'N/D';
        var asisClass = a.Asistencia >= 90 ? 'green' : (a.Asistencia >= 80 ? 'amber' : 'red');
        var asisCell = asistenciaDisponible ? (a.Asistencia + '%') : '&mdash;';

        var html = '<div style="font-size:16px;font-weight:500;color:var(--color-text-primary);margin-bottom:4px">' + escapeHtml(a.Nombre) + ' &mdash; Detalle completo</div>'
            + '<div style="font-size:12px;color:var(--color-text-secondary);margin-bottom:12px">' + escapeHtml(a.Nivel) + ' &middot; ' + escapeHtml(a.Modalidad) + ' &middot; Capacidad: ' + a.Capacidad + '</div>'
            + '<div class="detail-grid">'
            +   '<div class="mini-stat"><div class="mini-stat-label">Alumnos inscritos</div><div class="mini-stat-val">' + a.Alumnos + '</div></div>'
            +   '<div class="mini-stat"><div class="mini-stat-label">Cupos disponibles</div><div class="mini-stat-val">' + (a.Capacidad - a.Alumnos) + '</div></div>'
            +   '<div class="mini-stat"><div class="mini-stat-label">Buen rendimiento</div><div class="mini-stat-val ' + (bajoDisponible ? 'green' : '') + '">' + buenCell + '</div></div>'
            +   '<div class="mini-stat"><div class="mini-stat-label">Bajo rendimiento</div><div class="mini-stat-val ' + (bajoDisponible ? 'red' : '') + '">' + bajoCell + '</div></div>'
            +   '<div class="mini-stat"><div class="mini-stat-label">Asistencia</div><div class="mini-stat-val ' + (asistenciaDisponible ? asisClass : '') + '">' + asisCell + '</div></div>'
            +   '<div class="mini-stat"><div class="mini-stat-label">Alumnos con deuda</div><div class="mini-stat-val amber">' + a.Deuda + ' (' + pct(a.Deuda, a.Alumnos) + '%)</div></div>'
            + '</div>'
            + '<div style="margin-top:12px">'
            +   '<div style="font-size:12px;color:var(--color-text-secondary);margin-bottom:5px">Representacion visual de alumnos</div>'
            +   '<div class="alumnos-dots">' + buildDots(a) + '</div>'
            +   '<div class="dot-legend" style="margin-top:6px">'
            +     '<span class="dot-leg-item"><span class="dot ok" style="width:10px;height:10px"></span>Buen rendimiento</span>';
        if (bajoDisponible) {
            html += '<span class="dot-leg-item"><span class="dot low" style="width:10px;height:10px"></span>Bajo rendimiento</span>'
                + '<span class="dot-leg-item"><span class="dot low" style="width:10px;height:10px;border:2px solid #E74C3C;background:#FDECEA"></span>Bajo rend. + deuda</span>';
        }
        html += '<span class="dot-leg-item"><span class="dot debt" style="width:10px;height:10px"></span>Con deuda</span>'
            +   '</div>'
            + '</div>';

        var content = document.getElementById('da-detail-content');
        var panel = document.getElementById('da-detail');
        if (content) content.innerHTML = html;
        if (panel) {
            panel.classList.add('visible');
            panel.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    }

    function closeDetail() {
        var panel = document.getElementById('da-detail');
        if (panel) panel.classList.remove('visible');
    }

    function render() {
        var btns = document.querySelectorAll('#da-filters .filter-btn');
        for (var i = 0; i < btns.length; i++) {
            if (btns[i].getAttribute('data-filter') === currentFilter) btns[i].classList.add('active');
            else btns[i].classList.remove('active');
        }
        var jBtns = document.querySelectorAll('#da-filters-jornada .filter-btn');
        for (var b = 0; b < jBtns.length; b++) {
            if (jBtns[b].getAttribute('data-jornada') === currentJornada) jBtns[b].classList.add('active');
            else jBtns[b].classList.remove('active');
        }
        closeDetail();

        var filtered = [];
        for (var k = 0; k < aulas.length; k++) {
            var a = aulas[k];
            if (currentJornada !== 'all' && a.Modalidad !== currentJornada) continue;
            if (currentFilter === 'all') {
                filtered.push(a);
            } else if (currentFilter === 'danger') {
                if (getStatus(a) === 'danger') filtered.push(a);
            } else {
                if (a.Nivel === currentFilter) filtered.push(a);
            }
        }
        renderSummary(filtered);
        renderCards(filtered);
        renderChart(filtered);
    }

    function applyFilter(val) {
        currentFilter = val;
        render();
    }

    function applyJornada(val) {
        currentJornada = val;
        render();
    }

    function bindEvents() {
        var filters = document.getElementById('da-filters');
        if (filters) {
            filters.addEventListener('click', function (e) {
                var target = e.target;
                while (target && target !== filters && !target.classList.contains('filter-btn')) {
                    target = target.parentNode;
                }
                if (target && target.classList && target.classList.contains('filter-btn')) {
                    var val = target.getAttribute('data-filter');
                    if (val) applyFilter(val);
                }
            });
        }

        var jornadaFilters = document.getElementById('da-filters-jornada');
        if (jornadaFilters) {
            jornadaFilters.addEventListener('click', function (e) {
                var target = e.target;
                while (target && target !== jornadaFilters && !(target.classList && target.classList.contains('filter-btn'))) {
                    target = target.parentNode;
                }
                if (target && target.classList && target.classList.contains('filter-btn')) {
                    var val = target.getAttribute('data-jornada');
                    if (val) applyJornada(val);
                }
            });
        }

        var grid = document.getElementById('da-grid');
        if (grid) {
            grid.addEventListener('click', function (e) {
                var target = e.target;
                while (target && target !== grid && !(target.classList && target.classList.contains('aula-card'))) {
                    target = target.parentNode;
                }
                if (target && target.classList && target.classList.contains('aula-card')) {
                    var idAttr = target.getAttribute('data-aula-id');
                    if (idAttr !== null) openDetail(parseInt(idAttr, 10));
                }
            });
        }

        var closeBtn = document.getElementById('da-detail-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', closeDetail);
        }
    }

    function renderNote() {
        var note = document.getElementById('da-note');
        if (!note) return;
        if (!bajoDisponible || !asistenciaDisponible) {
            note.textContent = 'Rendimiento y asistencia pendientes de integracion con la API';
            note.style.display = '';
        } else {
            note.textContent = '';
            note.style.display = 'none';
        }
    }

    function init(url) {
        bindEvents();
        $.getJSON(url)
            .done(function (data) {
                aulas = (data && data.Aulas) ? data.Aulas : [];
                niveles = (data && data.Niveles) ? data.Niveles : [];
                bajoDisponible = !!(data && data.BajoDisponible);
                asistenciaDisponible = !!(data && data.AsistenciaDisponible);

                var jornadasSet = {};
                for (var i = 0; i < aulas.length; i++) {
                    var m = aulas[i].Modalidad;
                    if (m && m !== '—') jornadasSet[m] = true;
                }
                jornadas = Object.keys(jornadasSet).sort();

                var loader = document.getElementById('da-loading');
                if (loader && loader.parentNode) loader.parentNode.removeChild(loader);

                renderFilters();
                renderNote();

                var jorActual = jornadaPorHora();
                currentJornada = (jorActual && jornadasSet[jorActual]) ? jorActual : 'all';
                currentFilter = 'all';
                render();
            })
            .fail(function (xhr, status, err) {
                console.warn('DashboardAulas: error al cargar', status, err);
                var grid = document.getElementById('da-grid');
                if (grid) {
                    grid.innerHTML = '<div style="padding:1rem;color:var(--color-text-secondary);font-size:13px">No se pudo cargar la informacion.</div>';
                }
            });
    }

    return {
        init: init
    };
}());
