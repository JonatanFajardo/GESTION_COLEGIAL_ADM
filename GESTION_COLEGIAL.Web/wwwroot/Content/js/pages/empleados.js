var Empleados = (function () {
    var obj = {};

    var AVATAR_COLORS = [
        { bg: '#e0f2fe', text: '#0369a1' }, { bg: '#d1fae5', text: '#047857' },
        { bg: '#fef3c7', text: '#b45309' }, { bg: '#ede9fe', text: '#6d28d9' },
        { bg: '#fce7f3', text: '#be185d' }, { bg: '#dbeafe', text: '#1d4ed8' },
        { bg: '#d1fae5', text: '#065f46' }, { bg: '#fef9c3', text: '#a16207' },
        { bg: '#fee2e2', text: '#b91c1c' }, { bg: '#e0e7ff', text: '#4338ca' }
    ];

    function esActivo(v) {
        return v === true || v === 1 || v === "true" || v === "Activo" || v === "1";
    }

    obj.datatable = function (Direction) {
        var permisos = Direction.permisos || { crear: false, editar: false, eliminar: false };

        $(function () {
            var exportCols = [1, 2, 3, 4, 5]; // Código, Nombre, Estado, Título, Cargo (sin Id)

            var table = $('#datatable').DataTable({
                dom: "<'row m-2 my-3'<'col-md-7'><'col-md-5 d-flex justify-content-end custom-buttons'B>>" +
                     "<'row'<'col-sm-12'tr>>" +
                     "<'row'<'col-sm-5'i><'col-sm-7'p>>",
                order: [[2, "asc"]],
                deferRender: true,
                processing: true,
                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
                pageLength: 25,
                language: {
                    processing: '<div class="d-flex justify-content-center py-3"><i class="fa-solid fa-spinner fa-spin-pulse fa-3x" style="color:#F7A400"></i></div>',
                    zeroRecords: "No se encontraron resultados",
                    emptyTable: "Ningún dato disponible en esta tabla",
                    info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                    infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                    infoFiltered: "(filtrado de un total de _MAX_ registros)",
                    loadingRecords: " ",
                    paginate: { first: "Primero", last: "Último", next: "Siguiente", previous: "Anterior" }
                },
                buttons: [
                    {
                        text: '<i class="fas fa-sync-alt"></i>', titleAttr: 'Recargar', className: 'btn-reload',
                        action: function (e, dt) { dt.ajax.reload(); }
                    },
                    { extend: "pdfHtml5", title: "Listado de Empleados", text: "<i class='fas fa-file-pdf'></i> PDF", className: "btn btn-export-pdf", exportOptions: { columns: exportCols } },
                    { extend: "excelHtml5", title: "Listado de Empleados", text: "<i class='fas fa-file-excel'></i> Excel", className: "btn btn-export-excel", exportOptions: { columns: exportCols } }
                ],
                ajax: function (data, callback) {
                    $.ajax({
                        url: Direction.urlList, type: "GET", dataType: "json",
                        success: function (response) {
                            var rows = Array.isArray(response) ? response : (response.data || []);
                            callback({ data: rows });
                        },
                        error: function () { callback({ data: [] }); }
                    });
                },
                columnDefs: [
                    { targets: 0, data: "Emp_Id", visible: false },
                    { targets: 1, data: "Emp_Codigo" },
                    {
                        targets: 2, data: "Emp_Nombre",
                        render: function (data, type, row, meta) {
                            if (type !== "display") return data || "";
                            var name = data || "";
                            var initial = name.charAt(0).toUpperCase();
                            var av = AVATAR_COLORS[meta.row % AVATAR_COLORS.length];
                            return '<div style="display:flex;align-items:center;gap:10px;">' +
                                '<span style="display:inline-flex;align-items:center;justify-content:center;width:36px;height:36px;border-radius:50%;background:' + av.bg + ';color:' + av.text + ';font-weight:700;font-size:.875rem;flex-shrink:0;">' + initial + '</span>' +
                                '<span style="font-weight:600;color:#1e293b;">' + name + '</span></div>';
                        }
                    },
                    {
                        targets: 3, data: "EsActivo", className: "text-center",
                        render: function (data, type) {
                            var activo = esActivo(data);
                            if (type !== "display") return activo ? "Activo" : "Inactivo";
                            return activo
                                ? '<span class="badge-estado activo"><span class="dot-estado"></span>Activo</span>'
                                : '<span class="badge-estado inactivo"><span class="dot-estado"></span>Inactivo</span>';
                        }
                    },
                    { targets: 4, data: "Tit_Descripcion", render: function (d) { return d || "—"; } },
                    { targets: 5, data: "Car_Descripcion", render: function (d) { return d || "—"; } },
                    {
                        targets: 6, data: null, orderable: false, className: "text-center", width: 120,
                        render: function (data, type, row) {
                            if (type !== "display") return "";
                            var id = row["Emp_Id"];
                            var html = '<div class="d-flex justify-content-center align-items-center" style="white-space:nowrap;">';
                            html += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip detail-btn text-muted pr-2" title="Ver detalles"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-eye p-1 br-6 mb-1"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg></a>';
                            if (permisos.editar) {
                                html += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip edit-btn text-muted pr-2" title="Editar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
                            }
                            if (permisos.eliminar) {
                                html += '<span class="border-left mx-1" style="height:20px;"></span>';
                                html += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip delete-btn text-muted pl-2" title="Eliminar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
                            }
                            html += '</div>';
                            return html;
                        }
                    }
                ],
                drawCallback: function () {
                    var info = this.api().page.info();
                    $('#total-count').text(info.recordsTotal);
                }
            });

            // Poblar filtros de Cargo y Título con valores únicos al cargar los datos
            table.on('xhr', function () {
                var json = table.ajax.json();
                var rows = json && json.data ? json.data : [];
                llenarSelectUnico($('#filtro-cargo'), rows, 'Car_Descripcion');
                llenarSelectUnico($('#filtro-titulo'), rows, 'Tit_Descripcion');
            });

            function llenarSelectUnico($sel, rows, campo) {
                var actual = $sel.val();
                var vals = {};
                rows.forEach(function (r) { if (r[campo]) vals[r[campo]] = true; });
                var ordenados = Object.keys(vals).sort();
                $sel.find('option:not(:first)').remove();
                ordenados.forEach(function (v) { $sel.append('<option value="' + v + '">' + v + '</option>'); });
                if (actual) $sel.val(actual);
            }

            function exacto(val) { return val ? '^' + $.fn.dataTable.util.escapeRegex(val) + '$' : ''; }

            $('#custom-table-search').on('input', function () { table.search(this.value).draw(); });
            $('#filtro-estado').on('change', function () { table.column(3).search(exacto(this.value), true, false).draw(); });
            $('#filtro-cargo').on('change', function () { table.column(5).search(exacto(this.value), true, false).draw(); });
            $('#filtro-titulo').on('change', function () { table.column(4).search(exacto(this.value), true, false).draw(); });

            // Acciones
            table.on("click", ".detail-btn", function () { window.location = Direction.urlDetail + '/' + $(this).data("id"); });
            table.on("click", ".edit-btn", function () { window.location = Direction.urlUpdate + '/' + $(this).data("id"); });
            table.on("click", ".delete-btn", function () {
                $("#delete-item-id").val($(this).data("id"));
                Modals.configure({ displayName: "empleado", type: "delete" });
            });

            $(".dt-buttons").removeClass("btn-group");
        });
    };

    return obj;
}());
