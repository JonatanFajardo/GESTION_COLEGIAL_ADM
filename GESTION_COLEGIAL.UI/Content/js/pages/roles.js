var Roles = (function () {
    var obj = {};
    var _table = null;

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = [
                { FieldName: "Rol_Id", Size: 80 },
                { FieldName: "Rol_Descripcion", Size: 200 },
                {
                    FieldName: "CantidadPantallas", Size: 120,
                    Render: function (data) {
                        return '<span class="badge badge-info">' + data + ' pantallas</span>';
                    }
                },
                {
                    FieldName: "Rol_Estado", Size: 100,
                    Render: function (data) {
                        return data
                            ? '<span class="badge badge-success">Activo</span>'
                            : '<span class="badge badge-danger">Inactivo</span>';
                    }
                }
            ];

            // Construir columnDefs a partir del header
            var columnDefs = [];
            var i = 0;
            for (i; i < header.length; i++) {
                var col = { targets: i, data: header[i].FieldName };
                if (header[i].Size !== undefined) col.width = header[i].Size;
                if (header[i].Render !== undefined) {
                    col.render = header[i].Render;
                    col.className = 'text-center';
                }
                if (header[i].Visibility === false) col.visible = false;
                columnDefs.push(col);
            }

            // Columna de acciones con 4 botones: Detalle, Editar, Pantallas, Eliminar
            columnDefs.push({
                targets: i,
                className: "text-center",
                width: 180,
                render: function (data, type, row) {
                    if (type !== "display") return "";
                    var id = row[header[0].FieldName];
                    var botones = "";

                    // Boton Ver Detalles
                    botones += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip view-detail-btn text-muted pr-2" data-toggle="tooltip" data-placement="top" title="Ver detalles">';
                    botones += '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-eye p-1 br-6 mb-1"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg></a>';

                    // Boton Editar
                    botones += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip edit-btn text-muted pr-2" data-toggle="tooltip" data-placement="top" title="Editar">';
                    botones += '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';

                    // Boton Asignar Pantallas
                    botones += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip btn-pantallas pr-2" data-toggle="tooltip" data-placement="top" title="Asignar pantallas" style="color: #2196F3;">';
                    botones += '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-shield p-1 br-6 mb-1"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path></svg></a>';

                    // Boton Eliminar
                    botones += '<a href="javascript:void(0);" data-toggle="modal" data-target="#delete-modal" data-id="' + id + '" class="bs-tooltip delete-btn text-muted pl-2" data-toggle="tooltip" data-placement="top" title="Eliminar">';
                    botones += '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';

                    return botones;
                }
            });

            // Configuraciones del DataTable
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
                    emptyTable: "Ningun dato disponible en esta tabla",
                    info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                    infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                    infoFiltered: "(filtrado de un total de _MAX_ registros)",
                    infoPostFix: "",
                    search: "",
                    url: "",
                    infoThousands: ",",
                    loadingRecords: " ",
                    searchPlaceholder: "Buscar en la tabla...",
                    paginate: { first: "Primero", last: "Ultimo", next: "Siguiente", previous: "Anterior" },
                    aria: {
                        sortAscending: ": Activar para ordenar la columna de manera ascendente",
                        sortDescending: ": Activar para ordenar la columna de manera descendente"
                    }
                }
            });

            var exportOptions = { columns: [0, 1, 2, 3], orthogonal: "export" };

            _table = $('#datatable').DataTable({
                responsive: true,
                deferRender: true,
                buttons: [
                    {
                        title: "Recargar",
                        text: '<i class="fas fa-sync-alt"></i>',
                        titleAttr: 'Recargar datos de la tabla',
                        className: 'btn-reload',
                        action: function (e, dt) { dt.ajax.reload(); }
                    },
                    {
                        extend: "pdfHtml5",
                        title: "Exportar a PDF",
                        text: "<i class='fas fa-file-pdf'></i> PDF",
                        titleAttr: 'Exportar tabla a formato PDF',
                        className: "btn btn-export-pdf",
                        exportOptions: exportOptions
                    },
                    {
                        extend: "excelHtml5",
                        title: "Exportar a EXCEL",
                        text: "<i class='fas fa-file-excel'></i> Excel",
                        titleAttr: 'Exportar tabla a formato Excel',
                        className: "btn btn-export-excel",
                        exportOptions: exportOptions
                    },
                    {
                        attr: {
                            title: "Nuevo rol",
                            id: "add-btn",
                            class: "btn btn-primary btn-add-new",
                            'data-style': "zoom-in",
                            'data-toggle': "modal",
                            'data-target': "#edit-modal"
                        },
                        text: '<i class="fas fa-plus-circle"></i> Nuevo'
                    }
                ],
                ajax: function (data, callback) {
                    $.ajax({
                        url: Direction.urlList,
                        type: "GET",
                        dataType: "json",
                        success: function (response) { callback(response); },
                        error: function () { console.log('Error al cargar roles'); }
                    });
                },
                columnDefs: columnDefs
            });

            // Boton Nuevo del header tambien abre el modal
            $('#datatable').on('init.dt', function () {
                $('#add-btn').attr('data-toggle', 'modal').attr('data-target', '#edit-modal');
            });

            $(".dt-buttons").removeClass("btn-group");
        });
    };

    obj.getTable = function () {
        return _table;
    };

    return obj;
}());
