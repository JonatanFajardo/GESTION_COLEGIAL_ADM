var DashboardAdmin = (function () {
    'use strict';

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-HN');
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
            return '<div class="statbox widget box box-shadow mb-4"><div class="widget-header"><div class="row"><div class="col-12"><h4>Usuarios por Rol</h4></div></div></div><div class="widget-content widget-content-area">'
                + '<p class="text-muted mb-0">Sin datos disponibles.</p>'
                + '</div></div>';
        }

        var rows = '';
        for (var i = 0; i < data.length; i++) {
            var r = data[i];
            rows += '<tr>'
                + '<td>' + escapeHtml(r.Rol_Descripcion) + '</td>'
                + '<td class="text-right font-weight-bold">' + fmtNum(r.Total) + '</td>'
                + '</tr>';
        }

        return '<div class="statbox widget box box-shadow mb-4"><div class="widget-header"><div class="row"><div class="col-12"><h4>Usuarios por Rol</h4></div></div></div><div class="widget-content widget-content-area">'
            + '<table class="table table-hover table-sm mb-0">'
            +   '<thead><tr><th>Rol</th><th class="text-right">Total</th></tr></thead>'
            +   '<tbody>' + rows + '</tbody>'
            + '</table>'
            + '</div></div>';
    }

    // ── Bitácora ────────────────────────────────────────────────────────────
    function renderBitacora(data) {
        if (!data || data.length === 0) {
            return '<div class="statbox widget box box-shadow mb-4"><div class="widget-header"><div class="row"><div class="col-12"><h4>Bitácora reciente</h4></div></div></div><div class="widget-content widget-content-area">'
                + '<p class="text-muted mb-0">Sin datos disponibles.</p>'
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

        return '<div class="statbox widget box box-shadow mb-4"><div class="widget-header"><div class="row"><div class="col-12"><h4>Bitácora reciente</h4></div></div></div><div class="widget-content widget-content-area">'
            + '<div class="table-responsive">'
            +   '<table class="table table-hover table-sm mb-0">'
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
