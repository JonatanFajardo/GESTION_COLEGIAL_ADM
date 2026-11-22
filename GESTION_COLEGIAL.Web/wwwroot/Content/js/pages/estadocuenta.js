/**
 * Cuenta del alumno: cargos pendientes, pagos, saldo a favor y correcciones.
 * Correcciones (con permiso "Corregir cuentas"): anular pago, perdonar cargo, devolver dinero y usar saldo a favor.
 */
var CuentaAlumno = (function () {
    var cfg = {};
    var alumnoId = 0;
    var alumnos = [];            // lista para el buscador
    var cargos = [], pagos = [];
    var saldoFavor = 0, pendienteTotal = 0;
    var accion = null;           // { tipo, id, limite }

    var ACCIONES = {
        anular:   { titulo: "Anular pago", boton: "Anular pago", clase: "btn-danger", monto: false, destino: true, metodo: false, tipo: false,
                    texto: "El pago queda visible pero tachado y deja de contar; lo que cubría vuelve a quedar pendiente." },
        perdonar: { titulo: "Perdonar parte de un cargo", boton: "Perdonar", clase: "btn-primary", monto: true, destino: false, metodo: false, tipo: true,
                    texto: "Baja el monto del cargo sin cobrarlo (exoneración, beca o ajuste)." },
        devolver: { titulo: "Devolver dinero", boton: "Registrar devolución", clase: "btn-primary", monto: true, destino: false, metodo: true, tipo: false,
                    texto: "Registra que se le devolvió dinero al encargado; baja el saldo a favor." },
        saldo:    { titulo: "Usar saldo a favor", boton: "Aplicar saldo", clase: "btn-primary", monto: true, destino: false, metodo: false, tipo: false,
                    texto: "Aplica el saldo a favor a los cargos pendientes, del más viejo al más nuevo." }
    };
    var URL_ACCION = {
        anular: "/CuentasCobrar/AnularPagoAsync",
        perdonar: "/CuentasCobrar/PerdonarAsync",
        devolver: "/CuentasCobrar/DevolverSaldoAsync",
        saldo: "/CuentasCobrar/AplicarSaldoAsync"
    };
    var COLOR_CORRECCION = { "anul": "danger", "perd": "info", "exon": "info", "beca": "info", "ajust": "info", "devol": "warning", "saldo": "success", "aplic": "success" };

    function sinFilas(cols, texto) { return '<tr><td colspan="' + cols + '" class="text-center text-muted py-4">' + texto + '</td></tr>'; }

    // ── Alumno ──
    function datosAlumno() {
        return $.grep(alumnos, function (a) { return a.Alu_Id === alumnoId; })[0];
    }
    function pintarAlumno() {
        var a = datosAlumno();
        var html = a
            ? '<div class="font-weight-bold text-body">' + Caja.esc(a.Alu_Nombre) + '</div><div class="small">' +
              Caja.esc([a.Per_Identidad, a.Niv_Descripcion, a.Grado, a.Cur_Descripcion, a.Sec_Descripcion ? "Sección " + a.Sec_Descripcion : ""].filter(Boolean).join(" · ")) + '</div>'
            : 'Alumno #' + alumnoId;
        $("#info-alumno, #info-alumno-impresion").html(html);
        if (a) $("#f-alumno").val(a.Alu_Nombre + " · " + (a.Per_Identidad || ""));
    }
    function abrir(id) {
        alumnoId = id;
        $("#sin-alumno").addClass("d-none");
        $("#cuenta").removeClass("d-none");
        $(".acciones-cuenta").removeClass("d-none");
        $("#btn-cobrar").attr("href", "/Pagos/NuevoPago?alumnoId=" + alumnoId);
        if (history.replaceState) history.replaceState(null, "", "?alumnoId=" + alumnoId);
        pintarAlumno();
        cargar();
    }

    // ── Datos ──
    function cargar() {
        $.getJSON("/CuentasCobrar/ObtenerResumenFinancieroAsync", { alumnoId: alumnoId }).done(function (r) {
            // "Pendiente" y la mora salen de la lista de cargos (la misma que se ve abajo).
            var d = (r && r.data) || {};
            $("#k-cargado").text(Caja.dinero(d.totalDeuda));
            $("#k-pagado").text(Caja.dinero(d.totalPagado));
        });

        $.getJSON("/CuentasCobrar/SaldoFavorAsync", { alumnoId: alumnoId }).done(function (r) {
            saldoFavor = parseFloat(((r && r.data) || {}).Saldo) || 0;
            $("#k-favor").text(Caja.dinero(saldoFavor));
            botonesSaldo();
        }).fail(function () { $("#k-favor").text("—"); });

        // Cargos con saldo: esta lista trae lo que falta pagar de cada uno (abonos parciales incluidos).
        $.getJSON("/CuentasCobrar/ListByAlumnoAsync", { alumnoId: alumnoId }).done(function (r) {
            cargos = Caja.filas(r).filter(function (c) { return (parseFloat(c.MontoPendiente) || 0) > 0; }).map(function (c) {
                return {
                    cuentaCobrarId: c.CuentaCobrarId, conceptoPago: c.ConceptoDescripcion, observaciones: c.Observaciones,
                    fechaVencimiento: c.FechaVencimiento, montoOriginal: c.MontoOriginal, montoDescuento: c.MontoDescuento,
                    montoMora: c.MontoMora, montoTotal: c.MontoTotal, montoPendiente: c.MontoPendiente, estadoPago: c.EstadoPagoDescripcion
                };
            }).sort(function (a, b) { return new Date(a.fechaVencimiento) - new Date(b.fechaVencimiento); });
            $("#n-cargos").text(cargos.length);
            var mora = 0;
            pendienteTotal = 0;
            cargos.forEach(function (c) {
                pendienteTotal += parseFloat(c.montoPendiente != null ? c.montoPendiente : c.montoTotal) || 0;
                mora += parseFloat(c.montoMora) || 0;
            });
            $("#k-pendiente").text(Caja.dinero(pendienteTotal));
            $("#k-mora").text(mora > 0 ? "Incluye " + Caja.dinero(mora) + " de mora" : "Lo que debe hoy");
            botonesSaldo();
            var cols = cfg.puedeCorregir ? 9 : 8;
            $("#tb-cargos").html(cargos.length ? cargos.map(function (c, i) {
                var badge = c.estadoPago === "Vencido" ? "danger" : (c.estadoPago === "Pagado" ? "success" : "warning");
                return '<tr><td>' + Caja.esc(c.conceptoPago) + (c.observaciones ? '<div class="small text-muted">' + Caja.esc(c.observaciones) + '</div>' : '') + '</td>' +
                    '<td>' + Caja.fecha(c.fechaVencimiento) + '</td>' +
                    '<td class="text-right">' + Caja.dinero(c.montoOriginal) + '</td>' +
                    '<td class="text-right">' + Caja.dinero(c.montoDescuento) + '</td>' +
                    '<td class="text-right">' + Caja.dinero(c.montoMora) + '</td>' +
                    '<td class="text-right">' + Caja.dinero(c.montoTotal) + '</td>' +
                    '<td class="text-right font-weight-bold">' + Caja.dinero(c.montoPendiente) + '</td>' +
                    '<td><span class="badge badge-' + badge + '">' + Caja.esc(c.estadoPago) + '</span></td>' +
                    (cfg.puedeCorregir ? '<td class="text-center text-nowrap d-print-none"><button type="button" class="btn btn-sm btn-outline-primary" data-accion="perdonar" data-i="' + i + '"><i class="bi bi-heart mr-1"></i>Perdonar</button></td>' : '') +
                    '</tr>';
            }).join("") : sinFilas(cols, "Sin cargos pendientes."));
        }).fail(function () { $("#tb-cargos").html(sinFilas(9, "No se pudieron cargar los cargos.")); });

        $.getJSON("/CuentasCobrar/ObtenerHistoricoPagosAsync", { alumnoId: alumnoId }).done(function (r) {
            pagos = Caja.filas(r);
            $("#n-pagos").text(pagos.length);
            $("#tb-pagos").html(pagos.length ? pagos.map(function (p, i) {
                var anulado = p.esAnulado === true;
                var anular = cfg.puedeCorregir && !anulado
                    ? '<button type="button" class="btn btn-sm btn-outline-danger" data-accion="anular" data-i="' + i + '"><i class="bi bi-x-circle mr-1"></i>Anular</button>' : '';
                return '<tr' + (anulado ? ' class="fila-anulada"' : '') + '><td>' + Caja.fechaHora(p.fechaPago) + '</td>' +
                    '<td>' + Caja.esc(p.numeroRecibo || p.numeroReferencia || "") + '</td>' +
                    '<td>' + Caja.esc(p.formaPago) + (p.usuario ? '<div class="small text-muted">' + Caja.esc(p.usuario) + '</div>' : '') + '</td>' +
                    '<td class="text-wrap">' + Caja.esc(p.observaciones) + (anulado && p.motivoAnulacion ? '<div class="small">Anulado: ' + Caja.esc(p.motivoAnulacion) + '</div>' : '') + '</td>' +
                    '<td class="text-right font-weight-bold' + (anulado ? '' : ' text-success') + '">' + Caja.dinero(p.montoTotal) + '</td>' +
                    '<td class="sin-tachar">' + (anulado ? '<span class="badge badge-danger">Anulado</span>' : '') + '</td>' +
                    '<td class="text-center text-nowrap sin-tachar d-print-none">' +
                    '<a class="btn btn-sm btn-outline-primary mr-1" target="_blank" title="Imprimir recibo" aria-label="Imprimir recibo" href="/Pagos/Recibo?pagoId=' + p.pagoId + '"><i class="bi bi-printer"></i></a>' + anular + '</td></tr>';
            }).join("") : sinFilas(7, "Sin pagos registrados."));
        }).fail(function () { $("#tb-pagos").html(sinFilas(7, "No se pudieron cargar los pagos.")); });

        $.getJSON("/CuentasCobrar/CorreccionesAsync", { alumnoId: alumnoId }).done(function (r) {
            var filas = Caja.filas(r);
            $("#n-correcciones").text(filas.length);
            $("#tb-correcciones").html(filas.length ? filas.map(function (c) {
                var clave = (c.Tipo || "").toLowerCase().normalize("NFD").replace(/[̀-ͯ]/g, "");
                var color = "secondary";
                Object.keys(COLOR_CORRECCION).some(function (k) { if (clave.indexOf(k) >= 0) { color = COLOR_CORRECCION[k]; return true; } return false; });
                return '<tr><td class="text-nowrap">' + Caja.fechaHora(c.Fecha) + '</td>' +
                    '<td><span class="badge badge-' + color + '">' + Caja.esc(c.Tipo) + '</span></td>' +
                    '<td class="text-wrap">' + Caja.esc(c.Detalle) + '</td><td class="text-wrap">' + Caja.esc(c.Motivo) + '</td><td>' + Caja.esc(c.Usuario) + '</td>' +
                    '<td class="text-right">' + Caja.dinero(c.Monto) + '</td></tr>';
            }).join("") : sinFilas(6, "Sin correcciones registradas."));
        }).fail(function () { $("#tb-correcciones").html(sinFilas(6, "No se pudieron cargar las correcciones.")); });
    }

    function botonesSaldo() {
        $("#btn-devolver").prop("disabled", !(saldoFavor > 0));
        $("#btn-usar-saldo").prop("disabled", !(saldoFavor > 0 && pendienteTotal > 0));
    }

    // ── Ventana de correcciones ──
    function abrirAccion(tipo, $btn) {
        var c = ACCIONES[tipo], i = $btn.data("i"), ref = "", limite = null, monto = "", ayuda = "";
        accion = { tipo: tipo, id: null };
        if (tipo === "anular") {
            var p = pagos[i];
            accion.id = p.pagoId;
            ref = (p.numeroRecibo || p.numeroReferencia || "") + " · " + Caja.dinero(p.montoTotal);
        } else if (tipo === "perdonar") {
            var cg = cargos[i];
            accion.id = cg.cuentaCobrarId;
            var conPendiente = cg.montoPendiente != null;
            limite = parseFloat(conPendiente ? cg.montoPendiente : cg.montoTotal) || 0;
            monto = limite.toFixed(2);
            ref = cg.conceptoPago;
            ayuda = "Máximo " + Caja.dinero(limite) + (conPendiente ? " (lo que falta pagar del cargo)." : " (total del cargo).");
        } else if (tipo === "devolver") {
            limite = saldoFavor;
            monto = limite.toFixed(2);
            ayuda = "Máximo " + Caja.dinero(limite) + " (saldo a favor).";
        } else if (tipo === "saldo") {
            limite = Math.min(saldoFavor, pendienteTotal);
            monto = limite.toFixed(2);
            ayuda = "Máximo " + Caja.dinero(limite) + " (saldo a favor disponible, sin pasar de lo pendiente).";
        }
        accion.limite = limite;

        $("#modal-accion-titulo").text(c.titulo + (ref ? " · " + ref : ""));
        $("#modal-accion-texto").text(c.texto);
        $("#campo-monto").toggle(c.monto); $("#campo-destino").toggle(c.destino);
        $("#campo-metodo").toggle(c.metodo); $("#campo-tipo").toggle(c.tipo);
        $("#m-monto").val(monto).attr("max", limite || null).removeClass("is-invalid");
        $("#m-monto-ayuda").text(ayuda);
        $("input[name=m-destino]").prop("checked", false);
        $("#campo-destino").removeClass("is-invalid"); $("#m-destino-error").hide();
        $("#m-metodo").val("Efectivo"); $("#m-tipo").val("Exoneración");
        $("#m-motivo").val("").removeClass("is-invalid");
        $("#m-guardar").text(c.boton).removeClass("btn-primary btn-danger").addClass(c.clase).prop("disabled", false);
        $("#modal-accion").modal("show");
    }

    function validar() {
        var c = ACCIONES[accion.tipo], ok = true, datos = {};
        if (c.monto) {
            var m = parseFloat($("#m-monto").val());
            var err = "";
            if (!(m > 0)) err = "El monto debe ser mayor que cero.";
            else if (accion.limite != null && m > accion.limite + 0.0001) err = "El monto no puede pasar de " + Caja.dinero(accion.limite) + ".";
            $("#m-monto").toggleClass("is-invalid", !!err); $("#m-monto-error").text(err);
            if (err) ok = false;
            datos.monto = m;
        }
        if (c.destino) {
            var dest = $("input[name=m-destino]:checked").val();
            $("#m-destino-error").toggle(!dest);
            if (!dest) ok = false;
            datos.destino = dest;
        }
        var motivo = $.trim($("#m-motivo").val());
        $("#m-motivo").toggleClass("is-invalid", !motivo);
        if (!motivo) ok = false;
        datos.motivo = motivo;
        if (c.metodo) datos.metodo = $("#m-metodo").val();
        if (c.tipo) datos.tipo = $("#m-tipo").val();
        return ok ? datos : null;
    }

    function guardar(e) {
        e.preventDefault();
        var datos = validar();
        if (!datos) return;
        if (accion.tipo === "anular") datos.pagoId = accion.id;
        else if (accion.tipo === "perdonar") datos.cuentaCobrarId = accion.id;
        else datos.alumnoId = alumnoId;

        var $btn = $("#m-guardar").prop("disabled", true);
        $.ajax({
            url: URL_ACCION[accion.tipo],
            type: "POST",
            data: datos,
            headers: { RequestVerificationToken: $('input[name="__RequestVerificationToken"]').first().val() }
        }).done(function (r) {
            $("#modal-accion").modal("hide");
            Caja.avisar("success", (r && r.message) || "Listo.");
            cargar();
        }).fail(function (xhr) {
            Caja.avisar("error", Caja.errorDe(xhr));
        }).always(function () { $btn.prop("disabled", false); });
    }

    // ── Buscador ──
    function buscarPorTexto(v) {
        v = $.trim(v || "");
        var op = $("#lista-alumnos option").filter(function () { return this.value === v; })[0];
        if (op) return parseInt($(op).data("id"), 10);
        var soloDigitos = v.replace(/\D/g, "");
        if (soloDigitos.length === 13) {
            var a = $.grep(alumnos, function (x) { return (x.Per_Identidad || "").replace(/\D/g, "") === soloDigitos; })[0];
            if (a) return a.Alu_Id;
        }
        return 0;
    }

    function init(config) {
        cfg = config;
        $(function () {
            $.getJSON("/CuentasCobrar/AlumnosBuscarAsync").done(function (lista) {
                alumnos = Caja.filas(lista);
                $("#lista-alumnos").html(alumnos.map(function (a) {
                    return '<option value="' + Caja.esc(a.Alu_Nombre + " · " + (a.Per_Identidad || "")) + '" data-id="' + a.Alu_Id + '"></option>';
                }).join(""));
            }).always(function () {
                if (cfg.alumnoId > 0) abrir(cfg.alumnoId);
                else if (cfg.identidad) {
                    var id = buscarPorTexto(cfg.identidad);
                    if (id) abrir(id); else Caja.avisar("warning", "No se encontró un alumno con la identidad " + cfg.identidad + ".");
                }
            });

            $("#f-alumno").on("change", function () {
                var id = buscarPorTexto($(this).val());
                if (id && id !== alumnoId) abrir(id);
            }).on("keydown", function (e) {
                if (e.which !== 13) return;
                e.preventDefault();
                var id = buscarPorTexto($(this).val());
                if (id) abrir(id); else Caja.avisar("warning", "No se encontró el alumno. Elegilo de la lista o escribí su identidad completa.");
            });

            $("#btn-imprimir").on("click", function () {
                window.print();
            });
            if (cfg.puedeCorregir) {
                $(document).on("click", "[data-accion]", function () { abrirAccion($(this).data("accion"), $(this)); });
                $("#form-accion").on("submit", guardar);
            }
        });
    }

    return { init: init };
})();
