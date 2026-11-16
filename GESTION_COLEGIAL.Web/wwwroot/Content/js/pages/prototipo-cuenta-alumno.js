/** Prototipo: Cuenta del alumno con acciones nuevas (anular, devolver, perdonar, saldo a favor). */
$(function () {
    var alumnoId = window.PROTO_ALUMNO_ID;
    var ACCIONES = {
        anular:   { titulo: "Anular pago", texto: "El pago queda visible pero tachado, y lo que cubría vuelve a quedar pendiente.", monto: false, metodo: false, tipo: false },
        perdonar: { titulo: "Perdonar parte de un cargo", texto: "Baja el monto del cargo sin cobrarlo (exoneración, beca o ajuste).", monto: true, metodo: false, tipo: true },
        devolver: { titulo: "Devolver dinero", texto: "Registra que se le devolvió dinero al encargado; baja el saldo a favor.", monto: true, metodo: true, tipo: false },
        saldo:    { titulo: "Usar saldo a favor", texto: "Aplica el saldo a favor del alumno a sus cargos pendientes, del más viejo al más nuevo.", monto: true, metodo: false, tipo: false }
    };

    function sinFilas(cols, texto) { return '<tr><td colspan="' + cols + '" class="text-center text-muted py-4">' + texto + '</td></tr>'; }

    function cargar() {
        var a = $.grep(window._alumnos || [], function (x) { return x.Alu_Id === alumnoId; })[0];
        $("#info-alumno").html(a
            ? '<div class="font-weight-bold text-body">' + a.Alu_Nombre + '</div><div class="small">' + (a.Per_Identidad || "") + ' · ' + a.Niv_Descripcion + ' · ' + a.Grado + ' · Sección ' + a.Sec_Descripcion + '</div>'
            : 'Alumno #' + alumnoId);
        $("#btn-cobrar").attr("href", "/Pagos/NuevoPago?alumnoId=" + alumnoId);
        if (history.replaceState) history.replaceState(null, "", "?alumnoId=" + alumnoId);

        $.getJSON("/CuentasCobrar/ObtenerResumenFinancieroAsync", { alumnoId: alumnoId }).done(function (r) {
            var d = (r && r.data) || {};
            $("#k-pendiente").text(Proto.dinero(d.totalPendiente));
            $("#k-pagado").text(Proto.dinero(d.totalPagado));
            $("#k-cargado").text(Proto.dinero(d.totalDeuda));
        });

        $.getJSON("/CuentasCobrar/ObtenerCargosPendientesAsync", { alumnoId: alumnoId }).done(function (r) {
            var filas = Array.isArray(r) ? r : ((r && r.data) || []);
            $("#n-cargos").text(filas.length);
            $("#tb-cargos").html(filas.length ? filas.map(function (c) {
                var badge = c.estadoPago === "Vencido" ? "danger" : "warning";
                return '<tr><td>' + c.conceptoPago + '<div class="small text-muted">' + (c.observaciones || "") + '</div></td>' +
                    '<td>' + Proto.fecha(c.fechaVencimiento) + '</td>' +
                    '<td class="text-right">' + Proto.dinero(c.montoOriginal) + '</td>' +
                    '<td class="text-right">' + Proto.dinero(c.montoDescuento) + '</td>' +
                    '<td class="text-right">' + Proto.dinero(c.montoMora) + '</td>' +
                    '<td class="text-right font-weight-bold">' + Proto.dinero(c.montoTotal) + '</td>' +
                    '<td><span class="badge badge-' + badge + '">' + c.estadoPago + '</span></td>' +
                    '<td class="text-center text-nowrap"><button class="btn btn-sm btn-outline-primary" data-accion="perdonar" data-monto="' + c.montoTotal + '" data-ref="' + c.conceptoPago + '"><i class="bi bi-heart"></i> Perdonar</button></td></tr>';
            }).join("") : sinFilas(8, "Sin cargos pendientes."));
        });

        $.getJSON("/CuentasCobrar/ObtenerHistoricoPagosAsync", { alumnoId: alumnoId }).done(function (r) {
            var filas = Array.isArray(r) ? r : ((r && r.data) || []);
            $("#n-pagos").text(filas.length);
            $("#tb-pagos").html(filas.length ? filas.map(function (p) {
                return '<tr><td>' + new Date(p.fechaPago).toLocaleString("es-HN") + '</td>' +
                    '<td>' + (p.numeroReferencia || "") + '</td><td>' + (p.formaPago || "") + '</td>' +
                    '<td>' + (p.observaciones || "") + '</td><td>' + (p.usuario || "") + '</td>' +
                    '<td class="text-right font-weight-bold text-success">' + Proto.dinero(p.montoTotal) + '</td>' +
                    '<td class="text-center text-nowrap">' +
                    '<a class="btn btn-sm btn-outline-primary mr-1" target="_blank" href="/Pagos/Recibo?pagoId=' + p.pagoId + '"><i class="bi bi-printer"></i> Recibo</a>' +
                    '<button class="btn btn-sm btn-outline-danger" data-accion="anular" data-monto="' + p.montoTotal + '" data-ref="' + (p.numeroReferencia || "") + '"><i class="bi bi-x-circle"></i> Anular</button></td></tr>';
            }).join("") : sinFilas(7, "Sin pagos registrados."));
        });
    }

    // Ventana de acciones
    var accionActual = null;
    $(document).on("click", "[data-accion]", function () {
        var k = $(this).data("accion"), cfg = ACCIONES[k];
        accionActual = cfg;
        var ref = $(this).data("ref");
        $("#modal-accion-titulo").text(cfg.titulo + (ref ? " · " + ref : ""));
        $("#modal-accion-texto").text(cfg.texto);
        $("#campo-monto").toggle(cfg.monto); $("#campo-metodo").toggle(cfg.metodo); $("#campo-tipo").toggle(cfg.tipo);
        $("#m-monto").val($(this).data("monto") || ""); $("#m-motivo").val("").removeClass("is-invalid");
        $("#modal-accion").modal("show");
    });
    $("#m-guardar").on("click", function () {
        if (!$("#m-motivo").val().trim()) { $("#m-motivo").addClass("is-invalid").focus(); return; }
        $("#modal-accion").modal("hide");
        Proto.avisoNoGuarda(accionActual.titulo);
    });

    // Buscador de alumno
    Proto.cargarAlumnos().done(function (lista) {
        window._alumnos = lista;
        $("#lista-alumnos").html(lista.map(function (a) {
            return '<option value="' + a.Alu_Nombre + ' · ' + (a.Per_Identidad || "") + '" data-id="' + a.Alu_Id + '"></option>';
        }).join(""));
        cargar();
    }).fail(cargar);
    $("#f-alumno").on("change", function () {
        var v = $(this).val(), op = $('#lista-alumnos option').filter(function () { return this.value === v; })[0];
        if (op) { alumnoId = parseInt($(op).data("id"), 10); cargar(); }
    });
});
