/* =================================================================
   Organigrama Institucional — Dual Mode: Tree / Directory
   ================================================================= */
var Organigrama = (function () {
    "use strict";

    var obj = {};

    var PALETTE = ['#3a7bd5', '#00b09b', '#f7971e', '#8e44ad', '#e74c3c', '#16a085', '#d35400'];

    var _employees  = [];
    var _mode       = 'directory'; /* 'tree' | 'directory' */
    var _activeDept = 'all';
    var _sortBy     = 'name';

    /* ── Helpers ────────────────────────────────────────────────── */

    function softColor(hex, alpha) {
        var r = parseInt(hex.slice(1, 3), 16);
        var g = parseInt(hex.slice(3, 5), 16);
        var b = parseInt(hex.slice(5, 7), 16);
        return 'rgba(' + r + ',' + g + ',' + b + ',' + (alpha || 0.12) + ')';
    }

    function initials(fullName, codigo) {
        if (fullName && fullName.trim()) {
            var parts = fullName.trim().split(/\s+/).filter(function (p) { return p.length > 0; });
            if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
            return (parts[0].charAt(0) + parts[1].charAt(0)).toUpperCase();
        }
        if (codigo && codigo.trim()) return codigo.trim().slice(0, 2).toUpperCase();
        return '??';
    }

    function esc(str) {
        return $('<span>').text(str).html();
    }

    function groupByDep(employees) {
        var map = {}, order = [];
        employees.forEach(function (emp) {
            var dep = (emp.NombreDepartamento && emp.NombreDepartamento.trim())
                ? emp.NombreDepartamento.trim() : 'Sin Departamento';
            if (!map[dep]) { map[dep] = []; order.push(dep); }
            map[dep].push(emp);
        });
        return { map: map, order: order };
    }

    function buildDepColorIndex(employees) {
        var idx = {}, counter = 0;
        employees.forEach(function (emp) {
            var dep = (emp.NombreDepartamento && emp.NombreDepartamento.trim())
                ? emp.NombreDepartamento.trim() : 'Sin Departamento';
            if (!(dep in idx)) idx[dep] = counter++;
        });
        return idx;
    }

    function autoDetectMode(employees) {
        var realDeps = groupByDep(employees).order.filter(function (d) {
            return d !== 'Sin Departamento';
        });
        return realDeps.length >= 2 ? 'tree' : 'directory';
    }

    /* ── Stats ──────────────────────────────────────────────────── */

    function buildStats(employees) {
        var grouped   = groupByDep(employees);
        var cargosSet = {};
        employees.forEach(function (e) {
            if (e.Emp_Cargo && e.Emp_Cargo.trim()) cargosSet[e.Emp_Cargo.trim()] = 1;
        });
        $('#stat-empleados').text(employees.length);
        $('#stat-departamentos').text(grouped.order.length);
        $('#stat-cargos').text(Object.keys(cargosSet).length);
    }

    /* ── TREE VIEW ──────────────────────────────────────────────── */

    function updateConnector() {
        var $deps = $('#org-deps-row');
        if (!$deps.length) return;
        var $nodes = $deps.find('.org-dep');
        if ($nodes.length < 2) {
            $deps.css('--bar-left', '50%').css('--bar-right', '50%');
            return;
        }
        var cl = $deps.offset().left;
        var cw = $deps.outerWidth();
        var fc = $nodes.first().offset().left + $nodes.first().outerWidth() / 2 - cl;
        var lc = $nodes.last().offset().left  + $nodes.last().outerWidth()  / 2 - cl;
        $deps.css('--bar-left',  Math.max(0, fc) + 'px')
             .css('--bar-right', Math.max(0, cw - lc) + 'px');
    }

    function buildTree(employees) {
        var $c      = $('#org-chart-container');
        $c.empty().removeClass('org-dir-mode');

        var grouped  = groupByDep(employees);
        var $inner   = $('<div class="org-chart-inner"></div>');
        var $root    = $(
            '<div class="org-root">' +
                '<span class="root-icon"><i class="fa-solid fa-building-columns"></i></span>' +
                '<div class="root-title">Institución</div>' +
                '<div class="root-sub">' + employees.length + ' empleado' + (employees.length !== 1 ? 's' : '') + '</div>' +
            '</div>'
        );
        var $trunk   = $('<div class="org-trunk"></div>');
        var $depsRow = $('<div class="org-deps" id="org-deps-row"></div>');

        grouped.order.forEach(function (depName, idx) {
            var color     = PALETTE[idx % PALETTE.length];
            var colorSoft = softColor(color, 0.12);
            var emps      = grouped.map[depName];
            var $dep      = $('<div class="org-dep"></div>');

            var $depCard = $(
                '<div class="dep-card">' +
                    '<div class="dep-icon"><i class="fa-solid fa-layer-group"></i></div>' +
                    '<div class="dep-name">' + esc(depName) + '</div>' +
                    '<span class="dep-count"><i class="fa-solid fa-chevron-down"></i>' +
                        emps.length + ' miembro' + (emps.length !== 1 ? 's' : '') + '</span>' +
                '</div>'
            ).css({ '--dep-color': color, '--dep-color-soft': colorSoft });

            var $empList = $('<div class="dep-employees"></div>');
            emps.forEach(function (emp, empIdx) {
                var fallbackNum = String(emp.Emp_Id || (empIdx + 1));
                var nombre = (emp.Emp_NombreCompleto && emp.Emp_NombreCompleto.trim())
                    ? emp.Emp_NombreCompleto.trim() : 'Empleado #' + fallbackNum;
                var cargo  = (emp.Emp_Cargo && emp.Emp_Cargo.trim()) ? emp.Emp_Cargo.trim() : '—';
                var rawIni = initials(emp.Emp_NombreCompleto, emp.Emp_Codigo);
                var ini    = (rawIni !== '??') ? rawIni : (fallbackNum.length === 1 ? '0' + fallbackNum : fallbackNum.slice(0, 2));
                $('<div class="emp-card">' +
                    '<div class="emp-avatar">' + esc(ini) + '</div>' +
                    '<div class="emp-info">' +
                        '<div class="emp-name" title="' + esc(nombre) + '">' + esc(nombre) + '</div>' +
                        '<div class="emp-cargo" title="' + esc(cargo) + '">' + esc(cargo) + '</div>' +
                    '</div>' +
                '</div>').css('--avatar-bg', color).appendTo($empList);
            });

            $depCard.on('click', function () { $dep.toggleClass('collapsed'); });
            $dep.append($depCard).append($empList);
            $depsRow.append($dep);
        });

        $inner.append($root).append($trunk).append($depsRow);
        $c.append($inner);
        setTimeout(updateConnector, 50);
        $(window).off('resize.organigrama').on('resize.organigrama', updateConnector);
    }

    /* ── DIRECTORY VIEW ─────────────────────────────────────────── */

    function buildDirectory(employees) {
        var $c       = $('#org-chart-container');
        $c.empty().addClass('org-dir-mode');

        var grouped  = groupByDep(employees);
        var colorIdx = buildDepColorIndex(employees);

        /* Department filter pills */
        var $pills = $('<div class="org-dept-pills" id="org-dept-pills"></div>');
        $('<button class="dept-pill active" data-dept="all">' +
            'Todos <span class="pill-count">' + employees.length + '</span>' +
        '</button>').appendTo($pills);
        grouped.order.forEach(function (depName, idx) {
            var color = PALETTE[idx % PALETTE.length];
            $('<button class="dept-pill" data-dept="' + esc(depName) + '">' +
                esc(depName) + ' <span class="pill-count">' + grouped.map[depName].length + '</span>' +
            '</button>').css('--pill-color', color).appendTo($pills);
        });

        /* Sort bar */
        var $sortBar = $(
            '<div class="org-dir-sortbar">' +
                '<span class="sortbar-label"><i class="fa-solid fa-arrow-up-a-z"></i> Ordenar:</span>' +
                '<select id="org-sort-select" class="dir-sort-select">' +
                    '<option value="name">Nombre A → Z</option>' +
                    '<option value="name-desc">Nombre Z → A</option>' +
                    '<option value="dept">Departamento</option>' +
                    '<option value="id">Código</option>' +
                '</select>' +
            '</div>'
        );

        var $grid  = $('<div class="org-dir-grid" id="org-dir-grid"></div>');
        var $inner = $('<div class="org-dir-inner"></div>');
        $inner.append($pills).append($sortBar).append($grid);
        $c.append($inner);

        _activeDept = 'all';
        _sortBy     = 'name';
        renderDirCards(employees, 'all', 'name', colorIdx);

        $pills.on('click', '.dept-pill', function () {
            $pills.find('.dept-pill').removeClass('active');
            $(this).addClass('active');
            _activeDept = $(this).data('dept');
            $('#org-search-input').val('');
            renderDirCards(employees, _activeDept, _sortBy, colorIdx);
        });

        $('#org-sort-select').on('change', function () {
            _sortBy = $(this).val();
            renderDirCards(employees, _activeDept, _sortBy, colorIdx);
        });
    }

    function renderDirCards(employees, activeDept, sortBy, colorIdx) {
        var $grid = $('#org-dir-grid');
        $grid.empty();

        var filtered = employees.filter(function (emp) {
            if (activeDept === 'all') return true;
            var dep = (emp.NombreDepartamento && emp.NombreDepartamento.trim())
                ? emp.NombreDepartamento.trim() : 'Sin Departamento';
            return dep === activeDept;
        });

        filtered = filtered.slice().sort(function (a, b) {
            var na, nb;
            if (sortBy === 'name' || sortBy === 'name-desc') {
                na = (a.Emp_NombreCompleto || '').trim() || ('￿' + String(a.Emp_Id || 0));
                nb = (b.Emp_NombreCompleto || '').trim() || ('￿' + String(b.Emp_Id || 0));
                return sortBy === 'name' ? na.localeCompare(nb, 'es') : nb.localeCompare(na, 'es');
            }
            if (sortBy === 'dept') {
                na = (a.NombreDepartamento || 'Sin Departamento').trim();
                nb = (b.NombreDepartamento || 'Sin Departamento').trim();
                return na.localeCompare(nb, 'es');
            }
            return (a.Emp_Id || 0) - (b.Emp_Id || 0);
        });

        if (!filtered.length) {
            $('<div class="dir-empty"><i class="fa-solid fa-user-slash"></i><p>Sin empleados en este filtro.</p></div>')
                .appendTo($grid);
            return;
        }

        filtered.forEach(function (emp, index) {
            var dep   = (emp.NombreDepartamento && emp.NombreDepartamento.trim())
                ? emp.NombreDepartamento.trim() : 'Sin Departamento';
            var ci    = colorIdx[dep] !== undefined ? colorIdx[dep] : 0;
            var color = PALETTE[ci % PALETTE.length];
            var csoft = softColor(color, 0.12);

            var fallbackNum = String(emp.Emp_Id || (index + 1));
            var nombre = (emp.Emp_NombreCompleto && emp.Emp_NombreCompleto.trim())
                ? emp.Emp_NombreCompleto.trim() : 'Empleado #' + fallbackNum;
            var cargo  = (emp.Emp_Cargo && emp.Emp_Cargo.trim()) ? emp.Emp_Cargo.trim() : 'Sin cargo asignado';
            var rawIni = initials(emp.Emp_NombreCompleto, emp.Emp_Codigo);
            var ini    = (rawIni !== '??') ? rawIni : (fallbackNum.length === 1 ? '0' + fallbackNum : fallbackNum.slice(0, 2));
            var codigo = (emp.Emp_Codigo || '').trim();

            $('<div class="org-dir-card">' +
                '<div class="dir-avatar">' + esc(ini) + '</div>' +
                '<div class="dir-name" title="' + esc(nombre) + '">' + esc(nombre) + '</div>' +
                (codigo ? '<div class="dir-codigo">' + esc(codigo) + '</div>' : '') +
                '<div class="dir-cargo">' + esc(cargo) + '</div>' +
                '<span class="dir-dept-badge">' + esc(dep) + '</span>' +
            '</div>').css({ '--dir-color': color, '--dir-color-soft': csoft }).appendTo($grid);
        });
    }

    /* ── Search ─────────────────────────────────────────────────── */

    function bindSearch() {
        $('#org-search-input').off('input.org').on('input.org', function () {
            var q = $(this).val().trim().toLowerCase();

            if (_mode === 'tree') {
                if (!q) {
                    $('.emp-card').removeClass('dim match');
                    $('.org-dep').removeClass('collapsed');
                    return;
                }
                $('.emp-card').each(function () {
                    var hit = $(this).find('.emp-name').text().toLowerCase().indexOf(q) !== -1 ||
                              $(this).find('.emp-cargo').text().toLowerCase().indexOf(q) !== -1;
                    $(this).toggleClass('match', hit).toggleClass('dim', !hit);
                });
                $('.org-dep').each(function () {
                    if ($(this).find('.emp-card.match').length > 0) $(this).removeClass('collapsed');
                });
            } else {
                if (!q) {
                    $('.org-dir-card').removeClass('dim match');
                    return;
                }
                $('.org-dir-card').each(function () {
                    var hit = $(this).find('.dir-name').text().toLowerCase().indexOf(q) !== -1 ||
                              $(this).find('.dir-cargo').text().toLowerCase().indexOf(q) !== -1 ||
                              $(this).find('.dir-dept-badge').text().toLowerCase().indexOf(q) !== -1 ||
                              $(this).find('.dir-codigo').text().toLowerCase().indexOf(q) !== -1;
                    $(this).toggleClass('match', hit).toggleClass('dim', !hit);
                });
            }
        });
    }

    /* ── Expand / Collapse all (tree only) ──────────────────────── */

    function bindToggleAll() {
        $('#btn-expand-all').off('click.org').on('click.org', function () {
            if (_mode === 'tree') $('.org-dep').removeClass('collapsed');
        });
        $('#btn-collapse-all').off('click.org').on('click.org', function () {
            if (_mode === 'tree') $('.org-dep').addClass('collapsed');
        });
    }

    /* ── View-mode toggle ───────────────────────────────────────── */

    function applyMode(newMode) {
        _mode = newMode;
        $('#org-search-input').val('');
        if (newMode === 'tree') {
            $('#btn-mode-tree').addClass('active');
            $('#btn-mode-grid').removeClass('active');
            $('#org-tree-controls').removeClass('hidden');
            buildTree(_employees);
        } else {
            $('#btn-mode-grid').addClass('active');
            $('#btn-mode-tree').removeClass('active');
            $('#org-tree-controls').addClass('hidden');
            buildDirectory(_employees);
        }
        bindSearch();
    }

    function bindViewToggle() {
        $('#btn-mode-tree').off('click.org').on('click.org', function () {
            if (_mode !== 'tree') applyMode('tree');
        });
        $('#btn-mode-grid').off('click.org').on('click.org', function () {
            if (_mode !== 'directory') applyMode('directory');
        });
    }

    /* ── Public init ────────────────────────────────────────────── */

    obj.init = function (url) {
        $('#org-chart-container').html(
            '<div class="org-empty">' +
                '<i class="fa-solid fa-circle-notch fa-spin"></i>' +
                '<p>Cargando organigrama...</p>' +
            '</div>'
        );

        $.getJSON(url, function (response) {
            _employees = response && response.data ? response.data : [];
            buildStats(_employees);
            applyMode(autoDetectMode(_employees));
            bindToggleAll();
            bindViewToggle();
        }).fail(function () {
            $('#org-chart-container').html(
                '<div class="org-empty">' +
                    '<i class="fa-solid fa-triangle-exclamation"></i>' +
                    '<p>Error al cargar los datos del organigrama.</p>' +
                '</div>'
            );
        });
    };

    return obj;
}());
