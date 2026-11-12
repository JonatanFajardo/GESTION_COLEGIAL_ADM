var Ficha360EmpleadoAdm = (function () {
    var obj = {};

    function kv(label, val) {
        return '<div class="mb-2"><small class="text-muted">' + label + '</small><div class="fw-semibold">' + (val || '—') + '</div></div>';
    }

    obj.init = function (urlGet, urlDropdown) {
        $.getJSON(urlDropdown, function (data) {
            var sel = $("#f-emp");
            $.each(data, function (_, e) {
                sel.append('<option value="' + e.id + '">' + e.texto + '</option>');
            });
        });

        $("#btn-ver").on("click", function () {
            var id = $("#f-emp").val();
            if (!id) { alert("Seleccione un empleado."); return; }

            $.getJSON(urlGet, { Emp_Id: id }, function (res) {
                var d = res.item || res || {};
                $("#datos-personales").html(
                    kv("Nombre completo", d.Emp_NombreCompleto) +
                    kv("Código", d.Emp_Codigo) +
                    kv("Fecha nacimiento", d.Emp_FechaNacimiento) +
                    kv("Género", d.Emp_Genero)
                );
                $("#datos-laborales").html(
                    kv("Cargo", d.Car_Descripcion) +
                    kv("Título", d.Tit_Descripcion) +
                    kv("Fecha ingreso", d.Emp_FechaIngreso) +
                    kv("Antigüedad", d.Antiguedad)
                );
                $("#datos-historial").html(
                    kv("Vacaciones pendientes", d.VacacionesPendientes) +
                    kv("Asistencia mes", d.AsistenciaMes) +
                    kv("Estado", d.Emp_Estado)
                );
                $("#panel-ficha").show();
            });
        });
    };

    return obj;
}());
