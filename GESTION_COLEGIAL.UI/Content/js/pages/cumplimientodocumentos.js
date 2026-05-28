/* ========================================
   CUMPLIMIENTO DE DOCUMENTOS DE EMPLEADOS
   Módulo: CumplimientoDocumentos
   ======================================== */
var CumplimientoDocumentos = (function () {

    var _urlList     = '';
    var _urlDropdown = '';

    var AVATAR_COLORS = [
        '#3a7bd5', '#00b09b', '#f7971e',
        '#8e44ad', '#e74c3c', '#16a085', '#d35400'
    ];

    /* --------------------------------------------------
       INIT
    -------------------------------------------------- */
    function init(urlList, urlDropdown) {
        _urlList     = urlList;
        _urlDropdown = urlDropdown;

        _cargarDropdown();
        cargarDatos(null);

        $('#btn-filtrar').on('click', function () {
            var empId = $('#filter-empleado').val();
            cargarDatos(empId || null);
        });
    }

    /* --------------------------------------------------
       DROPDOWN DE EMPLEADOS
    -------------------------------------------------- */
    function _cargarDropdown() {
        $.ajax({
            url  : _urlDropdown,
            type : 'GET',
            success: function (data) {
                var $sel = $('#filter-empleado');
                $sel.find('option:not(:first)').remove();
                if (Array.isArray(data)) {
                    $.each(data, function (_, item) {
                        $sel.append(
                            $('<option>').val(item.id).text(item.texto)
                        );
                    });
                }
            },
            error: function () {
                console.warn('CumplimientoDocumentos: no se pudo cargar el dropdown de empleados.');
            }
        });
    }

    /* --------------------------------------------------
       CARGAR DATOS
    -------------------------------------------------- */
    function cargarDatos(empId) {
        var $grid = $('#cdoc-grid');
        $grid.html(
            '<div class="cdoc-loading col-12">' +
                '<span class="spinner-border spinner-border-sm text-secondary"></span>' +
                '<span>Cargando datos...</span>' +
            '</div>'
        );

        var params = {};
        if (empId) {
            params.Emp_Id = empId;
        }

        $.ajax({
            url    : _urlList,
            type   : 'GET',
            data   : params,
            success: function (response) {
                var docs = [];
                if (response && Array.isArray(response.data)) {
                    docs = response.data;
                } else if (Array.isArray(response)) {
                    docs = response;
                }

                var empleadosArr = _agruparPorEmpleado(docs);
                renderStats(docs, empleadosArr.length);
                renderCards(empleadosArr);
            },
            error: function () {
                $grid.html(
                    '<div class="cdoc-empty-state col-12">' +
                        '<i class="fa-solid fa-circle-exclamation"></i>' +
                        '<p>Error al cargar los datos. Intente nuevamente.</p>' +
                    '</div>'
                );
            }
        });
    }

    /* --------------------------------------------------
       AGRUPAR FILAS POR EMPLEADO
    -------------------------------------------------- */
    function _agruparPorEmpleado(docs) {
        var map   = {};
        var order = [];

        $.each(docs, function (_, row) {
            var id = row.Emp_Id;
            if (!map[id]) {
                map[id] = {
                    Emp_Id        : row.Emp_Id,
                    Emp_Codigo    : row.Emp_Codigo    || '',
                    NombreCompleto: row.NombreCompleto || '—',
                    Per_Identidad : row.Per_Identidad  || '',
                    documentos    : []
                };
                order.push(id);
            }
            map[id].documentos.push(row);
        });

        var result = [];
        $.each(order, function (_, id) {
            result.push(map[id]);
        });
        return result;
    }

    /* --------------------------------------------------
       RENDER STATS
    -------------------------------------------------- */
    function renderStats(docs, totalEmpleados) {
        var total      = docs.length;
        var entregados = 0;

        $.each(docs, function (_, d) {
            if (d.EsEntregado === true || d.EsEntregado === 1) {
                entregados++;
            }
        });

        var pendientes = total - entregados;
        var pct        = total > 0 ? Math.round((entregados / total) * 100) : 0;

        /* Porcentaje global */
        $('#cdoc-stat-pct').text(pct + '%');
        $('#cdoc-stat-progress')
            .attr('aria-valuenow', pct)
            .css('width', pct + '%')
            .removeClass('bg-success bg-warning bg-danger')
            .addClass(_progressClass(pct));

        /* Contadores */
        $('#cdoc-stat-entregados').text(entregados);
        $('#cdoc-stat-pendientes').text(pendientes);

        /* Subtítulo con número de empleados y total de documentos */
        var empCount = totalEmpleados || 0;
        $('#cdoc-stat-sub-global').text(
            empCount + ' empleado' + (empCount !== 1 ? 's' : '') +
            ', ' + total + ' documento' + (total !== 1 ? 's' : '') + ' en total'
        );
    }

    function _progressClass(pct) {
        if (pct >= 100) return 'bg-success';
        if (pct > 50)   return 'bg-warning';
        return 'bg-danger';
    }

    /* --------------------------------------------------
       RENDER CARDS
    -------------------------------------------------- */
    function renderCards(empleadosArr) {
        var $grid = $('#cdoc-grid');
        $grid.empty();

        if (!empleadosArr || empleadosArr.length === 0) {
            $grid.html(
                '<div class="cdoc-empty-state col-12">' +
                    '<i class="fa-solid fa-folder-open"></i>' +
                    '<p>No se encontraron registros de documentos.</p>' +
                '</div>'
            );
            return;
        }

        $.each(empleadosArr, function (_, emp) {
            var $col = $('<div>').addClass('col-xl-4 col-lg-6 col-md-12 mb-4');
            $col.html(_buildCard(emp));
            $grid.append($col);
        });
    }

    /* --------------------------------------------------
       CONSTRUIR UNA CARD DE EMPLEADO
    -------------------------------------------------- */
    function _buildCard(emp) {
        var docs       = emp.documentos;
        var total      = docs.length;
        var entregados = 0;

        $.each(docs, function (_, d) {
            if (d.EsEntregado === true || d.EsEntregado === 1) {
                entregados++;
            }
        });

        var pct              = total > 0 ? Math.round((entregados / total) * 100) : 0;
        var statusClass      = pct >= 100 ? 'status-full' : (pct > 50 ? 'status-partial' : 'status-low');
        var progressBarClass = pct >= 100 ? 'bg-success'  : (pct > 50 ? 'bg-warning'      : 'bg-danger');
        var avatarClass      = 'avatar-' + (emp.Emp_Id % 7);
        var initials         = _initiales(emp.NombreCompleto);

        /* Lista de documentos */
        var docItems = '';
        $.each(docs, function (_, d) {
            var esEntregado = (d.EsEntregado === true || d.EsEntregado === 1);
            var iconClass   = esEntregado ? 'fa-circle-check icon-ok' : 'fa-circle-xmark icon-pending';
            var badgeHtml   = '';

            if (d.EsObligatorio === true || d.EsObligatorio === 1) {
                var badgeType = esEntregado ? 'oblig-entregado' : 'oblig-pendiente';
                badgeHtml = '<span class="cdoc-badge-oblig ' + badgeType + '">Obligatorio</span>';
            }

            var fechaHtml = '';
            if (d.FechaEntrega) {
                fechaHtml = '<span class="cdoc-doc-fecha">' + _escHtml(_formatFecha(d.FechaEntrega)) + '</span>';
            }

            var titleAttr = d.Observacion
                ? ' title="' + _escHtml(d.Observacion) + '"'
                : '';

            docItems +=
                '<div class="cdoc-doc-item"' + titleAttr + '>' +
                    '<span class="cdoc-doc-icon fa-solid ' + iconClass + '"></span>' +
                    '<div class="cdoc-doc-body">' +
                        '<p class="cdoc-doc-name">' + _escHtml(d.Documento || 'Sin nombre') + '</p>' +
                        '<div class="cdoc-doc-meta">' + badgeHtml + fechaHtml + '</div>' +
                    '</div>' +
                '</div>';
        });

        return (
            '<div class="cdoc-emp-card ' + statusClass + '">' +
                '<div class="cdoc-emp-card-header">' +
                    '<div class="cdoc-emp-card-top">' +
                        '<div class="cdoc-avatar ' + avatarClass + '">' + _escHtml(initials) + '</div>' +
                        '<div class="cdoc-emp-info">' +
                            '<p class="cdoc-emp-name">' + _escHtml(emp.NombreCompleto) + '</p>' +
                            '<p class="cdoc-emp-code">' +
                                _escHtml(emp.Emp_Codigo) +
                                (emp.Per_Identidad ? ' &nbsp;·&nbsp; ' + _escHtml(emp.Per_Identidad) : '') +
                            '</p>' +
                        '</div>' +
                    '</div>' +
                    '<div class="cdoc-progress-bar-wrap">' +
                        '<div class="cdoc-progress-label">' +
                            '<span>' + entregados + ' de ' + total + ' entregados</span>' +
                            '<span class="pct-text">' + pct + '%</span>' +
                        '</div>' +
                        '<div class="progress" style="height:7px;">' +
                            '<div class="progress-bar ' + progressBarClass + '" role="progressbar" ' +
                                'aria-valuenow="' + pct + '" aria-valuemin="0" aria-valuemax="100" ' +
                                'style="width:' + pct + '%;">' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
                '<div class="cdoc-doc-list">' + docItems + '</div>' +
            '</div>'
        );
    }

    /* --------------------------------------------------
       UTILIDADES
    -------------------------------------------------- */

    /**
     * Devuelve las iniciales: primera letra del primer nombre +
     * primera letra del primer apellido.
     */
    function _initiales(nombre) {
        if (!nombre) return '?';
        var partes = nombre.trim().split(/\s+/);
        if (partes.length === 1) return partes[0].charAt(0).toUpperCase();
        return (partes[0].charAt(0) + partes[1].charAt(0)).toUpperCase();
    }

    /** Formatea fecha ISO → DD/MM/YYYY */
    function _formatFecha(fechaStr) {
        if (!fechaStr) return '';
        try {
            var d = new Date(fechaStr);
            if (isNaN(d.getTime())) return fechaStr;
            var dd   = String(d.getDate()).padStart(2, '0');
            var mm   = String(d.getMonth() + 1).padStart(2, '0');
            var yyyy = d.getFullYear();
            return dd + '/' + mm + '/' + yyyy;
        } catch (e) {
            return fechaStr;
        }
    }

    /** Escapa caracteres HTML para evitar XSS en innerHTML */
    function _escHtml(str) {
        if (str == null) return '';
        return String(str)
            .replace(/&/g,  '&amp;')
            .replace(/</g,  '&lt;')
            .replace(/>/g,  '&gt;')
            .replace(/"/g,  '&quot;')
            .replace(/'/g,  '&#39;');
    }

    /* --------------------------------------------------
       API PÚBLICA
    -------------------------------------------------- */
    return {
        init       : init,
        cargarDatos: cargarDatos,
        renderStats: renderStats,
        renderCards: renderCards
    };

}());
