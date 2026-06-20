/* ============================================================
   GESTIÓN COLEGIAL — PREFERENCIAS DE USUARIO
   ============================================================ */
(function () {
    var KEYS = {
        theme:    'gc-theme',
        dark:     'gc-dark-mode',
        sidebar:  'gc-sidebar-collapsed',
        pageLen:  'gc-page-length',
        density:  'gc-density',
        moneda:   'gc-moneda'
    };

    var THEME_CLASSES = ['gc-theme-blue'];

    var _currencies = {
        'Q': { symbol: 'Q', locale: 'es-GT' },
        'L': { symbol: 'L', locale: 'es-HN' },
        '$': { symbol: '$', locale: 'en-US' }
    };

    function get(key) {
        try { return localStorage.getItem(key); } catch (e) { return null; }
    }

    function set(key, val) {
        try { localStorage.setItem(key, val); } catch (e) {}
    }

    function _getCur() {
        return _currencies[get(KEYS.moneda)] || _currencies['L'];
    }

    function applyTheme(cls) {
        var html = document.documentElement;
        THEME_CLASSES.forEach(function (c) { html.classList.remove(c); });
        if (cls) html.classList.add(cls);
    }

    function applyDark(on) {
        document.documentElement.classList.toggle('gc-dark', !!on);
    }

    function applyDensity(compact) {
        document.documentElement.classList.toggle('gc-density-compact', !!compact);
    }

    // Apply all persisted prefs immediately (before DOMContentLoaded) to avoid flash
    applyTheme(get(KEYS.theme) || '');
    applyDark(get(KEYS.dark) === '1');
    applyDensity(get(KEYS.density) === 'compact');

    // Global currency formatters — used by all page scripts
    window.GC = window.GC || {};
    window.GC.fmtMoney = function (val) {
        var cur = _getCur();
        if (val === null || val === undefined || val === '') return cur.symbol + ' 0.00';
        var n = Number(val);
        if (isNaN(n)) return cur.symbol + ' 0.00';
        return cur.symbol + ' ' + n.toLocaleString(cur.locale, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    };
    window.GC.fmtMoneyShort = function (val) {
        var cur = _getCur();
        var n = Number(val) || 0;
        if (Math.abs(n) >= 1000000) return cur.symbol + ' ' + (n / 1000000).toFixed(1) + 'M';
        if (Math.abs(n) >= 1000)    return cur.symbol + ' ' + (n / 1000).toFixed(1) + 'K';
        return cur.symbol + ' ' + n.toFixed(0);
    };
    window.GC.currencySymbol = function () { return _getCur().symbol; };

    document.addEventListener('DOMContentLoaded', function () {

        // ── Sidebar collapsed preference ──
        if (get(KEYS.sidebar) === '1') {
            var container = document.getElementById('container');
            if (container) container.classList.add('sidebar-closed');
        }

        // ── Reflect state when modal opens ──
        $('#modal-ajustes').on('show.bs.modal', function () {
            var currentTheme   = get(KEYS.theme) || '';
            var isDark         = get(KEYS.dark) === '1';
            var isSidebarCol   = get(KEYS.sidebar) === '1';
            var currentLen     = get(KEYS.pageLen) || '10';
            var currentDensity = get(KEYS.density) || '';
            var currentMoneda  = get(KEYS.moneda) || 'Q';

            document.querySelectorAll('.ajustes-color-btn').forEach(function (btn) {
                btn.classList.toggle('active', (btn.dataset.theme || '') === currentTheme);
            });

            var darkEl = document.getElementById('toggle-dark-mode');
            if (darkEl) darkEl.checked = isDark;

            var sidebarEl = document.getElementById('toggle-sidebar-collapsed');
            if (sidebarEl) sidebarEl.checked = isSidebarCol;

            document.querySelectorAll('[data-page-length]').forEach(function (btn) {
                btn.classList.toggle('active', btn.dataset.pageLength === currentLen);
            });

            document.querySelectorAll('[data-density]').forEach(function (btn) {
                btn.classList.toggle('active', btn.dataset.density === currentDensity);
            });

            document.querySelectorAll('[data-moneda]').forEach(function (btn) {
                btn.classList.toggle('active', btn.dataset.moneda === currentMoneda);
            });
        });

        // ── Color swatches ──
        document.querySelectorAll('.ajustes-color-btn').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var cls = btn.dataset.theme || '';
                applyTheme(cls);
                set(KEYS.theme, cls);
                document.querySelectorAll('.ajustes-color-btn').forEach(function (b) {
                    b.classList.toggle('active', (b.dataset.theme || '') === cls);
                });
            });
        });

        // ── Dark mode ──
        var darkToggle = document.getElementById('toggle-dark-mode');
        if (darkToggle) {
            darkToggle.addEventListener('change', function () {
                applyDark(this.checked);
                set(KEYS.dark, this.checked ? '1' : '0');
            });
        }

        // ── Sidebar collapsed ──
        var sidebarToggle = document.getElementById('toggle-sidebar-collapsed');
        if (sidebarToggle) {
            sidebarToggle.addEventListener('change', function () {
                set(KEYS.sidebar, this.checked ? '1' : '0');
                var container = document.getElementById('container');
                if (container) container.classList.toggle('sidebar-closed', this.checked);
            });
        }

        // ── Page length ──
        document.querySelectorAll('[data-page-length]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var len = btn.dataset.pageLength;
                set(KEYS.pageLen, len);
                document.querySelectorAll('[data-page-length]').forEach(function (b) {
                    b.classList.toggle('active', b.dataset.pageLength === len);
                });
                // Apply immediately to any active DataTables
                if (typeof $ !== 'undefined' && $.fn && $.fn.dataTable) {
                    try {
                        $.fn.dataTable.tables({ api: true }).page.len(parseInt(len, 10)).draw();
                    } catch (e) {}
                }
            });
        });

        // ── Density ──
        document.querySelectorAll('[data-density]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var dens = btn.dataset.density;
                applyDensity(dens === 'compact');
                set(KEYS.density, dens);
                document.querySelectorAll('[data-density]').forEach(function (b) {
                    b.classList.toggle('active', b.dataset.density === dens);
                });
            });
        });

        // ── Moneda ──
        document.querySelectorAll('[data-moneda]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var code = btn.dataset.moneda;
                set(KEYS.moneda, code);
                document.querySelectorAll('[data-moneda]').forEach(function (b) {
                    b.classList.toggle('active', b.dataset.moneda === code);
                });
                document.dispatchEvent(new CustomEvent('gc:moneda'));
            });
        });
    });
})();
