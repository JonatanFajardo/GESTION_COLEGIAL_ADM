/* Directorio de graduados — drill-down: grid de promociones → detalle de promoción. */
var AlumnosGraduados = (function () {
    var cfg = null;
    var _all = [];
    var _porAnio = {};                 // { 2025: [g,g,g], 2024: [...] }
    var _promoActual = null;           // año seleccionado
    var _filtroPromoNivel = '__all__'; // filtro en landing
    var _filtroPromoTexto = '';
    var _filtroDetNivel = '__all__';   // filtro en detalle
    var _filtroDetTexto = '';

    var NIVELES = ['Preescolar', 'Primaria', 'Secundaria', 'Bachillerato'];
    var NIVEL_COLOR = {
        'Preescolar':   '#f59e0b',
        'Primaria':     '#3b82f6',
        'Secundaria':   '#10b981',
        'Bachillerato': '#8b5cf6'
    };
    var AVATAR_COLORS = [
        '#4f46e5', '#10b981', '#f59e0b', '#06b6d4',
        '#ec4899', '#8b5cf6', '#ef4444', '#14b8a6'
    ];

    function init(config) {
        cfg = config || {};
        $(document).ready(function () { cargar(); wireEvents(); });
    }

    function cargar() {
        $.getJSON(cfg.listUrl, function (resp) {
            _all = (resp && resp.data) ? resp.data : [];
            _porAnio = {};
            _all.forEach(function (g) {
                var a = g.AluGrad_AnioGraduacion || 'Sin año';
                (_porAnio[a] = _porAnio[a] || []).push(g);
            });
            renderKpis();
            renderChipsPromo();
            renderPromos();
        }).fail(function () {
            $('#grad-promos-container').html(vacio('No se pudo cargar el directorio.', 'exclamation-triangle'));
        });
    }

    function wireEvents() {
        // Landing
        $('#grad-search-promo').on('input', function () {
            _filtroPromoTexto = (this.value || '').trim();
            renderPromos();
        });
        $('#grad-promos-container').on('click', '.grad-promo-card', function () {
            abrirPromocion(parseInt($(this).data('anio'), 10));
        });

        // Detalle
        $('#btn-volver-promos').on('click', volverAPromos);
        $('#grad-search-detalle').on('input', function () {
            _filtroDetTexto = (this.value || '').trim().toLowerCase();
            renderDetalle();
        });
        $('#grad-detalle-container').on('click', '.grad-card', function () {
            verDetalle(parseInt($(this).data('id'), 10));
        });
    }

    // ═══════════ KPIs ═══════════
    function renderKpis() {
        var total = _all.length;
        var anios = Object.keys(_porAnio).map(Number).filter(function (n) { return !isNaN(n); })
                                       .sort(function (a, b) { return b - a; });
        var ultimo = anios[0];
        var porNivel = {};
        _all.forEach(function (g) {
            var n = g.AluGrad_NivelEducativo || '—';
            porNivel[n] = (porNivel[n] || 0) + 1;
        });
        var top = Object.keys(porNivel).sort(function (a, b) { return porNivel[b] - porNivel[a]; })[0] || '—';
        var pctTop = top !== '—' && total ? Math.round(porNivel[top] * 100 / total) + '%' : '';

        $('#kpi-total').text(total.toLocaleString('es-HN'));
        $('#kpi-promociones').text(anios.length);
        $('#kpi-promociones-hint').text(anios.length ? ('desde ' + anios[anios.length - 1]) : '');
        $('#kpi-ultimo-anio').text(ultimo || '—');
        $('#kpi-ultimo-count').text(ultimo ? (_porAnio[ultimo].length + ' egresados') : '');
        $('#kpi-nivel-top').text(top);
        $('#kpi-nivel-top-pct').text(pctTop);
    }

    // ═══════════ Chips landing (filtro por nivel presente en las promociones) ═══════════
    function renderChipsPromo() {
        var conteo = { __all__: 0 };
        NIVELES.forEach(function (n) { conteo[n] = 0; });
        _all.forEach(function (g) {
            conteo.__all__++;
            if (conteo[g.AluGrad_NivelEducativo] !== undefined) conteo[g.AluGrad_NivelEducativo]++;
        });
        var html = ''
            + '<button class="grad-chip active" data-nivel="__all__">'
            + 'Todos <span class="grad-chip-count">' + conteo.__all__ + '</span></button>';
        NIVELES.forEach(function (n) {
            if (!conteo[n]) return;
            html += '<button class="grad-chip" data-nivel="' + n + '">'
                 + n + ' <span class="grad-chip-count">' + conteo[n] + '</span></button>';
        });
        var $w = $('#grad-chips-niveles-promo').html(html);
        $w.off('click').on('click', '.grad-chip', function () {
            _filtroPromoNivel = $(this).data('nivel');
            $w.find('.grad-chip').removeClass('active');
            $(this).addClass('active');
            renderPromos();
        });
    }

    // ═══════════ Landing: grid de promociones ═══════════
    function renderPromos() {
        var anios = Object.keys(_porAnio).map(Number).filter(function (n) { return !isNaN(n); });

        // Filtro texto (año)
        if (_filtroPromoTexto) {
            anios = anios.filter(function (a) { return String(a).indexOf(_filtroPromoTexto) !== -1; });
        }
        // Filtro nivel (mostrar solo promos que tengan al menos 1 del nivel)
        if (_filtroPromoNivel !== '__all__') {
            anios = anios.filter(function (a) {
                return _porAnio[a].some(function (g) { return g.AluGrad_NivelEducativo === _filtroPromoNivel; });
            });
        }

        anios.sort(function (a, b) { return b - a; });

        var $c = $('#grad-promos-container').empty();
        if (!anios.length) {
            $c.html(vacio('No hay promociones con esos filtros.', 'inbox'));
            return;
        }

        var $grid = $('<div class="grad-promos-grid"></div>').appendTo($c);
        anios.forEach(function (a) {
            $grid.append(cardPromocion(a, _porAnio[a]));
        });
    }

    function cardPromocion(anio, graduados) {
        var mixCounts = {};
        graduados.forEach(function (g) {
            var n = g.AluGrad_NivelEducativo || '—';
            mixCounts[n] = (mixCounts[n] || 0) + 1;
        });
        var mixHtml = Object.keys(mixCounts).sort(function (a, b) { return mixCounts[b] - mixCounts[a]; })
            .map(function (n) {
                var color = NIVEL_COLOR[n] || '#94a3b8';
                return ''
                    + '<div class="grad-promo-mix-item">'
                    + '  <span class="grad-promo-mix-dot" style="background:' + color + '"></span>'
                    + '  <span class="grad-promo-mix-nombre">' + esc(n) + '</span>'
                    + '  <span class="grad-promo-mix-count">' + mixCounts[n] + '</span>'
                    + '</div>';
            }).join('');

        return ''
            + '<div class="grad-promo-card" data-anio="' + anio + '">'
            + '  <div class="grad-promo-top">'
            + '    <div class="grad-promo-top-icon"><i class="bi bi-mortarboard-fill"></i></div>'
            + '    <span>Promoción</span>'
            + '  </div>'
            + '  <div class="grad-promo-anio">' + anio + '</div>'
            + '  <div class="grad-promo-conteo"><strong>' + graduados.length + '</strong> ' + (graduados.length === 1 ? 'graduado' : 'graduados') + '</div>'
            + '  <div class="grad-promo-mix">' + mixHtml + '</div>'
            + '  <div class="grad-promo-cta">'
            + '    <span>Ver promoción</span>'
            + '    <i class="bi bi-arrow-right"></i>'
            + '  </div>'
            + '</div>';
    }

    // ═══════════ Detalle de una promoción ═══════════
    function abrirPromocion(anio) {
        _promoActual = anio;
        _filtroDetNivel = '__all__';
        _filtroDetTexto = '';
        $('#grad-search-detalle').val('');

        var lista = _porAnio[anio] || [];
        $('#detalle-anio').text('Promoción ' + anio);

        // Header sub: total + mix
        var mix = {};
        lista.forEach(function (g) {
            var n = g.AluGrad_NivelEducativo || '—';
            mix[n] = (mix[n] || 0) + 1;
        });
        var mixTxt = Object.keys(mix).sort(function (a, b) { return mix[b] - mix[a]; })
            .map(function (n) { return n + ' ' + mix[n]; }).join(' · ');
        $('#detalle-sub').text(lista.length + ' egresado' + (lista.length === 1 ? '' : 's') + ' · ' + mixTxt);

        // Chips de detalle (solo niveles presentes en esta promoción)
        var html = ''
            + '<button class="grad-chip active" data-nivel="__all__">'
            + 'Todos <span class="grad-chip-count">' + lista.length + '</span></button>';
        NIVELES.forEach(function (n) {
            if (!mix[n]) return;
            html += '<button class="grad-chip" data-nivel="' + n + '">'
                 + n + ' <span class="grad-chip-count">' + mix[n] + '</span></button>';
        });
        var $w = $('#grad-chips-niveles-detalle').html(html);
        $w.off('click').on('click', '.grad-chip', function () {
            _filtroDetNivel = $(this).data('nivel');
            $w.find('.grad-chip').removeClass('active');
            $(this).addClass('active');
            renderDetalle();
        });

        // Cambio de vista + breadcrumb
        $('#grad-view-promociones').hide();
        $('#grad-view-detalle').show();
        setBreadcrumb([
            { text: 'Directorio de graduados', link: true },
            { text: 'Promoción ' + anio, link: false }
        ]);
        window.scrollTo({ top: 0, behavior: 'smooth' });

        renderDetalle();
    }

    function renderDetalle() {
        var lista = (_porAnio[_promoActual] || []).filter(function (g) {
            if (_filtroDetNivel !== '__all__' && g.AluGrad_NivelEducativo !== _filtroDetNivel) return false;
            if (_filtroDetTexto) {
                var hay = ((g.AluGrad_NombreCompleto || '') + ' ' + (g.AluGrad_Identidad || '') + ' ' + (g.AluGrad_TituloObtenido || '')).toLowerCase();
                if (hay.indexOf(_filtroDetTexto) === -1) return false;
            }
            return true;
        });

        var $c = $('#grad-detalle-container');
        if (!lista.length) {
            $c.removeClass('grad-grid').html(vacio('No hay graduados con esos filtros.', 'inbox'));
            return;
        }
        $c.addClass('grad-grid').html(lista.map(cardGraduado).join(''));
    }

    function cardGraduado(g) {
        var iniciales = obtenerIniciales(g.AluGrad_NombreCompleto);
        var color = AVATAR_COLORS[Math.abs(hash(g.AluGrad_NombreCompleto || '')) % AVATAR_COLORS.length];
        var nivelClass = (g.AluGrad_NivelEducativo || '').toLowerCase();
        return ''
            + '<div class="grad-card" data-id="' + g.AluGrad_Id + '">'
            + '  <div class="grad-avatar" style="background:' + color + '">' + iniciales + '</div>'
            + '  <div class="grad-card-body">'
            + '    <div class="grad-card-nombre" title="' + esc(g.AluGrad_NombreCompleto) + '">' + esc(g.AluGrad_NombreCompleto) + '</div>'
            + '    <div class="grad-card-titulo" title="' + esc(g.AluGrad_TituloObtenido) + '">' + esc(g.AluGrad_TituloObtenido || '—') + '</div>'
            + '    <div class="grad-card-meta">'
            + '      <span class="grad-badge nivel-' + nivelClass + '">' + esc(g.AluGrad_NivelEducativo || '—') + '</span>'
            + '      <span class="grad-badge identidad">' + esc(g.AluGrad_Identidad || '') + '</span>'
            + '    </div>'
            + '  </div>'
            + '</div>';
    }

    function volverAPromos() {
        _promoActual = null;
        $('#grad-view-detalle').hide();
        $('#grad-view-promociones').show();
        setBreadcrumb([{ text: 'Directorio de graduados', link: false }]);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    function setBreadcrumb(items) {
        var html = '<li class="breadcrumb-item"><a href="/">Inicio</a></li>';
        items.forEach(function (it, i) {
            var isLast = i === items.length - 1;
            if (isLast || !it.link) {
                html += '<li class="breadcrumb-item active">' + esc(it.text) + '</li>';
            } else {
                html += '<li class="breadcrumb-item"><a href="#" data-bc-back="1">' + esc(it.text) + '</a></li>';
            }
        });
        $('#grad-breadcrumb').html(html);
        $('#grad-breadcrumb [data-bc-back]').off('click').on('click', function (e) {
            e.preventDefault();
            volverAPromos();
        });
    }

    // ═══════════ Modal ficha ═══════════
    function verDetalle(id) {
        $('#modal-graduado-body').html('Cargando...');
        $('#modal-graduado-detalle').modal('show');
        $.getJSON(cfg.findUrl, { id: id }, function (resp) {
            var d = (resp && resp.data) ? resp.data : null;
            if (!d) {
                $('#modal-graduado-body').html('<div class="alert alert-warning">No se encontró el registro.</div>');
                return;
            }
            $('#modal-graduado-body').html(renderFicha(d));
        });
    }

    // ═══════════ Ficha (contenido del modal) ═══════════
    function renderFicha(d) {
        var iniciales = obtenerIniciales(d.AluGrad_NombreCompleto);
        var color = AVATAR_COLORS[Math.abs(hash(d.AluGrad_NombreCompleto || '')) % AVATAR_COLORS.length];
        var nivelClass = (d.AluGrad_NivelEducativo || '').toLowerCase();

        var encargados = [];
        if (d.AluGrad_EncargadosJson) {
            try { encargados = JSON.parse(d.AluGrad_EncargadosJson) || []; }
            catch (e) { encargados = []; }
        }

        var sexoTxt = d.AluGrad_Sexo === 'F' ? 'Femenino' : d.AluGrad_Sexo === 'M' ? 'Masculino' : (d.AluGrad_Sexo || '—');
        var edad = calcularEdad(d.AluGrad_FechaNacimiento);

        return ''
            + '<div class="grad-ficha">'
            + '  <div class="grad-ficha-hero">'
            + '    <div class="grad-ficha-avatar" style="background:' + color + '">' + iniciales + '</div>'
            + '    <div class="grad-ficha-hero-body">'
            + '      <div class="grad-ficha-nombre">' + esc(d.AluGrad_NombreCompleto) + '</div>'
            + '      <div class="grad-ficha-titulo">' + esc(d.AluGrad_TituloObtenido || '—') + '</div>'
            + '      <div class="grad-ficha-badges">'
            + '        <span class="grad-badge nivel-' + nivelClass + '">' + esc(d.AluGrad_NivelEducativo || '—') + '</span>'
            + '        <span class="grad-badge identidad"><i class="bi bi-award"></i> Promoción ' + esc(d.AluGrad_AnioGraduacion || '—') + '</span>'
            + '      </div>'
            + '    </div>'
            + '  </div>'

            + '  <div class="grad-ficha-sections">'
            + seccion('Datos personales', 'person-vcard', [
                item('card-heading', 'Identidad', d.AluGrad_Identidad),
                item('gender-ambiguous', 'Sexo', sexoTxt),
                item('cake2', 'Fecha de nacimiento', fecha(d.AluGrad_FechaNacimiento) + (edad ? ' <span class="grad-ficha-hint">(' + edad + ' años)</span>' : '')),
                item('envelope', 'Correo', d.AluGrad_Correo ? '<a href="mailto:' + esc(d.AluGrad_Correo) + '">' + esc(d.AluGrad_Correo) + '</a>' : '—'),
                item('telephone', 'Teléfono', d.AluGrad_Telefono ? '<a href="tel:' + esc(String(d.AluGrad_Telefono).replace(/\s+/g,'')) + '">' + esc(d.AluGrad_Telefono) + '</a>' : '—')
              ])
            + seccion('Formación académica', 'mortarboard', [
                item('bookmark-star', 'Nivel educativo', d.AluGrad_NivelEducativo),
                item('patch-check', 'Título obtenido', d.AluGrad_TituloObtenido),
                item('box-arrow-in-right', 'Año de ingreso', d.AluGrad_AnioIngreso),
                item('box-arrow-right', 'Año de graduación', d.AluGrad_AnioGraduacion),
                item('archive', 'Archivado el', fecha(d.AluGrad_FechaArchivo))
              ])
            + seccion('Historial financiero', 'cash-coin', [
                item('cash-stack', 'Total pagado', '<strong>L. ' + Number(d.AluGrad_TotalPagado || 0).toLocaleString('es-HN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</strong>')
              ])
            + seccionEncargados(encargados)
            + '  </div>'
            + '</div>';
    }

    function seccion(titulo, icon, items) {
        return ''
            + '<div class="grad-ficha-section">'
            + '  <div class="grad-ficha-section-title"><i class="bi bi-' + icon + '"></i> ' + titulo + '</div>'
            + '  <div class="grad-ficha-grid">' + items.join('') + '</div>'
            + '</div>';
    }

    function item(icon, label, val) {
        var v = (val === null || val === undefined || val === '') ? '—' : val;
        return ''
            + '<div class="grad-ficha-item">'
            + '  <div class="grad-ficha-item-icon"><i class="bi bi-' + icon + '"></i></div>'
            + '  <div class="grad-ficha-item-body">'
            + '    <div class="grad-ficha-item-label">' + label + '</div>'
            + '    <div class="grad-ficha-item-value">' + v + '</div>'
            + '  </div>'
            + '</div>';
    }

    function seccionEncargados(list) {
        if (!list.length) return '';
        var cards = list.map(function (e) {
            var nombre = e.nombre || e.Nombre || '—';
            var parent = e.parentesco || e.Parentesco || '';
            var tel = e.telefono || e.Telefono || '';
            var ini = obtenerIniciales(nombre);
            var color = AVATAR_COLORS[Math.abs(hash(nombre)) % AVATAR_COLORS.length];
            return ''
                + '<div class="grad-ficha-enc">'
                + '  <div class="grad-ficha-enc-avatar" style="background:' + color + '">' + esc(ini) + '</div>'
                + '  <div class="grad-ficha-enc-body">'
                + '    <div class="grad-ficha-enc-nombre">' + esc(nombre) + '</div>'
                + (parent ? '    <div class="grad-ficha-enc-parent"><i class="bi bi-people"></i> ' + esc(parent) + '</div>' : '')
                + (tel ? '    <a class="grad-ficha-enc-tel" href="tel:' + esc(String(tel).replace(/\s+/g,'')) + '"><i class="bi bi-telephone"></i> ' + esc(tel) + '</a>' : '')
                + '  </div>'
                + '</div>';
        }).join('');
        return ''
            + '<div class="grad-ficha-section">'
            + '  <div class="grad-ficha-section-title"><i class="bi bi-people-fill"></i> Encargados</div>'
            + '  <div class="grad-ficha-encs">' + cards + '</div>'
            + '</div>';
    }

    function calcularEdad(f) {
        if (!f) return null;
        try {
            var d = new Date(f);
            if (isNaN(d.getTime())) return null;
            var hoy = new Date();
            var e = hoy.getFullYear() - d.getFullYear();
            var m = hoy.getMonth() - d.getMonth();
            if (m < 0 || (m === 0 && hoy.getDate() < d.getDate())) e--;
            return e >= 0 && e < 130 ? e : null;
        } catch (err) { return null; }
    }

    // ═══════════ Utilidades ═══════════
    function vacio(msg, icon) {
        return '<div class="grad-empty"><i class="bi bi-' + icon + '"></i><div>' + esc(msg) + '</div></div>';
    }
    function fecha(s) {
        if (!s) return '';
        try { return new Date(s).toLocaleDateString('es-HN'); } catch (e) { return s; }
    }
    function obtenerIniciales(nombre) {
        var p = (nombre || '').trim().split(/\s+/);
        if (!p.length || !p[0]) return '?';
        var a = p[0][0];
        var b = p.length > 2 ? p[2][0] : (p[1] ? p[1][0] : '');
        return ((a || '') + (b || '')).toUpperCase();
    }
    function hash(s) {
        var h = 0;
        for (var i = 0; i < s.length; i++) { h = ((h << 5) - h) + s.charCodeAt(i); h |= 0; }
        return h;
    }
    function esc(s) {
        return (s == null ? '' : String(s))
            .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    return { init: init };
})();
