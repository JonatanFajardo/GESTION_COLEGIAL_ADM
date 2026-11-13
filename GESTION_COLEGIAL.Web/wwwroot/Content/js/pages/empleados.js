/** Listado de empleados: tabla personalizable por usuario (ver components/gc-tabla.js). */
var Empleados = (function () {
    var obj = {};

    function esActivo(v) {
        return v === true || v === 1 || v === "true" || v === "Activo" || v === "1";
    }

    obj.datatable = function (Direction) {
        var permisos = Direction.permisos || { crear: false, editar: false, eliminar: false };

        $(function () {
            var $estado = $("#filtro-estado"), $cargo = $("#filtro-cargo"), $titulo = $("#filtro-titulo");

            var t = GcTabla.crear({
                elemento: "#tabla-empleados",
                nombre: "empleados",
                url: Direction.urlList,
                archivo: "empleados",
                tituloPdf: "Listado de empleados",
                filasPorPagina: 25,
                buscador: "#custom-table-search",
                buscarEn: ["Emp_Codigo", "Emp_Nombre", "Estado", "Tit_Descripcion", "Car_Descripcion"],
                filtroExtra: function (d) {
                    return (!$estado.val() || d.Estado === $estado.val()) &&
                        (!$cargo.val() || d.Car_Descripcion === $cargo.val()) &&
                        (!$titulo.val() || d.Tit_Descripcion === $titulo.val());
                },
                alCargar: function (filas) {
                    $("#total-count").text(filas.length);
                    llenarSelectUnico($cargo, filas, "Car_Descripcion");
                    llenarSelectUnico($titulo, filas, "Tit_Descripcion");
                },
                columnas: [
                    { title: "Código", field: "Emp_Codigo", width: 120 },
                    { title: "Nombre", field: "Emp_Nombre", minWidth: 220,
                      formatter: function (c) { return GcTabla.avatar(c.getValue(), c.getData().Per_Imagen); } },
                    { title: "Estado", field: "Estado", width: 130, hozAlign: "center",
                      mutator: function (v, d) { return esActivo(d.EsActivo) ? "Activo" : "Inactivo"; },
                      formatter: function (c) {
                          return c.getValue() === "Activo"
                              ? '<span class="badge badge-success">Activo</span>'
                              : '<span class="badge badge-secondary">Inactivo</span>';
                      } },
                    { title: "Título", field: "Tit_Descripcion", minWidth: 140,
                      formatter: function (c) { return c.getValue() || "—"; } },
                    { title: "Cargo", field: "Car_Descripcion", minWidth: 140,
                      formatter: function (c) { return c.getValue() || "—"; } },
                    { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 140, download: false,
                      formatter: function (c) { return GcTabla.acciones(c.getData().Emp_Id, permisos); } }
                ]
            });

            function llenarSelectUnico($sel, filas, campo) {
                var actual = $sel.val(), vals = {};
                filas.forEach(function (r) { if (r[campo]) vals[r[campo]] = true; });
                $sel.find("option:not(:first)").remove();
                Object.keys(vals).sort().forEach(function (v) { $sel.append($("<option>").val(v).text(v)); });
                if (actual) $sel.val(actual);
            }

            $estado.add($cargo).add($titulo).on("change", t.filtrar);

            // Acciones
            var $t = $("#tabla-empleados");
            $t.on("click", ".detail-btn", function () { window.location = Direction.urlDetail + "/" + $(this).data("id"); });
            $t.on("click", ".edit-btn", function () { window.location = Direction.urlUpdate + "/" + $(this).data("id"); });
            $t.on("click", ".delete-btn", function () {
                $("#delete-item-id").val($(this).data("id"));
                Modals.configure({ displayName: "empleado", type: "delete" });
            });
        });
    };

    return obj;
}());
