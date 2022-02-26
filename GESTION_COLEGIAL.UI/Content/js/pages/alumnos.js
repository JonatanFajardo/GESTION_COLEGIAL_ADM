var Alumnos = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Alu_Id",
                    Size: 200   
                },
                {
                    FieldName: "Per_Identidad"
                },
                {
                    FieldName: "Alu_Nombre"
                },
                {
                    FieldName: "Cno_Descripcion"
                },
                {
                    FieldName: "Est_Descripcion"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());

$("#Niv_Id").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetCursosNiveles/',
        data:data,
        dataType: "json",
        success: function (response) {
                
            if (response != null) {
                FillDropDown("#Cddl_CursoNiveles", response);
            }
            else {
            }
        }, 
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });    
});


$("#Cddl_CursoNiveles").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetModalidades/',
        data: data,
        dataType: "json",
        success: function (response) {

            if (response != null) {
                FillDropDown("#Cddl_Modalidades", response);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});


$("#Cddl_Modalidades").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetCursos/',
        data: data,
        dataType: "json",
        success: function (response) {

            if (response != null) {
                FillDropDown("#Cddl_Cursos", response.data.Cur_Id, response.data.Cur_Nombre);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});

$("#Cddl_Cursos").on("change", function (e) {
    var valueSelected = $(this).val();
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetSecciones/',
        data: data,
        dataType: "json",
        success: function (response) {

            if (response != null) {
                FillDropDown("#Cddl_CursoSecciones", response.data.Sec_Id, response.data.Sec_Descripcion);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});
$(function () {
});

/**
 * Permite ingresar data a un dropdown.
 * @param {any} dropDownId
 * @param {any} list El primer valor debe de ser el valor y el segundo el texto a mostrar.
 */
function FillDropDown(dropDownId, list) {
    $(dropDownId).empty();
    console.log(list);
    $.each(list, function (index, row) {
        console.log(row);
            //console.log(row[]);
        //if (index != 0) { 
        //    $(dropDownId).append("<option value='" + list["data"][0] + "' selected='selected'>" + Text + "</option>");
        //} else {
        //    $(dropDownId).append("<option value='" + Value + "'>" + Text + "</option>")
        //}
         if (index != 0) { 
             $(dropDownId).append("<option value='" + row.Value + "' selected='selected'>" + row.Text + "</option>");
        } else {
             $(dropDownId).append("<option value='" + row.Value + "'>" + row.Text + "</option>")
        }
    });
}