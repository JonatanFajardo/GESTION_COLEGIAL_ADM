﻿
var Alumnos = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {




            //console.log(DataTableConfig);
            datatableAlumnos.init(Direction);
        })
    }
    return obj;
}());




$("#Niv_Id").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetCursosNiveles/',
        data: data,
        dataType: "json",
        success: function (response) {
            if (response != null) {
                FillDropDown("#Cun_Id", response.data);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});

$("#Cun_Id").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetModalidades/',
        data: data,
        dataType: "json",
        success: function (response) {
            if (response != null) {
                FillDropDown("#Mda_Id", response.data);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});

$("#Mda_Id").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetCursos/',
        data: data,
        dataType: "json",
        success: function (response) {
            if (response != null) {
                FillDropDown("#Cur_Id", response.data);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});

$("#Cur_Id").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetSecciones/',
        data: data,
        dataType: "json",
        success: function (response) {
            if (response != null) {
                console.log(response);
                FillDropDown("#Sec_Id", response.data);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});
$(function () {
});

/**
 * Permite ingresar data a un dropdown.
 * @param {any} dropDownId
 * @param {any} list El primer valor debe de ser el valor y el segundo el texto a mostrar.
 */
function FillDropDown(dropDownId, list) {
    $(dropDownId).empty();
    console.log(list);
    $.each(list, function (index, row) {
        console.log(row);
        //console.log(row[]);
        //if (index != 0) {
        //    $(dropDownId).append("<option value='" + list["data"][0] + "' selected='selected'>" + Text + "</option>");
        //} else {
        //    $(dropDownId).append("<option value='" + Value + "'>" + Text + "</option>")
        //}
        if (index != 0) {
            $(dropDownId).append("<option value='" + row.Value + "' selected='selected'>" + row.Text + "</option>");
        } else {
            $(dropDownId).append("<option value='" + row.Value + "'>" + row.Text + "</option>")
        }
    });
}



        //Modal y mas
//==================================================
//Recorre el arreglo de encabezados
//==================================================

var Modals = (function () {
    var obj = {},
        action,
        table,
        $deleteModal;

    table = $("#datatable");

    function deleteModal(params) {
        $deleteModal = $(params.deleteModalId);
        $deleteModal.on("show.bs.modal", function () {
            var modalTitle = "Eliminar ", deleteBtnText = "Aceptar";
            $(params.deleteModalId + " .modal-title").html(modalTitle + params.displayName);
            $(params.deleteModalId + " .modal-footer .btn-danger").html("<i class='mdi mdi-content-save'></i> " + deleteBtnText);
        });
        $deleteModal.on("hidden.bs.modal", function () { });
    }

    function typeModal(params) {
        switch (params.type) {
            case 'delete':
                $(function () {
                    deleteModal(params);
                    $deleteModal.modal("show");
                });
                break;
        }
    }

    obj.configure = function (params) {
        console.log(params);
        if (params.editModalId === undefined)
            params.editModalId = "#edit-modal";

        if (params.deleteModalId === undefined)
            params.deleteModalId = "#delete-modal-secondary";

        $(function () {
            typeModal(params);
        });
    };

    obj.begin = function (xhr, settings) {
        if (action == "edit") {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        else {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        submitBtn.start();
    };

    obj.success = function (data, status, xhr) {
        if (data.success) {
            //$editModal.modal("hide");
            $deleteModal.modal("hide");
            table.DataTable().ajax.reload(null, false);
        }
        else {
            //$editModal.modal("hide");
            $deleteModal.modal("hide");
            alertConfig.alert("Ocurrió  un error.", data.error);
        }
        alertConfig.alert(data.message, data.type);
    };

    obj.failure = function (xhr, status, error) {
        console.log("Ocurrió  un error.");
    }

    obj.complete = function (xhr, status) {
        submitBtn.stop();
    };
    return obj;
}());

var datatableAlumnos = (function () {
    var obj = {};

    /**
     * Inicializa y configura el DataTable
     * @param {Object} DirectionUrls Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (DirectionUrls) {
        $(function () {
            //configuraciones
            $.extend(true, $.fn.dataTable.defaults, {
                dom: "<'row m-2 my-3'<'col-md-7'f> <'col-md-5 d-flex justify-content-end custom-buttons'B>>" +
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
                pageLenght: 20,
                displayLength: 20,
                language: {
                    processing: '<div class="d-flex justify-content-center py-3"><i class="fa-solid fa-spinner fa-spin-pulse fa-3x" style="color: #F7A400;"></i></div>',
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
                responsive: false,
                deferRender: true,
                //serverSide: true,
                buttons: [
                    {
                        text: '<i class="fa-solid fa-rotate-right"></i>',
                        titleAttr: 'Recargar tabla',
                        action: function (e, dt, config) {
                            dt.ajax.reload();
                        }
                    },
                    //{
                    //    title: "Exportar a CSV",
                    //    extend: "csvHtml5",
                    //    text: "<i class='mdi mdi-file-multiple-outline'></i> CSV",
                    //    className: "btn-secondary",
                    //    exportOptions: exportOptions
                    //},
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
                        text: '<span><i class="mdi mdi-plus-thick ladda-button"> Nuevo</i></span>'
                    }
                ],
                ajax: function (data, callback, settings) {
                    $.ajax({
                        url: DirectionUrls.urlList,
                        type: "GET",
                        dataType: "json",
                        success: function (response) {
                            console.log(response);
                            callback(response);
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            console.log('Error: ' + textStatus + ' - ' + errorThrown);
                            console.log('Detalles: ', jqXHR.responseText);
                        }
                    });
                },
                columns: [
                    {
                        data: "Per_Imagen",
                        render: function (data, type, row) {
                            const imageUrl = data ? data.replace('~/', '/') : '';
                            const fullImageUrl = imageUrl ? window.location.origin + imageUrl : '/Content/js/pages/default-profile.jpeg';
                            return `<img src="${fullImageUrl}" alt="Imagen" style="width: 50px; height: 50px; border-radius: 5px;" 
        onerror="this.onerror=null; this.src='/path/to/placeholder.png';">`;
                        },
                        targets: 1,
                        
                    },
                    {
                        data: "Alu_Nombre",
                        targets: 2,
                    },
                    {
                        data: "Per_Identidad",
                        targets: 3,
                    },
                    {
                        data: "Cno_Descripcion",
                        targets: 4,
                    },
                    {
                        data: "Mda_Descripcion",
                        targets: 5,
                    },
                    {
                        data: "Niv_Descripcion",
                        targets: 5,
                    },
                    {
                        data: "Sec_Descripcion",
                        targets: 5,
                    },
                    {
                        data: "AnioCursado",
                        targets: 6,
                    },

                    {
                        data: "Est_Descripcion",
                        render: function (data, type, row) {
                            // Determinar el color basado en el estado usando switch
                            let colorClass = '';
                            switch (data) {
                                case 'Activo':
                                    colorClass = 'shadow-none badge badge-success'; // Color verde
                                    break;
                                case 'Suspendido':
                                    colorClass = 'shadow-none badge badge-warning'; // Color rojo
                                    break;
                                case 'Graduado':
                                    colorClass = 'shadow-none badge badge-primary'; // Color rojo
                                    break;
                               
                            }

                            return `<span class="${colorClass}">${data}</span>`;
                        },
                        targets: 7,
                       
                    },
                    {
                        data: null, // No hay datos reales para esta columna
                        defaultContent: "",
                        orderable: false, 
                        className: "text-center",
                        width: 80,
                        render: function (data, type, row) {
                            botones = "";
                            if (type == "display") {
                                //botones += '<button class="btn btn-secondary btn-sm edit-btn ladda-button" data-style="zoom-in" data-id="' + row[head] + '"><span class"ladda-label"><i class="mdi mdi-square-edit-outline"></i></span></button>';
                                //botones += '<button class="btn btn-danger btn-sm ml-1 delete-btn-btn ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '"><span class"ladda-label"><i class="ion-trash-a"></i></span></button>';
                          
                                botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-id="' + row["Alu_Id"] + '" class="bs-tooltip edit-btn text-muted pr-2" data-toggle="tooltip" data-placement="top" title="" data-original-title="Edit"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
                                botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row["Alu_Id"] + '" class="bs-tooltip delete-btn text-muted pl-2" data-toggle="tooltip" data-placement="top" title="" data-original-title="Delete"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
                            }
                            return botones;
                        },
                        targets: 8,
                    }

                ]
            });

            // Evento click que Redirecciona.
            // Obtiene el id seleccionado en el boton, Redirecciona a la vista de editar.
            table.on("click", ".edit-btn", function (e) {
                var getIdEdit = $(this).data("id");
                window.location = `${DirectionUrls.urlUpdate}/${getIdEdit}`;
            });

            table.on("click", ".delete-btn-btn", function (e) {
                var getIdDelete = $(this).data("id");
                console.log(`getIdDelete: ${getIdDelete}`);
                $("#delete-item-id").val(getIdDelete);
                Modals.configure({
                    displayName: "empleado",
                    type: "delete"
                });
            });

            // Evento click de clase .add-btn.
            // Redirecciona a la vista de crear.
            var addBtn = $('#add-btn');
            addBtn.click(function () {
                window.location = `${DirectionUrls.urlInsert}`;
            });
        });

        //Eliminamos la agrupaciond de los botones.
        $(function () {
            $(".dt-buttons").removeClass("btn-group");
        });
    };

    return obj;
}());