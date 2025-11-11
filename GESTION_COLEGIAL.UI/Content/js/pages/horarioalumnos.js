var HorarioAlumnos = (function () {
    var obj = {};
    var modoEdicion = false;
    var horarioEditando = null;

    // Inicializar cuando la página esté lista
    obj.init = function () {
        $(function () {
            // Cargar datos iniciales
            cargarCursos();
            cargarCursosNiveles();
            cargarMaterias();
            cargarDias();
            cargarHoras();

            // Inicializar DataTable
            inicializarDataTable();

            // Configurar eventos
            configurarEventos();
        });
    };

    function cargarCursos() {
        $.ajax({
            type: "GET",
            url: '/HorarioAlumnos/GetCursosAsync',
            dataType: "json",
            success: function (response) {
                if (response && response.data) {
                    llenarDropdown('#Cur_Id', response.data, 'cur_Id', 'cur_Nombre');
                }
            },
            error: function (error) {
                console.log('Error cargando cursos:', error);
            }
        });
    }

    function cargarCursosNiveles() {
        $.ajax({
            type: "GET",
            url: '/HorarioAlumnos/GetCursosNivelesAsync',
            dataType: "json",
            success: function (response) {
                if (response && response.data) {
                    llenarDropdown('#Cun_Id', response.data, 'cun_Id', 'cun_Descripcion');
                }
            },
            error: function (error) {
                console.log('Error cargando niveles:', error);
            }
        });
    }

    function cargarMaterias() {
        $.ajax({
            type: "GET",
            url: '/HorarioAlumnos/GetMateriasAsync',
            dataType: "json",
            success: function (response) {
                if (response && response.data) {
                    llenarDropdown('#Mat_Id', response.data, 'mat_Id', 'mat_Nombre');
                }
            },
            error: function (error) {
                console.log('Error cargando materias:', error);
            }
        });
    }

    function cargarDias() {
        $.ajax({
            type: "GET",
            url: '/HorarioAlumnos/GetDiasAsync',
            dataType: "json",
            success: function (response) {
                if (response && response.data) {
                    llenarDropdown('#Dia_Id', response.data, 'dia_Id', 'dia_Descripcion');
                }
            },
            error: function (error) {
                console.log('Error cargando días:', error);
            }
        });
    }

    function cargarHoras() {
        $.ajax({
            type: "GET",
            url: '/HorarioAlumnos/GetHorasAsync',
            dataType: "json",
            success: function (response) {
                if (response && response.data) {
                    llenarDropdown('#HoAl_HoraInicio', response.data, 'hor_Id', 'hor_Hora');
                    llenarDropdown('#HoAl_HoraFinaliza', response.data, 'hor_Id', 'hor_Hora');
                }
            },
            error: function (error) {
                console.log('Error cargando horas:', error);
            }
        });
    }

    function llenarDropdown(selector, data, valueField, textField) {
        var dropdown = $(selector);
        dropdown.empty();
        dropdown.append('<option value="">Seleccione...</option>');

        if (data && data.length > 0) {
            $.each(data, function (index, item) {
                dropdown.append($('<option></option>')
                    .val(item[valueField])
                    .text(item[textField]));
            });
        }
    }

    function inicializarDataTable() {
        var table = $('#datatable').DataTable({
            processing: true,
            serverSide: false,
            ajax: {
                url: '/HorarioAlumnos/ListAsync',
                type: 'GET',
                dataSrc: 'data'
            },
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return '<div>' + (row.cur_Nombre || 'N/A') + '<br><small class="text-muted">' + (row.cun_Descripcion || '') + '</small></div>';
                    }
                },
                {
                    data: 'mat_Nombre',
                    defaultContent: 'N/A'
                },
                {
                    data: 'dia_Descripcion',
                    defaultContent: 'N/A'
                },
                {
                    data: 'hoAl_HoraInicioDescripcion',
                    defaultContent: 'N/A'
                },
                {
                    data: 'hoAl_HoraFinalizaDescripcion',
                    defaultContent: 'N/A'
                },
                {
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return '<div class="btn-group" role="group">' +
                            '<button type="button" class="btn btn-sm btn-primary edit-btn" data-id="' + row.hoAl_Id + '" title="Editar">' +
                            '<i class="mdi mdi-pencil"></i>' +
                            '</button>' +
                            '<button type="button" class="btn btn-sm btn-danger delete-btn" data-id="' + row.hoAl_Id + '" title="Eliminar">' +
                            '<i class="mdi mdi-delete"></i>' +
                            '</button>' +
                            '</div>';
                    }
                }
            ],
            language: {
                processing: '<div class="d-flex justify-content-center py-3"><i class="fa-solid fa-spinner fa-spin-pulse fa-3x" style="color: #F7A400;"></i></div>',
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No se encontraron horarios registrados",
                emptyTable: "No hay horarios disponibles",
                info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                infoEmpty: "Mostrando 0 a 0 de 0 registros",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                search: "Buscar:",
                searchPlaceholder: "Buscar horarios...",
                paginate: {
                    first: "Primero",
                    last: "Último",
                    next: "Siguiente",
                    previous: "Anterior"
                }
            },
            responsive: true,
            dom: 'Bfrtip',
            buttons: [
                {
                    text: '<i class="mdi mdi-refresh"></i>',
                    titleAttr: 'Recargar',
                    className: 'btn btn-secondary',
                    action: function (e, dt, node, config) {
                        dt.ajax.reload();
                    }
                }
            ]
        });

        // Eventos de los botones
        $('#datatable tbody').on('click', '.edit-btn', function () {
            var id = $(this).data('id');
            cargarHorario(id);
        });

        $('#datatable tbody').on('click', '.delete-btn', function () {
            var id = $(this).data('id');
            confirmarEliminar(id);
        });
    }

    function configurarEventos() {
        // Botón cancelar
        $('#cancel-btn').on('click', function () {
            cancelarEdicion();
        });

        // Limpiar formulario al enviar
        $('#horario-form').on('submit', function () {
            // La validación y envío se manejan con Ajax.BeginForm
        });
    }

    function cargarHorario(id) {
        $.ajax({
            type: 'GET',
            url: '/HorarioAlumnos/FindAsync',
            data: { id: id },
            success: function (response) {
                if (response && response.data) {
                    modoEdicion = true;
                    horarioEditando = response.data;

                    // Cambiar título
                    $('#form-title').text('Editar Horario');

                    // Cambiar la acción del formulario
                    $('#horario-form').attr('action', '/HorarioAlumnos/EditAsync');

                    // Llenar el formulario
                    $('#HoAl_Id').val(response.data.hoAl_Id);
                    $('#Cur_Id').val(response.data.cur_Id);
                    $('#Cun_Id').val(response.data.cun_Id);
                    $('#Mat_Id').val(response.data.mat_Id);
                    $('#Dia_Id').val(response.data.dia_Id);
                    $('#HoAl_HoraInicio').val(response.data.hoAl_HoraInicio);
                    $('#HoAl_HoraFinaliza').val(response.data.hoAl_HoraFinaliza);

                    // Mostrar botón cancelar y cambiar texto del botón submit
                    $('#cancel-btn').show();
                    $('#submit-btn').html('<i class="mdi mdi-content-save"></i> Actualizar');

                    // Scroll al formulario
                    $('html, body').animate({
                        scrollTop: $('#horario-form').offset().top - 100
                    }, 500);
                }
            },
            error: function (error) {
                console.log('Error al cargar horario:', error);
                toastr.error('Error al cargar el horario');
            }
        });
    }

    function cancelarEdicion() {
        modoEdicion = false;
        horarioEditando = null;

        // Limpiar formulario
        $('#horario-form')[0].reset();
        $('#HoAl_Id').val('0');

        // Restaurar título y botones
        $('#form-title').text('Registrar Nuevo Horario');
        $('#horario-form').attr('action', '/HorarioAlumnos/CreateAsync');
        $('#cancel-btn').hide();
        $('#submit-btn').html('<i class="mdi mdi-content-save"></i> Guardar');
    }

    function confirmarEliminar(id) {
        $('#delete-item-id').val(id);
        $('#delete-modal-secondary').modal('show');
    }

    // Callbacks para Ajax.BeginForm
    obj.onFormBegin = function () {
        // Deshabilitar botón de envío
        $('#submit-btn').prop('disabled', true);
    };

    obj.onFormSuccess = function (response) {
        $('#submit-btn').prop('disabled', false);

        if (response.success) {
            toastr.success(response.message || 'Operación exitosa');
            cancelarEdicion();
            $('#datatable').DataTable().ajax.reload();
        } else {
            toastr.error(response.message || 'Ocurrió un error');
        }
    };

    obj.onFormFailure = function (error) {
        $('#submit-btn').prop('disabled', false);
        console.log('Error en formulario:', error);
        toastr.error('Error al procesar la solicitud');
    };

    obj.onDeleteBegin = function () {
        // Mostrar loader en modal
    };

    obj.onDeleteSuccess = function (response) {
        if (response.success) {
            toastr.success(response.message || 'Horario eliminado exitosamente');
            $('#delete-modal-secondary').modal('hide');
            $('#datatable').DataTable().ajax.reload();
        } else {
            toastr.error(response.message || 'Error al eliminar');
        }
    };

    obj.onDeleteComplete = function () {
        // Ocultar loader
    };

    obj.onDeleteFailure = function (error) {
        console.log('Error al eliminar:', error);
        toastr.error('Error al eliminar el horario');
    };

    return obj;
}());

// Inicializar cuando el documento esté listo
$(document).ready(function () {
    HorarioAlumnos.init();
});
