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
                FillDropDown("#Cun_Id", response.data);
            }
            else {
            }
        }, 
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });    
});


$("#Cun_Id").on("change", function (e) {
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
                FillDropDown("#Mda_Id", response.data);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});


$("#Mda_Id").on("change", function (e) {
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
                FillDropDown("#Cur_Id", response.data);
            }
            else {
            }
        },
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
});

$("#Cur_Id").on("change", function (e) {
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
                console.log(response);
                FillDropDown("#Sec_Id", response.data);
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