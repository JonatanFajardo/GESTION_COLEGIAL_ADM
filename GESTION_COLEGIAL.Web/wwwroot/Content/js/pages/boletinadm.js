var BoletinAdm = (function () {
    var obj = {};

    obj.init = function (urlGet, urlDropdowns) {
        $.getJSON(urlDropdowns, function (data) {
            var sa = $("#f-alu");
            $.each(data.alumnos, function (_, a) {
                sa.append('<option value="' + a.id + '">' + a.texto + '</option>');
            });
        });

        $("#btn-ver").on("click", function () {
            var alu  = $("#f-alu").val();
            var anio = $("#f-anio").val();
            if (!alu || !anio) { alert("Seleccione el alumno y el año."); return; }

            $.getJSON(urlGet, { Alu_Id: alu, Anio: anio }, function (res) {
                // La API puede devolver directo el array o envuelto en .data
                var data = (res && res.data) ? res.data : (res || []);
                var tbody = $("#tbody-boletin").empty();

                if (!data || data.length === 0) {
                    tbody.append('<tr><td colspan="5" class="text-center text-muted">Sin notas registradas para este alumno.</td></tr>');
                    $("#panel-boletin").show();
                    return;
                }

                // Agrupar por materia: { Mat_Nombre: { P1: nota, P2: nota, P3: nota } }
                var materias = {};
                $.each(data, function (_, r) {
                    var mat = r.Mat_Nombre || "Sin materia";
                    var par = r.Par_Numero || r.Pac_Numero || r.Pac_Descripcion || "P?";
                    if (!materias[mat]) materias[mat] = {};
                    materias[mat][par] = r.Not_Nota;
                });

                $.each(materias, function (mat, parciales) {
                    var p1 = parciales[1] || parciales["1"] || parciales["Parcial 1"] || "-";
                    var p2 = parciales[2] || parciales["2"] || parciales["Parcial 2"] || "-";
                    var p3 = parciales[3] || parciales["3"] || parciales["Parcial 3"] || "-";

                    // Calcular promedio solo con los que tienen nota numérica
                    var vals = [p1, p2, p3].filter(function (v) { return !isNaN(parseFloat(v)); });
                    var prom = vals.length > 0
                        ? (vals.reduce(function (a, b) { return parseFloat(a) + parseFloat(b); }, 0) / vals.length).toFixed(2)
                        : "-";

                    var cls = (!isNaN(parseFloat(prom)) && parseFloat(prom) < 60) ? 'text-danger fw-bold' : '';

                    tbody.append(
                        '<tr>' +
                        '<td>' + mat + '</td>' +
                        '<td class="text-center">' + p1 + '</td>' +
                        '<td class="text-center">' + p2 + '</td>' +
                        '<td class="text-center">' + p3 + '</td>' +
                        '<td class="text-center ' + cls + '">' + prom + '</td>' +
                        '</tr>'
                    );
                });

                $("#panel-boletin").show();
            });
        });
    };

    return obj;
}());
