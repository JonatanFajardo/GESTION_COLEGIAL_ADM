$(document).ready(function () {
    var table = $('#datatable').DataTable({
        ajax: {
            url: "/CuentasCobrar/ListAsync",
            dataSrc: function (json) {
                // Los datos vienen envueltos en {data: [...]}
                var data = json.data || json;
                console.log('Datos procesados:', data);
                return data;
            }
        },
        columns: [
            { data: 'Fila' },
            { data: 'Concepto' },
            { data: 'Alumno' },
            {
                data: 'Pendiente',
                render: function (data) {
                    return '<strong class="text-primary">L ' + parseFloat(data).toFixed(2) + '</strong>';
                }
            },
            {
                data: 'FechaVence',
                render: function (data) {
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-HN');
                    }
                    return '';
                }
            },
            {
                data: 'EstadoPago',
                render: function (data) {
                    if (data === 'Pendiente') return '<span class="badge badge-warning">Pendiente</span>';
                    if (data === 'Pagado') return '<span class="badge badge-success">Pagado</span>';
                    if (data === 'Vencido') return '<span class="badge badge-danger">Vencido</span>';
                    return '<span class="badge badge-secondary">' + data + '</span>';
                }
            }
        ],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json'
        },
        responsive: true,
        order: [[0, 'asc']]
    });
});
