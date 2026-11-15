

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
            $(params.deleteModalId + " .modal-footer .btn-danger").html("<i class='bi bi-check-lg mr-1'></i> " + deleteBtnText);
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
            GcTabla.recargar();
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

/** Listado de alumnos: tabla personalizable por usuario (ver components/gc-tabla.js). */
var datatableAlumnos = (function () {
    var obj = {};

    function calcularEdad(fechaNac) {
        if (!fechaNac) return "";
        var fecha = new Date(fechaNac);
        // La API manda 0001-01-01 cuando no trae la fecha: no inventar una edad.
        if (isNaN(fecha.getTime()) || fecha.getFullYear() < 1900) return "—";
        var hoy = new Date();
        var edad = hoy.getFullYear() - fecha.getFullYear();
        var m = hoy.getMonth() - fecha.getMonth();
        if (m < 0 || (m === 0 && hoy.getDate() < fecha.getDate())) edad--;
        return edad >= 0 ? edad : "";
    }

    var BADGE_ESTADO = { "Activo": "badge-success", "Suspendido": "badge-warning", "Graduado": "badge-primary" };

    obj.init = function (DirectionUrls) {
        $(function () {
            var permisos = DirectionUrls.permisos || { crear: true, editar: true, eliminar: true };

            GcTabla.crear({
                elemento: "#tabla-alumnos",
                nombre: "alumnos",
                url: DirectionUrls.urlList,
                archivo: "alumnos",
                tituloPdf: "Listado de alumnos",
                agrupar: [
                    { campos: ["Cno_Descripcion"], texto: "Grado" },
                    { campos: ["Cno_Descripcion", "Sec_Descripcion"], texto: "Grado → Sección" },
                    { campos: ["Est_Descripcion"], texto: "Estado" }
                ],
                buscarEn: ["Alu_Nombre", "Per_Identidad", "Edad", "Cno_Descripcion", "Sec_Descripcion", "Est_Descripcion"],
                columnas: [
                    { title: "Alumno", field: "Alu_Nombre", minWidth: 190,
                      formatter: function (c) { return GcTabla.avatar(c.getValue(), c.getData().Per_Imagen); } },
                    { title: "Identidad", field: "Per_Identidad", minWidth: 130 },
                    { title: "Edad", field: "Edad", hozAlign: "center", width: 110, sorter: "number",
                      mutator: function (v, d) { return calcularEdad(d.Per_FechaNacimiento); } },
                    { title: "Grado", field: "Cno_Descripcion", minWidth: 120 },
                    { title: "Sección", field: "Sec_Descripcion", width: 125 },
                    { title: "Estado", field: "Est_Descripcion", width: 135, hozAlign: "center",
                      formatter: function (c) {
                          return '<span class="shadow-none badge ' + (BADGE_ESTADO[c.getValue()] || "badge-secondary") + '">' + (c.getValue() || "") + '</span>';
                      } },
                    { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 140, download: false,
                      formatter: function (c) { return GcTabla.acciones(c.getData().Alu_Id, permisos, 'data-toggle="modal" data-target="#delete-modal"'); } }
                ]
            });

            // Acciones por fila (mismo comportamiento que antes)
            var $t = $("#tabla-alumnos");
            $t.on("click", ".detail-btn", function () { window.location = DirectionUrls.urlDetail + "/" + $(this).data("id"); });
            $t.on("click", ".edit-btn", function () { window.location = DirectionUrls.urlUpdate + "/" + $(this).data("id"); });
            $t.on("click", ".delete-btn-btn", function () {
                $("#delete-item-id").val($(this).data("id"));
                Modals.configure({ displayName: "empleado", type: "delete" });
            });
        });
    };

    return obj;
}());
