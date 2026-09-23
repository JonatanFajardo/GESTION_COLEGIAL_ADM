/* ========================================
   CUMPLIMIENTO DE DOCUMENTOS DE EMPLEADOS
   Módulo: CumplimientoDocumentos
   ======================================== */
var CumplimientoDocumentos = (function () {

    var _urlList     = '';
    var _urlDropdown = '';
    var _urlMarcar   = '';
    var _docsIndex   = {};   // key "empId||documento" -> fila

    var AVATAR_COLORS = [
        '#3a7bd5', '#00b09b', '#f7971e',
        '#8e44ad', '#e74c3c', '#16a085', '#d35400'
    ];

    /* --------------------------------------------------
       INIT
    -------------------------------------------------- */
    function init(urlList, urlDropdown, urlMarcar) {
        _urlList     = urlList;
        _urlDropdown = urlDropdown;
        _urlMarcar   = urlMarcar || '';

        _cargarDropdown();
        cargarDatos(null);

        $('#btn-filtrar').on('click', function () {
            var empId = $('#filter-empleado').val();
            cargarDatos(empId || null);
        });

        // Abrir modal al hacer clic en un documento (en cualquiera de las dos pestañas)
        $('#cdoc-grid-incompletos, #cdoc-grid-completos').on('click', '.cdoc-doc-item', function () {
            _abrirModal($(this).attr('data-key'));
        });

        // Cambio de pestaña Incompletos / Completos
        $('#cdoc-tabs').on('click', '.cdoc-tab-item', function () {
            _activarTab($(this).attr('data-tab'));
        });

        // Guardar
        $('#cdoc-form').on('submit', function (e) {
            e.preventDefault();
            _guardar();
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
        // Vuelve a la pestaña principal (Incompletos) y muestra el spinner ahí.
        _activarTab('incompletos');
        $('#cdoc-grid-completos').empty();
        $('#cdoc-grid-incompletos').html(
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

                // Indexar filas para el modal
                _docsIndex = {};
                $.each(docs, function (_, r) {
                    _docsIndex[r.Emp_Id + '||' + r.Documento] = r;
                });

                var empleadosArr = _agruparPorEmpleado(docs);
                renderStats(docs, empleadosArr.length);
                renderCards(empleadosArr);
            },
            error: function () {
                $('#cdoc-grid-incompletos').html(
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
    /**
     * Un documento se considera ENTREGADO cuando tiene fecha de entrega.
     * (Bug #6: se eliminó el switch; el estado se deriva de la presencia de la fecha.)
     */
    function _esEntregado(d) {
        return !!(d && d.FechaEntrega);
    }

    function renderStats(docs, totalEmpleados) {
        var total      = docs.length;
        var entregados = 0;

        $.each(docs, function (_, d) {
            if (_esEntregado(d)) {
                entregados++;
            }
        });

        var pendientes = total - entregados;
        var pct        = total > 0 ? Math.round((entregados / total) * 100) : 0;

        /* Vencimientos */
        var vencidos = 0, porVencer = 0;
        $.each(docs, function (_, d) {
            if (d.EstadoVencimiento === 'vencido')     vencidos++;
            else if (d.EstadoVencimiento === 'por-vencer') porVencer++;
        });
        $('#cdoc-stat-vencidos').text(vencidos);
        $('#cdoc-stat-porvencer').text(porVencer);

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
        var $inc = $('#cdoc-grid-incompletos').empty();
        var $com = $('#cdoc-grid-completos').empty();

        var nInc = 0, nCom = 0;

        if (empleadosArr && empleadosArr.length) {
            $.each(empleadosArr, function (_, emp) {
                var completo = _empCompleto(emp);
                var $col = $('<div>').addClass('col-xl-4 col-lg-6 col-md-12 mb-4');
                $col.html(_buildCard(emp));
                if (completo) { $com.append($col); nCom++; }
                else          { $inc.append($col); nInc++; }
            });
        }

        // Estados vacíos por pestaña
        if (nInc === 0) {
            $inc.html(
                '<div class="cdoc-empty-state col-12">' +
                    '<i class="fa-solid fa-circle-check"></i>' +
                    '<p>No hay empleados con documentación pendiente.</p>' +
                '</div>'
            );
        }
        if (nCom === 0) {
            $com.html(
                '<div class="cdoc-empty-state col-12">' +
                    '<i class="fa-solid fa-folder-open"></i>' +
                    '<p>Aún no hay empleados con documentación completa.</p>' +
                '</div>'
            );
        }

        $('#cdoc-count-incompletos').text(nInc);
        $('#cdoc-count-completos').text(nCom);
    }

    /** Empleado completo = 100% de sus documentos entregados (y tiene al menos uno). */
    function _empCompleto(emp) {
        var docs = emp.documentos || [];
        if (docs.length === 0) return false;
        for (var i = 0; i < docs.length; i++) {
            if (!_esEntregado(docs[i])) return false;
        }
        return true;
    }

    /** Alterna la pestaña activa Incompletos / Completos. */
    function _activarTab(tab) {
        var esCompletos = (tab === 'completos');
        $('#cdoc-tabs .cdoc-tab-item').removeClass('active')
            .filter('[data-tab="' + tab + '"]').addClass('active');
        $('#cdoc-grid-incompletos').prop('hidden', esCompletos);
        $('#cdoc-grid-completos').prop('hidden', !esCompletos);
    }

    /* --------------------------------------------------
       CONSTRUIR UNA CARD DE EMPLEADO
    -------------------------------------------------- */
    function _buildCard(emp) {
        var docs       = emp.documentos;
        var total      = docs.length;
        var entregados = 0;

        $.each(docs, function (_, d) {
            if (_esEntregado(d)) {
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
            var esEntregado = _esEntregado(d);
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

            // Badge de vencimiento
            var vencHtml = '';
            if (d.EstadoVencimiento === 'vencido') {
                vencHtml = '<span class="cdoc-venc-badge cdoc-venc-vencido">Vencido</span>';
            } else if (d.EstadoVencimiento === 'por-vencer') {
                vencHtml = '<span class="cdoc-venc-badge cdoc-venc-por-vencer">Por vencer' +
                           (d.Doc_FechaVencimiento ? ' · ' + _escHtml(_formatFecha(d.Doc_FechaVencimiento)) : '') + '</span>';
            } else if (d.EstadoVencimiento === 'vigente' && d.Doc_FechaVencimiento) {
                vencHtml = '<span class="cdoc-venc-badge cdoc-venc-vigente">Vence ' + _escHtml(_formatFecha(d.Doc_FechaVencimiento)) + '</span>';
            }

            var titleAttr = d.Observacion
                ? ' title="' + _escHtml(d.Observacion) + '"'
                : ' title="Clic para registrar entrega o vencimiento"';

            var key = d.Emp_Id + '||' + (d.Documento || '');

            docItems +=
                '<div class="cdoc-doc-item" data-key="' + _escHtml(key) + '"' + titleAttr + '>' +
                    '<span class="cdoc-doc-icon fa-solid ' + iconClass + '"></span>' +
                    '<div class="cdoc-doc-body">' +
                        '<p class="cdoc-doc-name">' + _escHtml(d.Documento || 'Sin nombre') + '</p>' +
                        '<div class="cdoc-doc-meta">' + badgeHtml + fechaHtml + vencHtml + '</div>' +
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
       MODAL — MARCAR ENTREGADO / VENCIMIENTO
    -------------------------------------------------- */
    function _fechaInput(val) {
        if (!val) return '';
        var d = new Date(val);
        if (isNaN(d.getTime())) return '';
        return d.getFullYear() + '-' +
               String(d.getMonth() + 1).padStart(2, '0') + '-' +
               String(d.getDate()).padStart(2, '0');
    }

    function _abrirModal(key) {
        var d = _docsIndex[key];
        if (!d) return;

        $('#cdoc-form-error').hide().text('');
        $('#cdoc-emp-id').val(d.Emp_Id);
        $('#cdoc-documento').val(d.Documento || '');
        $('#cdoc-modal-title').text(d.Documento || 'Documento');
        $('#cdoc-modal-emp').text(d.NombreCompleto || '');
        $('#cdoc-fecha-entrega').val(_fechaInput(d.FechaEntrega));
        $('#cdoc-fecha-vencimiento').val(_fechaInput(d.Doc_FechaVencimiento));
        $('#cdoc-observacion').val(d.Observacion || '');
        $('#cdoc-modal').modal('show');
    }

    function _guardar() {
        var fechaEntrega = $('#cdoc-fecha-entrega').val();
        // Bug #6: el estado "entregado" se deriva de la presencia de la fecha de entrega.
        var entregado = !!fechaEntrega;

        var payload = {
            Emp_Id: $('#cdoc-emp-id').val(),
            Documento: $('#cdoc-documento').val(),
            EsEntregado: entregado,
            FechaEntrega: fechaEntrega || '',
            Doc_FechaVencimiento: $('#cdoc-fecha-vencimiento').val(),
            Observacion: $('#cdoc-observacion').val()
        };

        $.post(_urlMarcar, payload, function (res) {
            if (res && res.success) {
                $('#cdoc-modal').modal('hide');
                var empId = $('#filter-empleado').val();
                cargarDatos(empId || null);
            } else {
                $('#cdoc-form-error').text('No se pudo guardar. Intente de nuevo.').show();
            }
        }).fail(function () {
            $('#cdoc-form-error').text('Error de conexión al guardar.').show();
        });
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
