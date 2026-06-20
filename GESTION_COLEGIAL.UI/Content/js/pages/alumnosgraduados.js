/* Modulo: Directorio de alumnos graduados.
 * DataTable con busqueda + modal de ficha individual.
 */
var AlumnosGraduados = (function () {
    var cfg = null;
    var dt = null;

    function init(config) {
        cfg = config || {};
        $(document).ready(function () { build(); });
    }

    function build() {
        dt = $('#tbl-graduados').DataTable({
            ajax: {
                url: cfg.listUrl,
                dataSrc: function (resp) { return (resp && resp.data) ? resp.data : []; }
            },
            columns: [
                { data: 'AluGrad_Identidad' },
                { data: 'AluGrad_NombreCompleto' },
                { data: 'AluGrad_NivelEducativo', defaultContent: '' },
                { data: 'AluGrad_TituloObtenido', defaultContent: '' },
                { data: 'AluGrad_AnioGraduacion' },
                {
                    data: 'AluGrad_TotalPagado',
                    render: function (v) {
                        var n = (v === null || v === undefined) ? 0 : Number(v);
                        return 'L. ' + n.toFixed(2);
                    }
                },
                {
                    data: 'AluGrad_Id', orderable: false, searchable: false,
                    render: function (id) {
                        return '<button class="btn btn-sm btn-info btn-ver-graduado" data-id="' + id + '">'
                            + '<i class="bi bi-eye"></i> Ver</button>';
                    }
                }
            ],
            pageLength: 25,
            order: [[4, 'desc']],
            language: dataTableEspanol()
        });

        $('#tbl-graduados tbody').on('click', '.btn-ver-graduado', function () {
            var id = parseInt($(this).data('id'), 10);
            verDetalle(id);
        });
    }

    function verDetalle(id) {
        $('#modal-graduado-body').html('Cargando...');
        $('#modal-graduado-detalle').modal('show');
        $.getJSON(cfg.findUrl, { id: id }, function (resp) {
            var d = (resp && resp.data) ? resp.data : null;
            if (!d) {
                $('#modal-graduado-body').html('<div class="alert alert-warning">No se encontró el registro.</div>');
                return;
            }
            var html = ''
                + '<dl class="row">'
                + linea('Identidad', d.AluGrad_Identidad)
                + linea('Nombre', d.AluGrad_NombreCompleto)
                + linea('Sexo', d.AluGrad_Sexo)
                + linea('Fecha de nacimiento', formatoFecha(d.AluGrad_FechaNacimiento))
                + linea('Correo', d.AluGrad_Correo)
                + linea('Teléfono', d.AluGrad_Telefono)
                + linea('Nivel educativo', d.AluGrad_NivelEducativo)
                + linea('Título obtenido', d.AluGrad_TituloObtenido)
                + linea('Año ingreso', d.AluGrad_AnioIngreso)
                + linea('Año graduación', d.AluGrad_AnioGraduacion)
                + linea('Total pagado', 'L. ' + Number(d.AluGrad_TotalPagado || 0).toFixed(2))
                + linea('Archivado el', formatoFecha(d.AluGrad_FechaArchivo))
                + '</dl>';
            if (d.AluGrad_EncargadosJson) {
                html += '<h6>Encargados</h6><pre class="small">' + d.AluGrad_EncargadosJson + '</pre>';
            }
            $('#modal-graduado-body').html(html);
        });
    }

    function linea(label, val) {
        return '<dt class="col-sm-4">' + label + '</dt><dd class="col-sm-8">' + (val || '—') + '</dd>';
    }
    function formatoFecha(s) {
        if (!s) return '';
        try { return new Date(s).toLocaleDateString('es-HN'); } catch (e) { return s; }
    }

    function dataTableEspanol() {
        return {
            sProcessing: 'Procesando...',
            sLengthMenu: 'Mostrar _MENU_ registros',
            sZeroRecords: 'No se encontraron resultados',
            sEmptyTable: 'Ningún dato disponible',
            sInfo: '_START_ a _END_ de _TOTAL_',
            sInfoEmpty: '0 a 0 de 0',
            sInfoFiltered: '(filtrado de _MAX_)',
            sSearch: 'Buscar:',
            oPaginate: { sFirst: 'Primero', sLast: 'Último', sNext: 'Siguiente', sPrevious: 'Anterior' }
        };
    }

    return { init: init };
})();
