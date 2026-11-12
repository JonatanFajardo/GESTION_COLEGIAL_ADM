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

        // ── Institución (nombre y dirección) ──
        var _instOriginal = { Nombre: '', Direccion: '' };

        function _instRefreshDirtyState() {
            var n = (document.getElementById('ajustes-instituto-nombre') || {}).value || '';
            var d = (document.getElementById('ajustes-instituto-direccion') || {}).value || '';
            var dirty = (n.trim() !== _instOriginal.Nombre) || (d.trim() !== _instOriginal.Direccion);
            var validos = n.trim().length > 0 && d.trim().length > 0;
            var btn = document.getElementById('btn-ajustes-instituto-guardar');
            if (btn) btn.disabled = !(dirty && validos);
            var ok = document.getElementById('ajustes-instituto-ok');
            if (ok) ok.style.display = 'none';
        }

        function _instLoad() {
            $.getJSON('/InstitutoConfig/GetAsync', function (resp) {
                var d = (resp && resp.data) || { Nombre: '', Direccion: '' };
                _instOriginal.Nombre = d.Nombre || '';
                _instOriginal.Direccion = d.Direccion || '';
                var nEl = document.getElementById('ajustes-instituto-nombre');
                var dEl = document.getElementById('ajustes-instituto-direccion');
                if (nEl) nEl.value = _instOriginal.Nombre;
                if (dEl) dEl.value = _instOriginal.Direccion;
                document.getElementById('ajustes-instituto-actions').style.display = '';
                document.getElementById('ajustes-instituto-confirm').style.display = 'none';
                document.getElementById('ajustes-instituto-error').style.display = 'none';
                document.getElementById('ajustes-instituto-ok').style.display = 'none';
                _instRefreshDirtyState();
            }).fail(function () {
                var err = document.getElementById('ajustes-instituto-error');
                if (err) { err.textContent = 'No se pudo cargar la configuración del instituto.'; err.style.display = ''; }
            });
        }

        $('#modal-ajustes').on('shown.bs.modal', function () { _instLoad(); });

        ['ajustes-instituto-nombre', 'ajustes-instituto-direccion'].forEach(function (id) {
            var el = document.getElementById(id);
            if (el) el.addEventListener('input', _instRefreshDirtyState);
        });

        var btnGuardar = document.getElementById('btn-ajustes-instituto-guardar');
        if (btnGuardar) btnGuardar.addEventListener('click', function () {
            document.getElementById('ajustes-instituto-actions').style.display = 'none';
            document.getElementById('ajustes-instituto-confirm').style.display = '';
            document.getElementById('ajustes-instituto-error').style.display = 'none';
        });

        var btnCancelar = document.getElementById('btn-ajustes-instituto-cancelar');
        if (btnCancelar) btnCancelar.addEventListener('click', function () {
            document.getElementById('ajustes-instituto-actions').style.display = '';
            document.getElementById('ajustes-instituto-confirm').style.display = 'none';
        });

        var btnConfirmar = document.getElementById('btn-ajustes-instituto-confirmar');
        if (btnConfirmar) btnConfirmar.addEventListener('click', function () {
            var nombre = document.getElementById('ajustes-instituto-nombre').value.trim();
            var direccion = document.getElementById('ajustes-instituto-direccion').value.trim();
            var token = $('#ajustes-instituto-form input[name="__RequestVerificationToken"]').val();
            btnConfirmar.disabled = true;
            $.ajax({
                url: '/InstitutoConfig/SaveAsync',
                type: 'POST',
                data: { __RequestVerificationToken: token, Nombre: nombre, Direccion: direccion },
                success: function (resp) {
                    btnConfirmar.disabled = false;
                    if (resp && resp.success) {
                        _instOriginal.Nombre = nombre;
                        _instOriginal.Direccion = direccion;
                        document.getElementById('ajustes-instituto-actions').style.display = '';
                        document.getElementById('ajustes-instituto-confirm').style.display = 'none';
                        var ok = document.getElementById('ajustes-instituto-ok');
                        ok.style.display = '';
                        _instRefreshDirtyState();
                        document.dispatchEvent(new CustomEvent('gc:instituto-actualizado', { detail: { Nombre: nombre, Direccion: direccion } }));
                    } else {
                        var err = document.getElementById('ajustes-instituto-error');
                        err.textContent = (resp && resp.message) || 'No se pudo guardar.';
                        err.style.display = '';
                    }
                },
                error: function () {
                    btnConfirmar.disabled = false;
                    var err = document.getElementById('ajustes-instituto-error');
                    err.textContent = 'Error de red al guardar.';
                    err.style.display = '';
                }
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

/* ── Sidebar en móvil/tablet (< 992px): hamburguesa abre/cierra el cajón + overlay ── */
document.addEventListener('DOMContentLoaded', function () {
    var container = document.getElementById('container');
    var overlay = document.querySelector('.overlay');
    if (!container) return;
    function setDrawer(open) {
        container.classList.toggle('sbar-open', open);
        if (overlay) overlay.classList.toggle('show', open);
        document.documentElement.classList.toggle('sidebar-noneoverflow', open); // bloquea el scroll del fondo (regla existente < 992px)
    }
    document.addEventListener('click', function (e) {
        if (e.target.closest('.sidebarCollapse')) { e.preventDefault(); setDrawer(!container.classList.contains('sbar-open')); }
        else if (e.target.closest('.overlay')) { setDrawer(false); }
    });
    document.addEventListener('keydown', function (e) { if (e.key === 'Escape') setDrawer(false); });
    var mq = window.matchMedia('(min-width: 992px)');
    var onMq = function () { if (mq.matches) setDrawer(false); };
    if (mq.addEventListener) mq.addEventListener('change', onMq); else mq.addListener(onMq);
});
