/* Vínculos familiares: cola de Secretaría para ligar encargados con sus hijos.
   Dos listas (alumnos sin encargado / encargados sin hijos) y un cuadro para ligar con buscador. */
(function () {
    'use strict';

    var $raiz = $('#vf-raiz');
    var urls = {
        alumnos: $raiz.data('urlAlumnos'), encargados: $raiz.data('urlEncargados'),
        opciones: $raiz.data('urlOpciones'), vincular: $raiz.data('urlVincular')
    };
    var LOTE = 50;
    var estado = { alumnos: [], encargados: [], visibles: { alumnos: LOTE, encargados: LOTE }, opciones: null, ultimoParentesco: '', actual: null, filtradas: [] };

    function esc(s) {
        return String(s === null || s === undefined ? '' : s)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }
    function norm(s) {
        return String(s || '').toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '');
    }
    function aviso(texto, tipo) {
        if (typeof alertConfig !== 'undefined' && alertConfig.alert) alertConfig.alert(texto, tipo);
        else alert(texto);
    }
    function datos(res) { return (res && res.data) ? res.data : []; }

    // ── Indicadores ──────────────────────────────────────────────────────────
    function pintarIndicadores() {
        var o = estado.opciones;
        var totalAlu = o ? o.alumnos.length : 0, totalEnc = o ? o.encargados.length : 0;
        var sinEnc = estado.alumnos.length, sinHijos = estado.encargados.length;
        $('#vf-kpi-alumnos').text(sinEnc);
        $('#vf-kpi-alumnos-nota').text(totalAlu ? 'de ' + totalAlu + ' alumnos registrados' : ' ');
        $('#vf-kpi-encargados').text(sinHijos);
        $('#vf-kpi-encargados-nota').text(totalEnc ? 'de ' + totalEnc + ' encargados registrados' : ' ');
        var pct = totalAlu ? Math.max(0, Math.min(100, Math.round((totalAlu - sinEnc) * 100 / totalAlu))) : 0;
        $('#vf-kpi-avance').text(totalAlu ? pct + ' %' : '—');
        $('#vf-kpi-barra').css('width', pct + '%');
    }

    // Se pintan de a LOTE filas para que la lista no pese con cientos de alumnos.
    function mostrarMas(lista, total) {
        var resto = total - estado.visibles[lista];
        var $c = $('#vf-mas-' + lista);
        if (resto > 0) $c.prop('hidden', false).find('button').text('Mostrar ' + Math.min(LOTE, resto) + ' más (faltan ' + resto + ')');
        else $c.prop('hidden', true);
    }

    // ── Listas ───────────────────────────────────────────────────────────────
    function pintarAlumnos() {
        var q = norm($('#vf-buscar-alumnos').val());
        var filas = estado.alumnos.filter(function (a) { return !q || norm(a.Nombre + ' ' + a.Identidad + ' ' + a.Detalle).indexOf(q) >= 0; });
        var $t = $('#vf-filas-alumnos').empty();
        if (!filas.length) {
            mostrarMas('alumnos', 0);
            $t.append('<tr><td colspan="4" class="text-center text-muted">' +
                (estado.alumnos.length ? 'Ningún alumno coincide con la búsqueda.' : 'Todos los alumnos activos ya tienen encargado.') + '</td></tr>');
            return;
        }
        mostrarMas('alumnos', filas.length);
        filas.slice(0, estado.visibles.alumnos).forEach(function (a) {
            $t.append('<tr><td>' + esc(a.Nombre) + '</td><td>' + esc(a.Identidad) + '</td><td>' + esc(a.Detalle) +
                '</td><td class="text-right"><button type="button" class="btn btn-sm btn-primary vf-ligar-alumno" data-id="' + a.Id + '">' +
                '<i class="bi bi-link-45deg mr-1" aria-hidden="true"></i>Ligar encargado</button></td></tr>');
        });
    }

    function pintarEncargados() {
        var q = norm($('#vf-buscar-encargados').val());
        var filas = estado.encargados.filter(function (e) { return !q || norm(e.Nombre + ' ' + e.Identidad + ' ' + e.Telefono).indexOf(q) >= 0; });
        var $t = $('#vf-filas-encargados').empty();
        if (!filas.length) {
            mostrarMas('encargados', 0);
            $t.append('<tr><td colspan="4" class="text-center text-muted">' +
                (estado.encargados.length ? 'Ningún encargado coincide con la búsqueda.' : 'Todos los encargados ya tienen al menos un hijo ligado.') + '</td></tr>');
            return;
        }
        mostrarMas('encargados', filas.length);
        filas.slice(0, estado.visibles.encargados).forEach(function (e) {
            $t.append('<tr><td>' + esc(e.Nombre) + '</td><td>' + esc(e.Identidad) + '</td><td>' + esc(e.Telefono) +
                '</td><td class="text-right"><button type="button" class="btn btn-sm btn-primary vf-ligar-encargado" data-id="' + e.Id + '">' +
                '<i class="bi bi-link-45deg mr-1" aria-hidden="true"></i>Ligar hijo</button></td></tr>');
        });
    }

    function cargar() {
        $.getJSON(urls.alumnos).done(function (r) { estado.alumnos = datos(r); pintarAlumnos(); pintarIndicadores(); })
            .fail(function () { $('#vf-filas-alumnos').html('<tr><td colspan="4" class="text-center text-danger">No se pudo cargar la lista.</td></tr>'); });
        $.getJSON(urls.encargados).done(function (r) { estado.encargados = datos(r); pintarEncargados(); pintarIndicadores(); })
            .fail(function () { $('#vf-filas-encargados').html('<tr><td colspan="4" class="text-center text-danger">No se pudo cargar la lista.</td></tr>'); });
        $.getJSON(urls.opciones).done(function (r) {
            estado.opciones = r.data || { alumnos: [], encargados: [], parentescos: [] };
            var $p = $('#vf-parentesco').empty().append('<option value="">-- Seleccionar parentesco --</option>');
            estado.opciones.parentescos.forEach(function (p) { $p.append($('<option>').val(p.Value).text(p.Text)); });
            pintarIndicadores();
        });
    }

    // ── Cuadro para ligar ────────────────────────────────────────────────────
    function llenarOpciones() {
        var q = norm($('#vf-buscar-opcion').val());
        var $s = $('#vf-opcion').empty();
        var lista = estado.actual.lista.filter(function (o) { return !q || norm(o.Text + ' ' + o.Identidad).indexOf(q) >= 0; });
        lista.slice(0, 200).forEach(function (o) {
            $s.append($('<option>').val(o.Value).text((o.Text || '') + (o.Identidad ? ' (' + o.Identidad + ')' : '') + (o.Marca || '')));
        });
        $('#vf-modal-ayuda').text(lista.length > 200 ? 'Se muestran 200 de ' + lista.length + ': escriba más letras para acotar.'
            : (lista.length ? lista.length + ' resultado(s).' : 'Sin resultados.'));
        if (lista.length === 1) $s.val(String(lista[0].Value));
    }

    function abrir(modo, id) {
        if (!estado.opciones) { aviso('Las listas todavía están cargando. Intente de nuevo en un momento.', 'error'); return; }
        var o = estado.opciones, fijo, lista;
        if (modo === 'alumno') {
            var a = estado.alumnos.filter(function (x) { return x.Id === id; })[0];
            if (!a) return;
            var sinHijos = {};
            estado.encargados.forEach(function (e) { sinHijos[e.Id] = true; });
            // Primero los encargados que aún no tienen hijos.
            lista = o.encargados.map(function (e) { return { Value: e.Value, Text: e.Text, Identidad: e.Identidad, Marca: sinHijos[e.Value] ? ' · sin hijos' : '', Orden: sinHijos[e.Value] ? 0 : 1 }; })
                .sort(function (x, y) { return x.Orden - y.Orden; });
            fijo = 'Alumno: <strong>' + esc(a.Nombre) + '</strong>' + (a.Detalle ? ' (' + esc(a.Detalle) + ')' : '');
            $('#vf-modal-titulo').text('Ligar un encargado al alumno');
            $('#vf-modal-etiqueta').text('Encargado');
            $('#vf-buscar-opcion').attr('placeholder', 'Escriba parte del nombre o la identidad del encargado');
        } else {
            var e = estado.encargados.filter(function (x) { return x.Id === id; })[0];
            if (!e) return;
            var sinEnc = {};
            estado.alumnos.forEach(function (x) { sinEnc[x.Id] = true; });
            lista = o.alumnos.map(function (x) { return { Value: x.Value, Text: x.Text, Identidad: x.Identidad, Marca: sinEnc[x.Value] ? ' · sin encargado' : '', Orden: sinEnc[x.Value] ? 0 : 1 }; })
                .sort(function (x, y) { return x.Orden - y.Orden; });
            fijo = 'Encargado: <strong>' + esc(e.Nombre) + '</strong>' + (e.Telefono ? ' (tel. ' + esc(e.Telefono) + ')' : '');
            $('#vf-modal-titulo').text('Ligar un hijo al encargado');
            $('#vf-modal-etiqueta').text('Alumno');
            $('#vf-buscar-opcion').attr('placeholder', 'Escriba parte del nombre o la identidad del alumno');
        }
        estado.actual = { modo: modo, id: id, lista: lista };
        $('#vf-modal-fijo').html(fijo);
        $('#vf-buscar-opcion').val('');
        $('#vf-parentesco').val(estado.ultimoParentesco);
        llenarOpciones();
        $('#vf-modal').modal('show');
        setTimeout(function () { $('#vf-buscar-opcion').trigger('focus'); }, 300);
    }

    function guardar() {
        var a = estado.actual;
        var opcion = $('#vf-opcion').val(), par = $('#vf-parentesco').val();
        if (!opcion) { aviso('Elija ' + (a.modo === 'alumno' ? 'el encargado' : 'el alumno') + ' de la lista.', 'error'); return; }
        if (!par) { aviso('Elija el parentesco.', 'error'); return; }
        var aluId = a.modo === 'alumno' ? a.id : opcion, encId = a.modo === 'alumno' ? opcion : a.id;
        var $btn = $('#vf-guardar').prop('disabled', true);
        $.post(urls.vincular, { aluId: aluId, encId: encId, parId: par }, null, 'json')
            .done(function (r) {
                if (r && r.success) {
                    estado.ultimoParentesco = par;
                    $('#vf-modal').modal('hide');
                    aviso(r.message, 'success');
                    cargar();
                } else aviso((r && r.message) || 'No se pudo ligar.', 'error');
            })
            .fail(function () { aviso('No se pudo ligar. Revise su conexión e intente de nuevo.', 'error'); })
            .always(function () { $btn.prop('disabled', false); });
    }

    $(document).on('click', '.vf-ligar-alumno', function () { abrir('alumno', parseInt($(this).data('id'), 10)); });
    $(document).on('click', '.vf-ligar-encargado', function () { abrir('encargado', parseInt($(this).data('id'), 10)); });
    $('#vf-buscar-alumnos').on('input', function () { estado.visibles.alumnos = LOTE; pintarAlumnos(); });
    $('#vf-buscar-encargados').on('input', function () { estado.visibles.encargados = LOTE; pintarEncargados(); });
    $(document).on('click', '.vf-mas', function () {
        var l = $(this).data('lista');
        estado.visibles[l] += LOTE;
        if (l === 'alumnos') pintarAlumnos(); else pintarEncargados();
    });
    $('#vf-buscar-opcion').on('input', function () { if (estado.actual) llenarOpciones(); });
    $('#vf-opcion').on('dblclick', function () { if ($(this).val() && $('#vf-parentesco').val()) guardar(); });
    $('#vf-guardar').on('click', guardar);

    cargar();
}());
