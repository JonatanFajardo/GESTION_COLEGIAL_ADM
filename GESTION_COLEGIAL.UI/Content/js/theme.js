/* ============================================================
   GESTIÓN COLEGIAL — THEME SWITCHER
   ============================================================ */
(function () {
    var STORAGE_KEY = 'gc-theme';
    var THEMES = {
        amarillo: '',
        azul: 'gc-theme-blue'
    };

    function applyTheme(cls) {
        var html = document.documentElement;
        Object.values(THEMES).forEach(function (c) { if (c) html.classList.remove(c); });
        if (cls) html.classList.add(cls);
    }

    function saveTheme(cls) {
        try { localStorage.setItem(STORAGE_KEY, cls || ''); } catch (e) {}
    }

    function loadTheme() {
        try { return localStorage.getItem(STORAGE_KEY) || ''; } catch (e) { return ''; }
    }

    function updateSwatches(activeClass) {
        document.querySelectorAll('.ajustes-color-btn').forEach(function (btn) {
            var isActive = (btn.dataset.theme || '') === (activeClass || '');
            btn.classList.toggle('active', isActive);
        });
    }

    // Aplicar tema guardado inmediatamente
    applyTheme(loadTheme());

    document.addEventListener('DOMContentLoaded', function () {
        // Reflejar tema actual en los swatches al abrir el modal
        $('#modal-ajustes').on('show.bs.modal', function () {
            updateSwatches(loadTheme());
        });

        // Click en swatch
        document.querySelectorAll('.ajustes-color-btn').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var cls = btn.dataset.theme || '';
                applyTheme(cls);
                saveTheme(cls);
                updateSwatches(cls);
            });
        });
    });
})();
