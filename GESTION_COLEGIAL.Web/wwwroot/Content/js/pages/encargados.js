/** Listado de encargados: tabla personalizable por usuario (ver components/gc-tabla.js). */
var Encargados = (function () {
    var obj = {};

    function esActivo(v) {
        return v === true || v === 1 || v === "true" || v === "Activo";
    }

    obj.datatable = function (Direction) {
        $(function () {
            // Esta pantalla no recibe permisos: se muestran las tres acciones, como antes.
            var permisos = Direction.permisos || { editar: true, eliminar: true };

            GcTabla.crear({
                elemento: "#tabla-encargados",
                nombre: "encargados",
                url: Direction.urlList,
                archivo: "encargados",
                tituloPdf: "Listado de encargados",
                filasPorPagina: 25,
                buscador: "#custom-table-search",
                buscarEn: ["Enc_Id", "Per_Identidad", "Enc_Nombre", "Per_Telefono", "Enc_Ocupacion", "Estado"],
                alCargar: function (filas) { $("#total-count").text(filas.length); },
                columnas: [
                    { title: "Id", field: "Enc_Id", width: 90,
                      formatter: function (c) { return '<span class="badge outline-badge-primary">' + c.getValue() + '</span>'; } },
                    { title: "Identidad", field: "Per_Identidad", minWidth: 130 },
                    { title: "Nombre", field: "Enc_Nombre", minWidth: 200,
                      formatter: function (c) { return '<span class="font-weight-bold">' + (c.getValue() || "") + '</span>'; } },
                    { title: "Teléfono", field: "Per_Telefono", minWidth: 120 },
                    { title: "Ocupación", field: "Enc_Ocupacion", minWidth: 140,
                      formatter: function (c) { return '<span class="text-muted">' + (c.getValue() || "—") + '</span>'; } },
                    { title: "Estado", field: "Estado", width: 130, hozAlign: "center",
                      mutator: function (v, d) { return esActivo(d.EsActivo) ? "Activo" : "Inactivo"; },
                      formatter: function (c) {
                          return c.getValue() === "Activo"
                              ? '<span class="badge badge-success">Activo</span>'
                              : '<span class="badge badge-secondary">Inactivo</span>';
                      } },
                    { title: "Acciones", field: "Acciones", headerSort: false, hozAlign: "center", width: 140, download: false,
                      formatter: function (c) { return GcTabla.acciones(c.getData().Enc_Id, permisos); } }
                ]
            });

            // Acciones de filas
            var $t = $("#tabla-encargados");
            $t.on("click", ".detail-btn", function () { window.location = Direction.urlDetail + "/" + $(this).data("id"); });
            $t.on("click", ".edit-btn", function () { window.location = Direction.urlUpdate + "/" + $(this).data("id"); });
            $t.on("click", ".delete-btn", function () {
                $("#delete-item-id").val($(this).data("id"));
                Modals.configure({ displayName: "encargado", type: "delete" });
            });
        });
    };

    return obj;
}());
