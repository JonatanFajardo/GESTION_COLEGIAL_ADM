$(document).ready(function () {
    $('#datatable').DataTable({
        ajax: {
            url: '/Pagos/ListAsync',
            dataSrc: 'data'
        },
        columns: [
            { data: 'Fila' },
            {
                data: 'FechaPago',
                render: function (data) {
                    return new Date(data).toLocaleDateString('es-HN');
                }
            },
            { data: 'Alumno' },
            { data: 'Encargado' },
            { data: 'FormaPago' },
            {
                data: 'MontoTotal',
                render: function (data) {
                    return '<strong class="text-success">L ' + parseFloat(data).toFixed(2) + '</strong>';
                }
            },
            { data: 'Usuario' },
            {
                data: null,
                render: function (data) {
                    return '<button class="btn btn-sm btn-primary recibo-btn" data-id="' + data.PagoId + '"><i class="mdi mdi-printer"></i> Recibo</button>';
                }
            }
        ],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json'
        }
    });

    // Evento para el botón de recibo
    $('#datatable').on('click', '.recibo-btn', function () {
        var pagoId = $(this).data('id');
        window.open('/Pagos/Recibo?pagoId=' + pagoId, '_blank');
    });
});
