//==================================================
// DataTable Partials - Para tablas con detalles en modal
//==================================================

var datatablePartials = (function () {
    var obj = {};

    /**
     * @param {Object} directions Objeto con todas las URLs del controlador
     * @param {any} header
     */
    obj.createDatatable = function (directions, header) {
        var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
        var table = $('#datatable').DataTable({
            responsive: true,
            deferRender: true,
            buttons: [
                {
                    text: '<i class="fa-solid fa-rotate-right"></i>',
                    titleAttr: 'Recargar tabla',
                    action: function (e, dt, config) {
                        dt.ajax.reload();
                    }
                },
                {
                    extend: "pdfHtml5",
                    title: "Exportar a PDF",
                    text: "<i class='mdi mdi-file-pdf-outline'></i> PDF",
                    class: "btn btn-secondary",
                    exportOptions: exportOptions
                },
                {
                    extend: "excelHtml5",
                    title: "Exportar a EXCEL",
                    text: "<i class='mdi mdi-file-excel-outline'></i> Excel",
                    class: "btn btn-secondary",
                    exportOptions: exportOptions
                },
                {
                    attr: {
                        title: "Añadir nuevo elemento",
                        id: "add-btn",
                        class: "btn btn-primary",
                        'data-style': "zoom-in",
                        'data-toggle': "modal",
                        'data-target': "#edit-modal"
                    },
                    text: '<i class="mdi mdi-plus-thick ladda-button"> Nuevo</i>'
                }
            ],
            ajax: function (data, callback, settings) {
                var url = directions.listUrl;
                // Si viene un id, agregarlo a la URL
                if (directions.id) {
                    url = url + "/" + directions.id;
                }
                $.ajax({
                    url: url,
                    type: "GET",
                    dataType: "json",
                    success: function (response) {
                        callback(response);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log('Error: ' + textStatus + ' - ' + errorThrown);
                        console.log('Detalles: ', jqXHR.responseText);
                    }
                });
            },
            columnDefs: obj.dataHeader(header)
        });
    }

    /**
     * Configura el datatable.
     */
    obj.config = function () {
        $.extend(true, $.fn.dataTable.defaults, {
            dom: "<'row m-2 my-3'<'col-md-7'f> <'col-md-5 d-flex justify-content-end custom-buttons'B>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'i><'col-sm-7'p>>",

            order: [],
            scrollCollapse: true,
            paging: true,
            stateSave: true,
            processing: true,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
            pageLenght: 10,
            displayLength: 10,
            language: {
                processing: '<div class="d-flex justify-content-center py-3"><i class="fa-solid fa-spinner fa-spin-pulse fa-3x" style="color: #F7A400;"></i></div>',
                lengthMenu: " _MENU_ ",
                zeroRecords: "No se encontraron resultados",
                emptyTable: "Ningún dato disponible en esta tabla",
                info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                infoFiltered: "(filtrado de un total de _MAX_ registros)",
                infoPostFix: "",
                search: '<i class="fa-solid fa-magnifying-glass"></i>',
                url: "",
                infoThousands: ",",
                loadingRecords: " ",
                searchPlaceholder: "Buscar en la tabla...",
                paginate: {
                    first: "Primero",
                    last: "Último",
                    next: "Siguiente",
                    previous: "Anterior"
                },
                aria: {
                    sortAscending: ": Activar para ordenar la columna de manera ascendente",
                    sortDescending: ": Activar para ordenar la columna de manera descendente"
                }
            }
        });
    }

    /**
     * Inicializa el datatable.
     * @param {Object} directions Objeto con todas las URLs del controlador (listUrl, detailsUrl, id, etc.)
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (directions, header) {
        obj.directions = directions || {};
        this.config();
        this.createDatatable(directions, header);
        this.customizeControls();
    };

    /**
     * Personaliza los controles del datatable
     */
    obj.customizeControls = function () {
        $(function () {
            $(".dt-buttons").removeClass("btn-group");
        });

        $('#datatable').on('init.dt', function () {
            $('#add-btn')
                .attr('data-toggle', 'modal')
                .attr('data-target', '#edit-modal');
        });
    };

    /**
     * Configura el header del DataTable
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     * @returns
     */
    obj.dataHeader = function (header) {
        var _header = header;
        var head = [];

        var i = 0;
        for (i; i < _header.length; i++) {
            head.push({
                targets: i,
                data: _header[i].FieldName
            })

            // Entra si se desea deshabilitar la columna
            if (_header[i].Visibility == false || _header[i].Visibility != undefined) {
                head[i]['visible'] = false
            }

            // Entra si se desea indicar un ancho especifico
            if (_header[i].Size != undefined) {
                head[i]['width'] = _header[i].Size
            }
        }

        head.push({
            targets: i,
            className: "text-center",
            width: 120,
            render: function (data, type, row) {
                var botones = "";
                var head = _header[0].FieldName;
                if (type == "display") {
                    // Boton Ver Detalles
                    botones += '<a href="javascript:void(0);" data-id="' + row[head] + '" class="bs-tooltip view-detail-btn text-muted pr-2" data-toggle="tooltip" data-placement="top" title="Ver detalles"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-eye p-1 br-6 mb-1"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg></a>';
                    // Boton Editar
                    botones += '<a href="javascript:void(0);" data-id="' + row[head] + '" class="bs-tooltip edit-btn text-muted pr-2" data-toggle="tooltip" data-placement="top" title="Editar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
                    // Boton Eliminar
                    botones += '<a href="javascript:void(0);" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '" class="bs-tooltip delete-btn text-muted pl-2" data-toggle="tooltip" data-placement="top" title="Eliminar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
                }
                return botones;
            }
        })
        return head;
    };

    return obj;
}());

// Configuracion global para Catalogs (detalles en modal)
var Catalogs = (function () {
    var obj = {};

    obj.configure = function (config) {
        obj.displayName = config.displayName || "";
        obj.getUrl = config.getUrl || "";
        obj.detailGetUrl = config.detailGetUrl || "";
        obj.detailModalId = config.detailModalId || "#detail-modal";
    };

    return obj;
}());

// Funcion global para ver detalles en modal
function viewDetailPartial(id) {
    // Verificar si datatablePartials.directions tiene detailsUrl configurado
    if (datatablePartials.directions && datatablePartials.directions.detailsUrl) {
        window.location.href = datatablePartials.directions.detailsUrl + '/' + id;
        return;
    }

    // Usar Catalogs para modal
    if (typeof Catalogs === 'undefined' || (!Catalogs.detailGetUrl && !Catalogs.getUrl)) {
        console.error('datatablePartials.directions.detailsUrl no está configurado y Catalogs tampoco.');
        return;
    }

    var detailModalId = Catalogs.detailModalId || '#detail-modal';

    if ($(detailModalId).length === 0) {
        console.error('Modal ' + detailModalId + ' no encontrado');
        return;
    }

    var getUrl = Catalogs.detailGetUrl || Catalogs.getUrl;

    $.ajax({
        url: getUrl,
        type: 'GET',
        data: { id: id },
        success: function(response) {
            if (response && response.data) {
                fillDetailModal(response.data);
                $(detailModalId).modal('show');
            } else {
                console.error('Respuesta sin datos:', response);
            }
        },
        error: function(xhr, status, error) {
            console.error('Error al cargar los detalles:', error);
        }
    });
}

// Funcion auxiliar para llenar el modal con los datos
function fillDetailModal(data) {
    for (var key in data) {
        if (data.hasOwnProperty(key)) {
            var elementId = '#detail-' + key.toLowerCase().replace(/_/g, '-');
            var $element = $(elementId);

            if ($element.length > 0) {
                var value = data[key];

                // Formatear valores especiales
                if (value === null || value === undefined) {
                    value = 'N/A';
                } else if (typeof value === 'number' && (key.toLowerCase().includes('precio') || key.toLowerCase().includes('monto'))) {
                    value = 'L. ' + parseFloat(value).toFixed(2);
                } else if (key.toLowerCase().includes('fecha') && value) {
                    value = new Date(value).toLocaleDateString('es-HN');
                }

                $element.text(value);
            }
        }
    }
}

// Event handler para el boton de ver detalles
$(document).on('click', '.view-detail-btn', function () {
    var id = $(this).data('id');
    viewDetailPartial(id);
});
