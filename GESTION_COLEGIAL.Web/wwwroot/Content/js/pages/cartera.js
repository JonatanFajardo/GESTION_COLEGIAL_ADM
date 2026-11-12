/* ============================================================
   HUB DE CARTERA — cartera.js
   Consolida las vistas de deuda en un solo hub con tabs.
   NO duplica backend: cada tab consume su endpoint existente.
     · Tab "aging"  → /AgingCuentas/ListAsync
     · Tab "nivel"  → /MorosidadPorNivel/ListAsync
     · Tab "top"    → /TopDeudores/ListAsync
     · Tab "pend"   → /PendientesCobrar/ListAsync  (también alimenta los KPIs de arriba)
   Carga bajo demanda: cada tab se pide la primera vez que se abre.
   ============================================================ */
var Cartera = (function () {
    var obj = {};
    var urls = {};

    // Estado de tabs y datos cacheados
    var tabActiva = "nivel";
    var cargada = { aging: false, nivel: false, top: false, pend: false };
    var pendRows = [];   // filas de PendientesCobrar (reutilizadas por KPIs y tab Pendientes)

    // ─── Helpers ─────────────────────────────────────────────────────────────────
    function rows(resp) {
        if (!resp) return [];
        if (Array.isArray(resp)) return resp;
        if (resp.data) return resp.data;
        return [];
    }

    function fmt(v) {
        var n = parseFloat(v) || 0;
        return "L. " + n.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    }

    function esc(s) {
        return String(s == null ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }

    function num(v) { return parseFloat(v) || 0; }

    // Parámetro de precarga: prefiere identidad (ruta de búsqueda probada),
    // cae a alumnoId si la fila no trae identidad.
    function idParam(row) {
        var ident = row && (row.Alu_Identidad || row.Alu_identidad);
        if (ident) return "?identidad=" + encodeURIComponent(ident);
        var id = row && (row.Alu_Id || row.AlumnoId);
        return id ? "?alumnoId=" + id : "";
    }

    // Enlace a "Registrar pago" (NuevoPago) con el alumno precargado.
    function linkPago(row) {
        return (urls.urlNuevoPago || "/Pagos/NuevoPago") + idParam(row);
    }
    function btnPago(row) {
        return '<a class="cartera-btn-pago" href="' + linkPago(row) + '" title="Ir a registrar el pago de este alumno">' +
               '<i class="fa-solid fa-cash-register"></i> Registrar pago</a>';
    }

    function initials(name) {
        if (!name) return "?";
        var p = String(name).trim().split(/\s+/);
        return ((p[0] ? p[0][0] : "") + (p.length > 1 ? p[p.length - 1][0] : "")).toUpperCase();
    }

    function avatarPalette(i) {
        var p = ["av-rose", "av-amber", "av-violet", "av-emerald", ""];
        return p[i % p.length];
    }

    var BAR_CLS = ["b-rose", "b-amber", "b-cyan", "b-violet", "b-emerald", ""];
    var AGING_CLS = ["", "a-31", "a-61", "a-91", "a-120"];

    // ─── Init ────────────────────────────────────────────────────────────────────
    obj.init = function (config) {
        urls = config || {};

        $("#cartera-tabs").on("click", ".aa-tab-item", function (e) {
            e.preventDefault();
            var tab = $(this).data("tab");
            if (!tab || tab === tabActiva) return;
            activarTab(tab);
        });

        // Filtros del tab Pendientes (client-side)
        $("#cartera-pend-buscar").on("input", renderPendientes);
        $("#cartera-pend-estado").on("change", renderPendientes);
        $("#cartera-pend-concepto").on("change", renderPendientes);

        // Los KPIs de arriba dependen de PendientesCobrar → se cargan al arranque
        cargarPendientes(function () { pintarKpis(pendRows); });

        // ── Drill-down curso → sección → alumno (toggle) ──
        $("#cartera-drill").on("click", ".drill-row[data-exp]", function () {
            $(this).toggleClass("open");
            $(this).next(".drill-children").slideToggle(120);
        });

        // ── Solicitar cobro (correo simulado) ──
        $("#cartera-btn-solicitar").on("click", abrirSolicitar);
        $("#cartera-modal-solicitar").on("click", ".sc-scope", function () {
            scopeSel = $(this).data("scope");
            $(".sc-scope").removeClass("active");
            $(this).addClass("active");
            renderPreview();
        });
        $("#sc-btn-enviar").on("click", enviarSolicitud);

        // Tab por defecto
        activarTab("nivel");
    };

    // ─── Solicitar cobro (simulación de correo a encargados) ─────────────────────
    var scopeSel = "todos";

    // Agrupa pendRows por alumno según el alcance; devuelve destinatarios y monto.
    function calcScope(scope) {
        var byAlu = {};
        pendRows.forEach(function (r) {
            var dias = parseInt(r.DiasVencida, 10); if (isNaN(dias)) dias = 0;
            var pend = num(r.Cco_MontoPendiente);
            if (pend <= 0) return;
            if (scope === "vencido" && dias <= 0) return;
            if (scope === "critico" && dias <= 90) return;
            var k = r.Alu_NombreCompleto || "—";
            if (!byAlu[k]) byAlu[k] = 0;
            byAlu[k] += pend;
        });
        var nombres = Object.keys(byAlu);
        var monto = nombres.reduce(function (s, k) { return s + byAlu[k]; }, 0);
        return { destinatarios: nombres.length, monto: monto, primer: nombres[0] || "—" };
    }

    function abrirSolicitar() {
        scopeSel = "todos";
        $(".sc-scope").removeClass("active");
        $(".sc-scope[data-scope='todos']").addClass("active");

        ["todos", "vencido", "critico"].forEach(function (sc) {
            var r = calcScope(sc);
            $("#sc-num-" + sc).text(r.destinatarios);
            $("#sc-monto-" + sc).text(fmt(r.monto));
        });

        $("#sc-form").show();
        $("#sc-result").hide().empty();
        $("#sc-btn-enviar").show().prop("disabled", false);
        $("#sc-btn-cerrar").text("Cancelar");
        renderPreview();
        $("#cartera-modal-solicitar").modal("show");
    }

    function renderPreview() {
        var r = calcScope(scopeSel);
        var hoy = new Date();
        var limite = new Date(hoy.getTime() + 5 * 86400000);
        var fLimite = ("0" + limite.getDate()).slice(-2) + "/" + ("0" + (limite.getMonth() + 1)).slice(-2) + "/" + limite.getFullYear();

        var texto =
            "Para: encargado de " + (r.primer) + "  (y " + Math.max(0, r.destinatarios - 1) + " más)\n" +
            "Asunto: Recordatorio de pago — Instituto Nova Sula\n\n" +
            "Estimado encargado:\n\n" +
            "Le recordamos que en el sistema figura un saldo pendiente asociado a su representado. " +
            "Le agradecemos regularizar el pago a más tardar el " + fLimite + " para evitar recargos por mora.\n\n" +
            "Puede acercarse a caja o comunicarse con administración. Si ya realizó el pago, ignore este mensaje.\n\n" +
            "Atentamente,\nAdministración — Instituto Nova Sula";
        $("#sc-preview").text(texto);
        $("#sc-btn-enviar").html('<i class="fa-solid fa-paper-plane"></i> Enviar ' + r.destinatarios + ' recordatorio' + (r.destinatarios === 1 ? "" : "s"));
    }

    function enviarSolicitud() {
        var r = calcScope(scopeSel);
        if (r.destinatarios === 0) { return; }

        var $btn = $("#sc-btn-enviar");
        $btn.prop("disabled", true).html('<i class="fa-solid fa-spinner fa-spin"></i> Enviando…');

        // Simulación: no hay servidor de correo. Se muestra el resultado tras un breve retardo.
        setTimeout(function () {
            $("#sc-form").hide();
            $("#sc-result").show().html(
                '<div class="sc-result">' +
                    '<div class="sc-result-icon"><i class="fa-solid fa-circle-check"></i></div>' +
                    '<h4 style="margin:10px 0 4px;font-weight:800;color:#0f172a;">Recordatorios enviados</h4>' +
                    '<p style="color:#64748b;margin:0;">Se enviaron <strong>' + r.destinatarios + '</strong> recordatorio' + (r.destinatarios === 1 ? "" : "s") +
                        ' de cobro por correo a los encargados, por un total de <strong>' + fmt(r.monto) + '</strong>.</p>' +
                    '<p style="color:#92400e;font-size:.78rem;margin-top:10px;">(Simulación — no se envió ningún correo real.)</p>' +
                '</div>'
            );
            $btn.hide();
            $("#sc-btn-cerrar").text("Cerrar");
        }, 900);
    }

    function activarTab(tab) {
        tabActiva = tab;
        $("#cartera-tabs .aa-tab-item").removeClass("active");
        $("#cartera-tabs .aa-tab-item[data-tab='" + tab + "']").addClass("active");
        $(".aa-tab-panel").hide();
        $("#tab-" + tab).show();
        cargarTabActiva();
    }

    function cargarTabActiva() {
        if (cargada[tabActiva]) return;
        cargada[tabActiva] = true;
        if (tabActiva === "aging") cargarPendientes(renderAging);
        else if (tabActiva === "nivel") cargarPendientes(renderDrill);
        else if (tabActiva === "top") cargarPendientes(renderTop);
        else if (tabActiva === "pend") cargarPendientes(renderPendientes);
    }

    // ─── KPIs superiores (fuente: PendientesCobrar) ──────────────────────────────
    function pintarKpis(data) {
        var total = 0, vencido = 0, orig = 0, pag = 0;
        var alumnos = {};

        data.forEach(function (r) {
            var pend = num(r.Cco_MontoPendiente);
            total += pend;
            var dias = parseInt(r.DiasVencida, 10);
            if (!isNaN(dias) && dias > 0) vencido += pend;
            orig += num(r.Cco_MontoOriginal);
            pag += num(r.Cco_MontoTotal);   // monto ya pagado sobre la cuenta
            if (r.Alu_NombreCompleto) alumnos[r.Alu_NombreCompleto] = true;
        });

        var rec = orig > 0 ? (pag / orig * 100) : 0;
        var nAlumnos = Object.keys(alumnos).length;

        $("#cartera-kpi-total").text(fmt(total));
        $("#cartera-kpi-total-sub").text(data.length + (data.length === 1 ? " cuenta abierta" : " cuentas abiertas"));
        $("#cartera-kpi-vencido").text(fmt(vencido));
        $("#cartera-kpi-vencido-sub").text(total > 0 ? (vencido / total * 100).toFixed(1) + "% de la cartera" : "—");
        $("#cartera-kpi-recuperacion").text(rec.toFixed(1) + "%");
        $("#cartera-kpi-alumnos").text(nAlumnos);
        $("#cartera-kpi-alumnos-sub").text("con saldo abierto");
    }

    // ─── TAB 1: Aging (derivado de pendRows) ─────────────────────────────────────
    function cargarAging() {
        cargada.aging = true;
        cargarPendientes(renderAging);
    }

    function renderAging() {
        var base = pendRows.filter(function (r) { return num(r.Cco_MontoPendiente) > 0; });

        // Buckets por días vencidos (misma escala que el tab Pendientes)
        var bk = [{ v: 0, c: 0 }, { v: 0, c: 0 }, { v: 0, c: 0 }, { v: 0, c: 0 }, { v: 0, c: 0 }];
        var totalMonto = 0;
        base.forEach(function (r) {
            var d = parseInt(r.DiasVencida, 10); if (isNaN(d)) d = 0;
            var i = aging5(d);
            var p = num(r.Cco_MontoPendiente);
            bk[i].v += p; bk[i].c++; totalMonto += p;
        });

        var wrap = $("#cartera-aging-row").empty();
        if (base.length === 0) {
            wrap.html('<div class="fd-empty"><i class="fa-solid fa-inbox"></i><p>Sin saldos pendientes</p></div>');
        } else {
            PEND_BUCKETS.forEach(function (label, i) {
                wrap.append(
                    '<div class="fd-aging ' + AGING_CLS[i] + '">' +
                        '<p class="fd-aging-label">' + esc(label) + '</p>' +
                        '<p class="fd-aging-amount">' + esc(fmt(bk[i].v)) + '</p>' +
                        '<p class="fd-aging-count">' + bk[i].c + (bk[i].c === 1 ? ' cuenta' : ' cuentas') + '</p>' +
                    '</div>'
                );
            });
        }
        $("#cartera-aging-chip").text(fmt(totalMonto) + " · " + base.length + " cuentas");

        // Tabla detalle por cuenta
        var tb = $("#cartera-aging-table tbody").empty();
        if (base.length === 0) {
            tb.append('<tr><td colspan="4" class="text-center text-muted py-3">Sin registros.</td></tr>');
        } else {
            base.slice().sort(function (a, b) { return num(b.Cco_MontoPendiente) - num(a.Cco_MontoPendiente); }).forEach(function (r) {
                var d = parseInt(r.DiasVencida, 10); if (isNaN(d)) d = 0;
                tb.append(
                    '<tr>' +
                        '<td>' + esc(r.Alu_NombreCompleto || "—") + '</td>' +
                        '<td>' + esc(PEND_BUCKETS[aging5(d)]) + '</td>' +
                        '<td class="text-right">' + esc(fmt(r.Cco_MontoPendiente)) + '</td>' +
                        '<td class="text-right">' + (d > 0 ? d + " d" : "—") + '</td>' +
                    '</tr>'
                );
            });
        }
    }

    // ─── TAB 2: Drill-down Curso → Sección → Alumnos ─────────────────────────────
    // Se arma en cliente desde pendRows (PendientesCobrar), que ya trae por cuenta:
    // Cur_Nombre, Sec_Descripcion, Alu_Id, Alu_NombreCompleto, monto y días.
    function cargarNivel() {
        cargada.nivel = true;
        cargarPendientes(renderDrill);
    }

    // Agrupa filas por una clave, conservando el orden de aparición.
    function grupoOrden(arr, keyFn) {
        var map = {}, order = [];
        arr.forEach(function (r) {
            var k = keyFn(r); if (k == null || k === "") k = "—";
            if (!map[k]) { map[k] = []; order.push(k); }
            map[k].push(r);
        });
        return { map: map, order: order };
    }

    // Suma pendiente, vencido y cuenta alumnos únicos de un grupo de filas.
    function drillAgg(arr) {
        var pend = 0, venc = 0, alu = {};
        arr.forEach(function (r) {
            var p = num(r.Cco_MontoPendiente); if (p <= 0) return;
            pend += p;
            var d = parseInt(r.DiasVencida, 10) || 0; if (d > 0) venc += p;
            alu[r.Alu_Id] = true;
        });
        return { pend: pend, venc: venc, alumnos: Object.keys(alu).length };
    }

    function drillNodo(cls, nombre, a) {
        return '<div class="drill-row ' + cls + '" data-exp>' +
            '<span class="drill-name"><i class="fa-solid fa-chevron-right drill-caret"></i>' + esc(nombre) + '</span>' +
            '<span class="drill-metric">' + fmt(a.pend) + '</span>' +
            '<span class="drill-metric ' + (a.venc > 0 ? 'drill-venc' : '') + '">' + (a.venc > 0 ? fmt(a.venc) : '—') + '</span>' +
            '<span class="drill-metric drill-count">' + a.alumnos + '</span>' +
            '</div>';
    }

    function drillBtnEC(row) {
        return '<a class="cartera-btn-ec" href="' + (urls.urlEstadoCuenta || "#") + idParam(row) + '" title="Estado de cuenta"><i class="fa-solid fa-file-invoice"></i></a>';
    }
    function drillBtnPago(row) {
        return '<a class="cartera-btn-ec cartera-btn-pago-mini" href="' + linkPago(row) + '" title="Registrar pago"><i class="fa-solid fa-cash-register"></i></a>';
    }

    function renderDrill() {
        var cont = $("#cartera-drill").empty();
        var base = pendRows.filter(function (r) { return num(r.Cco_MontoPendiente) > 0; });

        if (base.length === 0) {
            cont.html('<div class="fd-empty"><i class="fa-solid fa-circle-check"></i><p>Sin deuda pendiente</p></div>');
            $("#cartera-drill-chip").text("—");
            return;
        }

        var tot = drillAgg(base);
        $("#cartera-drill-chip").text(fmt(tot.pend) + " · " + tot.alumnos + (tot.alumnos === 1 ? " alumno" : " alumnos"));

        cont.append(
            '<div class="drill-row drill-head">' +
                '<span class="drill-name">Curso / Sección / Alumno</span>' +
                '<span class="drill-metric">Por cobrar</span>' +
                '<span class="drill-metric">Vencido</span>' +
                '<span class="drill-metric drill-count">Alum.</span>' +
            '</div>'
        );

        var gc = grupoOrden(base, function (r) { return r.Cur_Nombre || "Sin curso"; });
        gc.order.sort(function (a, b) { return drillAgg(gc.map[b]).pend - drillAgg(gc.map[a]).pend; });

        gc.order.forEach(function (curso) {
            var cursoRows = gc.map[curso];
            var html = '<div class="drill-node">' + drillNodo("lvl0", curso, drillAgg(cursoRows)) +
                       '<div class="drill-children" style="display:none">';

            var gs = grupoOrden(cursoRows, function (r) { return r.Sec_Descripcion || "Sin sección"; });
            gs.order.sort(function (a, b) { return drillAgg(gs.map[b]).pend - drillAgg(gs.map[a]).pend; });

            gs.order.forEach(function (sec) {
                var secRows = gs.map[sec];
                html += '<div class="drill-node">' + drillNodo("lvl1", sec, drillAgg(secRows)) +
                        '<div class="drill-children" style="display:none">';

                var ga = grupoOrden(secRows, function (r) { return r.Alu_Id; });
                // ordena alumnos por monto pendiente desc
                ga.order.sort(function (a, b) { return drillAgg(ga.map[b]).pend - drillAgg(ga.map[a]).pend; });

                ga.order.forEach(function (aluId) {
                    var aluRows = ga.map[aluId];
                    var a = drillAgg(aluRows);
                    var dias = 0;
                    aluRows.forEach(function (r) { var d = parseInt(r.DiasVencida, 10) || 0; if (d > dias) dias = d; });
                    var diasChip = dias > 0 ? '<span class="drill-dias venc">' + dias + ' d</span>' : '<span class="drill-dias">al día</span>';

                    html += '<div class="drill-row lvl2">' +
                        '<span class="drill-name">' + esc(aluRows[0].Alu_NombreCompleto || "—") + '</span>' +
                        '<span class="drill-metric">' + fmt(a.pend) + '</span>' +
                        '<span class="drill-metric">' + diasChip + '</span>' +
                        '<span class="drill-actions">' + drillBtnEC(aluRows[0]) + drillBtnPago(aluRows[0]) + '</span>' +
                        '</div>';
                });

                html += '</div></div>';
            });

            html += '</div></div>';
            cont.append(html);
        });
    }

    // ─── TAB 3: Top Deudores (derivado de pendRows) ──────────────────────────────
    function cargarTop() {
        cargada.top = true;
        cargarPendientes(renderTop);
    }

    function renderTop() {
        var base = pendRows.filter(function (r) { return num(r.Cco_MontoPendiente) > 0; });

        // Agrupa por alumno
        var byAlu = {};
        base.forEach(function (r) {
            var k = r.Alu_Id;
            if (!byAlu[k]) byAlu[k] = { nombre: r.Alu_NombreCompleto || "—", deuda: 0, cuotas: 0, row: r };
            byAlu[k].deuda += num(r.Cco_MontoPendiente);
            byAlu[k].cuotas++;
        });
        var arr = Object.keys(byAlu).map(function (k) { return byAlu[k]; });
        arr.sort(function (a, b) { return b.deuda - a.deuda; });
        arr = arr.slice(0, 25); // top 25

        var rank = $("#cartera-top-rank").empty();
        $("#cartera-top-chip").text(arr.length + (arr.length === 1 ? " alumno" : " alumnos"));

        if (arr.length === 0) {
            rank.html('<div class="fd-empty"><i class="fa-solid fa-user-group"></i><p>Sin deudores registrados</p></div>');
            return;
        }
        arr.forEach(function (d, i) {
            rank.append(
                '<div class="fd-rank-item">' +
                    '<div class="fd-rank-num">' + (i + 1) + '</div>' +
                    '<div>' +
                        '<p class="fd-rank-name">' + esc(d.nombre) + '</p>' +
                        '<p class="fd-rank-sub">' + d.cuotas + (d.cuotas === 1 ? ' cuota pendiente' : ' cuotas pendientes') + '</p>' +
                    '</div>' +
                    '<div class="fd-rank-amt">' + esc(fmt(d.deuda)) + '</div>' +
                    '<div style="padding-left:12px;">' + btnPago(d.row) + '</div>' +
                '</div>'
            );
        });
    }

    // ─── TAB 4: Pendientes ───────────────────────────────────────────────────────
    // Estado de carga de pendRows: la data se pide UNA vez y todos los
    // consumidores (KPIs + cada tab) se encolan hasta que llega.
    var pendState = "idle"; // idle | loading | ready
    var pendCbs = [];
    function cargarPendientes(callback) {
        if (pendState === "ready") { if (callback) callback(); return; }
        if (callback) pendCbs.push(callback);
        if (pendState === "loading") return;
        pendState = "loading";
        $.get(urls.urlPend, function (resp) {
            pendRows = rows(resp);
            poblarConceptos(pendRows);
            pendState = "ready";
            var cbs = pendCbs; pendCbs = [];
            cbs.forEach(function (cb) { cb(); });
        }).fail(function () {
            pendRows = [];
            pendState = "ready";
            var cbs = pendCbs; pendCbs = [];
            cbs.forEach(function (cb) { cb(); });
        });
    }

    function poblarConceptos(data) {
        var set = {};
        data.forEach(function (r) { if (r.Cpa_Descripcion) set[r.Cpa_Descripcion] = true; });
        var sel = $("#cartera-pend-concepto");
        sel.find("option:not([value=''])").remove();
        Object.keys(set).sort().forEach(function (c) {
            sel.append('<option value="' + esc(c) + '">' + esc(c) + '</option>');
        });
    }

    function aging5(dias) {
        if (dias <= 0) return 0;
        if (dias <= 30) return 1;
        if (dias <= 60) return 2;
        if (dias <= 90) return 3;
        return 4;
    }
    var PEND_BUCKETS = ["Al día / por vencer", "1 - 30 días", "31 - 60 días", "61 - 90 días", "+90 días"];

    function renderPendientes() {
        var q = ($("#cartera-pend-buscar").val() || "").toLowerCase().trim();
        var estado = $("#cartera-pend-estado").val();
        var concepto = $("#cartera-pend-concepto").val();

        var data = pendRows.filter(function (r) {
            if (concepto && (r.Cpa_Descripcion || "") !== concepto) return false;
            if (q) {
                var hay = (r.Alu_NombreCompleto || "").toLowerCase().indexOf(q) >= 0
                       || (r.Cpa_Descripcion || "").toLowerCase().indexOf(q) >= 0;
                if (!hay) return false;
            }
            var dias = parseInt(r.DiasVencida, 10);
            if (isNaN(dias)) dias = 0;
            if (estado === "vencido" && dias <= 0) return false;
            if (estado === "hoy" && dias !== 0) return false;
            if (estado === "proximo" && !(dias < 0 && dias >= -7)) return false;
            return true;
        });

        // Aging buckets
        var total = 0;
        var bk = [{ c: 0, v: 0 }, { c: 0, v: 0 }, { c: 0, v: 0 }, { c: 0, v: 0 }, { c: 0, v: 0 }];
        var conceptos = {};
        data.forEach(function (r) {
            var pend = num(r.Cco_MontoPendiente);
            total += pend;
            var dias = parseInt(r.DiasVencida, 10);
            if (isNaN(dias)) dias = 0;
            var bi = aging5(dias);
            bk[bi].c++; bk[bi].v += pend;
            var c = r.Cpa_Descripcion || "Sin concepto";
            conceptos[c] = (conceptos[c] || 0) + pend;
        });

        var agingWrap = $("#cartera-pend-aging").empty();
        PEND_BUCKETS.forEach(function (label, i) {
            agingWrap.append(
                '<div class="fd-aging ' + AGING_CLS[i] + '">' +
                    '<p class="fd-aging-label">' + esc(label) + '</p>' +
                    '<p class="fd-aging-amount">' + esc(fmt(bk[i].v)) + '</p>' +
                    '<p class="fd-aging-count">' + bk[i].c + (bk[i].c === 1 ? ' cuenta' : ' cuentas') + '</p>' +
                '</div>'
            );
        });
        $("#cartera-pend-criticas").text(bk[4].c + " críticas (+90 d)");
        $("#cartera-pend-count").text(data.length + (data.length === 1 ? " cuenta" : " cuentas"));

        // Conceptos bars
        var concArr = Object.keys(conceptos).map(function (k) { return { name: k, val: conceptos[k] }; });
        concArr.sort(function (a, b) { return b.val - a.val; });
        concArr = concArr.slice(0, 7);
        var bars = $("#cartera-pend-conceptos").empty();
        if (concArr.length === 0) {
            bars.html('<div class="fd-empty"><i class="fa-solid fa-tags"></i><p>Sin conceptos pendientes</p></div>');
        } else {
            concArr.forEach(function (it, idx) {
                var pct = total > 0 ? (it.val / total * 100) : 0;
                bars.append(
                    '<div class="fd-bar-item">' +
                        '<div class="fd-bar-top"><span class="fd-bar-name">' + esc(it.name) + '</span>' +
                        '<span class="fd-bar-val">' + esc(fmt(it.val)) + ' · ' + pct.toFixed(1) + '%</span></div>' +
                        '<div class="fd-bar-track"><div class="fd-bar-fill ' + BAR_CLS[idx % BAR_CLS.length] + '" style="width:' + pct.toFixed(1) + '%"></div></div>' +
                    '</div>'
                );
            });
        }

        // Feed
        var sorted = data.slice().sort(function (a, b) {
            return (parseInt(b.DiasVencida, 10) || 0) - (parseInt(a.DiasVencida, 10) || 0);
        });
        var feed = $("#cartera-pend-feed").empty();
        if (sorted.length === 0) {
            feed.html('<div class="fd-empty"><i class="fa-solid fa-circle-check"></i><p>No hay cuentas pendientes con los filtros actuales</p></div>');
            return;
        }
        sorted.forEach(function (r, idx) {
            var dias = parseInt(r.DiasVencida, 10);
            var chip, chipCls;
            if (isNaN(dias)) { chip = "Sin fecha"; chipCls = ""; }
            else if (dias > 30) { chip = dias + " días vencido"; chipCls = "c-rose"; }
            else if (dias > 0) { chip = dias + " días vencido"; chipCls = "c-amber"; }
            else if (dias === 0) { chip = "Vence hoy"; chipCls = "c-amber"; }
            else { chip = Math.abs(dias) + " días por vencer"; chipCls = "c-cyan"; }

            var origv = num(r.Cco_MontoOriginal);
            var pagv = num(r.Cco_MontoTotal);
            var pendv = num(r.Cco_MontoPendiente);
            var pct = origv > 0 ? Math.min(100, (pagv / origv) * 100) : 0;

            feed.append(
                '<div class="fd-feed-item">' +
                    '<div class="fd-feed-avatar ' + avatarPalette(idx) + '">' + esc(initials(r.Alu_NombreCompleto)) + '</div>' +
                    '<div class="fd-feed-body">' +
                        '<p class="fd-feed-title">' + esc(r.Alu_NombreCompleto || "—") + '</p>' +
                        '<p class="fd-feed-meta">' +
                            '<span><i class="fa-solid fa-tag"></i>' + esc(r.Cpa_Descripcion || "—") + '</span>' +
                            '<span><i class="fa-solid fa-coins"></i>' + esc(fmt(origv)) + ' original</span>' +
                            '<span><i class="fa-solid fa-circle-check"></i>' + esc(fmt(pagv)) + ' pagado (' + pct.toFixed(0) + '%)</span>' +
                        '</p>' +
                    '</div>' +
                    '<div class="fd-feed-right">' +
                        '<p class="fd-feed-amount amt-rose">' + esc(fmt(pendv)) + '</p>' +
                        '<span class="fd-chip ' + chipCls + '">' + esc(chip) + '</span>' +
                        '<div style="margin-top:6px;">' + btnPago(r) + '</div>' +
                    '</div>' +
                '</div>'
            );
        });
    }

    return obj;
}());
