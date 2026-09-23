var VacacionesAdm = (function () {
    var obj = {};
    var urlList, urlCreate, urlApprove, urlSaldo;
    var table;
    var saldoActual = null;        // saldo del empleado seleccionado
    var solicitudesEmpleado = [];  // rangos existentes del empleado (para solapamiento)

    var TIPO_LABEL = { V: "Vacaciones", P: "Permiso", I: "Incapacidad" };
    var EST_LABEL = { P: "Pendiente", A: "Aprobado", R: "Rechazado" };

    function fmtFecha(val) {
        if (!val) return "—";
        var d = new Date(val);
        if (isNaN(d)) return val;
        var dd = ("0" + d.getDate()).slice(-2);
        var mm = ("0" + (d.getMonth() + 1)).slice(-2);
        return dd + "/" + mm + "/" + d.getFullYear();
    }

    // Días calendario inclusivos entre dos fechas (yyyy-mm-dd)
    function diasEntre(ini, fin) {
        var a = new Date(ini), b = new Date(fin);
        if (isNaN(a) || isNaN(b)) return 0;
        return Math.floor((b - a) / 86400000) + 1;
    }

    function badgeTipo(code) {
        var c = (code || "").charAt(0).toUpperCase();
        return '<span class="vac-badge vac-badge-' + c + '">' + (TIPO_LABEL[c] || code || "—") + '</span>';
    }
    function badgeEstado(code) {
        var c = (code || "").charAt(0).toUpperCase();
        return '<span class="vac-badge vac-badge-est-' + c + '">' + (EST_LABEL[c] || code || "—") + '</span>';
    }

    function showError(msg) {
        var el = $("#vac-form-error");
        if (!msg) { el.hide().text(""); return; }
        el.text(msg).show();
    }

    function actualizarDiasCalc() {
        var ini = $("input[name='Vac_FechaInicio']").val();
        var fin = $("input[name='Vac_FechaFin']").val();
        if (ini && fin) {
            var d = diasEntre(ini, fin);
            $("#vac-dias-calc").val(d > 0 ? d + (d === 1 ? " día" : " días") : "—");
        } else {
            $("#vac-dias-calc").val("—");
        }
        evaluarGuardar();
    }

    function tipoActual() {
        return ($("select[name='Vac_Tipo']").val() || "").charAt(0).toUpperCase();
    }

    function cargarSaldo(empId) {
        saldoActual = null;
        solicitudesEmpleado = [];

        if (!empId) { aplicarTipo(); return; }

        // Rangos existentes del empleado (para validar solapamiento)
        $.getJSON(urlList + "?Emp_Id=" + empId, function (res) {
            var rows = Array.isArray(res) ? res : (res.data || []);
            solicitudesEmpleado = rows.filter(function (r) {
                return (r.Vac_Estado || "").charAt(0).toUpperCase() !== "R";
            });
        });

        if (!urlSaldo) { aplicarTipo(); return; }
        $.getJSON(urlSaldo + "?Emp_Id=" + empId, function (res) {
            var d = Array.isArray(res) ? res[0] : ((res.data && res.data[0]) || res.data || res);
            if (d) {
                saldoActual = d;
                $("#vac-saldo-derecho").text(d.DiasDerecho != null ? d.DiasDerecho : "—");
                $("#vac-saldo-tomados").text(d.DiasTomados != null ? d.DiasTomados : "—");
                $("#vac-saldo-disponible").text(d.DiasDisponibles != null ? d.DiasDisponibles : "—");
                $("#vac-saldo-antig").html(
                    (d.Anos != null ? d.Anos + (d.Anos === 1 ? " año de servicio" : " años de servicio") : "") +
                    (d.DiasPendientes ? '<br>' + d.DiasPendientes + ' día(s) en solicitudes pendientes' : "")
                );
                $(".vac-saldo-item--dispo").toggleClass("agotado", (d.DiasDisponibles || 0) <= 0);
            }
            aplicarTipo();
        });
    }

    // Muestra/oculta panel de saldo, campos de hora y reevalúa el botón Guardar según el Tipo
    function aplicarTipo() {
        var tipo = tipoActual();
        var panel = $("#vac-saldo-panel"), msg = $("#vac-saldo-msg");
        var empId = $("select[name='Emp_Id']").val();

        // Campos de hora: solo para Permiso
        $("#vac-horas-row").toggle(tipo === "P");

        // Panel de saldo: solo para Vacaciones
        if (tipo === "V" && empId) {
            var d = saldoActual;
            if (d && d.Anos !== undefined && d.Anos < 1) {
                msg.html('<i class="bi bi-info-circle"></i> Este empleado aún no cumple 1 año de servicio, por lo que no tiene derecho a vacaciones remuneradas (Código del Trabajo de Honduras). Sí puede registrar permisos o incapacidades.').show();
                panel.hide();
            } else if (d) {
                panel.show();
                msg.hide();
            } else {
                panel.hide();
                msg.hide();
            }
        } else {
            panel.hide();
            msg.hide();
        }

        evaluarGuardar();
    }

    // Bloquea el botón Guardar cuando Vacaciones sin saldo disponible
    function evaluarGuardar() {
        var btn = $("#form-nueva button[type='submit']");
        var tipo = tipoActual();
        var bloquear = false;

        if (tipo === "V" && saldoActual) {
            var dispo = saldoActual.DiasDisponibles;
            var ini = $("input[name='Vac_FechaInicio']").val();
            var fin = $("input[name='Vac_FechaFin']").val();
            var dias = (ini && fin) ? diasEntre(ini, fin) : 0;
            if ((saldoActual.Anos != null && saldoActual.Anos < 1) || (dispo != null && dispo <= 0)) {
                bloquear = true;
                showError("El empleado no tiene días de vacaciones disponibles.");
            } else if (dispo != null && dias > 0 && dias > dispo) {
                bloquear = true;
                showError("Los días solicitados (" + dias + ") superan el saldo disponible (" + dispo + ").");
            }
        }

        btn.prop("disabled", bloquear);
        if (!bloquear) showError("");
    }

    function solapaConExistentes(ini, fin) {
        var a = new Date(ini), b = new Date(fin);
        for (var i = 0; i < solicitudesEmpleado.length; i++) {
            var r = solicitudesEmpleado[i];
            var c = new Date(r.Vac_FechaInicio), d = new Date(r.Vac_FechaFin);
            if (a <= d && c <= b) return r; // rangos se traslapan
        }
        return null;
    }

    function validar() {
        showError("");
        var empId = $("select[name='Emp_Id']").val();
        var ini = $("input[name='Vac_FechaInicio']").val();
        var fin = $("input[name='Vac_FechaFin']").val();
        var tipo = ($("select[name='Vac_Tipo']").val() || "").charAt(0).toUpperCase();

        if (!empId) { showError("Seleccione un empleado."); return false; }
        if (!ini || !fin) { showError("Indique la fecha de inicio y de fin."); return false; }
        if (new Date(fin) < new Date(ini)) { showError("La fecha de fin no puede ser anterior a la de inicio."); return false; }

        var solapa = solapaConExistentes(ini, fin);
        if (solapa) {
            showError("El empleado ya tiene una solicitud (" + (EST_LABEL[(solapa.Vac_Estado || "").charAt(0)] || "") +
                      ") del " + fmtFecha(solapa.Vac_FechaInicio) + " al " + fmtFecha(solapa.Vac_FechaFin) + " que se traslapa con estas fechas.");
            return false;
        }

        // Permiso: la hora es obligatoria
        if (tipo === "P") {
            var hIni = $("input[name='Vac_HoraInicio']").val();
            var hFin = $("input[name='Vac_HoraFin']").val();
            if (!hIni || !hFin) { showError("Para un permiso indique la hora de inicio y de fin."); return false; }
            if (ini === fin && hFin <= hIni) { showError("La hora de fin debe ser posterior a la hora de inicio."); return false; }
        }

        // Validación de saldo solo para Vacaciones
        if (tipo === "V") {
            if (!saldoActual) { showError("No se pudo obtener el saldo de vacaciones del empleado."); return false; }
            if (saldoActual.Anos != null && saldoActual.Anos < 1) {
                showError("El empleado no tiene derecho a vacaciones remuneradas todavía (menos de 1 año de servicio).");
                return false;
            }
            if (saldoActual.DiasDisponibles != null && saldoActual.DiasDisponibles <= 0) {
                showError("El empleado no tiene días de vacaciones disponibles.");
                return false;
            }
            var dias = diasEntre(ini, fin);
            if (saldoActual.DiasDisponibles != null && dias > saldoActual.DiasDisponibles) {
                showError("Los días solicitados (" + dias + ") superan el saldo disponible (" + saldoActual.DiasDisponibles + ").");
                return false;
            }
        }
        return true;
    }

    obj.init = function (ul, uc, ua, urlDropdowns, us) {
        urlList = ul; urlCreate = uc; urlApprove = ua; urlSaldo = us;

        if (urlDropdowns) {
            $.getJSON(urlDropdowns, function (data) {
                var sel = $("select[name='Emp_Id']");
                $.each(data.empleados, function (_, e) {
                    sel.append('<option value="' + e.id + '">' + e.texto + '</option>');
                });
            });
        }

        table = $("#datatable-vac").DataTable({
            ajax: { url: urlList, dataSrc: function (json) { return Array.isArray(json) ? json : (json.data || []); } },
            columns: [
                { data: "NombreEmpleado" },
                { data: "Vac_Tipo", render: badgeTipo },
                { data: "Vac_FechaInicio", render: fmtFecha },
                { data: "Vac_FechaFin", render: fmtFecha },
                { data: "Vac_DiasTotal", className: "text-center" },
                { data: "Vac_Estado", render: badgeEstado },
                { data: null, orderable: false, render: function (r) {
                    if ((r.Vac_Estado || "").charAt(0).toUpperCase() === "P")
                        return '<button class="btn btn-sm btn-success btn-aprobar" data-id="' + r.Vac_Id + '">Aprobar</button> ' +
                               '<button class="btn btn-sm btn-danger btn-rechazar" data-id="' + r.Vac_Id + '">Rechazar</button>';
                    return "";
                }}
            ],
            language: { url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" }
        });

        // Saldo al cambiar de empleado
        $("select[name='Emp_Id']").on("change", function () { cargarSaldo($(this).val()); });
        // Mostrar/ocultar panel de saldo y campos de hora según el Tipo
        $("select[name='Vac_Tipo']").on("change", function () { aplicarTipo(); });
        // Recalcular días al cambiar fechas
        $("input[name='Vac_FechaInicio'], input[name='Vac_FechaFin']").on("change", function () {
            actualizarDiasCalc();
        });
        // Reevaluar al cambiar las horas del permiso
        $("input[name='Vac_HoraInicio'], input[name='Vac_HoraFin']").on("change", function () {
            showError("");
        });

        // Limpiar estado al abrir el modal
        $("#modal-nueva").on("show.bs.modal", function () {
            $("#form-nueva")[0].reset();
            saldoActual = null; solicitudesEmpleado = [];
            $("#vac-saldo-panel").hide(); $("#vac-saldo-msg").hide();
            $("#vac-horas-row").hide();
            $("#vac-dias-calc").val("—");
            $("#form-nueva button[type='submit']").prop("disabled", false);
            showError("");
            aplicarTipo();
        });

        $("#form-nueva").on("submit", function (e) {
            e.preventDefault();
            if (!validar()) return;
            var data = $(this).serializeArray();
            // El API no tiene columna de horas: para Permiso se anexan a la observación.
            if (tipoActual() === "P") {
                var hIni = $("input[name='Vac_HoraInicio']").val();
                var hFin = $("input[name='Vac_HoraFin']").val();
                var motivoIdx = -1, prefijo = "Horario: " + hIni + " - " + hFin;
                for (var i = 0; i < data.length; i++) {
                    if (data[i].name === "Vac_Motivo") { motivoIdx = i; break; }
                }
                if (motivoIdx >= 0) {
                    data[motivoIdx].value = prefijo + (data[motivoIdx].value ? ". " + data[motivoIdx].value : "");
                } else {
                    data.push({ name: "Vac_Motivo", value: prefijo });
                }
            }
            // No enviar los campos de hora crudos al backend (no existen en el API)
            data = data.filter(function (f) { return f.name !== "Vac_HoraInicio" && f.name !== "Vac_HoraFin"; });
            $.post(urlCreate, $.param(data), function (res) {
                if (res && res.success) { $("#modal-nueva").modal("hide"); table.ajax.reload(); }
                else { showError("No se pudo guardar la solicitud. Intente de nuevo."); }
            });
        });

        $(document).on("click", ".btn-aprobar", function () {
            $.post(urlApprove, { Vac_Id: $(this).data("id"), Vac_Estado: "Aprobado" },
                function () { table.ajax.reload(); });
        });
        $(document).on("click", ".btn-rechazar", function () {
            $.post(urlApprove, { Vac_Id: $(this).data("id"), Vac_Estado: "Rechazado" },
                function () { table.ajax.reload(); });
        });
    };

    return obj;
}());
