//==================================================
//Recorre el arreglo de encabezados
//==================================================

var datatable = (function () {
    var obj = {};
    //serverSide



    /**
     * Inicializa y configura el DataTable 
     * @param {Object} listUrl Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (listUrl, header) {
        $(function () {


            
            //$("input").removeClass(".form-control-sm")
            //configuraciones
            $.extend(true, $.fn.dataTable.defaults, {
            
                dom: "<'row datatable-header'<'col-sm-7'f><'col-sm-5'B>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-5'i><'col-sm-7'p>>",

                order: [],
                scrollCollapse: true,
                paging: true,
                stateSave: true,
                //bLengthChange: false,
                //bInfo: false,
                processing: true,
                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
                pageLenght: 10,
                displayLength: 10,
                language: {
                    processing: "Procesando...",
                    lengthMenu: " _MENU_ ",
                    zeroRecords: "No se encontraron resultados",
                    emptyTable: "Ningún dato disponible en esta tabla",
                    info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                    infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                    infoFiltered: "(filtrado de un total de _MAX_ registros)",
                    infoPostFix: "",
                    search: "",
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




            var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
            var table = $('#datatable').DataTable({
                //serverSide: true,
                buttons: [
                    //{
                    //text: '<i class="mdi mdi-refresh"> Recargar</i>',
                    //titleAttr: 'Recargar tabla',
                    //action: function (e, dt, config) {
                    //    dt.ajax.reload();
                    //}
                    //},
                    {
                        extend: "collection",
                        text: '<i class="mdi mdi-export"> Exportar</i>',
                        titleAttr: 'Exportar esta tabla',
                        buttons: [{
                            extend: "excelHtml5",
                            text: '<i class="mdi mdi-file-excel"> Excel</i>',
                            exportOptions: exportOptions
                        },
                        {
                            extend: "csvHtml5",
                            text: '<i class="mdi mdi-file-excel-outline"> CSV</i>',
                            exportOptions: exportOptions
                        },
                        {
                            extend: "pdfHtml5",
                            text: '<i class="mdi mdi-file-pdf"> CSV</i>',
                            exportOptions: exportOptions
                        }
                        ]
                    },
                    {
                        text: '<i class="mdi mdi-plus-box-outline" > Nuevo</i>',
                        attr: {
                            tittle: "Añadir nuevo elemento",
                            onclick: "obj.RedirectNew"
                        }
                    }
                ],
                ajax: function (data, callback, settings) {
                    $.ajax({
                        url: listUrl,
                        type: "GET",
                        dataType: "json",
                        success: function (response) {
                            callback(response);
                        },
                    });
                },
                columnDefs: obj.dataHeader(header)

            });







        });
    };


    //obj.RedirectNew = function (tabla) {
    //    $(function () {
    //        window.location = '/' + tabla + '/Agregar';

    //    })
    //}


    /**
     * Configura el header del DataTable
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     * @returns 
     */
    obj.dataHeader = function (header) {
        var _header = header;
        head = [];
        var i = 0;
        for (i; i < _header.length; i++) {

            head.push({
                targets: i,
                data: _header[i]
            })
        }

        head.push({
            targets: i,
            className: "text-center",
            width: 80,
            render: function (data, type, row) {
                botones = "";
                var head = _header[0];
                if (type == "display") {
                    botones += '<button class="btn btn-secondary btn-sm" onclick=RedirectEdit(' + row[head] + ')><i class="mdi mdi-square-edit-outline"></i></button>';
                    botones += '<button class="btn btn-danger btn-sm ml-1" onclick="getIdDelete(' + row[head] + ')"><i class="ion-trash-a"></i></button>';
                    //botones += '<button class="btn btn-secondary btn-sm" href="/Usuarios/Editar/1"><i class="mdi mdi-square-edit-outline"></i></button>';

                }
                return botones;
            }
        })
        return head;
    };



    return obj;
}());








function RedirectEdit(params) {
    window.location = '/' + tabla + '/Edit/' + params + '';
}



//appetsConfig.Catalogs = (function () {
//    var obj = {};
//    //url de donde sacare la data.
//    obj.configureTable = function (params) {
//        $(function () {
//            var exportOptions = { columns: [0, 1, 2], orthogonal: "export" }, canEdit, canDelete;
//            var table = $('#datatable').DataTable({
//                buttons: [
//                    {
//                        text: '<i class="mdi mdi- refresh"> Recargar</i>',
//                        titleAttr: 'Regargar tabla',
//                        action: function (e, dt, node, config) {
//                            dt.ajax.reload();
//                        }
//                    },
//                    {
//                        extend: "collection",
//                        text: "<i class='mdi mdi-export'></i> Exportar",
//                        titleAttr: "Exportar esta tabla",
//                        buttons: [
//                            {
//                                extend: "excelHtml5",
//                                text: "<i class='mdi mdi-file-excel-outline'></i> Excel",
//                                exportOptions: exportOptions
//                            },
//                            {
//                                extend: "csvHtml5",
//                                text: "<i class='mdi mdi-file-multiple-outline'></i> CSV",
//                                exportOptions: exportOptions
//                            },
//                            {
//                                extend: "pdfHtml5",
//                                text: "<i class='mdi mdi-file-pdf-outline'></i> PDF",
//                                exportOptions: exportOptions
//                            }
//                        ]
//                    },
//                    {
//                        text: '<i class="mdi mdi-plus-thick" id="add-btn" data-toggle="modal" data-target="#edit-modal"> Nuevo</i>',
//                        attr: {
//                            title: "Añadir nueva especie",
//                            id: "add-btn"
//                        }
//                    }
//                ],
//                ajax: function (data, callback, settings) {
//                    $.ajax({
//                        url: params.listUrl,
//                        type: "GET",
//                        dataType: "json",
//                        success: function (response) {
//                            callback(response);
//                            table.column(-1).visible(true);
//                        },
//                    });
//                },
//                columnDefs: [
//                    {
//                        targets: 0,
//                        data: 'espc_Id'
//                    },
//                    {
//                        targets: 1,
//                        data: 'espc_Descripcion'
//                    },
//                    {
//                        targets: 2,
//                        className: "text-center",
//                        width: 80,
//                        render: function (data, type, row) {
//                            botones = "";
//                            if (type == "display") {
//                                botones += '<button class="btn btn-secondary btn-sm edit-btn ladda-button" data-style="zoom-in" data-id="' + row.espc_Id + '"><span class"ladda-label"><i class="mdi mdi-square-edit-outline"></i></span></button>';
//                                //botones += '<button class="btn btn-danger btn-sm" onclick="EliminarRole(' + row.comp_Id + ')"><i class="mdi mdi-trash-can"></i></button>';
//                            }
//                            return botones;
//                        }
//                    }
//                ]
//            });
//        });
//        $('#datatable').on('init.dt', function () {
//            $('#add-btn')
//                .attr('data-toggle', 'modal')
//                .attr('data-target', '#edit-modal');
//        });
//    };

//    return obj;
//}());