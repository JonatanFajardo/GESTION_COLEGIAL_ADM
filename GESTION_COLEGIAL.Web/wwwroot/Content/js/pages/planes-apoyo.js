/* Planes de apoyo — directorio (indicadores, filtros con cascada Nivel → Grado → Sección, exportar),
   "Nuevo plan" (buscador de alumnos, aviso de plan abierto, validación en vivo) y el plan (cambiar
   estado con motivo, editar datos, pasos, notas). Todo texto que escribió una persona se escapa
   antes de pintarlo. */
(function ($) {
    "use strict";

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    /* Sin tildes y en minúsculas, para buscar "jose" y encontrar "José". */
    function normalizar(s) {
        return String(s || "").normalize("NFD").replace(/[̀-ͯ]/g, "").toLowerCase();
    }

    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];
    function fecha(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v || "");
        return m ? (+m[3]) + " " + MESES[+m[2] - 1] + " " + m[1] : "";
    }
    function iso(v) { var m = /^(\d{4}-\d{2}-\d{2})/.exec(v || ""); return m ? m[1] : ""; }

    function iniciales(nombre) {
        return String(nombre || "").split(" ").filter(Boolean).slice(0, 2)
            .map(function (p) { return p.charAt(0); }).join("").toUpperCase();
    }

    var CATEGORIAS = {
        refuerzo: { texto: "Refuerzo académico", clase: "outline-badge-primary" },
        orientacion: { texto: "Orientación", clase: "outline-badge-info" },
        conducta: { texto: "Conducta", clase: "outline-badge-warning" },
        salud: { texto: "Salud", clase: "outline-badge-danger" },
        adecuacion: { texto: "Adecuación curricular", clase: "outline-badge-success" }
    };
    var ESTADOS = {
        borrador: { texto: "Borrador", clase: "badge-secondary" },
        activo: { texto: "Activo", clase: "badge-success" },
        en_pausa: { texto: "En pausa", clase: "badge-warning" },
        completado: { texto: "Completado", clase: "badge-info" },
        cancelado: { texto: "Cancelado", clase: "badge-dark" }
    };
    var ABIERTOS = ["borrador", "activo", "en_pausa"];

    function token() { return $('input[name="__RequestVerificationToken"]').first().val(); }

    function enviar(url, datos) {
        return $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(datos),
            headers: { RequestVerificationToken: token() },
            dataType: "json"
        });
    }

    /* Ir a la página que devuelve el servidor. Si solo cambia el "#ancla" (misma página), el navegador
       no recarga: entonces se recarga a mano para ver el paso o la nota recién agregada. */
    function ir(url) {
        var a = document.createElement("a");
        a.href = url;
        if (a.pathname.toLowerCase() === location.pathname.toLowerCase() && a.search === location.search) {
            if (a.hash) history.replaceState(null, "", a.hash);
            location.reload();
        } else {
            location.href = url;
        }
    }

    function errorGeneral(mensaje) {
        var $g = $("#pa-error-general");
        if (!mensaje) { $g.attr("hidden", true).text(""); return; }
        $g.text(mensaje).removeAttr("hidden");
        window.scrollTo({ top: Math.max(0, $g.offset().top - 90), behavior: "smooth" });
    }

    /* Errores por campo: <p data-error="campo"> y el control con el mismo id. */
    function marcar($form, campo, mensaje) {
        $form.find("#" + campo).addClass("is-invalid").attr("aria-invalid", "true");
        $form.find('[data-error="' + campo + '"]').text(mensaje || "");
    }
    function desmarcar($form, campo) {
        $form.find("#" + campo).removeClass("is-invalid").removeAttr("aria-invalid");
        $form.find('[data-error="' + campo + '"]').text("");
    }

    /* ═══════════════════════ Directorio ═══════════════════════ */
    var $tabla = $("#tabla-planes-apoyo");
    if ($tabla.length) {
        var urlPlan = String($tabla.data("url-plan") || "/PlanesApoyo/Plan");
        var $estado = $("#pa-estado"), $categoria = $("#pa-categoria");
        var $nivel = $("#pa-nivel"), $grado = $("#pa-grado"), $seccion = $("#pa-seccion");
        var $buscar = $("#pa-buscar"), $soloVencidos = $("#pa-solo-vencidos"), $soloMios = $("#pa-solo-mios");
        var num = function (v) { var n = parseInt(v, 10); return isNaN(n) ? null : n; };

        // ── Indicadores (la API ya excluye lo confidencial ajeno) ──
        $.getJSON($tabla.data("url-resumen")).done(function (r) {
            var d = (r && r.data) || {};
            var n = function (v) { return (v || 0).toLocaleString("es-HN"); };
            $("#kpi-activos").text(n(d.Activos));
            $("#kpi-activos-texto").text(n(d.Abiertos) + " abiertos en total (borrador, activo o en pausa)");
            $("#kpi-vencidos").text(n(d.RevisionVencida));
            $("#kpi-vencidos-ver").prop("hidden", !d.RevisionVencida);
            $("#kpi-atrasados").text(n(d.PasosAtrasados));
            $("#kpi-completados").text(n(d.CompletadosAnio));
            if (d.AnioNombre) $("#kpi-completados-titulo").text("Completados en " + d.AnioNombre);
        });

        // ── Cascada Nivel → Grado → Sección (opciones pedidas al servidor según el padre) ──
        function llenar($select, items, vacio) {
            $select.empty().append($("<option>", { value: "", text: vacio }));
            (items || []).forEach(function (it) { $select.append($("<option>", { value: it.Value, text: it.Text })); });
        }
        function sinPadre($select, texto) { llenar($select, [], texto); $select.prop("disabled", true); }

        $nivel.on("change", function () {
            sinPadre($grado, "Elija un nivel primero");
            sinPadre($seccion, "Elija un grado");
            aplicar();
            var niv = num($nivel.val());
            if (niv === null) return;
            $grado.find("option").first().text("Cargando…");
            $.getJSON($tabla.data("url-grados"), { id: niv }).done(function (r) {
                if (num($nivel.val()) !== niv) return;   // cambió mientras cargaba
                llenar($grado, (r && r.data) || [], "Todos los grados");
                $grado.prop("disabled", false);
            }).fail(function () { sinPadre($grado, "No se pudieron cargar"); });
        });

        $grado.on("change", function () {
            sinPadre($seccion, "Elija un grado");
            aplicar();
            var cun = num($grado.val());
            if (cun === null) return;
            $seccion.find("option").first().text("Cargando…");
            $.getJSON($tabla.data("url-secciones"), { id: cun }).done(function (r) {
                if (num($grado.val()) !== cun) return;
                llenar($seccion, (r && r.data) || [], "Todas");
                $seccion.prop("disabled", false);
            }).fail(function () { sinPadre($seccion, "No se pudieron cargar"); });
        });

        function pasaFiltros(d) {
            var est = $estado.val();
            if (est === "abiertos" && ABIERTOS.indexOf(d.Apo_Estado) < 0) return false;
            if (est && est !== "abiertos" && est !== "todos" && d.Apo_Estado !== est) return false;
            if ($categoria.val() && d.Apo_Categoria !== $categoria.val()) return false;
            var niv = num($nivel.val()), cun = num($grado.val()), sec = num($seccion.val());
            if (niv !== null && d.Niv_Id !== niv) return false;
            if (cun !== null && d.Cun_Id !== cun) return false;
            if (sec !== null && d.Sec_Id !== sec) return false;
            if ($soloVencidos.prop("checked") && !d.RevisionVencida) return false;
            if ($soloMios.prop("checked") && !d.EsResponsable && !d.TienePasos) return false;
            return true;
        }

        function hayFiltros() {
            return $estado.val() !== "abiertos" || !!$categoria.val() || !!$nivel.val() || !!$buscar.val()
                || $soloVencidos.prop("checked") || $soloMios.prop("checked");
        }

        function aplicar() {
            t.filtrar();
            $("#pa-limpiar").prop("hidden", !hayFiltros());
        }

        var t = GcTabla.crear({
            elemento: "#tabla-planes-apoyo",
            nombre: "planes-apoyo",
            url: $tabla.data("url"),
            archivo: "planes-de-apoyo",
            tituloPdf: "Planes de apoyo",
            buscador: "#pa-buscar",
            buscarEn: ["Alumno", "Identidad", "Apo_Titulo", "Responsable", "Ubicacion", "CategoriaTexto"],
            filasPorPagina: 20,
            vacio: "Ningún plan coincide con los filtros",
            agrupar: [{ campos: ["Ubicacion"], texto: "Grado y sección" }, { campos: ["CategoriaTexto"], texto: "Categoría" }],
            filtroExtra: pasaFiltros,
            // Los filtros que llegan en el enlace (?estado=…&vencidos=true) se aplican al tener datos.
            alCargar: function () { aplicar(); },
            // 6 columnas a la vista para que quepan a 1366 px; grado y categoría también van como columnas
            // ocultas (para exportarlas y para "Agrupar por"; el usuario las puede mostrar en "Columnas").
            ocultas: ["Ubicacion", "CategoriaTexto"],
            columnas: [
                { title: "Alumno", field: "Alumno", minWidth: 165, widthGrow: 1.6,
                  titleDownload: "Alumno (identidad)",
                  accessorDownload: function (v, d) { return d.Identidad ? v + " (" + d.Identidad + ")" : v; },
                  formatter: function (c) {
                      var d = c.getData(), nombre = esc(d.Alumno);
                      var enlace = '<a class="pa-enlace" href="' + esc(urlPlan + "/" + d.Apo_Id) + '">' + nombre + "</a>";
                      var ubic = [d.Grado, d.Seccion].filter(Boolean).join(" · ") || "Sin grado";
                      return GcTabla.avatar(nombre, d.Foto ? esc(d.Foto) : null).replace("<span>" + nombre + "</span>",
                          '<span class="pa-nombre">' + enlace + '<span class="pa-sub">' + esc(ubic) + "</span></span>");
                  } },
                { title: "Plan", field: "Apo_Titulo", minWidth: 180, widthGrow: 2,
                  formatter: function (c) {
                      var d = c.getData(), cat = CATEGORIAS[d.Apo_Categoria] || { texto: d.Apo_Categoria, clase: "outline-badge-secondary" };
                      return '<div><span class="badge ' + cat.clase + ' mb-1">' + esc(cat.texto) + "</span>"
                          + (d.Apo_EsConfidencial ? ' <span class="badge outline-badge-danger mb-1"><i class="bi bi-lock-fill mr-1" aria-hidden="true"></i>Confidencial</span>' : "")
                          + '<span class="d-block">' + esc(d.Apo_Titulo) + "</span></div>";
                  } },
                { title: "Responsable", field: "Responsable", minWidth: 150, widthGrow: 1.2,
                  formatter: function (c) { return c.getValue() ? esc(c.getValue()) : '<span class="text-muted">—</span>'; } },
                { title: "Revisión", field: "RevisionTexto", minWidth: 120,
                  mutator: function (v, d) {
                      if (ABIERTOS.indexOf(d.Apo_Estado) < 0) return d.Apo_FechaFin ? "Terminó el " + fecha(d.Apo_FechaFin) : "Terminado";
                      return fecha(d.Apo_FechaRevision) || "Sin fecha";
                  },
                  sorter: function (a, b, fa, fb) {
                      return String(iso(fa.getData().Apo_FechaRevision) || "9999").localeCompare(String(iso(fb.getData().Apo_FechaRevision) || "9999"));
                  },
                  formatter: function (c) {
                      var d = c.getData();
                      if (ABIERTOS.indexOf(d.Apo_Estado) < 0) return '<span class="text-muted">' + esc(c.getValue()) + "</span>";
                      if (d.RevisionVencida)
                          return '<div><span class="pa-vencida">' + esc(c.getValue()) + '</span><span class="d-block"><span class="badge badge-danger">Vencida</span></span></div>';
                      return esc(c.getValue());
                  } },
                { title: "Pasos", field: "PasosTexto", minWidth: 110, headerSort: false,
                  mutator: function (v, d) { return d.PasosHechos + " de " + d.PasosTotal; },
                  formatter: function (c) {
                      var d = c.getData();
                      if (!d.PasosTotal) return '<span class="text-muted">Sin pasos</span>';
                      var pct = Math.round(d.PasosHechos * 100 / d.PasosTotal);
                      var html = '<div class="w-100">' + esc(c.getValue())
                          + '<div class="pa-mini-barra" aria-hidden="true"><span style="width:' + pct + '%"></span></div>';
                      if (d.PasosAtrasados) html += '<span class="pa-sub pa-vencida">' + d.PasosAtrasados + (d.PasosAtrasados === 1 ? " atrasado" : " atrasados") + "</span>";
                      if (d.MisPasosPendientes) html += '<span class="pa-sub">' + d.MisPasosPendientes + (d.MisPasosPendientes === 1 ? " suyo pendiente" : " suyos pendientes") + "</span>";
                      return html + "</div>";
                  } },
                { title: "Estado", field: "EstadoTexto", minWidth: 110,
                  mutator: function (v, d) { return (ESTADOS[d.Apo_Estado] || {}).texto || d.Apo_Estado; },
                  formatter: function (c) {
                      var e = ESTADOS[c.getData().Apo_Estado] || { clase: "badge-secondary" };
                      return '<span class="badge ' + e.clase + '">' + esc(c.getValue()) + "</span>";
                  } },
                { title: "Grado", field: "Ubicacion", download: true,
                  mutator: function (v, d) { return [d.Grado, d.Seccion].filter(Boolean).join(" · ") || "Sin grado"; },
                  formatter: function (c) { return esc(c.getValue()); } },
                { title: "Categoría", field: "CategoriaTexto", download: true,
                  mutator: function (v, d) { return (CATEGORIAS[d.Apo_Categoria] || {}).texto || d.Apo_Categoria; },
                  formatter: function (c) { return esc(c.getValue()); } }
            ]
        });

        // Clic en la fila (fuera de la casilla y de los enlaces) → el plan.
        t.tabla.on("rowClick", function (e, row) {
            if ($(e.target).closest("a, button, input, label, .tabulator-row-header").length) return;
            window.location.href = urlPlan + "/" + row.getData().Apo_Id;
        });
        t.tabla.on("dataFiltered", function (f, rows) { $("#pa-total").text(rows.length.toLocaleString("es-HN")); });

        $estado.add($categoria).add($seccion).add($soloVencidos).add($soloMios).on("change", aplicar);
        $buscar.on("input", function () { $("#pa-limpiar").prop("hidden", !hayFiltros()); });

        $("#kpi-vencidos-ver").on("click", function (e) {
            e.preventDefault();
            $estado.val("abiertos");
            $soloVencidos.prop("checked", true);
            aplicar();
            document.getElementById("tabla-planes-apoyo").scrollIntoView({ behavior: "smooth", block: "start" });
        });

        $("#pa-limpiar").on("click", function () {
            $estado.val("abiertos");
            $categoria.val("");
            $nivel.val("");
            sinPadre($grado, "Elija un nivel primero");
            sinPadre($seccion, "Elija un grado");
            $buscar.val("");
            $soloVencidos.add($soloMios).prop("checked", false);
            aplicar();
        });

        $("#pa-exportar").on("click", function () {
            t.tabla.download("xlsx", "planes-de-apoyo.xlsx", { sheetName: "Planes de apoyo" }, "active");
        });
    }

    /* ═══════════════════════ Nuevo plan ═══════════════════════ */
    var $nuevo = $("#pa-form-nuevo");
    if ($nuevo.length) {
        var alumnos = [], elegido = null, enviando = false, sucio = false, confTocado = false, activar = true;
        var $q = $("#pa-alumno-buscar"), $res = $("#pa-alumno-resultados");
        var confidenciales = String($nuevo.data("confidenciales") || "").split(",");

        function pintarResultados() {
            var q = normalizar($q.val()).trim();
            $res.empty();
            if (q.length < 2) return;
            var digitos = q.replace(/\D/g, "");
            var partes = q.split(/\s+/).filter(Boolean);
            var hallados = alumnos.filter(function (a) {
                if (digitos.length >= 2 && /^\d[\d-\s]*$/.test(q)) return String(a.Identidad || "").replace(/\D/g, "").indexOf(digitos) >= 0;
                var n = a._n;
                return partes.every(function (p) { return n.indexOf(p) >= 0; });
            }).slice(0, 20);
            if (!hallados.length) {
                $res.append('<div class="list-group-item text-muted">Ningún alumno activo coincide.</div>');
                return;
            }
            hallados.forEach(function (a, i) {
                $res.append('<button type="button" class="list-group-item list-group-item-action" role="option" data-alu="' + a.Alu_Id + '"'
                    + (i === 0 ? ' aria-selected="true"' : "") + ">"
                    + '<strong>' + esc(a.Nombre) + "</strong>"
                    + '<span class="pa-sub">' + esc([a.Ubicacion, a.Nivel].filter(Boolean).join(" · ") || "Sin grado")
                    + (a.Identidad ? " · " + esc(a.Identidad) : "") + "</span></button>");
            });
        }

        function elegir(a) {
            elegido = a;
            $("#pa-buscador").attr("hidden", true);
            $("#pa-alumno-iniciales").text(iniciales(a.Nombre));
            $("#pa-alumno-nombre").text(a.Nombre);
            $("#pa-alumno-detalle").text([a.Ubicacion, a.Nivel, a.Identidad ? "Identidad " + a.Identidad : ""].filter(Boolean).join(" · "));
            $("#pa-alumno-elegido").removeAttr("hidden");
            desmarcar($nuevo, "alumno");
            $res.empty();
            sucio = true;
            // Aviso (sin bloquear) si ya tiene un plan abierto.
            var $aviso = $("#pa-ya-tiene").attr("hidden", true).empty();
            $.getJSON($nuevo.data("url-abiertos"), { aluId: a.Alu_Id }).done(function (r) {
                var planes = (r && r.data) || [];
                if (!elegido || elegido.Alu_Id !== a.Alu_Id || !planes.length) return;
                var html = '<i class="bi bi-exclamation-triangle mr-1" aria-hidden="true"></i><strong>'
                    + (planes.length === 1 ? "Este alumno ya tiene un plan abierto:" : "Este alumno ya tiene " + planes.length + " planes abiertos:")
                    + "</strong><ul class=\"mb-1 mt-1 pl-3\">";
                planes.forEach(function (p) {
                    html += '<li><a href="' + esc(p.Url) + '">' + esc(p.Apo_Titulo) + "</a> · " + esc(p.Categoria) + " · " + esc(p.Estado) + "</li>";
                });
                html += "</ul>Puede seguir y abrir otro, o agregar pasos al que ya tiene.";
                $aviso.html(html).removeAttr("hidden");
            });
        }

        $.getJSON($nuevo.data("url-alumnos")).done(function (r) {
            alumnos = ((r && r.data) || []).map(function (a) { a._n = normalizar(a.Nombre); return a; });
            $q.prop("disabled", false).attr("placeholder", "Ej. María López o 0801");
            var pre = parseInt($nuevo.data("alumno"), 10);
            if (pre) {
                var a = alumnos.filter(function (x) { return x.Alu_Id === pre; })[0];
                if (a) elegir(a); else marcar($nuevo, "alumno", "Ese alumno no está activo: solo se abren planes a alumnos con matrícula activa.");
                sucio = false;
            }
        }).fail(function () {
            $q.attr("placeholder", "No se pudieron cargar los alumnos");
        });

        $q.on("input", pintarResultados).on("keydown", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                $res.find("[data-alu]").first().trigger("click");
            } else if (e.key === "ArrowDown") {
                e.preventDefault();
                $res.find("[data-alu]").first().trigger("focus");
            }
        });
        $res.on("keydown", "[data-alu]", function (e) {
            if (e.key === "ArrowDown") { e.preventDefault(); $(this).next("[data-alu]").trigger("focus"); }
            if (e.key === "ArrowUp") { e.preventDefault(); var $p = $(this).prev("[data-alu]"); ($p.length ? $p : $q).trigger("focus"); }
        });
        $res.on("click", "[data-alu]", function () {
            var id = parseInt($(this).data("alu"), 10);
            var a = alumnos.filter(function (x) { return x.Alu_Id === id; })[0];
            if (a) elegir(a);
        });
        $("#pa-alumno-cambiar").on("click", function () {
            elegido = null;
            $("#pa-alumno-elegido, #pa-ya-tiene").attr("hidden", true);
            $("#pa-buscador").removeAttr("hidden");
            $q.val("").trigger("focus");
        });

        // Categoría: Salud y Orientación se marcan confidenciales (si la persona no tocó el interruptor).
        function confidencialPorCategoria() {
            if (!confTocado) $("#confidencial").prop("checked", confidenciales.indexOf($("#categoria").val()) >= 0);
        }
        $("#categoria").on("change", function () { confidencialPorCategoria(); if ($(this).val()) desmarcar($nuevo, "categoria"); });
        $("#confidencial").on("change", function () { confTocado = true; });
        confidencialPorCategoria();

        function contar() { $("#titulo-cuenta").text(($("#titulo").val() || "").length + "/150"); }
        $("#titulo").on("input", function () { contar(); if ($(this).val().trim()) desmarcar($nuevo, "titulo"); });
        contar();

        function validarFechas() {
            var ini = $("#inicio").val(), rev = $("#revision").val(), ok = true;
            if (!ini) { marcar($nuevo, "inicio", "Indique cuándo empieza el plan."); ok = false; } else desmarcar($nuevo, "inicio");
            if (!rev) { marcar($nuevo, "revision", "Indique cuándo se revisa el plan."); ok = false; }
            else if (ini && rev < ini) { marcar($nuevo, "revision", "La revisión no puede ser antes del inicio del plan."); ok = false; }
            else desmarcar($nuevo, "revision");
            if (ini) $("#revision").attr("min", ini);
            return ok;
        }
        $("#inicio, #revision").on("change blur", validarFechas);
        $("#responsable").on("change", function () { if ($(this).val()) desmarcar($nuevo, "responsable"); });
        $nuevo.on("input change", "input, select, textarea", function () { sucio = true; });

        function validarTodo() {
            var ok = true;
            if (!elegido) { marcar($nuevo, "alumno", "Elija el alumno."); ok = false; }
            if (!$("#categoria").val()) { marcar($nuevo, "categoria", "Elija la categoría del plan."); ok = false; }
            if (!($("#titulo").val() || "").trim()) { marcar($nuevo, "titulo", "Escriba el objetivo del plan."); ok = false; }
            ok = validarFechas() && ok;
            if (!$("#responsable").val()) { marcar($nuevo, "responsable", "Elija al responsable del plan."); ok = false; }
            if (!ok) {
                var $primero = $nuevo.find(".is-invalid").first();
                if (!elegido) $q.trigger("focus"); else $primero.trigger("focus");
            }
            return ok;
        }

        $nuevo.find("[type=submit]").on("click", function () { activar = $(this).data("activar") === 1 || $(this).data("activar") === "1"; });

        $nuevo.on("submit", function (e) {
            e.preventDefault();
            if (enviando) return;
            errorGeneral(null);
            if (!validarTodo()) return;
            enviando = true;
            var $botones = $nuevo.find("[type=submit]").prop("disabled", true);
            enviar($nuevo.data("url"), {
                Alu_Id: elegido.Alu_Id,
                Apo_Categoria: $("#categoria").val(),
                Apo_Titulo: $("#titulo").val(),
                Apo_Resumen: $("#resumen").val(),
                Apo_FechaInicio: $("#inicio").val(),
                Apo_FechaRevision: $("#revision").val(),
                Usu_IdResponsable: parseInt($("#responsable").val(), 10),
                Apo_EsConfidencial: $("#confidencial").prop("checked"),
                Activar: activar
            }).done(function (r) {
                if (r && r.success) { sucio = false; ir(r.redirect); return; }
                if (r && r.field && $nuevo.find('[data-error="' + r.field + '"]').length) {
                    marcar($nuevo, r.field, r.message);
                    if (r.field === "alumno") $q.trigger("focus"); else $("#" + r.field).trigger("focus");
                } else errorGeneral((r && r.message) || "No se pudo abrir el plan.");
            }).fail(function () {
                errorGeneral("No se pudo conectar. Revise la conexión e intente de nuevo.");
            }).always(function () {
                enviando = false;
                $botones.prop("disabled", false);
            });
        });

        $(window).on("beforeunload", function (e) {
            if (sucio && !enviando) { e.preventDefault(); return ""; }
        });
    }

    /* ═══════════════════════ El plan ═══════════════════════ */
    var $plan = $("#pa-plan");
    if ($plan.length) {
        var apoId = parseInt($plan.data("plan"), 10);
        var estadoActual = String($plan.data("estado") || "");
        var ocupado = false;

        function resultado(r, $form, general) {
            if (r && r.success) { ir(r.redirect); return true; }
            if ($form && r && r.field && $form.find('[data-error="' + r.field + '"]').length) {
                marcar($form, r.field, r.message);
                $form.find("#" + r.field).trigger("focus");
            } else errorGeneral((r && r.message) || general);
            return false;
        }

        // ── Cambiar estado (ventana con motivo; obligatorio al pausar, cancelar y reabrir) ──
        var TEXTOS = {
            activo: "El plan empieza a aplicarse. Los responsables verán sus pasos.",
            reanudar: "El plan vuelve a aplicarse después de la pausa.",
            reabrir: "El plan vuelve a estar activo y acepta pasos y notas otra vez.",
            en_pausa: "El plan se detiene por un tiempo, sin terminarse.",
            completado: "El alumno ya no necesita el plan. No se podrán agregar pasos ni notas, salvo que se reabra.",
            cancelado: "El plan se deja sin terminar. Un plan cancelado ya no se puede reabrir."
        };
        var $modal = $("#pa-modal-estado");
        $("[data-transicion]").on("click", function () {
            var destino = String($(this).data("transicion")), pide = String($(this).data("pide-motivo")) === "1";
            var clave = destino === "activo" ? (estadoActual === "completado" ? "reabrir" : estadoActual === "en_pausa" ? "reanudar" : "activo") : destino;
            $("#pa-modal-destino").val(destino).data("pide", pide);
            $("#pa-modal-titulo").text($(this).data("accion") + " el plan");
            $("#pa-modal-texto").text(TEXTOS[clave] || "");
            $("#pa-modal-motivo-etiqueta").html(pide ? 'Motivo <span class="text-danger">*</span>' : "Comentario (opcional)");
            $("#pa-modal-motivo").val("").removeClass("is-invalid").attr("placeholder", pide ? "Por qué" : "Algo que deba quedar en el historial");
            $("#pa-modal-error").text("");
            $("#pa-modal-confirmar").text($(this).data("accion"))
                .toggleClass("btn-danger", destino === "cancelado").toggleClass("btn-primary", destino !== "cancelado");
            $modal.modal("show");
        });
        $modal.on("shown.bs.modal", function () { $("#pa-modal-motivo").trigger("focus"); });
        $("#pa-modal-confirmar").on("click", function () {
            if (ocupado) return;
            var destino = $("#pa-modal-destino").val(), pide = $("#pa-modal-destino").data("pide");
            var motivo = ($("#pa-modal-motivo").val() || "").trim();
            if (pide && !motivo) {
                $("#pa-modal-motivo").addClass("is-invalid").trigger("focus");
                $("#pa-modal-error").text("Escriba el motivo del cambio.");
                return;
            }
            ocupado = true;
            var $b = $(this).prop("disabled", true);
            enviar($plan.data("url-estado"), { Apo_Id: apoId, Estado: destino, Motivo: motivo }).done(function (r) {
                if (r && r.success) { ir(r.redirect); return; }
                $("#pa-modal-error").text((r && r.message) || "No se pudo cambiar el estado del plan.");
            }).fail(function () {
                $("#pa-modal-error").text("No se pudo conectar. Intente de nuevo.");
            }).always(function () { ocupado = false; $b.prop("disabled", false); });
        });

        // ── Editar datos ──
        var $editar = $("#pa-form-editar");
        if ($editar.length) {
            var originales = {};
            $editar.find("input, select, textarea").each(function () { originales[this.id] = this.type === "checkbox" ? this.checked : $(this).val(); });
            var confPlan = String($plan.data("confidenciales") || "").split(",");

            $("#pa-editar").on("click", function () {
                $editar.removeAttr("hidden");
                $(this).attr("aria-expanded", "true").prop("hidden", true);
                $("#titulo").trigger("focus");
            });
            $("#pa-editar-cancelar").on("click", function () {
                $editar.find("input, select, textarea").each(function () {
                    if (this.type === "checkbox") this.checked = originales[this.id]; else $(this).val(originales[this.id]);
                    desmarcar($editar, this.id);
                });
                $editar.attr("hidden", true);
                $("#pa-editar").attr("aria-expanded", "false").prop("hidden", false);
                errorGeneral(null);
            });
            $editar.find("#categoria").on("change", function () { if (confPlan.indexOf($(this).val()) >= 0) $("#confidencial").prop("checked", true); });
            $editar.find("#inicio, #revision").on("change", function () {
                var ini = $("#inicio").val(), rev = $("#revision").val();
                if (ini && rev && rev < ini) marcar($editar, "revision", "La revisión no puede ser antes del inicio del plan.");
                else desmarcar($editar, "revision");
            });

            $editar.on("submit", function (e) {
                e.preventDefault();
                if (ocupado) return;
                errorGeneral(null);
                var ok = true;
                if (!($("#titulo").val() || "").trim()) { marcar($editar, "titulo", "Escriba el objetivo del plan."); ok = false; } else desmarcar($editar, "titulo");
                if (!$("#inicio").val()) { marcar($editar, "inicio", "Indique cuándo empieza el plan."); ok = false; } else desmarcar($editar, "inicio");
                if (!$("#revision").val()) { marcar($editar, "revision", "Indique cuándo se revisa el plan."); ok = false; }
                else if ($("#inicio").val() && $("#revision").val() < $("#inicio").val()) { marcar($editar, "revision", "La revisión no puede ser antes del inicio del plan."); ok = false; }
                if (!$("#responsable").val()) { marcar($editar, "responsable", "Elija al responsable del plan."); ok = false; }
                if (!ok) { $editar.find(".is-invalid").first().trigger("focus"); return; }
                ocupado = true;
                var $b = $editar.find("[type=submit]").prop("disabled", true);
                enviar($plan.data("url-editar"), {
                    Apo_Id: apoId,
                    Apo_Version: parseInt($plan.data("version"), 10),
                    Apo_Categoria: $("#categoria").val(),
                    Apo_Titulo: $("#titulo").val(),
                    Apo_Resumen: $("#resumen").val(),
                    Apo_FechaInicio: $("#inicio").val(),
                    Apo_FechaRevision: $("#revision").val(),
                    Usu_IdResponsable: parseInt($("#responsable").val(), 10),
                    Apo_EsConfidencial: $("#confidencial").prop("checked")
                }).done(function (r) { resultado(r, $editar, "No se pudieron guardar los cambios del plan."); })
                  .fail(function () { errorGeneral("No se pudo conectar. Revise la conexión e intente de nuevo."); })
                  .always(function () { ocupado = false; $b.prop("disabled", false); });
            });
        }

        // ── Pasos ──
        var $paso = $("#pa-form-paso");
        $paso.on("submit", function (e) {
            e.preventDefault();
            if (ocupado) return;
            errorGeneral(null);
            var desc = ($("#paso-descripcion").val() || "").trim(), resp = $("#paso-responsable").val(), lim = $("#paso-fecha").val(), ok = true;
            if (!desc) { marcar($paso, "paso-descripcion", "Diga qué tiene que pasar."); ok = false; } else desmarcar($paso, "paso-descripcion");
            if (!resp) { marcar($paso, "paso-responsable", "Elija quién lo hace."); ok = false; } else desmarcar($paso, "paso-responsable");
            if (!lim) { marcar($paso, "paso-fecha", "Indique la fecha límite."); ok = false; } else desmarcar($paso, "paso-fecha");
            if (!ok) { $paso.find(".is-invalid").first().trigger("focus"); return; }
            ocupado = true;
            var $b = $paso.find("[type=submit]").prop("disabled", true);
            enviar($plan.data("url-paso"), { Apo_Id: apoId, Apa_Descripcion: desc, Apa_FechaLimite: lim, Usu_IdResponsable: parseInt(resp, 10) })
                .done(function (r) { resultado(r, $paso, "No se pudo agregar el paso."); })
                .fail(function () { errorGeneral("No se pudo conectar. Revise la conexión e intente de nuevo."); })
                .always(function () { ocupado = false; $b.prop("disabled", false); });
        });
        $paso.on("input change", "input, select", function () { if ($(this).val()) desmarcar($paso, this.id); });

        $("[data-completar]").on("click", function () {
            if (ocupado) return;
            ocupado = true;
            var $b = $(this).prop("disabled", true);
            enviar($plan.data("url-completar"), { Apo_Id: apoId, Apa_Id: parseInt($(this).data("completar"), 10) })
                .done(function (r) { resultado(r, null, "No se pudo marcar el paso como hecho."); })
                .fail(function () { errorGeneral("No se pudo conectar. Revise la conexión e intente de nuevo."); })
                .always(function () { ocupado = false; $b.prop("disabled", false); });
        });

        // Confirmación (quitar un paso o eliminar el borrador).
        var confirmar = null;
        function preguntar(titulo, texto, boton, accion) {
            confirmar = accion;
            $("#pa-confirmar-titulo").text(titulo);
            $("#pa-confirmar-texto").text(texto);
            $("#pa-confirmar-error").text("");
            $("#pa-confirmar-si").text(boton);
            $("#pa-modal-pregunta").modal("show");
        }
        $("#pa-confirmar-si").on("click", function () {
            if (ocupado || !confirmar) return;
            ocupado = true;
            var $b = $(this).prop("disabled", true);
            confirmar().done(function (r) {
                if (r && r.success) { ir(r.redirect); return; }
                $("#pa-confirmar-error").text((r && r.message) || "No se pudo completar.");
            }).fail(function () {
                $("#pa-confirmar-error").text("No se pudo conectar. Intente de nuevo.");
            }).always(function () { ocupado = false; $b.prop("disabled", false); });
        });
        $("[data-quitar]").on("click", function () {
            var id = parseInt($(this).data("quitar"), 10);
            preguntar("Quitar paso", "Se quitará el paso «" + String($(this).data("texto") || "") + "». Los pasos hechos no se quitan.", "Sí, quitar",
                function () { return enviar($plan.data("url-quitar"), { Apo_Id: apoId, Apa_Id: id }); });
        });
        $("#pa-eliminar").on("click", function () {
            preguntar("Eliminar borrador", "El borrador se eliminará y dejará de aparecer en los planes de apoyo.", "Sí, eliminar",
                function () { return enviar($plan.data("url-eliminar"), { Apo_Id: apoId }); });
        });

        // ── Notas ──
        var $nota = $("#pa-form-nota");
        $nota.on("submit", function (e) {
            e.preventDefault();
            if (ocupado) return;
            errorGeneral(null);
            var texto = ($("#nota").val() || "").trim();
            if (!texto) { marcar($nota, "nota", "Escriba la nota."); $("#nota").trigger("focus"); return; }
            desmarcar($nota, "nota");
            ocupado = true;
            var $b = $nota.find("[type=submit]").prop("disabled", true);
            enviar($plan.data("url-nota"), { Apo_Id: apoId, Apn_Texto: texto })
                .done(function (r) { resultado(r, $nota, "No se pudo agregar la nota."); })
                .fail(function () { errorGeneral("No se pudo conectar. Revise la conexión e intente de nuevo."); })
                .always(function () { ocupado = false; $b.prop("disabled", false); });
        });
    }

    /* ═══════════════════════ Hoja para imprimir ═══════════════════════ */
    $("[data-imprimir]").on("click", function () { window.print(); });
}(jQuery));
