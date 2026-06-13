var MapaAulasAdm = (function () {
    var obj = {};

    function nivelOcupacion(actuales) {
        if (actuales === 0) return { cls: 'success', label: 'Libre' };
        if (actuales <= 2) return { cls: 'info',    label: 'Baja' };
        if (actuales <= 4) return { cls: 'warning', label: 'Media' };
        return { cls: 'danger', label: 'Alta' };
    }

    obj.init = function (urlGet) {
        $.getJSON(urlGet, function (data) {
            var rows = data || [];
            var panel = $("#panel-aulas").empty();

            if (!rows.length) {
                panel.html('<div class="col-12 text-center text-muted py-5">No hay aulas registradas.</div>');
                return;
            }

            $.each(rows, function (_, a) {
                var nivel = nivelOcupacion(a.HorariosActuales);
                panel.append(
                    '<div class="col-6 col-md-4 col-lg-3">' +
                        '<div class="card p-3 h-100 border-' + nivel.cls + '">' +
                            '<div class="d-flex justify-content-between align-items-start mb-2">' +
                                '<h6 class="fw-bold mb-0">' + a.Aul_Descripcion + '</h6>' +
                                '<span class="badge badge-' + nivel.cls + '">' + nivel.label + '</span>' +
                            '</div>' +
                            '<div class="text-muted small">Horarios año actual: <strong>' + a.HorariosActuales + '</strong></div>' +
                            '<div class="text-muted small">Total histórico: ' + a.Horarios_Total + '</div>' +
                        '</div>' +
                    '</div>'
                );
            });
        });
    };

    return obj;
}());
