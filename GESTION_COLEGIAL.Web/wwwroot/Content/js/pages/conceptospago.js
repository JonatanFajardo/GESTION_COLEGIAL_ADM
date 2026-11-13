var ConceptosPago = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "ConceptoPagoId",
                    Size: 100
                },
                {
                    FieldName: "Descripcion"
                },
                {
                    FieldName: "EsRecurrente",
                    Size: 130,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === true || data === 'true' || data === 'True') {
                                return '<span class="badge outline-badge-success"><i class="fas fa-check-circle mr-1"></i>RECURRENTE</span>';
                            } else {
                                return '<span class="badge outline-badge-secondary"><i class="fas fa-minus-circle mr-1"></i>NO RECURRENTE</span>';
                            }
                        }
                        return data;
                    }
                },
                {
                    FieldName: "EsObligatorio",
                    Size: 140,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === true || data === 'true' || data === 'True') {
                                return '<span class="badge outline-badge-warning"><i class="fas fa-exclamation-circle mr-1"></i>OBLIGATORIO</span>';
                            } else {
                                return '<span class="badge outline-badge-secondary"><i class="fas fa-minus-circle mr-1"></i>OPCIONAL</span>';
                            }
                        }
                        return data;
                    }
                },
                {
                    FieldName: "EsActivo",
                    Size: 110
                },
                {
                    FieldName: "CantTarifas",
                    Size: 100
                }
            ]
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
