var Deudores = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            $('#datatable').DataTable({
                ajax: {
                    url: Direction.urlList,
                    dataSrc: function (json) {
                        var data = json.data || json;
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
                            return '<strong class="text-danger">L ' + parseFloat(data).toFixed(2) + '</strong>';
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
                order: [[3, 'desc']]
            });
        });
    }
    return obj;
}());
