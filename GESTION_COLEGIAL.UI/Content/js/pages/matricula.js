/* =====================================================================
 * matricula.js — Wizard de Matrícula nueva (4 pasos)
 * Paso 1: datos del alumno + cascada académica
 *         Nivel → Grado → Modalidad → Curso → Sección → Estado.
 *         Cada hijo arranca deshabilitado y se recarga vía endpoint al
 *         cambiar el padre; al cambiar un padre se limpian los hijos.
 * Paso 2: vincular/crear encargado con parentesco.
 * Paso 3: checklist de documentos (referencia, no persistido — ver hueco API).
 * Paso 4: resumen + confirmación → crea alumno y (si aplica) encargado.
 *
 * Reutiliza únicamente endpoints proxy de MatriculaController, que a su
 * vez llaman a los servicios existentes de Alumnos/Encargados/Parentescos.
 * ===================================================================== */
var Matricula = (function () {
    "use strict";

    var obj = {};
    var cfg = {};
    var pasoActual = 1;
    var TOTAL = 4;
    var encMode = "existente"; // "existente" | "nuevo"
    var alumnoCreadoId = 0;

    // -------------------------------------------------- init
    obj.init = function (config) {
        cfg = config || {};

        cargarParentescos();
        cargarEncargados();
        wireCascada();
        wireNav();
        wireEncModo();
        wireIdentidad();

        irAPaso(1);
    };

    // -------------------------------------------------- Navegación de pasos
    function wireNav() {
        $("#btnNext").on("click", function () {
            if (!validarPaso(pasoActual)) return;
            if (pasoActual < TOTAL) {
                if (pasoActual === 3) construirResumen(); // al entrar al 4
                irAPaso(pasoActual + 1);
            }
        });
        $("#btnPrev").on("click", function () {
            if (pasoActual > 1) irAPaso(pasoActual - 1);
        });
        $("#btnConfirm").on("click", confirmarMatricula);
    }

    function irAPaso(n) {
        pasoActual = n;

        $(".mat-panel").removeClass("active");
        $('.mat-panel[data-panel="' + n + '"]').addClass("active");

        // Barra de progreso
        $("#matSteps li").each(function () {
            var s = parseInt($(this).data("step"), 10);
            $(this).removeClass("active done");
            if (s < n) $(this).addClass("done");
            else if (s === n) $(this).addClass("active");
        });

        // Construir resumen si aterrizamos directo en el paso 4
        if (n === 4) construirResumen();

        // Botones
        $("#btnPrev").prop("hidden", n === 1);
        $("#btnNext").prop("hidden", n === TOTAL);
        $("#btnConfirm").prop("hidden", n !== TOTAL);
    }

    // -------------------------------------------------- Cascada académica (§1.1)
    function wireCascada() {
        $("#Niv_Id").on("change", function () {
            var id = $(this).val();
            resetHijo("#Cun_Id", "-- Seleccione el grado --");
            resetHijo("#Mda_Id", "-- Seleccione la modalidad --");
            resetHijo("#Cur_Id", "-- Seleccione el curso --");
            resetHijo("#Sec_Id", "-- Seleccione la sección --");
            if (id) cargarHijo(cfg.urlGrados, id, "#Cun_Id");
        });

        $("#Cun_Id").on("change", function () {
            var id = $(this).val();
            resetHijo("#Mda_Id", "-- Seleccione la modalidad --");
            resetHijo("#Cur_Id", "-- Seleccione el curso --");
            resetHijo("#Sec_Id", "-- Seleccione la sección --");
            if (id) cargarHijo(cfg.urlModalidades, id, "#Mda_Id");
        });

        $("#Mda_Id").on("change", function () {
            var id = $(this).val();
            resetHijo("#Cur_Id", "-- Seleccione el curso --");
            resetHijo("#Sec_Id", "-- Seleccione la sección --");
            if (id) cargarHijo(cfg.urlCursos, id, "#Cur_Id");
        });

        $("#Cur_Id").on("change", function () {
            var id = $(this).val();
            resetHijo("#Sec_Id", "-- Seleccione la sección --");
            if (id) cargarHijo(cfg.urlSecciones, id, "#Sec_Id");
        });
    }

    function resetHijo(sel, placeholder) {
        $(sel).empty()
            .append('<option value="">' + placeholder + '</option>')
            .prop("disabled", true)
            .val("");
    }

    function cargarHijo(url, parentId, sel) {
        $.ajax({
            type: "GET",
            url: url,
            data: { id: parentId },
            dataType: "json"
        }).done(function (res) {
            var lista = (res && res.data) ? res.data : [];
            var $sel = $(sel).empty().append('<option value="">-- Seleccionar --</option>');
            $.each(lista, function (_, it) {
                $sel.append('<option value="' + it.Value + '">' + it.Text + '</option>');
            });
            $sel.prop("disabled", lista.length === 0);
        }).fail(function () {
            $(sel).prop("disabled", true);
        });
    }

    // -------------------------------------------------- Duplicado por identidad
    function wireIdentidad() {
        $("#Per_Identidad").on("blur", function () {
            var ident = ($(this).val() || "").trim();
            var $aviso = $("#identidadAviso");
            $aviso.text("").css("color", "");
            if (ident.length !== 13) return;

            $.ajax({
                type: "GET",
                url: cfg.urlBuscarAlumno,
                data: { identidad: ident },
                dataType: "json"
            }).done(function (res) {
                if (res && res.data && res.data.Alu_Id) {
                    $aviso.css("color", "#b91c1c")
                        .text("Ya existe un alumno con esta identidad: " + (res.data.Alu_Nombre || ""));
                } else {
                    $aviso.css("color", "#059669").text("Identidad disponible.");
                }
            });
        });
    }

    // -------------------------------------------------- Encargado
    function wireEncModo() {
        $(".mat-toggle-btn").on("click", function () {
            $(".mat-toggle-btn").removeClass("active");
            $(this).addClass("active");
            encMode = $(this).data("enc-mode");
            if (encMode === "existente") {
                $("#encExistente").show();
                $("#encNuevo").hide();
            } else {
                $("#encExistente").hide();
                $("#encNuevo").show();
            }
        });
    }

    function cargarEncargados() {
        $.ajax({ type: "GET", url: cfg.urlEncargados, dataType: "json" })
            .done(function (res) {
                var lista = (res && res.data) ? res.data : [];
                var $sel = $("#encExistenteSel");
                $.each(lista, function (_, it) {
                    var extra = it.Identidad ? " (" + it.Identidad + ")" : "";
                    $sel.append('<option value="' + it.Value + '">' + it.Text + extra + '</option>');
                });
            });
    }

    function cargarParentescos() {
        $.ajax({ type: "GET", url: cfg.urlParentescos, dataType: "json" })
            .done(function (res) {
                var lista = (res && res.data) ? res.data : [];
                $(".mat-par").each(function () {
                    var $sel = $(this);
                    $.each(lista, function (_, it) {
                        $sel.append('<option value="' + it.Value + '">' + it.Text + '</option>');
                    });
                });
            });
    }

    // -------------------------------------------------- Validación por paso
    function validarPaso(n) {
        if (n === 1) return validarPaso1();
        if (n === 2) return validarPaso2();
        return true; // pasos 3 y 4 no bloquean
    }

    function limpiarErrores(scope) {
        $(scope).find(".mat-field").removeClass("has-error");
    }

    function marcarError($input) {
        $input.closest(".mat-field").addClass("has-error");
    }

    function validarPaso1() {
        var $panel = $('.mat-panel[data-panel="1"]');
        limpiarErrores($panel);
        var ok = true;
        var primero = null;

        $panel.find("[data-req]").each(function () {
            var $f = $(this);
            var val = ($f.val() || "").toString().trim();
            var len = $f.data("len");
            var malo = (val === "") || (len && val.length !== parseInt(len, 10));
            if (malo) {
                marcarError($f);
                ok = false;
                if (!primero) primero = $f;
            }
        });

        if (!ok && primero) primero.focus();
        return ok;
    }

    function validarPaso2() {
        limpiarErrores($('.mat-panel[data-panel="2"]'));
        var ok = true;
        var primero = null;

        if (encMode === "existente") {
            var enc = $("#encExistenteSel").val();
            var par = $("#parExistenteSel").val();
            if (!enc) { marcarError($("#encExistenteSel")); ok = false; primero = primero || $("#encExistenteSel"); }
            if (!par) { marcarError($("#parExistenteSel")); ok = false; primero = primero || $("#parExistenteSel"); }
        } else {
            $("#encNuevo").find("[data-req-enc]").each(function () {
                var $f = $(this);
                if (($f.val() || "").toString().trim() === "") {
                    marcarError($f); ok = false; primero = primero || $f;
                }
            });
            if (!$("#parNuevoSel").val()) {
                marcarError($("#parNuevoSel")); ok = false; primero = primero || $("#parNuevoSel");
            }
        }

        if (!ok && primero) primero.focus();
        return ok;
    }

    // -------------------------------------------------- Resumen (Paso 4)
    function txtSel(sel) {
        var $o = $(sel + " option:selected");
        return $o.length && $o.val() ? $o.text() : "—";
    }
    function val(id) { return ($("#" + id).val() || "").toString().trim() || "—"; }

    function fila(k, v) {
        return '<div class="mat-row"><span>' + k + '</span><span>' + (v || "—") + '</span></div>';
    }

    function construirResumen() {
        // Alumno
        var nombre = [val("Per_PrimerNombre"), $("#Per_SegundoNombre").val(),
            val("Per_ApellidoPaterno"), $("#Per_ApellidoMaterno").val()]
            .filter(function (x) { return x && x !== "—"; }).join(" ");
        var htmlAlu = fila("Nombre", nombre) +
            fila("Identidad", val("Per_Identidad")) +
            fila("Nacimiento", val("Per_FechaNacimiento")) +
            fila("Sexo", $("#Per_Sexo option:selected").text()) +
            fila("Teléfono", val("Per_Telefono")) +
            fila("Correo", val("Per_CorreoElectronico"));
        $("#sumAlumno").html('<h4><i class="fas fa-user"></i> Alumno</h4>' + htmlAlu);

        // Académico (cascada)
        var htmlAca = fila("Nivel", txtSel("#Niv_Id")) +
            fila("Grado", txtSel("#Cun_Id")) +
            fila("Modalidad", txtSel("#Mda_Id")) +
            fila("Curso", txtSel("#Cur_Id")) +
            fila("Sección", txtSel("#Sec_Id")) +
            fila("Estado", txtSel("#Est_Id"));
        $("#sumAcademico").html('<h4><i class="fas fa-layer-group"></i> Ubicación académica</h4>' + htmlAca);

        // Encargado
        var htmlEnc;
        if (encMode === "existente") {
            htmlEnc = fila("Modo", "Vincular existente") +
                fila("Encargado", txtSel("#encExistenteSel")) +
                fila("Parentesco", txtSel("#parExistenteSel"));
        } else {
            var encNom = [$("#enc_Per_PrimerNombre").val(), $("#enc_Per_ApellidoPaterno").val()]
                .filter(function (x) { return x; }).join(" ");
            htmlEnc = fila("Modo", "Registrar nuevo") +
                fila("Nombre", encNom || "—") +
                fila("Identidad", $("#enc_Per_Identidad").val() || "—") +
                fila("Parentesco", txtSel("#parNuevoSel"));
        }
        $("#sumEncargado").html('<h4><i class="fas fa-user-shield"></i> Encargado</h4>' + htmlEnc);

        // Documentos marcados
        var docs = [];
        $(".mat-doc-check:checked").each(function () { docs.push($(this).data("doc")); });
        var htmlDoc = docs.length
            ? docs.map(function (d) { return fila(d, "✓"); }).join("")
            : '<div class="mat-row"><span>Sin documentos marcados</span><span>—</span></div>';
        $("#sumDocumentos").html('<h4><i class="fas fa-folder-open"></i> Documentos marcados</h4>' + htmlDoc);
    }

    // -------------------------------------------------- Confirmar (guardar)
    function payloadAlumno() {
        return {
            Alu_Id: 0,
            Per_Id: 0,
            Per_EsActivo: true,
            Per_Identidad: $("#Per_Identidad").val(),
            Per_PrimerNombre: $("#Per_PrimerNombre").val(),
            Per_SegundoNombre: $("#Per_SegundoNombre").val(),
            Per_ApellidoPaterno: $("#Per_ApellidoPaterno").val(),
            Per_ApellidoMaterno: $("#Per_ApellidoMaterno").val(),
            Per_FechaNacimiento: $("#Per_FechaNacimiento").val(),
            Per_CorreoElectronico: $("#Per_CorreoElectronico").val(),
            Per_Telefono: $("#Per_Telefono").val(),
            Per_Direccion: $("#Per_Direccion").val(),
            Per_Sexo: $("#Per_Sexo").val(),
            Niv_Id: $("#Niv_Id").val(),
            Cun_Id: $("#Cun_Id").val(),
            Mda_Id: $("#Mda_Id").val(),
            Cur_Id: $("#Cur_Id").val(),
            Sec_Id: $("#Sec_Id").val(),
            Est_Id: $("#Est_Id").val()
        };
    }

    function payloadEncargadoNuevo() {
        return {
            Enc_Id: 0,
            Per_Id: 0,
            EsActivo: "true",
            Per_EsActivo: true,
            Enc_Ocupacion: $("#enc_Enc_Ocupacion").val(),
            Per_Identidad: $("#enc_Per_Identidad").val(),
            Per_PrimerNombre: $("#enc_Per_PrimerNombre").val(),
            Per_SegundoNombre: $("#enc_Per_SegundoNombre").val(),
            Per_ApellidoPaterno: $("#enc_Per_ApellidoPaterno").val(),
            Per_ApellidoMaterno: $("#enc_Per_ApellidoMaterno").val(),
            Per_FechaNacimiento: $("#enc_Per_FechaNacimiento").val(),
            Per_CorreoElectronico: $("#enc_Per_CorreoElectronico").val(),
            Per_Telefono: $("#enc_Per_Telefono").val(),
            Per_Direccion: $("#enc_Per_Direccion").val(),
            Per_Sexo: $("#enc_Per_Sexo").val()
        };
    }

    function confirmarMatricula() {
        // Revalida pasos con datos requeridos
        if (!validarPaso1()) { irAPaso(1); return; }
        if (!validarPaso2()) { irAPaso(2); return; }

        var $btn = $("#btnConfirm").prop("disabled", true).html('<i class="fas fa-spinner fa-spin"></i> Guardando...');

        // 1) Crear alumno
        $.ajax({
            type: "POST",
            url: cfg.urlGuardarAlu,
            data: payloadAlumno(),
            dataType: "json"
        }).done(function (res) {
            if (!res || !res.success) {
                alertar("No se pudo crear el alumno. Verifique los datos e intente de nuevo.", "error");
                restaurarBoton($btn);
                return;
            }
            alumnoCreadoId = (res.item && res.item.alu_Id) ? res.item.alu_Id : 0;

            // 2) Crear encargado nuevo si aplica (vínculo alumno-encargado: hueco de API)
            if (encMode === "nuevo") {
                $.ajax({
                    type: "POST",
                    url: cfg.urlGuardarEnc,
                    data: payloadEncargadoNuevo(),
                    dataType: "json"
                }).done(function (r2) {
                    finalizar(!!(r2 && r2.success));
                }).fail(function () {
                    finalizar(false);
                });
            } else {
                finalizar(true, true); // encargado existente: sin escritura de vínculo (hueco API)
            }
        }).fail(function () {
            alertar("Error de comunicación al crear el alumno.", "error");
            restaurarBoton($btn);
        });
    }

    function finalizar(encOk, vinculoPendiente) {
        var notaEnc = "";
        if (encMode === "nuevo" && !encOk) {
            notaEnc = '<p style="color:#b91c1c;"><i class="fas fa-triangle-exclamation"></i> El alumno se creó, pero el encargado no pudo registrarse. Regístrelo desde Encargados.</p>';
        } else if (encMode === "nuevo" && encOk) {
            notaEnc = '<p style="color:#92400e;"><i class="fas fa-circle-info"></i> Encargado registrado. El vínculo alumno–encargado deberá confirmarse en la ficha del alumno (no soportado por el API en el alta).</p>';
        } else if (vinculoPendiente) {
            notaEnc = '<p style="color:#92400e;"><i class="fas fa-circle-info"></i> El vínculo con el encargado existente deberá confirmarse en la ficha del alumno (no soportado por el API en el alta).</p>';
        }

        var linkDocs = alumnoCreadoId
            ? '<a href="' + cfg.urlDocumentos + '" class="mat-btn mat-btn-next" style="text-decoration:none;"><i class="fas fa-folder-open"></i> Gestionar documentos</a>'
            : "";
        var linkAlu = '<a href="' + cfg.urlAlumnos + '" class="mat-btn mat-btn-prev" style="text-decoration:none;"><i class="fas fa-list"></i> Ir al listado</a>';

        $("#matResultado").html(
            '<div class="mat-success-box">' +
            '<div class="mat-success-icon"><i class="fas fa-circle-check"></i></div>' +
            '<h3>Matrícula registrada</h3>' +
            '<p class="mat-hint">El alumno fue creado correctamente.</p>' +
            notaEnc +
            '<div style="display:flex;gap:.75rem;justify-content:center;margin-top:1rem;flex-wrap:wrap;">' +
            linkAlu + linkDocs +
            '</div></div>'
        ).show();

        $(".mat-summary-grid").hide();
        $("#matNav").hide();
        alertar("Matrícula registrada correctamente.", "success");
    }

    function restaurarBoton($btn) {
        $btn.prop("disabled", false).html('<i class="fas fa-check"></i> Confirmar matrícula');
    }

    function alertar(msg, tipo) {
        // Usa el sistema de alertas del proyecto si está disponible; si no, fallback.
        if (typeof alertConfig !== "undefined" && alertConfig.alert) {
            alertConfig.alert(msg, tipo);
        } else if (typeof Swal !== "undefined") {
            Swal.fire({ text: msg, icon: tipo === "error" ? "error" : "success" });
        } else {
            alert(msg);
        }
    }

    return obj;
}());
