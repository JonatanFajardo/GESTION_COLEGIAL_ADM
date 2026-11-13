// Si la foto no carga, el avatar de CORK muestra las iniciales.
window.gcAvatarSinFoto = function (img) {
    img.outerHTML = '<span class="avatar-title rounded-circle">' + (img.getAttribute('data-ini') || '') + '</span>';
};

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
            if (window.tablaAlumnos) window.tablaAlumnos.replaceData();
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

/**
 * Listado de alumnos con Tabulator: columnas con ancho y orden ajustables,
 * columnas fijas (pin), densidad y todo guardado por usuario en el navegador.
 */
var datatableAlumnos = (function () {
    var obj = {};

    var DENSIDADES = ["comoda", "normal", "compacta"];

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

    function iniciales(nombre) {
        return (nombre || "").split(" ").filter(Boolean).slice(0, 2)
            .map(function (p) { return p.charAt(0); }).join("").toUpperCase();
    }

    function badgeEstado(estado) {
        var clase = { "Activo": "badge-success", "Suspendido": "badge-warning", "Graduado": "badge-primary" }[estado] || "badge-secondary";
        return '<span class="shadow-none badge ' + clase + '">' + (estado || "") + '</span>';
    }

    function botonesAccion(row, permisos) {
        var b = '<div class="d-flex justify-content-center align-items-center text-nowrap">';
        b += '<a href="javascript:void(0);" data-id="' + row.Alu_Id + '" class="bs-tooltip detail-btn text-muted pr-2" title="Ver detalles"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-eye p-1 br-6 mb-1"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg></a>';
        if (permisos.editar) {
            b += '<a href="javascript:void(0);" data-id="' + row.Alu_Id + '" class="bs-tooltip edit-btn text-muted pr-2" title="Editar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
        }
        if (permisos.eliminar) {
            b += '<span class="border-left mx-1"></span>';
            b += '<a href="javascript:void(0);" data-toggle="modal" data-target="#delete-modal" data-id="' + row.Alu_Id + '" class="bs-tooltip delete-btn text-muted pl-2" title="Eliminar"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
        }
        return b + '</div>';
    }

    obj.init = function (DirectionUrls) {
        $(function () {
            var permisos = DirectionUrls.permisos || { crear: true, editar: true, eliminar: true };
            var $contenedor = $("#tabla-alumnos");
            var claveUsuario = "gc-tabla-alumnos-" + ($contenedor.data("usuario") || "anonimo");

            // ── Columnas por defecto (el orden de este arreglo es el orden inicial) ──
            var COLUMNAS = {
                Alu_Nombre: {
                    title: "Alumno", field: "Alu_Nombre", minWidth: 200,
                    formatter: function (cell) {
                        var row = cell.getData(), nombre = cell.getValue() || "", ini = iniciales(nombre);
                        var avatar = row.Per_Imagen
                            ? '<img class="rounded-circle" src="' + row.Per_Imagen.replace("~/", "/") + '" alt="" data-ini="' + ini + '" onerror="gcAvatarSinFoto(this)">'
                            : '<span class="avatar-title rounded-circle">' + ini + '</span>';
                        return '<div class="d-flex align-items-center"><div class="avatar avatar-sm mr-2">' + avatar + '</div><span>' + nombre + '</span></div>';
                    }
                },
                Per_Identidad: { title: "Identidad", field: "Per_Identidad", minWidth: 130 },
                Edad: {
                    title: "Edad", field: "Edad", hozAlign: "center", width: 110, sorter: "number",
                    mutator: function (value, data) { return calcularEdad(data.Per_FechaNacimiento); }
                },
                Cno_Descripcion: { title: "Grado", field: "Cno_Descripcion", minWidth: 120 },
                Sec_Descripcion: { title: "Sección", field: "Sec_Descripcion", width: 125 },
                Est_Descripcion: {
                    title: "Estado", field: "Est_Descripcion", width: 135, hozAlign: "center",
                    formatter: function (cell) { return badgeEstado(cell.getValue()); }
                },
                Acciones: {
                    title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 140,
                    download: false,
                    formatter: function (cell) { return botonesAccion(cell.getData(), permisos); }
                }
            };
            var ORDEN_DEFECTO = Object.keys(COLUMNAS);

            // ── Estado guardado por usuario ──
            function estadoPorDefecto() {
                return { orden: ORDEN_DEFECTO.slice(), anchos: {}, fijas: [], densidad: "normal" };
            }
            function leerEstado() {
                try {
                    var e = JSON.parse(localStorage.getItem(claveUsuario) || "null");
                    if (!e || !Array.isArray(e.orden)) return estadoPorDefecto();
                    // Solo columnas que existen; las nuevas se agregan al final.
                    e.orden = e.orden.filter(function (f) { return COLUMNAS[f]; });
                    ORDEN_DEFECTO.forEach(function (f) { if (e.orden.indexOf(f) < 0) e.orden.push(f); });
                    e.fijas = (e.fijas || []).filter(function (f) { return COLUMNAS[f]; });
                    e.anchos = e.anchos || {};
                    if (DENSIDADES.indexOf(e.densidad) < 0) e.densidad = "normal";
                    return e;
                } catch (x) { return estadoPorDefecto(); }
            }
            function guardarEstado() {
                try { localStorage.setItem(claveUsuario, JSON.stringify(estado)); } catch (x) { }
            }
            var estado = leerEstado();

            // ── Botón de pin en cada encabezado ──
            function tituloConPin(cell) {
                var col = cell.getColumn(), campo = col.getField();
                var fija = estado.fijas.indexOf(campo) >= 0;
                var envoltura = document.createElement("span");
                envoltura.className = "d-inline-flex align-items-center";
                var texto = document.createElement("span");
                texto.textContent = col.getDefinition().title;
                var pin = document.createElement("button");
                pin.type = "button";
                pin.className = "btn btn-link btn-sm p-0 ml-2 tabla-pin" + (fija ? " text-primary" : " text-muted");
                pin.title = fija ? "Soltar columna" : "Fijar columna";
                pin.setAttribute("aria-label", pin.title);
                pin.innerHTML = '<i class="bi ' + (fija ? "bi-pin-angle-fill" : "bi-pin-angle") + '"></i>';
                pin.addEventListener("click", function (e) {
                    e.stopPropagation();
                    alternarFija(campo);
                });
                pin.addEventListener("mousedown", function (e) { e.stopPropagation(); });
                envoltura.appendChild(texto);
                envoltura.appendChild(pin);
                return envoltura;
            }

            // Arma las columnas según el estado: las fijas van primero (Tabulator las ancla a la izquierda).
            function construirColumnas() {
                var orden = estado.fijas.concat(estado.orden.filter(function (f) { return estado.fijas.indexOf(f) < 0; }));
                return orden.map(function (campo) {
                    var def = $.extend({}, COLUMNAS[campo]);
                    def.titleFormatter = tituloConPin;
                    def.frozen = estado.fijas.indexOf(campo) >= 0;
                    if (estado.anchos[campo]) def.width = estado.anchos[campo];
                    return def;
                });
            }

            function alternarFija(campo) {
                var i = estado.fijas.indexOf(campo);
                if (i >= 0) estado.fijas.splice(i, 1); else estado.fijas.push(campo);
                tomarOrdenYAnchos();
                guardarEstado();
                table.setColumns(construirColumnas());
            }

            function tomarOrdenYAnchos() {
                var orden = [];
                table.getColumns().forEach(function (c) {
                    var f = c.getField();
                    if (!COLUMNAS[f]) return;
                    orden.push(f);
                });
                estado.orden = orden;
            }

            // ── Densidad ──
            function aplicarDensidad(d) {
                DENSIDADES.forEach(function (x) { $contenedor.removeClass("tabla-densidad-" + x); });
                $contenedor.addClass("tabla-densidad-" + d);
                $("#alumnos-densidad").val(d);
            }
            aplicarDensidad(estado.densidad);

            // ── Tabla ──
            var table = new Tabulator("#tabla-alumnos", {
                ajaxURL: DirectionUrls.urlList,
                ajaxResponse: function (url, params, response) {
                    return (response && response.data) ? response.data : (response || []);
                },
                layout: "fitColumns",
                movableColumns: true,
                resizableColumnFit: false,
                columnDefaults: { resizable: true, headerHozAlign: "left", vertAlign: "middle" },
                columns: construirColumnas(),
                pagination: true,
                paginationSize: 20,
                paginationSizeSelector: [10, 20, 50, 100, true],
                paginationCounter: "rows",
                placeholder: "Ningún dato disponible en esta tabla",
                locale: "es",
                langs: {
                    es: {
                        data: { loading: "Cargando…", error: "No se pudo cargar la información" },
                        pagination: {
                            page_size: "Filas", page_title: "Mostrar página", first: "Primero", first_title: "Primera página",
                            last: "Último", last_title: "Última página", prev: "Anterior", prev_title: "Página anterior",
                            next: "Siguiente", next_title: "Página siguiente", all: "Todos",
                            counter: { showing: "Mostrando", of: "de", rows: "registros", pages: "páginas" }
                        }
                    }
                }
            });
            window.tablaAlumnos = table;

            table.on("columnMoved", function () {
                tomarOrdenYAnchos();
                // Si se arrastró una columna suelta entre las fijas, se vuelve a ordenar.
                var fijasAlInicio = estado.orden.slice(0, estado.fijas.length).every(function (f) { return estado.fijas.indexOf(f) >= 0; });
                guardarEstado();
                if (!fijasAlInicio) table.setColumns(construirColumnas());
            });
            table.on("columnResized", function (column) {
                estado.anchos[column.getField()] = Math.round(column.getWidth());
                guardarEstado();
            });

            // ── Barra: buscador, densidad, recargar, exportar, restablecer ──
            $("#alumnos-buscar").on("input", function () {
                var q = $(this).val().toString().trim().toLowerCase();
                if (!q) { table.clearFilter(); return; }
                table.setFilter(function (d) {
                    return [d.Alu_Nombre, d.Per_Identidad, d.Cno_Descripcion, d.Sec_Descripcion, d.Est_Descripcion, d.Edad]
                        .some(function (v) { return v !== null && v !== undefined && v.toString().toLowerCase().indexOf(q) >= 0; });
                });
            });
            $("#alumnos-densidad").on("change", function () {
                estado.densidad = $(this).val();
                aplicarDensidad(estado.densidad);
                guardarEstado();
                table.redraw(true);
            });
            $("#alumnos-recargar").on("click", function () { table.replaceData(); });
            $("#alumnos-excel").on("click", function () {
                table.download("xlsx", "alumnos.xlsx", { sheetName: "Alumnos" });
            });
            $("#alumnos-pdf").on("click", function () {
                table.download("pdf", "alumnos.pdf", { orientation: "landscape", title: "Listado de alumnos" });
            });
            $("#alumnos-restablecer").on("click", function () {
                try { localStorage.removeItem(claveUsuario); } catch (x) { }
                estado = estadoPorDefecto();
                aplicarDensidad(estado.densidad);
                table.setColumns(construirColumnas());
                table.redraw(true);
            });

            // ── Acciones por fila (mismo comportamiento que antes) ──
            $contenedor.on("click", ".detail-btn", function () {
                window.location = DirectionUrls.urlDetail + "/" + $(this).data("id");
            });
            $contenedor.on("click", ".edit-btn", function () {
                window.location = DirectionUrls.urlUpdate + "/" + $(this).data("id");
            });
            $contenedor.on("click", ".delete-btn-btn", function () {
                $("#delete-item-id").val($(this).data("id"));
                Modals.configure({ displayName: "empleado", type: "delete" });
            });
        });
    };

    return obj;
}());
