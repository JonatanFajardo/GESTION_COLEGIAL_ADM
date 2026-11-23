

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




/* Cascada académica del formulario de alumnos: Nivel → Grado → Modalidad → Curso → Sección.
   Cada hijo se pide filtrado por su padre, arranca deshabilitado sin padre y se limpia al cambiar el padre. */
(function () {
    var CASCADA = [
        { padre: "#Niv_Id", hijo: "#Cun_Id", url: "/Alumnos/GetCursosNiveles", texto: "-- Seleccionar Curso --" },
        { padre: "#Cun_Id", hijo: "#Mda_Id", url: "/Alumnos/GetModalidades", texto: "-- Seleccionar Modalidad --" },
        { padre: "#Mda_Id", hijo: "#Cur_Id", url: "/Alumnos/GetCursos", texto: "-- Seleccionar Cursos --" },
        { padre: "#Cur_Id", hijo: "#Sec_Id", url: "/Alumnos/GetSecciones", texto: "-- Seleccionar Curso secciones --" }
    ];

    function vaciar(sel, texto) {
        $(sel).empty().append($("<option>").val("").text(texto)).val("").prop("disabled", true);
    }

    // Limpia todos los niveles debajo del paso i.
    function limpiarDesde(i) {
        for (var k = i; k < CASCADA.length; k++) vaciar(CASCADA[k].hijo, CASCADA[k].texto);
    }

    CASCADA.forEach(function (paso, i) {
        $(paso.padre).on("change", function () {
            var id = $(this).val();
            limpiarDesde(i);
            if (!id) return;
            $.ajax({ type: "GET", url: paso.url, data: { id: id }, dataType: "json" })
                .done(function (response) {
                    var lista = (response && response.data) ? response.data : [];
                    var $hijo = $(paso.hijo).empty().append($("<option>").val("").text(paso.texto));
                    $.each(lista, function (_, row) { $hijo.append($("<option>").val(row.Value).text(row.Text)); });
                    $hijo.val("").prop("disabled", lista.length === 0);
                })
                .fail(function () { alert("No se pudieron cargar las opciones. Intente de nuevo."); });
        });
    });

    // Al abrir el formulario, los hijos sin opciones (alumno nuevo) quedan deshabilitados.
    $(function () {
        CASCADA.forEach(function (paso) {
            if ($(paso.hijo).length && $(paso.hijo).find("option[value!='']").length === 0) $(paso.hijo).prop("disabled", true);
        });
        // Deshabilitados no se envían: al guardar se vuelven a habilitar.
        $("form[action*='Guardar']").on("submit", function () {
            CASCADA.forEach(function (paso) { $(paso.hijo).prop("disabled", false); });
        });
    });
}());

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
