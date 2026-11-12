/* Generación de constancia de graduación (PDF vía pdfmake).
 * Toma los datos del graduado ya cargados en el modal + el nombre/dirección
 * del instituto desde /InstitutoConfig/GetAsync (con cache local en sesión).
 */
var ConstanciaGraduado = (function () {
    var _instituto = null;

    function precargarInstituto() {
        if (_instituto) return;
        $.getJSON('/InstitutoConfig/GetAsync', function (resp) {
            _instituto = (resp && resp.data) || { Nombre: 'Instituto', Direccion: '' };
        }).fail(function () {
            _instituto = { Nombre: 'Instituto', Direccion: '' };
        });
    }

    document.addEventListener('gc:instituto-actualizado', function (e) {
        _instituto = e.detail || _instituto;
    });

    var MESES = ['enero','febrero','marzo','abril','mayo','junio',
                 'julio','agosto','septiembre','octubre','noviembre','diciembre'];

    function _fechaLarga(d) {
        return d.getDate() + ' días del mes de ' + MESES[d.getMonth()] + ' de ' + d.getFullYear();
    }

    function generar(graduado) {
        console.log('[Constancia] click, graduado=', graduado);
        if (!graduado) { alert('No hay graduado seleccionado.'); return; }
        if (typeof pdfMake === 'undefined') {
            alert('pdfmake NO cargado. Revise la consola / red del navegador.');
            console.error('[Constancia] pdfMake es undefined');
            return;
        }
        console.log('[Constancia] pdfMake OK, generando...');
        var inst = _instituto || { Nombre: 'Instituto', Direccion: '' };
        var hoy = new Date();
        var codigo = 'G-' + graduado.AluGrad_Id;
        (function () {

            var doc = {
                pageSize: 'LETTER',
                pageMargins: [70, 70, 70, 60],
                content: [
                    { text: (inst.Nombre || '').toUpperCase(), style: 'membrete' },
                    { text: inst.Direccion || '', style: 'subMembrete' },
                    { text: ' ', margin: [0, 10, 0, 10] },
                    { canvas: [{ type: 'line', x1: 0, y1: 0, x2: 470, y2: 0, lineWidth: 0.5, lineColor: '#888' }] },
                    { text: 'CONSTANCIA', style: 'titulo', margin: [0, 20, 0, 20] },
                    {
                        text: [
                            'Se hace constar que ',
                            { text: graduado.AluGrad_NombreCompleto || '—', bold: true },
                            ', portador(a) de la tarjeta de identidad número ',
                            { text: graduado.AluGrad_Identidad || '—', bold: true },
                            ', cursó y aprobó satisfactoriamente el nivel de ',
                            { text: graduado.AluGrad_NivelEducativo || '—', bold: true },
                            ' en esta institución, graduándose en el año ',
                            { text: String(graduado.AluGrad_AnioGraduacion || '—'), bold: true },
                            ' con el título de ',
                            { text: graduado.AluGrad_TituloObtenido || '—', bold: true },
                            '.'
                        ],
                        style: 'parrafo'
                    },
                    {
                        text: [
                            'Y para los fines que al interesado convenga, se extiende la presente constancia a los ',
                            { text: _fechaLarga(hoy), bold: true }, '.'
                        ],
                        style: 'parrafo',
                        margin: [0, 15, 0, 60]
                    },
                    {
                        columns: [
                            { width: '*', text: '' },
                            {
                                width: 220,
                                stack: [
                                    { canvas: [{ type: 'line', x1: 0, y1: 0, x2: 220, y2: 0, lineWidth: 0.5 }] },
                                    { text: 'Firma y sello', alignment: 'center', margin: [0, 6, 0, 0], fontSize: 10 }
                                ]
                            },
                            { width: '*', text: '' }
                        ]
                    },
                    { text: 'Código de verificación: ' + codigo, style: 'pieCodigo', margin: [0, 40, 0, 0] }
                ],
                styles: {
                    membrete:    { fontSize: 16, bold: true, alignment: 'center' },
                    subMembrete: { fontSize: 10, alignment: 'center', color: '#555' },
                    titulo:      { fontSize: 18, bold: true, alignment: 'center', decoration: 'underline' },
                    parrafo:     { fontSize: 12, alignment: 'justify', lineHeight: 1.5 },
                    pieCodigo:   { fontSize: 9, alignment: 'center', color: '#666' }
                },
                defaultStyle: { font: 'Roboto' }
            };

            var nombreArchivo = 'Constancia_' + (graduado.AluGrad_NombreCompleto || 'graduado').replace(/\s+/g, '_') + '.pdf';
            try {
                pdfMake.createPdf(doc).download(nombreArchivo);
                console.log('[Constancia] descarga disparada:', nombreArchivo);
            } catch (err) {
                console.error('[Constancia] createPdf falló:', err);
                alert('Error al generar el PDF: ' + err.message);
            }
        })();
    }

    return { generar: generar, precargarInstituto: precargarInstituto };
})();
