/* ============================================================
   empleadoscumpleanos.js
   Módulo: Cumpleaños del Personal
   ============================================================ */

var EmpleadosCumpleanos = (function () {
    'use strict';

    var obj        = {};
    var _urlList   = '';
    var _mesActivo = 1;

    /* ---------- Datos ---------- */

    var MESES = [
        'Enero', 'Febrero', 'Marzo', 'Abril',
        'Mayo', 'Junio', 'Julio', 'Agosto',
        'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];

    var MESES_CORTOS = [
        'Ene', 'Feb', 'Mar', 'Abr',
        'May', 'Jun', 'Jul', 'Ago',
        'Sep', 'Oct', 'Nov', 'Dic'
    ];

    var AVATAR_COLORS = [
        '#3a7bd5', '#00b09b', '#f7971e', '#8e44ad',
        '#e74c3c', '#16a085', '#d35400'
    ];

    /* ---------- Inicialización pública ---------- */

    obj.init = function (urlList) {
        _urlList   = urlList;
        _mesActivo = new Date().getMonth() + 1;

        _renderMonthNav();
        _loadMes(_mesActivo);
    };

    /* ---------- Construcción del nav de meses ---------- */

    function _renderMonthNav() {
        var $ul = $('<ul>', { 'class': 'nav nav-pills' });

        for (var i = 1; i <= 12; i++) {
            var $a = $('<a>', {
                'class'      : 'nav-link' + (i === _mesActivo ? ' active' : ''),
                'href'       : '#',
                'data-mes'   : i,
                'text'       : MESES_CORTOS[i - 1]
            });

            var $li = $('<li>', { 'class': 'nav-item' }).append($a);
            $ul.append($li);
        }

        $('#bday-month-nav').empty().append($ul);

        /* Delegación de click */
        $('#bday-month-nav').on('click', '.nav-link', function (e) {
            e.preventDefault();
            var mes = parseInt($(this).data('mes'), 10);
            if (mes === _mesActivo) return;

            _mesActivo = mes;
            $('#bday-month-nav .nav-link').removeClass('active');
            $(this).addClass('active');
            _loadMes(mes);
        });
    }

    /* ---------- Carga de datos ---------- */

    function _loadMes(mes) {
        _showLoading();

        $.ajax({
            url      : _urlList,
            data     : { Mes: mes },
            dataType : 'json',
            success  : function (res) {
                var data = (res && Array.isArray(res.data))
                    ? res.data
                    : (Array.isArray(res) ? res : []);
                _renderSummary(data, mes);
                _renderCards(data);
            },
            error: function () {
                _renderSummary([], mes);
                _renderEmpty();
            }
        });
    }

    /* ---------- Banner de resumen ---------- */

    function _renderSummary(data, mes) {
        var total    = data.length;
        var hoyItems = data.filter(function (e) { return _diasRestantes(e.Per_FechaNacimiento) === 0; });
        var mesNombre = MESES[mes - 1];

        var html = '<div class="bday-summary">';
        html += '<div class="d-flex align-items-center flex-wrap">';

        /* Contador */
        html += '<div class="mr-4 mb-2">';
        html += '<div class="bday-summary-count">' + total + '</div>';
        html += '<div class="bday-summary-label">';
        html += (total === 1 ? 'empleado cumple' : 'empleados cumplen');
        html += ' a&ntilde;os en <strong>' + mesNombre + '</strong>';
        html += '</div>';
        html += '</div>';

        /* Badge "Hoy" si aplica */
        if (hoyItems.length > 0) {
            html += '<div class="mb-2">';
            html += '<span class="bday-today-alert">';
            html += '&#127874; ';
            if (hoyItems.length === 1) {
                var primerNombre = (hoyItems[0].NombreCompleto || '').split(' ')[0] || 'un empleado';
                html += '&iexcl;Hoy cumple a&ntilde;os ' + _escapeHtml(primerNombre) + '!';
            } else {
                html += '&iexcl;Hoy hay ' + hoyItems.length + ' cumplea&ntilde;os!';
            }
            html += '</span>';
            html += '</div>';
        }

        html += '</div>';
        html += '</div>';

        $('#bday-summary').html(html);
    }

    /* ---------- Grid de tarjetas ---------- */

    function _renderCards(data) {
        if (!data || data.length === 0) {
            _renderEmpty();
            return;
        }

        var html = '';

        for (var i = 0; i < data.length; i++) {
            var emp = data[i];
            html += _buildCard(emp);
        }

        $('#bday-grid').html(html);
    }

    function _buildCard(emp) {
        var colorIndex = (emp.Emp_Id || 0) % 7;
        var color      = AVATAR_COLORS[colorIndex];
        var dias       = _diasRestantes(emp.Per_FechaNacimiento);
        var isHoy      = dias === 0;
        var cardClass  = 'bday-card' + (isHoy ? ' bday-today' : '');

        /* Avatar */
        var avatarHtml;
        if (emp.Per_Imagen) {
            avatarHtml = '<img src="' + _escapeHtml(emp.Per_Imagen) + '" '
                + 'alt="' + _escapeHtml(emp.NombreCompleto) + '" '
                + 'class="bday-avatar-img" />';
        } else {
            avatarHtml = '<div class="bday-avatar" '
                + 'style="background-color:' + color + '">'
                + _initiales(emp.NombreCompleto)
                + '</div>';
        }

        /* Badge de días */
        var badgeHtml = _buildBadge(dias);

        /* Fecha formateada */
        var fechaStr = _formatFecha(emp.Per_FechaNacimiento);

        var html = '<div class="col-xl-3 col-lg-4 col-md-6 col-sm-12 mb-4">';
        html += '<div class="' + cardClass + '" '
            + 'style="--bday-color:' + color + '">';
        html += '<div class="bday-card-body">';

        /* Cabecera: avatar + nombre */
        html += '<div class="d-flex align-items-center mb-3">';
        html += avatarHtml;
        html += '<div class="ml-3">';
        html += '<p class="bday-card-name">' + _escapeHtml(emp.NombreCompleto) + '</p>';
        if (emp.NombreCargo) {
            html += '<p class="bday-card-role">' + _escapeHtml(emp.NombreCargo) + '</p>';
        }
        html += '</div>';
        html += '</div>'; /* d-flex */

        html += '<div class="bday-card-divider"></div>';

        /* Datos */
        html += '<div class="bday-card-meta">';

        html += '<div class="d-flex justify-content-between align-items-center mb-1">';
        html += '<span><i class="fa-solid fa-calendar-days mr-1"></i>' + fechaStr + '</span>';
        html += '<span class="font-weight-bold">' + (emp.Edad || '') + ' a&ntilde;os</span>';
        html += '</div>';

        if (emp.Per_Telefono) {
            html += '<div class="mb-1"><i class="fa-solid fa-phone mr-1"></i>'
                + _escapeHtml(emp.Per_Telefono) + '</div>';
        }

        if (emp.Per_CorreoElectronico) {
            html += '<div class="mb-2 text-truncate">'
                + '<i class="fa-solid fa-envelope mr-1"></i>'
                + _escapeHtml(emp.Per_CorreoElectronico)
                + '</div>';
        }

        html += '</div>'; /* bday-card-meta */

        /* Badge de días */
        html += '<div class="mt-2">' + badgeHtml + '</div>';

        html += '</div>'; /* bday-card-body */
        html += '</div>'; /* bday-card */
        html += '</div>'; /* col */

        return html;
    }

    function _buildBadge(diasRestantes) {
        if (diasRestantes === 0) {
            return '<span class="bday-badge bday-badge-today">&#127874; &iexcl;Hoy!</span>';
        }
        if (diasRestantes > 0 && diasRestantes <= 7) {
            return '<span class="bday-badge bday-badge-soon">En ' + diasRestantes + ' d&iacute;a' + (diasRestantes === 1 ? '' : 's') + '</span>';
        }
        if (diasRestantes > 7) {
            return '<span class="bday-badge bday-badge-later">En ' + diasRestantes + ' d&iacute;as</span>';
        }
        /* Negativo: ya pasó este mes */
        var abs = Math.abs(diasRestantes);
        return '<span class="bday-badge bday-badge-later">Hace ' + abs + ' d&iacute;a' + (abs === 1 ? '' : 's') + '</span>';
    }

    /* ---------- Estado vacío ---------- */

    function _renderEmpty() {
        var html = '<div class="col-12">';
        html += '<div class="bday-empty">';
        html += '<span class="bday-empty-icon"><i class="fa-solid fa-cake-candles"></i></span>';
        html += '<p class="bday-empty-text">No hay cumplea&ntilde;os en este mes.</p>';
        html += '</div>';
        html += '</div>';
        $('#bday-grid').html(html);
    }

    /* ---------- Estado de carga ---------- */

    function _showLoading() {
        var html = '<div class="col-12">';
        html += '<div class="bday-loading">';
        html += '<div class="spinner-border text-primary" role="status">';
        html += '<span class="sr-only">Cargando...</span>';
        html += '</div>';
        html += '<p class="mt-3">Cargando empleados...</p>';
        html += '</div>';
        html += '</div>';
        $('#bday-grid').html(html);
    }

    /* ---------- Helpers ---------- */

    function _initiales(nombreCompleto) {
        if (!nombreCompleto) return '?';
        var trimmed = nombreCompleto.trim();
        if (!trimmed) return '?';
        var partes = trimmed.split(/\s+/).filter(function (p) { return p.length > 0; });
        if (partes.length === 0) return '?';
        var primera = partes[0].charAt(0).toUpperCase();
        /* Usar el primer apellido (segunda palabra) como segunda inicial.
           Si solo hay una palabra, repetir la primera inicial.           */
        var segunda = partes.length > 1 ? partes[1].charAt(0).toUpperCase() : primera;
        return primera + segunda;
    }

    function _parseFecha(valor) {
        if (!valor) return null;
        /* Formato /Date(ms)/ que produce JavaScriptSerializer de ASP.NET MVC */
        var msMatch = /\/Date\((-?\d+)\)\//.exec(String(valor));
        if (msMatch) return new Date(parseInt(msMatch[1], 10));
        /* ISO 8601 u otros formatos estándar */
        var d = new Date(valor);
        return isNaN(d.getTime()) ? null : d;
    }

    /* Calcula días que faltan hasta el próximo cumpleaños este año.
       Devuelve 0 = hoy, positivo = faltan N días, negativo = ya pasó. */
    function _diasRestantes(fechaValor) {
        var nacimiento = _parseFecha(fechaValor);
        if (!nacimiento) return null;
        var hoy   = new Date();
        var bday  = new Date(hoy.getFullYear(), nacimiento.getMonth(), nacimiento.getDate());
        var diff  = Math.round((bday - new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate())) / 86400000);
        return diff;
    }

    function _formatFecha(fechaValor) {
        var d = _parseFecha(fechaValor);
        if (!d) return '—';
        var nombres = [
            'enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio',
            'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'
        ];
        return d.getDate() + ' de ' + nombres[d.getMonth()];
    }

    function _escapeHtml(str) {
        if (str == null) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    return obj;
}());
