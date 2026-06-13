var Ficha360AlumnoAdm = (function () {
    var obj = {};

    function kv(label, val) {
        return '<div class="mb-2"><small class="text-muted">' + label + '</small><div class="fw-semibold">' + (val || '—') + '</div></div>';
    }

    obj.init = function (urlGet, urlDropdown) {
        $.getJSON(urlDropdown, function (data) {
            var sel = $("#f-alu");
            $.each(data, function (_, a) {
                sel.append('<option value="' + a.id + '">' + a.texto + '</option>');
            });
        });

        $("#btn-ver").on("click", function () {
            var id = $("#f-alu").val();
            if (!id) { alert("Seleccione un alumno."); return; }

            $.getJSON(urlGet, { Alu_Id: id }, function (res) {
                var d = res.item || res || {};
                $("#datos-personales").html(
                    kv("Nombre completo", d.Alu_NombreCompleto) +
                    kv("Fecha nacimiento", d.Alu_FechaNacimiento) +
                    kv("Género", d.Alu_Genero) +
                    kv("Encargado", d.Enc_NombreCompleto) +
                    kv("Teléfono encargado", d.Enc_Telefono)
                );
                $("#datos-academicos").html(
                    kv("Sección", d.Sec_Descripcion) +
                    kv("Curso", d.Cur_Nombre) +
                    kv("Nivel", d.Niv_Descripcion) +
                    kv("Año cursado", d.AnioCursado) +
                    kv("Promedio", d.Promedio)
                );
                $("#datos-financieros").html(
                    kv("Saldo pendiente", d.SaldoPendiente) +
                    kv("Último pago", d.UltimoPago) +
                    kv("Estado cuenta", d.EstadoCuenta)
                );
                $("#panel-ficha").show();
            });
        });
    };

    return obj;
}());
