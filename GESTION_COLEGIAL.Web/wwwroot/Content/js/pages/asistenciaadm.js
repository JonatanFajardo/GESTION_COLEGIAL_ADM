/* ============================================================
   Docencia Diaria — hub con tabs: Pase de Lista / Plan de Clases / Tareas
   Un solo selector de horario compartido (#dd-hor) arriba de las tabs.
   Reusa los endpoints de AsistenciaAdm, PlanClasesAdm y TareasAdm.
   ============================================================ */
var AsistenciaAdm = (function () {
    var obj = {};
    var urls = {};
    var horId = "";           // horario compartido
    var fecha = "";           // solo tab Pase de Lista
    var tabActiva = "asistencia";
    var planCache = {};       // Pla_Id -> row
    var tareaCache = {};      // Tar_Id -> row
    var cargado = { asistencia: false, plan: false, tareas: false };

    function esc(v) {
        if (v === null || v === undefined) return "";
        return String(v)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    /* ---------------- Tabs ---------------- */
    function activarTab(tab) {
        tabActiva = tab;
        $(".dd-tab-item").removeClass("active");
        $(".dd-tab-item[data-tab='" + tab + "']").addClass("active");
        $(".dd-panel").attr("hidden", true);
        $("#dd-panel-" + tab).removeAttr("hidden");
        // Cargar en diferido: al entrar al tab con horario ya elegido
        if (horId && !cargado[tab]) recargarTab(tab);
    }

    function recargarTab(tab) {
        if (!horId) return;
        if (tab === "asistencia") { if (fecha) cargarAsistencia(); }
        else if (tab === "plan") cargarPlan();
        else if (tab === "tareas") cargarTareas();
    }

    function onHorarioChange() {
        horId = $("#dd-hor").val();
        cargado = { asistencia: false, plan: false, tareas: false };
        $("#btn-nuevo-plan, #btn-nueva-tarea").prop("disabled", !horId);
        $("#tbody-asistencia, #tbody-plan, #tbody-tareas").empty();
        if (horId) recargarTab(tabActiva);
    }

    /* ---------------- Tab 1: Pase de Lista ---------------- */
    function cargarAsistencia() {
        horId = $("#dd-hor").val();
        fecha = $("#f-fecha").val();
        if (!horId || !fecha) { alert("Seleccione horario y fecha."); return; }

        $.getJSON(urls.asisList, { Hor_Id: horId, Fecha: fecha }, function (data) {
            cargado.asistencia = true;
            var tbody = $("#tbody-asistencia").empty();
            if (!data || data.length === 0) {
                tbody.append('<tr><td colspan="4" class="text-center text-muted">Sin registros para este horario.</td></tr>');
                return;
            }
            $.each(data, function (_, row) {
                var estados = ["Presente", "Ausente", "Tardanza", "Justificado"].map(function (e) {
                    var sel = row.Asi_Estado === e ? " selected" : "";
                    return "<option" + sel + ">" + e + "</option>";
                }).join("");
                tbody.append(
                    "<tr data-asi='" + (row.Asi_Id || 0) + "' data-alu='" + row.Alu_Id + "'>" +
                    "<td>" + esc(row.Alu_NombreCompleto) + "</td>" +
                    "<td><select class='form-control est-select'>" + estados + "</select></td>" +
                    "<td><input class='form-control obs-input' value=\"" + esc(row.Asi_Observacion) + "\"></td>" +
                    "<td><button class='btn btn-sm btn-primary btn-marcar'>Guardar</button></td></tr>"
                );
            });
        });
    }

    function guardarFila(tr, cb) {
        $.post(urls.asisMarcar, {
            Asi_Id:          tr.data("asi"),
            Hor_Id:          horId,
            Alu_Id:          tr.data("alu"),
            Asi_Fecha:       fecha,
            Asi_Estado:      tr.find(".est-select").val(),
            Asi_Observacion: tr.find(".obs-input").val()
        }, function (res) { if (cb) cb(res); });
    }

    function guardarTodos() {
        $("#tbody-asistencia tr[data-alu]").each(function () { guardarFila($(this)); });
    }

    /* ---------------- Tab 2: Plan de Clases ---------------- */
    function estadoBadgePlan(est) {
        return est === "Impartido"
            ? '<span class="badge badge-success">Impartido</span>'
            : '<span class="badge badge-warning">Pendiente</span>';
    }

    function cargarPlan() {
        if (!horId) { alert("Seleccione un horario."); return; }
        $.getJSON(urls.planList, { Hor_Id: horId }, function (data) {
            cargado.plan = true;
            planCache = {};
            var rows = data || [];
            var tbody = $("#tbody-plan").empty();
            if (!rows.length) {
                tbody.append('<tr><td colspan="6" class="text-center text-muted">Sin contenido registrado.</td></tr>');
                return;
            }
            $.each(rows, function (_, r) {
                planCache[r.Pla_Id] = r;
                tbody.append(
                    '<tr data-id="' + r.Pla_Id + '">' +
                    '<td>' + r.Pla_Semana + '</td>' +
                    '<td>' + esc(r.Pla_Tema) + '</td>' +
                    '<td class="text-truncate" style="max-width:200px" title="' + esc(r.Pla_Objetivos) + '">' + (esc(r.Pla_Objetivos) || '—') + '</td>' +
                    '<td>' + (esc(r.Pla_Recursos) || '—') + '</td>' +
                    '<td>' + estadoBadgePlan(r.Pla_Estado) + '</td>' +
                    '<td>' +
                        '<button class="btn btn-xs btn-primary plan-editar me-1">Editar</button>' +
                        '<button class="btn btn-xs btn-danger plan-eliminar">Eliminar</button>' +
                    '</td></tr>'
                );
            });
        });
    }

    /* ---------------- Tab 3: Tareas ---------------- */
    function estadoBadgeTarea(est) {
        return est === "Cerrada"
            ? '<span class="badge badge-secondary">Cerrada</span>'
            : '<span class="badge badge-success">Activa</span>';
    }

    function cargarTareas() {
        if (!horId) { alert("Seleccione un horario."); return; }
        $.getJSON(urls.tareaList, { Hor_Id: horId }, function (data) {
            cargado.tareas = true;
            tareaCache = {};
            var rows = data || [];
            var tbody = $("#tbody-tareas").empty();
            if (!rows.length) {
                tbody.append('<tr><td colspan="6" class="text-center text-muted">Sin tareas registradas.</td></tr>');
                return;
            }
            $.each(rows, function (_, r) {
                tareaCache[r.Tar_Id] = r;
                var f = r.Tar_FechaEntrega ? r.Tar_FechaEntrega.substring(0, 10) : '—';
                tbody.append(
                    '<tr data-id="' + r.Tar_Id + '">' +
                    '<td>' + esc(r.Tar_Titulo) + '</td>' +
                    '<td class="text-truncate" style="max-width:180px" title="' + esc(r.Tar_Descripcion) + '">' + (esc(r.Tar_Descripcion) || '—') + '</td>' +
                    '<td>' + f + '</td>' +
                    '<td>' + r.Tar_Punteo + '</td>' +
                    '<td>' + estadoBadgeTarea(r.Tar_Estado) + '</td>' +
                    '<td>' +
                        '<button class="btn btn-xs btn-primary tarea-editar me-1">Editar</button>' +
                        '<button class="btn btn-xs btn-danger tarea-eliminar">Eliminar</button>' +
                    '</td></tr>'
                );
            });
        });
    }

    /* ---------------- Init ---------------- */
    obj.init = function (u) {
        urls = u;

        var hoy = new Date().toISOString().split("T")[0];
        $("#f-fecha").val(hoy);
        fecha = hoy;

        // Horarios compartidos (un solo dropdown para los 3 tabs)
        $.getJSON(urls.dropdowns, function (data) {
            var sel = $("#dd-hor");
            $.each(data.horarios, function (_, h) {
                sel.append('<option value="' + h.id + '">' + esc(h.texto) + '</option>');
            });
        });

        $("#dd-hor").on("change", onHorarioChange);

        // Tabs
        $("#dd-tabs").on("click", ".dd-tab-item", function (e) {
            e.preventDefault();
            activarTab($(this).data("tab"));
        });

        /* --- Pase de Lista --- */
        $("#btn-cargar-asistencia").on("click", cargarAsistencia);
        $("#btn-guardar-todos").on("click", guardarTodos);
        $("#f-fecha").on("change", function () {
            fecha = $(this).val();
            cargado.asistencia = false;
            if (horId && fecha) cargarAsistencia();
        });
        $(document).on("click", ".btn-marcar", function () { guardarFila($(this).closest("tr")); });

        /* --- Plan de Clases --- */
        $("#btn-nuevo-plan").on("click", function () {
            $("#plan-modal-titulo").text("Nuevo Tema");
            $("#form-plan")[0].reset();
            $("#Pla_Id").val(0);
            $("#plan-hor-hidden").val(horId);
            $("#plan-div-estado").hide();
            $("#modal-plan").modal("show");
        });

        $(document).on("click", ".plan-editar", function () {
            var r = planCache[$(this).closest("tr").data("id")];
            if (!r) return;
            $("#plan-modal-titulo").text("Editar Tema");
            $("#Pla_Id").val(r.Pla_Id);
            $("#plan-hor-hidden").val(r.Hor_Id);
            $("#form-plan [name='Pla_Semana']").val(r.Pla_Semana);
            $("#form-plan [name='Pla_Tema']").val(r.Pla_Tema);
            $("#form-plan [name='Pla_Objetivos']").val(r.Pla_Objetivos);
            $("#form-plan [name='Pla_Recursos']").val(r.Pla_Recursos);
            $("#form-plan [name='Pla_Estado']").val(r.Pla_Estado);
            $("#plan-div-estado").show();
            $("#modal-plan").modal("show");
        });

        $(document).on("click", ".plan-eliminar", function () {
            var id = $(this).closest("tr").data("id");
            if (!confirm("¿Eliminar este tema?")) return;
            $.post(urls.planDelete, { Pla_Id: id }, function (res) { if (res && res.success) cargarPlan(); });
        });

        $("#form-plan").on("submit", function (e) {
            e.preventDefault();
            var id = parseInt($("#Pla_Id").val());
            var url = id ? urls.planEdit : urls.planCreate;
            $.post(url, $(this).serialize(), function (res) {
                if (res && res.success) { $("#modal-plan").modal("hide"); cargarPlan(); }
            });
        });

        /* --- Tareas --- */
        $("#btn-nueva-tarea").on("click", function () {
            $("#tarea-modal-titulo").text("Nueva Tarea");
            $("#form-tarea")[0].reset();
            $("#Tar_Id").val(0);
            $("#tarea-hor-hidden").val(horId);
            $("#tarea-div-estado").hide();
            $("#modal-tarea").modal("show");
        });

        $(document).on("click", ".tarea-editar", function () {
            var r = tareaCache[$(this).closest("tr").data("id")];
            if (!r) return;
            $("#tarea-modal-titulo").text("Editar Tarea");
            $("#Tar_Id").val(r.Tar_Id);
            $("#tarea-hor-hidden").val(r.Hor_Id);
            $("#form-tarea [name='Tar_Titulo']").val(r.Tar_Titulo);
            $("#form-tarea [name='Tar_Descripcion']").val(r.Tar_Descripcion);
            $("#form-tarea [name='Tar_FechaEntrega']").val(r.Tar_FechaEntrega ? r.Tar_FechaEntrega.substring(0, 10) : '');
            $("#form-tarea [name='Tar_Punteo']").val(r.Tar_Punteo);
            $("#form-tarea [name='Tar_Estado']").val(r.Tar_Estado);
            $("#tarea-div-estado").show();
            $("#modal-tarea").modal("show");
        });

        $(document).on("click", ".tarea-eliminar", function () {
            var id = $(this).closest("tr").data("id");
            if (!confirm("¿Eliminar esta tarea?")) return;
            $.post(urls.tareaDelete, { Tar_Id: id }, function (res) { if (res && res.success) cargarTareas(); });
        });

        $("#form-tarea").on("submit", function (e) {
            e.preventDefault();
            var id = parseInt($("#Tar_Id").val());
            var url = id ? urls.tareaEdit : urls.tareaCreate;
            $.post(url, $(this).serialize(), function (res) {
                if (res && res.success) { $("#modal-tarea").modal("hide"); cargarTareas(); }
            });
        });
    };

    return obj;
}());
