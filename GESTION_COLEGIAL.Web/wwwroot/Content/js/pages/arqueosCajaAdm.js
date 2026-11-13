var ArqueosCajaAdm = (function () {
    var obj = {};

    var _urlLast = "";
    var _urlCreate = "";
    var _urlPagosDia = "";

    // Esperado calculado desde PagosDelDia, por bucket que soporta la tabla finanza.tbArqueosCaja.
    var _esperado = { efectivo: 0, transferencia: 0, tarjeta: 0, otros: 0, otrosLabels: {} };

    function sym() { return window.GC ? window.GC.currencySymbol() : "L"; }

    function money(v) {
        if (window.GC) return window.GC.fmtMoney(v);
        var n = Number(v) || 0;
        return "L " + n.toLocaleString("es-HN", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function moneySigned(v) {
        var n = Number(v) || 0;
        var s = money(Math.abs(n));
        if (n > 0.0001) return "+" + s;
        if (n < -0.0001) return "-" + s;
        return s;
    }

    function hoyISO() {
        var d = new Date();
        var m = String(d.getMonth() + 1).padStart(2, "0");
        var dd = String(d.getDate()).padStart(2, "0");
        return d.getFullYear() + "-" + m + "-" + dd;
    }

    // Clasifica una forma de pago en el bucket del arqueo.
    function clasificar(desc) {
        var d = String(desc || "").toLowerCase();
        if (d.indexOf("efectivo") >= 0) return "efectivo";
        if (d.indexOf("transfer") >= 0) return "transferencia";
        if (d.indexOf("tarjeta") >= 0) return "tarjeta";
        return "otros";
    }

    function kv(label, val) {
        return '<div class="row mb-2"><div class="col-5 text-muted small">' + label + '</div>' +
               '<div class="col-7 font-weight-bold">' + (val !== undefined && val !== null ? val : '—') + '</div></div>';
    }

    // ── Panel: último arqueo registrado ──
    function renderUltimo(res) {
        var d = (res && (res.item || res)) || null;
        if (d && d.Arq_Id) {
            var total = (d.Arq_TotalGeneral != null)
                ? d.Arq_TotalGeneral
                : (Number(d.Arq_TotalEfectivo || 0) + Number(d.Arq_TotalTransferencia || 0) + Number(d.Arq_TotalTarjeta || 0));
            var html =
                kv("Fecha", d.Arq_Fecha ? String(d.Arq_Fecha).substring(0, 10) : "—") +
                '<div class="row">' +
                    '<div class="col-6 mb-2"><div class="border rounded p-2 h-100"><div class="small text-muted text-uppercase font-weight-bold">Efectivo</div><div class="h6 mb-0 font-weight-bold text-dark">' + money(d.Arq_TotalEfectivo) + '</div></div></div>' +
                    '<div class="col-6 mb-2"><div class="border rounded p-2 h-100"><div class="small text-muted text-uppercase font-weight-bold">Transferencia</div><div class="h6 mb-0 font-weight-bold text-dark">' + money(d.Arq_TotalTransferencia) + '</div></div></div>' +
                    '<div class="col-6 mb-2"><div class="border rounded p-2 h-100"><div class="small text-muted text-uppercase font-weight-bold">Tarjeta</div><div class="h6 mb-0 font-weight-bold text-dark">' + money(d.Arq_TotalTarjeta) + '</div></div></div>' +
                    '<div class="col-6 mb-2"><div class="border rounded p-2 h-100"><div class="small text-muted text-uppercase font-weight-bold">Observación</div><div class="small font-weight-bold text-dark">' + (d.Arq_Observaciones || "—") + '</div></div></div>' +
                    '<div class="col-12"><div class="alert alert-light-primary mb-0"><div class="small text-uppercase font-weight-bold">Total general</div><div class="h5 mb-0 font-weight-bold text-primary">' + money(total) + '</div></div></div>' +
                '</div>';
            $("#info-arqueo").html(html);
            $("#panel-ultimo").show();
            $("#panel-sin-arqueo").hide();
        }
    }

    // ── Esperado del sistema (PagosDelDia) para la fecha del arqueo ──
    function cargarEsperado(fecha) {
        _esperado = { efectivo: 0, transferencia: 0, tarjeta: 0, otros: 0, otrosLabels: {} };
        $("#arq-estado-carga").text("Consultando pagos registrados por el sistema...");
        $.get(_urlPagosDia, { fecha: fecha }, function (resp) {
            var rows = (resp && resp.data) ? resp.data : [];
            rows.forEach(function (r) {
                var m = Number(r.Pag_MontoTotal) || 0;
                var b = clasificar(r.Fpa_Descripcion);
                _esperado[b] += m;
                if (b === "otros") {
                    var lbl = r.Fpa_Descripcion || "Sin definir";
                    _esperado.otrosLabels[lbl] = (_esperado.otrosLabels[lbl] || 0) + m;
                }
            });
            aplicarEsperado();
            var n = rows.length;
            $("#arq-estado-carga").text(n === 0
                ? "El sistema no registró pagos en esta fecha."
                : n + (n === 1 ? " pago registrado por el sistema." : " pagos registrados por el sistema."));
        }).fail(function () {
            aplicarEsperado();
            $("#arq-estado-carga").text("No se pudo consultar los pagos del sistema.");
        });
    }

    // Vuelca el esperado a la tabla y prellena lo contado con el esperado (el cajero ajusta al conteo real).
    function aplicarEsperado() {
        ["efectivo", "transferencia", "tarjeta"].forEach(function (b) {
            $("#esp-" + b).text(money(_esperado[b]));
            $("input[name='Arq_Total" + cap(b) + "']").val((_esperado[b] || 0).toFixed(2));
        });

        // Nota de "otros" (formas que la tabla de arqueo no cubre, ej. Cheque)
        if (_esperado.otros > 0.0001) {
            var partes = Object.keys(_esperado.otrosLabels).map(function (k) {
                return k + ": " + money(_esperado.otrosLabels[k]);
            });
            $("#arq-otros-note")
                .html('<i class="bi bi-exclamation-triangle"></i> El sistema registró ' + money(_esperado.otros) +
                      ' en otras formas de pago no cubiertas por el arqueo (' + partes.join(" · ") +
                      '). No se incluyen en el cuadre.')
                .show();
        } else {
            $("#arq-otros-note").hide();
        }
        recalcular();
    }

    function cap(s) { return s.charAt(0).toUpperCase() + s.slice(1); }

    function setDif(id, dif) {
        var el = $("#" + id);
        el.text(moneySigned(dif));
        el.removeClass("text-success text-danger text-muted");
        if (Math.abs(dif) < 0.005) el.addClass("text-muted");
        else if (dif > 0) el.addClass("text-success");   // sobrante
        else el.addClass("text-danger");                // faltante
    }

    // ── Recalcula diferencias contado - esperado ──
    function recalcular() {
        var totEsp = 0, totCon = 0;
        ["efectivo", "transferencia", "tarjeta"].forEach(function (b) {
            var esp = Number(_esperado[b]) || 0;
            var con = parseFloat($("input[name='Arq_Total" + cap(b) + "']").val()) || 0;
            totEsp += esp;
            totCon += con;
            setDif("dif-" + b, con - esp);
        });

        $("#esp-total").text(money(totEsp));
        $("#con-total").text(money(totCon));
        var difTotal = totCon - totEsp;
        setDif("dif-total", difTotal);

        var banner = $("#arq-banner");
        if (Math.abs(difTotal) < 0.005) {
            banner.removeClass("alert-light-danger").addClass("alert-light-success")
                .html('<i class="bi bi-check-circle-fill mr-1"></i> La caja cuadra con el sistema.')
                .show();
        } else if (difTotal > 0) {
            banner.removeClass("alert-light-success").addClass("alert-light-danger")
                .html('<i class="bi bi-exclamation-octagon-fill mr-1"></i> Sobrante de ' + money(difTotal) + ' respecto al sistema.')
                .show();
        } else {
            banner.removeClass("alert-light-success").addClass("alert-light-danger")
                .html('<i class="bi bi-exclamation-octagon-fill mr-1"></i> Faltante de ' + money(Math.abs(difTotal)) + ' respecto al sistema.')
                .show();
        }
    }

    obj.init = function (opts) {
        opts = opts || {};
        _urlLast = opts.urlLast || "";
        _urlCreate = opts.urlCreate || "";
        _urlPagosDia = opts.urlPagosDia || "";

        // Último arqueo del cajero.
        $.getJSON(_urlLast, renderUltimo);

        // Fecha por defecto = hoy y primera carga del esperado.
        var fInput = $("#arq-fecha");
        if (!fInput.val()) fInput.val(hoyISO());
        cargarEsperado(fInput.val());

        // Cambiar fecha recarga el esperado del sistema.
        fInput.on("change", function () { cargarEsperado(this.value || hoyISO()); });

        // Recalcular al escribir lo contado.
        $(".arq-input-contado").on("input", recalcular);

        // Recalcular si cambia la moneda global.
        document.addEventListener("gc:moneda", function () { aplicarEsperado(); });

        // Guardar arqueo.
        $("#form-arqueo").on("submit", function (e) {
            e.preventDefault();
            $.post(_urlCreate, $(this).serialize(), function (res) {
                if (res && res.success) {
                    $("#modal-arqueo").modal("hide");
                    location.reload();
                }
            });
        });
    };

    return obj;
}());
