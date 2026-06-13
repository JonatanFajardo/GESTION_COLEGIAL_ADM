var DashboardAdmin = (function () {
    'use strict';

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-GT');
    }

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = (value !== null && value !== undefined) ? value : '—';
    }

    function fmtFecha(val) {
        if (!val) return '';
        try {
            var d = new Date(val);
            if (isNaN(d.getTime())) return val;
            var pad = function (n) { return n < 10 ? '0' + n : '' + n; };
            return d.getFullYear() + '-' + pad(d.getMonth() + 1) + '-' + pad(d.getDate())
                + ' ' + pad(d.getHours()) + ':' + pad(d.getMinutes());
        } catch (e) {
            return val;
        }
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

    // ── KPIs ────────────────────────────────────────────────────────────────
    function renderKpis(k) {
        setText('kpi-totalUsuarios',  fmtNum(k.TotalUsuariosActivos));
        setText('kpi-accesosHoy',     fmtNum(k.AccionesHoy));
        setText('kpi-rolesActivos',   fmtNum(k.TotalRoles));
        // No hay un contador directo de "alertas" en KpisResult;
        // mostramos 0 por defecto para mantener el placeholder vivo.
        setText('kpi-alertasSistema', fmtNum(0));
    }

    // ── Usuarios por Rol ────────────────────────────────────────────────────
    function renderUsuariosPorRol(data) {
        if (!data || data.length === 0) {
            return '<div class="card mb-3"><div class="card-body">'
                + '<h5 class="card-title">Usuarios por Rol</h5>'
                + '<p style="color:#94a3b8;margin:0;">Sin datos disponibles.</p>'
                + '</div></div>';
        }

        var rows = '';
        for (var i = 0; i < data.length; i++) {
            var r = data[i];
            rows += '<tr>'
                + '<td>' + escapeHtml(r.Rol_Descripcion) + '</td>'
                + '<td style="text-align:right;font-weight:700;">' + fmtNum(r.Total) + '</td>'
                + '</tr>';
        }

        return '<div class="card mb-3"><div class="card-body">'
            + '<h5 class="card-title">Usuarios por Rol</h5>'
            + '<table class="table table-sm table-striped mb-0">'
            +   '<thead><tr><th>Rol</th><th style="text-align:right;">Total</th></tr></thead>'
            +   '<tbody>' + rows + '</tbody>'
            + '</table>'
            + '</div></div>';
    }

    // ── Bitácora ────────────────────────────────────────────────────────────
    function renderBitacora(data) {
        if (!data || data.length === 0) {
            return '<div class="card mb-3"><div class="card-body">'
                + '<h5 class="card-title">Bitácora reciente</h5>'
                + '<p style="color:#94a3b8;margin:0;">Sin datos disponibles.</p>'
                + '</div></div>';
        }

        var rows = '';
        for (var i = 0; i < data.length; i++) {
            var b = data[i];
            rows += '<tr>'
                + '<td>' + escapeHtml(fmtFecha(b.Bit_Fecha)) + '</td>'
                + '<td>' + escapeHtml(b.Usu_Name) + '</td>'
                + '<td>' + escapeHtml(b.Bit_Accion) + '</td>'
                + '<td>' + escapeHtml(b.Bit_Tabla) + '</td>'
                + '<td>' + escapeHtml(b.Bit_Descripcion) + '</td>'
                + '<td>' + escapeHtml(b.Bit_Ip) + '</td>'
                + '</tr>';
        }

        return '<div class="card mb-3"><div class="card-body">'
            + '<h5 class="card-title">Bitácora reciente</h5>'
            + '<div class="table-responsive">'
            +   '<table class="table table-sm table-striped mb-0">'
            +     '<thead><tr>'
            +       '<th>Fecha</th><th>Usuario</th><th>Acción</th>'
            +       '<th>Tabla</th><th>Descripción</th><th>IP</th>'
            +     '</tr></thead>'
            +     '<tbody>' + rows + '</tbody>'
            +   '</table>'
            + '</div>'
            + '</div></div>';
    }

    function renderDetalle(d) {
        var cont = document.getElementById('dashboard-detail');
        if (!cont) return;
        cont.innerHTML =
              renderUsuariosPorRol(d.UsuariosPorRol || [])
            + renderBitacora(d.Bitacora || []);
    }

    var obj = {};

    obj.init = function (url) {
        $.getJSON(url, function (res) {
            // El UI controller hace passthrough crudo del API (PascalCase, sin envoltura).
            var d = res || {};
            renderKpis(d.Kpis || {});
            renderDetalle(d);
        }).fail(function (xhr, status, err) {
            console.warn('DashboardAdmin: no se pudo cargar el resumen.', status, err);
        });
    };

    return obj;
}());
