/* Fichas de salud — directorio (filtros en cascada, contadores, exportar), ficha del alumno
   (modo lectura / edición con validación en vivo) y hoja para imprimir. */
(function ($) {
    "use strict";

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    /* "99998888" → "9999-8888"; otro texto se deja igual. */
    function telefono(t) {
        if (!t) return "";
        var d = String(t).replace(/\D/g, "");
        return d.length === 8 ? d.substr(0, 4) + "-" + d.substr(4) : String(t).trim();
    }

    var MESES = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];
    function fecha(v) {
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(v || "");
        return m ? (+m[3]) + " " + MESES[+m[2] - 1] + " " + m[1] : "";
    }

    /* ═══════════════════════ Directorio ═══════════════════════ */
    var $tabla = $("#tabla-fichas-salud");
    if ($tabla.length) {
        var urlFicha = String($tabla.data("url-ficha") || "/FichasSalud/Ficha");
        var $nivel = $("#fsa-nivel"), $grado = $("#fsa-grado"), $seccion = $("#fsa-seccion");
        var $buscar = $("#fsa-buscar"), $soloSin = $("#fsa-solo-sin"), $soloAlerta = $("#fsa-solo-alerta"), $soloSinTel = $("#fsa-solo-sin-tel");
        var filas = [];

        var num = function (v) { var n = parseInt(v, 10); return isNaN(n) ? null : n; };

        /* ¿La fila entra en el nivel / grado / sección elegidos? */
        function enCascada(d) {
            var niv = num($nivel.val()), cun = num($grado.val()), sec = num($seccion.val());
            return (niv === null || d.Niv_Id === niv) && (cun === null || d.Cun_Id === cun) && (sec === null || d.Sec_Id === sec);
        }

        /* Llena un select con opciones únicas (valor, texto) en el orden dado; sin padre queda deshabilitado. */
        function llenar($select, items, vacioConPadre, vacioSinPadre, hayPadre) {
            $select.empty().append($("<option>", { value: "", text: hayPadre ? vacioConPadre : vacioSinPadre }));
            if (hayPadre) {
                var vistos = {};
                items.forEach(function (it) {
                    if (it.v === null || it.v === undefined || vistos[it.v]) return;
                    vistos[it.v] = true;
                    $select.append($("<option>", { value: it.v, text: it.t || "—" }));
                });
            }
            $select.prop("disabled", !hayPadre);
        }

        function llenarNiveles() {
            var actual = $nivel.val();
            var items = filas.slice().sort(function (a, b) { return (a.Niv_Id || 0) - (b.Niv_Id || 0); })
                .map(function (f) { return { v: f.Niv_Id, t: f.Nivel }; });
            llenar($nivel, items, "Todos los niveles", "Todos los niveles", items.length > 0);
            if (actual && $nivel.find('option[value="' + actual + '"]').length) $nivel.val(actual);
        }

        function llenarGrados() {
            var niv = num($nivel.val());
            var items = filas.filter(function (f) { return f.Niv_Id === niv; })
                .sort(function (a, b) { return (a.Cun_Id || 0) - (b.Cun_Id || 0); })
                .map(function (f) { return { v: f.Cun_Id, t: f.Grado }; });
            llenar($grado, items, "Todos los grados", "Elija un nivel primero", niv !== null);
        }

        function llenarSecciones() {
            var niv = num($nivel.val()), cun = num($grado.val());
            var items = filas.filter(function (f) { return f.Niv_Id === niv && f.Cun_Id === cun; })
                .sort(function (a, b) { return String(a.Seccion || "").localeCompare(String(b.Seccion || ""), "es"); })
                .map(function (f) { return { v: f.Sec_Id, t: f.Seccion }; });
            llenar($seccion, items, "Todas las secciones", "Elija un grado primero", cun !== null);
        }

        /* Contadores con el nivel / grado / sección elegidos (sin los interruptores ni la búsqueda). */
        function contar() {
            var universo = filas.filter(enCascada);
            var total = universo.length;
            var con = universo.filter(function (f) { return f.TieneFicha; }).length;
            var alerta = universo.filter(function (f) { return f.TieneAlerta; }).length;
            var sinTel = universo.filter(function (f) { return !f.TieneTelefono; }).length;
            var pct = total ? Math.round(con * 100 / total) : 0;
            $("#kpi-con").text(con.toLocaleString("es-HN"));
            $("#kpi-con-barra").css("width", pct + "%");
            $("#kpi-con-texto").text(total ? pct + " % de " + total.toLocaleString("es-HN") + " alumnos activos" : "Sin alumnos con estos filtros");
            $("#kpi-sin").text((total - con).toLocaleString("es-HN"));
            $("#kpi-alerta").text(alerta.toLocaleString("es-HN"));
            $("#kpi-tel").text(sinTel.toLocaleString("es-HN"));
        }

        function hayFiltros() {
            return !!($nivel.val() || $buscar.val() || $soloSin.prop("checked") || $soloAlerta.prop("checked") || $soloSinTel.prop("checked"));
        }

        function aplicar() {
            t.filtrar();
            contar();
            $("#fsa-limpiar").prop("hidden", !hayFiltros());
        }

        var t = GcTabla.crear({
            elemento: "#tabla-fichas-salud",
            nombre: "fichas-salud",
            url: $tabla.data("url"),
            archivo: "fichas-de-salud",
            tituloPdf: "Fichas de salud",
            buscador: "#fsa-buscar",
            buscarEn: ["Alumno", "Identidad", "AlertaTexto", "Llamar"],
            filasPorPagina: 25,
            vacio: "Ningún alumno coincide con los filtros",
            agrupar: [{ campos: ["Ubicacion"], texto: "Grado y sección" }],
            filtroExtra: function (d) {
                return enCascada(d)
                    && (!$soloSin.prop("checked") || !d.TieneFicha)
                    && (!$soloAlerta.prop("checked") || d.TieneAlerta)
                    && (!$soloSinTel.prop("checked") || !d.TieneTelefono);
            },
            alCargar: function (datos) {
                filas = datos || [];
                llenarNiveles();
                contar();
            },
            columnas: [
                { title: "Alumno", field: "Alumno", minWidth: 180, widthGrow: 2,
                  // En el archivo exportado: "Nombre (identidad)".
                  titleDownload: "Alumno (identidad)",
                  accessorDownload: function (v, d) { return d.Identidad ? v + " (" + d.Identidad + ")" : v; },
                  formatter: function (c) {
                      var d = c.getData();
                      // El avatar saca las iniciales del texto que recibe: se le da el nombre (escapado), no el enlace.
                      var nombre = esc(d.Alumno);
                      var enlace = '<a class="fsa-enlace" href="' + urlFicha + "/" + d.Alu_Id + '">' + nombre + '</a>';
                      return GcTabla.avatar(nombre, d.Foto ? esc(d.Foto) : null).replace("<span>" + nombre + "</span>",
                          '<span class="fsa-nombre">' + enlace + '<span class="fsa-sub">' + esc(d.Identidad || "") + '</span></span>');
                  } },
                { title: "Grado", field: "Ubicacion",
                  mutator: function (v, d) { return [d.Grado, d.Seccion].filter(Boolean).join(" · ") || "—"; } },
                { title: "Sangre", field: "Sangre", hozAlign: "center",
                  mutator: function (v, d) { return d.TieneFicha ? (d.TipoSangre || "Desconocido") : ""; },
                  formatter: function (c) {
                      var v = c.getValue();
                      if (!v) return '<span class="text-muted">—</span>';
                      return v === "Desconocido" ? '<span class="text-muted small">Desconocido</span>' : '<strong>' + esc(v) + '</strong>';
                  } },
                { title: "Alerta", field: "AlertaTexto", minWidth: 150, widthGrow: 1.6,
                  mutator: function (v, d) { return d.TieneAlerta ? (d.Alerta || "") : ""; },
                  formatter: function (c) {
                      var v = c.getValue();
                      return v
                          ? '<span class="badge badge-danger fsa-badge-alerta"><i class="bi bi-heart-pulse-fill mr-1" aria-hidden="true"></i>' + esc(v) + '</span>'
                          : '<span class="text-muted">—</span>';
                  } },
                { title: "Llamar a", field: "Llamar", widthGrow: 1.5,
                  mutator: function (v, d) {
                      if (d.EncargadoTelefono) return (d.Encargado || "Encargado") + " · " + telefono(d.EncargadoTelefono);
                      if (d.ContactoTelefono) return (d.ContactoNombre || "Contacto") + " · " + telefono(d.ContactoTelefono) + " (contacto adicional)";
                      return d.Encargado ? d.Encargado + " · sin teléfono" : "Sin teléfono";
                  },
                  formatter: function (c) {
                      var d = c.getData();
                      // Un solo contenedor: la celda de Tabulator es flex y pondría los hijos en fila.
                      if (d.EncargadoTelefono)
                          return '<div>' + esc(d.Encargado || "Encargado") + '<span class="fsa-sub"><i class="bi bi-telephone mr-1" aria-hidden="true"></i>' + esc(telefono(d.EncargadoTelefono)) + '</span></div>';
                      if (d.ContactoTelefono)
                          return '<div>' + esc(d.ContactoNombre || "Contacto") + '<span class="fsa-sub"><i class="bi bi-telephone mr-1" aria-hidden="true"></i>' + esc(telefono(d.ContactoTelefono)) + ' <span class="text-nowrap">(adicional)</span></span></div>';
                      return '<div>' + (d.Encargado ? esc(d.Encargado) + " " : "") + '<span class="badge outline-badge-warning">Sin teléfono</span></div>';
                  } },
                { title: "Actualizada", field: "Actualizacion",
                  mutator: function (v, d) { return d.TieneFicha ? fecha(d.Actualizada) : "Sin ficha"; },
                  sorter: function (a, b, filaA, filaB) {
                      return String(filaA.getData().Actualizada || "").localeCompare(String(filaB.getData().Actualizada || ""));
                  },
                  formatter: function (c) {
                      return c.getData().TieneFicha ? esc(c.getValue()) : '<span class="badge outline-badge-warning">Sin ficha</span>';
                  } }
            ]
        });

        // Clic en la fila (fuera de la casilla y de los botones) → ficha del alumno.
        t.tabla.on("rowClick", function (e, row) {
            if ($(e.target).closest("a, button, input, label, .tabulator-row-header").length) return;
            window.location.href = urlFicha + "/" + row.getData().Alu_Id;
        });

        $nivel.on("change", function () { llenarGrados(); llenarSecciones(); aplicar(); });
        $grado.on("change", function () { llenarSecciones(); aplicar(); });
        $seccion.on("change", aplicar);
        $buscar.on("input", function () { $("#fsa-limpiar").prop("hidden", !hayFiltros()); });
        $soloSin.add($soloAlerta).add($soloSinTel).on("change", aplicar);

        $("#fsa-limpiar").on("click", function () {
            $nivel.val("");
            llenarGrados();
            llenarSecciones();
            $buscar.val("");
            $soloSin.add($soloAlerta).add($soloSinTel).prop("checked", false);
            aplicar();
        });

        // Total de filas visibles (con todos los filtros).
        t.tabla.on("dataFiltered", function (f, rows) { $("#fsa-total").text(rows.length.toLocaleString("es-HN")); });

        $("#fsa-exportar").on("click", function () {
            t.tabla.download("xlsx", "fichas-de-salud.xlsx", { sheetName: "Fichas de salud" }, "active");
        });
    }

    /* ═══════════════════════ Ficha del alumno ═══════════════════════ */
    var $ficha = $("#fsa-ficha");
    var $form = $("#fsa-form");
    if ($ficha.length && $form.length) {
        var editando = false, sucio = false, enviando = false;
        var $campos = $form.find("input, select, textarea").not("[type=hidden]");
        var $alertaSi = $("#Fsa_TieneAlerta"), $alerta = $("#Fsa_Alerta");
        var TELEFONO = /^\d{4}-?\d{4}$/;

        // Valores originales para "Cancelar".
        var originales = {};
        $campos.each(function () { originales[this.id] = this.type === "checkbox" ? this.checked : $(this).val(); });

        // En modo lectura un campo vacío dice "Sin registrar": los ejemplos del placeholder
        // (ej. "Asma, diabetes…") solo se muestran al editar, para que nadie los lea como datos.
        $campos.filter("input[type=text], input[type=tel], textarea").each(function () {
            $(this).attr("data-ejemplo", $(this).attr("placeholder") || "");
        });
        function placeholders(edicion) {
            $campos.filter("[data-ejemplo]").each(function () {
                $(this).attr("placeholder", edicion ? $(this).attr("data-ejemplo") : "Sin registrar");
            });
        }
        placeholders(false);

        function token() { return $('input[name="__RequestVerificationToken"]').first().val(); }

        function $error(campo) { return $form.find('[data-error="' + campo + '"]'); }

        function marcar(campo, mensaje) {
            var $c = $("#Fsa_" + campo);
            $c.addClass("is-invalid").attr("aria-invalid", "true");
            $error(campo).text(mensaje).addClass("d-block");
        }

        function desmarcar(campo) {
            $("#Fsa_" + campo).removeClass("is-invalid").removeAttr("aria-invalid");
            $error(campo).text("").removeClass("d-block");
        }

        function errorGeneral(mensaje) {
            var $g = $("#fsa-error-general");
            if (!mensaje) { $g.attr("hidden", true).text(""); return; }
            $g.text(mensaje).removeAttr("hidden");
            window.scrollTo({ top: Math.max(0, $g.offset().top - 90), behavior: "smooth" });
        }

        function contarAlerta() {
            var n = ($alerta.val() || "").length;
            $("#fsa-alerta-cuenta").text(n + "/150");
        }

        /* El texto de la alerta solo se escribe cuando hay alerta (y entonces es obligatorio). */
        function actualizarAlerta() {
            var si = $alertaSi.prop("checked");
            $alerta.prop("disabled", !editando || !si);
            $("#fsa-alerta-req").prop("hidden", !(editando && si));
            if (!si) desmarcar("Alerta");
        }

        function validarAlerta() {
            if ($alertaSi.prop("checked") && !($alerta.val() || "").trim()) {
                marcar("Alerta", "Escriba la alerta que verá el docente.");
                return false;
            }
            desmarcar("Alerta");
            return true;
        }

        function validarTelefono(campo) {
            var v = ($("#Fsa_" + campo).val() || "").trim();
            if (v && !TELEFONO.test(v)) { marcar(campo, "Escriba 8 dígitos (ej. 9999-9999)."); return false; }
            desmarcar(campo);
            return true;
        }

        /* Contacto adicional: si hay nombre o parentesco, el teléfono es obligatorio; si hay teléfono, el nombre. */
        function validarContacto() {
            var nombre = ($("#Fsa_ContactoEmergenciaNombre").val() || "").trim();
            var parentesco = ($("#Fsa_ContactoEmergenciaParentesco").val() || "").trim();
            var tel = ($("#Fsa_ContactoEmergenciaTelefono").val() || "").trim();
            var ok = validarTelefono("ContactoEmergenciaTelefono");
            if (ok && !tel && (nombre || parentesco)) { marcar("ContactoEmergenciaTelefono", "Escriba el teléfono del contacto adicional."); ok = false; }
            if (tel && !nombre) { marcar("ContactoEmergenciaNombre", "Escriba el nombre del contacto adicional."); ok = false; }
            else desmarcar("ContactoEmergenciaNombre");
            return ok;
        }

        function validarTodo() {
            var ok = validarAlerta();
            ok = validarTelefono("MedicoTelefono") && ok;
            ok = validarContacto() && ok;
            if (!ok) $form.find(".is-invalid").first().trigger("focus");
            return ok;
        }

        function modoEdicion(on) {
            editando = on;
            $campos.prop("disabled", !on);
            placeholders(on);
            actualizarAlerta();
            $("#fsa-editar").prop("hidden", on);
            $("#fsa-cancelar, #fsa-guardar, #fsa-pie, #fsa-ayuda-edicion").prop("hidden", !on);
            if (on) {
                contarAlerta();
                ($alertaSi.prop("checked") ? $alerta : $alertaSi).trigger("focus");
            } else {
                $("#fsa-alerta-cuenta").text("");
            }
        }

        function cancelar() {
            $campos.each(function () {
                if (this.type === "checkbox") this.checked = originales[this.id];
                else $(this).val(originales[this.id]);
            });
            $form.find(".is-invalid").each(function () { desmarcar(this.id.replace(/^Fsa_/, "")); });
            errorGeneral(null);
            sucio = false;
            if ($ficha.data("tiene-ficha") !== true) {
                $form.attr("hidden", true);
                $("#fsa-invitacion").removeAttr("hidden");
                editando = false;
                return;
            }
            modoEdicion(false);
        }

        $("#fsa-crear").on("click", function () {
            $("#fsa-invitacion").attr("hidden", true);
            $form.removeAttr("hidden");
            modoEdicion(true);
        });
        $("#fsa-editar").on("click", function () { modoEdicion(true); });
        $("#fsa-cancelar").add($form.find("[data-cancelar]")).on("click", cancelar);

        $campos.on("input change", function () { if (editando) sucio = true; });
        $alertaSi.on("change", function () { actualizarAlerta(); if (this.checked) $alerta.trigger("focus"); });
        $alerta.on("input", function () { contarAlerta(); if ($alerta.hasClass("is-invalid")) validarAlerta(); })
               .on("blur", validarAlerta);

        // Teléfonos: solo dígitos y guion; con 8 dígitos se agrega el guion solo.
        $form.find("[data-telefono]").on("input", function () {
            var limpio = this.value.replace(/[^\d-]/g, "");
            var d = limpio.replace(/\D/g, "");
            if (d.length === 8 && limpio.indexOf("-") < 0) limpio = d.substr(0, 4) + "-" + d.substr(4);
            if (limpio !== this.value) this.value = limpio;
            var campo = this.id.replace(/^Fsa_/, "");
            if ($(this).hasClass("is-invalid")) validarTelefono(campo);
        }).on("blur", function () {
            var campo = this.id.replace(/^Fsa_/, "");
            if (campo === "ContactoEmergenciaTelefono") validarContacto(); else validarTelefono(campo);
        });
        $("#Fsa_ContactoEmergenciaNombre, #Fsa_ContactoEmergenciaParentesco").on("blur", function () {
            if ($("#Fsa_ContactoEmergenciaTelefono").hasClass("is-invalid") || $("#Fsa_ContactoEmergenciaNombre").hasClass("is-invalid")) validarContacto();
        });

        $form.on("submit", function (e) {
            e.preventDefault();
            if (!editando || enviando) return;
            errorGeneral(null);
            if (!validarTodo()) return;

            var version = $ficha.data("version");
            var datos = {
                Alu_Id: $ficha.data("alumno"),
                Fsa_Version: version === "" || version === undefined ? null : version,
                Fsa_TieneAlerta: $alertaSi.prop("checked"),
                Fsa_Alerta: $alerta.val(),
                Fsa_TipoSangre: $("#Fsa_TipoSangre").val(),
                Fsa_Alergias: $("#Fsa_Alergias").val(),
                Fsa_Condiciones: $("#Fsa_Condiciones").val(),
                Fsa_Medicamentos: $("#Fsa_Medicamentos").val(),
                Fsa_RestriccionesAlimentarias: $("#Fsa_RestriccionesAlimentarias").val(),
                Fsa_MedicoNombre: $("#Fsa_MedicoNombre").val(),
                Fsa_MedicoTelefono: $("#Fsa_MedicoTelefono").val(),
                Fsa_Hospital: $("#Fsa_Hospital").val(),
                Fsa_Seguro: $("#Fsa_Seguro").val(),
                Fsa_SeguroPoliza: $("#Fsa_SeguroPoliza").val(),
                Fsa_ContactoEmergenciaNombre: $("#Fsa_ContactoEmergenciaNombre").val(),
                Fsa_ContactoEmergenciaParentesco: $("#Fsa_ContactoEmergenciaParentesco").val(),
                Fsa_ContactoEmergenciaTelefono: $("#Fsa_ContactoEmergenciaTelefono").val(),
                Fsa_Notas: $("#Fsa_Notas").val()
            };

            enviando = true;
            var $botones = $form.find("[type=submit]").prop("disabled", true);
            $.ajax({
                url: $ficha.data("url-guardar"),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(datos),
                headers: { RequestVerificationToken: token() },
                dataType: "json"
            }).done(function (r) {
                if (r && r.success) {
                    sucio = false;
                    window.location.href = r.redirect;
                    return;
                }
                if (r && r.field && $("#Fsa_" + r.field).length) {
                    marcar(r.field, r.message || "Revise este dato.");
                    $("#Fsa_" + r.field).trigger("focus");
                } else {
                    errorGeneral((r && r.message) || "No se pudo guardar la ficha de salud.");
                }
            }).fail(function () {
                errorGeneral("No se pudo conectar. Revise la conexión e intente de nuevo.");
            }).always(function () {
                enviando = false;
                $botones.prop("disabled", false);
            });
        });

        // Aviso si sale de la página con cambios sin guardar.
        $(window).on("beforeunload", function (e) {
            if (editando && sucio && !enviando) { e.preventDefault(); return ""; }
        });
    }

    /* ═══════════════════════ Hoja para imprimir ═══════════════════════ */
    $("[data-imprimir]").on("click", function () { window.print(); });
}(jQuery));
