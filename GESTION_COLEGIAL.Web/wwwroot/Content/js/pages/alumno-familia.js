/* Pestaña Familia de la ficha del alumno: ligar / desligar un encargado ya registrado.
   Usa delegación porque el contenido de la pestaña se carga por AJAX. */
(function () {
    'use strict';

    var $modal = null;

    function recargarFamilia(aluId) {
        var $panel = $('#af-tab-content-familia');
        $.get('/Alumnos/GetFamiliaAsync', { id: aluId }, function (html) { $panel.html(html); }, 'html');
    }

    function aviso(texto, tipo) {
        if (typeof alertConfig !== 'undefined' && alertConfig.alert) alertConfig.alert(texto, tipo);
        else alert(texto);
    }

    function armarModal() {
        if ($modal) return $modal;
        $modal = $(
            '<div class="modal fade" id="modal-ligar-encargado" tabindex="-1" role="dialog" aria-hidden="true">' +
            '<div class="modal-dialog modal-dialog-centered" role="document"><div class="modal-content">' +
            '<div class="modal-header"><h5 class="modal-title">Ligar un encargado al alumno</h5>' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Cerrar"><span aria-hidden="true">&times;</span></button></div>' +
            '<div class="modal-body">' +
            '<div class="form-group"><label for="fam-enc">Encargado registrado</label><select id="fam-enc" class="custom-select"></select></div>' +
            '<div class="form-group mb-0"><label for="fam-par">Parentesco</label><select id="fam-par" class="custom-select"></select></div>' +
            '<small class="form-text text-muted mt-2">Si el encargado todavía no existe, regístrelo primero en <a href="/Encargados/Index">Encargados</a>.</small>' +
            '</div>' +
            '<div class="modal-footer"><button type="button" class="btn" data-dismiss="modal">Cancelar</button>' +
            '<button type="button" class="btn btn-primary" id="fam-guardar"><i class="bi bi-link-45deg mr-1"></i> Ligar</button></div>' +
            '</div></div></div>'
        ).appendTo('body');
        return $modal;
    }

    $(document).on('click', '.af-fam-ligar', function () {
        var aluId = $(this).data('alu');
        var $m = armarModal();
        $m.data('alu', aluId);
        var $enc = $m.find('#fam-enc').empty().append($('<option>').val('').text('Cargando...'));
        var $par = $m.find('#fam-par').empty();
        $.getJSON('/Alumnos/OpcionesFamiliaAsync').done(function (res) {
            var d = (res && res.data) || { encargados: [], parentescos: [] };
            $enc.empty().append($('<option>').val('').text('-- Seleccionar encargado --'));
            $.each(d.encargados, function (_, e) {
                $enc.append($('<option>').val(e.Value).text((e.Text || '') + (e.Identidad ? ' (' + e.Identidad + ')' : '')));
            });
            $par.empty().append($('<option>').val('').text('-- Seleccionar parentesco --'));
            $.each(d.parentescos, function (_, p) { $par.append($('<option>').val(p.Value).text(p.Text)); });
        }).fail(function () { $enc.empty().append($('<option>').val('').text('No se pudo cargar la lista')); });
        $m.modal('show');
    });

    $(document).on('click', '#fam-guardar', function () {
        var $m = $('#modal-ligar-encargado');
        var aluId = $m.data('alu'), encId = $('#fam-enc').val(), parId = $('#fam-par').val();
        if (!encId || !parId) { aviso('Elija el encargado y el parentesco.', 'error'); return; }
        var $btn = $(this).prop('disabled', true);
        $.post('/Alumnos/VincularEncargadoAsync', { aluId: aluId, encId: encId, parId: parId }, null, 'json')
            .done(function (r) {
                if (r && r.success) { $m.modal('hide'); recargarFamilia(aluId); aviso(r.message, 'success'); }
                else aviso((r && r.message) || 'No se pudo ligar al encargado.', 'error');
            })
            .fail(function () { aviso('No se pudo ligar al encargado.', 'error'); })
            .always(function () { $btn.prop('disabled', false); });
    });

    $(document).on('click', '.af-fam-desligar', function () {
        var aluId = $(this).data('alu'), encId = $(this).data('enc'), nombre = $(this).data('nombre') || 'este encargado';
        if (!confirm('¿Desligar a ' + nombre + ' de este alumno? El encargado seguirá registrado en el sistema.')) return;
        $.post('/Alumnos/DesvincularEncargadoAsync', { aluId: aluId, encId: encId }, null, 'json')
            .done(function (r) {
                if (r && r.success) { recargarFamilia(aluId); aviso(r.message, 'success'); }
                else aviso((r && r.message) || 'No se pudo desligar al encargado.', 'error');
            })
            .fail(function () { aviso('No se pudo desligar al encargado.', 'error'); });
    });
}());
