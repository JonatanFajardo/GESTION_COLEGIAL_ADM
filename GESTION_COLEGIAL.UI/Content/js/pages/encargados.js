var Encargados = (function () {
    var obj = {};

    var ID_COLORS = [
        { bg: '#fff7ed', text: '#ea580c', border: '#fed7aa' },
        { bg: '#f5f3ff', text: '#7c3aed', border: '#ddd6fe' },
        { bg: '#eff6ff', text: '#2563eb', border: '#bfdbfe' },
        { bg: '#f0fdf4', text: '#16a34a', border: '#bbf7d0' },
        { bg: '#fdf4ff', text: '#9333ea', border: '#e9d5ff' },
        { bg: '#fef2f2', text: '#dc2626', border: '#fecaca' },
        { bg: '#ecfdf5', text: '#059669', border: '#a7f3d0' },
        { bg: '#fefce8', text: '#ca8a04', border: '#fef08a' },
        { bg: '#e0f2fe', text: '#0369a1', border: '#7dd3fc' },
        { bg: '#fce7f3', text: '#be185d', border: '#f9a8d4' }
    ];

    var AVATAR_COLORS = [
        { bg: '#e0f2fe', text: '#0369a1' },
        { bg: '#d1fae5', text: '#047857' },
        { bg: '#fef3c7', text: '#b45309' },
        { bg: '#ede9fe', text: '#6d28d9' },
        { bg: '#fce7f3', text: '#be185d' },
        { bg: '#dbeafe', text: '#1d4ed8' },
        { bg: '#d1fae5', text: '#065f46' },
        { bg: '#fef9c3', text: '#a16207' },
        { bg: '#fee2e2', text: '#b91c1c' },
        { bg: '#e0e7ff', text: '#4338ca' }
    ];

    obj.datatable = function (Direction) {
        $(function () {
            var tableInstance = $('#datatable').DataTable({
                dom: "<'row'<'col-sm-12'tr>><'row dt-footer-enc'<'col-sm-5'i><'col-sm-7'p>>",
                order: [],
                paging: true,
                processing: true,
                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
                displayLength: 25,
                language: {
                    processing: '<div class="d-flex justify-content-center py-3"><i class="fa-solid fa-spinner fa-spin-pulse fa-3x" style="color:#F7A400"></i></div>',
                    zeroRecords: "No se encontraron resultados",
                    emptyTable: "Ningún dato disponible en esta tabla",
                    info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                    infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                    infoFiltered: "(filtrado de un total de _MAX_ registros)",
                    infoThousands: ",",
                    loadingRecords: " ",
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
                },
                buttons: [
                    {
                        extend: "pdfHtml5",
                        title: "Listado de Encargados",
                        exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
                    },
                    {
                        extend: "excelHtml5",
                        title: "Listado de Encargados",
                        exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
                    }
                ],
                drawCallback: function () {
                    var info = this.api().page.info();
                    $('#total-count').text(info.recordsTotal);
                },
                ajax: function (data, callback) {
                    $.ajax({
                        url: Direction.urlList,
                        type: "GET",
                        dataType: "json",
                        success: function (response) {
                            var rows = Array.isArray(response) ? response : (response.data || []);
                            callback({ data: rows });
                        },
                        error: function () {
                            callback({ data: [] });
                        }
                    });
                },
                columnDefs: [
                    {
                        targets: 0,
                        data: "Enc_Id",
                        render: function (data, type, row, meta) {
                            if (type !== "display") return data;
                            var c = ID_COLORS[meta.row % ID_COLORS.length];
                            return '<span style="display:inline-flex;align-items:center;justify-content:center;min-width:34px;height:34px;padding:0 8px;border-radius:50%;background:' + c.bg + ';color:' + c.text + ';border:1px solid ' + c.border + ';font-weight:700;font-size:0.875rem;">' + data + '</span>';
                        }
                    },
                    {
                        targets: 1,
                        data: "Per_Identidad"
                    },
                    {
                        targets: 2,
                        data: "Enc_Nombre",
                        render: function (data, type, row, meta) {
                            if (type !== "display") return data;
                            var name = data || '';
                            var initial = name.charAt(0).toUpperCase();
                            var av = AVATAR_COLORS[meta.row % AVATAR_COLORS.length];
                            return '<div style="display:flex;align-items:center;gap:10px;">' +
                                '<span style="display:inline-flex;align-items:center;justify-content:center;width:36px;height:36px;border-radius:50%;background:' + av.bg + ';color:' + av.text + ';font-weight:700;font-size:0.875rem;flex-shrink:0;">' + initial + '</span>' +
                                '<span style="font-weight:600;color:#1e293b;">' + name + '</span>' +
                                '</div>';
                        }
                    },
                    {
                        targets: 3,
                        data: "Per_Telefono"
                    },
                    {
                        targets: 4,
                        data: "Enc_Ocupacion",
                        render: function (data) {
                            return '<span style="color:#94a3b8;">' + (data || '—') + '</span>';
                        }
                    },
                    {
                        targets: 5,
                        data: "EsActivo",
                        render: function (data) {
                            var isActive = data === true || data === 1 || data === "true" || data === "Activo";
                            return isActive
                                ? '<span class="badge-estado-enc activo-enc"><span class="dot-enc"></span>Activo</span>'
                                : '<span class="badge-estado-enc inactivo-enc"><span class="dot-enc"></span>Inactivo</span>';
                        }
                    },
                    {
                        targets: 6,
                        data: null,
                        orderable: false,
                        className: "text-center",
                        render: function (data, type, row) {
                            if (type !== "display") return '';
                            var id = row["Enc_Id"];
                            return '<div class="d-flex justify-content-center align-items-center" style="white-space:nowrap;">' +
                                '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip detail-btn text-muted pr-2" data-toggle="tooltip" data-placement="top" title="Ver detalles"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-eye p-1 br-6 mb-1"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg></a>' +
                                '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip edit-btn text-muted pr-2" data-toggle="tooltip" data-placement="top" title="Editar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>' +
                                '<span class="border-left mx-1" style="height:20px;"></span>' +
                                '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip delete-btn text-muted pl-2" data-toggle="tooltip" data-placement="top" title="Eliminar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>' +
                                '</div>';
                        }
                    }
                ]
            });

            // Buscador personalizado
            $('#custom-table-search').on('input', function () {
                tableInstance.search($(this).val()).draw();
            });

            // Botones de exportación
            $('#btn-export-pdf').on('click', function () {
                tableInstance.button(0).trigger();
            });
            $('#btn-export-excel').on('click', function () {
                tableInstance.button(1).trigger();
            });

            // Acciones de filas
            tableInstance.on("click", ".detail-btn", function () {
                window.location = Direction.urlDetail + '/' + $(this).data("id");
            });

            tableInstance.on("click", ".edit-btn", function () {
                window.location = Direction.urlUpdate + '/' + $(this).data("id");
            });

            tableInstance.on("click", ".delete-btn", function () {
                $("#delete-item-id").val($(this).data("id"));
                Modals.configure({ displayName: "encargado", type: "delete" });
            });

            // Quitar agrupación de botones de DataTables
            $(".dt-buttons").removeClass("btn-group");
        });
    };

    return obj;
}());
