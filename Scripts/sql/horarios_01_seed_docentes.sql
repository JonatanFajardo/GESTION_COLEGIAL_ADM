/* ============================================================================
   horarios_01_seed_docentes.sql
   ----------------------------------------------------------------------------
   Inserta 50 docentes ficticios (personas + empleados) y puebla la matriz
   profesor-materia (tbEmpleados_tbMaterias) para que el generador de horarios
   tenga con quien armar las clases.

   Idempotente: vuelve a correrlo y no duplica filas (filtra por Per_Identidad
   y por (Emp_Id, Mat_Id)).

   Estructura:
     1. Tabla temporal #NuevosDocentes con los 50 candidatos + area academica.
     2. INSERT en tbPersonas para los que no existan.
     3. INSERT en tbEmpleados (Tit_Id segun area, Car_Id=3 Docencia,
        Dep_Id=3 Area Docente).
     4. Asignacion de materias a docentes NUEVOS y EXISTENTES.

   Requisitos: ejecutar contra DB_GestionColegial schema app.
   ============================================================================ */

USE DB_GestionColegial;
GO
SET NOCOUNT ON;
GO

DECLARE @UsuarioRegistra INT = 1; -- jona (admin)

/* ----------------------------------------------------------------------------
   1. Catalogo de los 50 nuevos docentes
   Columnas:
     N      = numero secuencial
     Area   = MAT|LEN|CIE|ING|CON|SOC|TEC|ART (para Tit_Id y materias)
     Tit_Id = titulo profesional
   ---------------------------------------------------------------------------- */

IF OBJECT_ID('tempdb..#NuevosDocentes') IS NOT NULL DROP TABLE #NuevosDocentes;

CREATE TABLE #NuevosDocentes (
    N               INT NOT NULL PRIMARY KEY,
    Area            CHAR(3)                       COLLATE DATABASE_DEFAULT NOT NULL,
    Tit_Id          INT NOT NULL,
    Identidad       VARCHAR(13)                   COLLATE DATABASE_DEFAULT NOT NULL,
    PrimerNombre    NVARCHAR(50)                  COLLATE DATABASE_DEFAULT NOT NULL,
    SegundoNombre   NVARCHAR(50)                  COLLATE DATABASE_DEFAULT NULL,
    ApellidoPaterno NVARCHAR(50)                  COLLATE DATABASE_DEFAULT NOT NULL,
    ApellidoMaterno NVARCHAR(50)                  COLLATE DATABASE_DEFAULT NULL,
    FechaNacimiento DATE NOT NULL,
    Telefono        VARCHAR(9)                    COLLATE DATABASE_DEFAULT NOT NULL,
    Email           VARCHAR(150)                  COLLATE DATABASE_DEFAULT NOT NULL,
    Direccion       VARCHAR(150)                  COLLATE DATABASE_DEFAULT NOT NULL,
    Sexo            CHAR(1)                       COLLATE DATABASE_DEFAULT NOT NULL
);

INSERT INTO #NuevosDocentes
    (N, Area, Tit_Id, Identidad, PrimerNombre, SegundoNombre, ApellidoPaterno, ApellidoMaterno, FechaNacimiento, Telefono, Email, Direccion, Sexo)
VALUES
    -- 1..10  MAT (Tit 2 Mat / Tit 4 Fisica)
    ( 1, 'MAT', 2, '0801197512453', 'Carlos',    'Eduardo',     'García',     'Mejía',     '1975-03-12', '98214567', 'garcia.mejia.ce@colegio.hn',     'Col. Kennedy, Tegucigalpa', 'M'),
    ( 2, 'MAT', 2, '0501198208721', 'María',     'Fernanda',    'López',      'Hernández', '1982-07-25', '97451238', 'lopez.hernandez.mf@colegio.hn', 'Barrio Guamilito, San Pedro Sula', 'F'),
    ( 3, 'MAT', 4, '0801196834521', 'José',      'Antonio',     'Martínez',   'Discua',    '1968-11-04', '89236754', 'martinez.discua.ja@colegio.hn', 'Col. Miraflores, Tegucigalpa', 'M'),
    ( 4, 'MAT', 2, '0301199011234', 'Ana',       'Lucía',       'Rodríguez',  'Cálix',     '1990-05-18', '96123489', 'rodriguez.calix.al@colegio.hn', 'Res. Villa Alicia, Comayagua', 'F'),
    ( 5, 'MAT', 4, '0801197922189', 'Luis',      'Alberto',     'Hernández',  'Bonilla',   '1979-09-30', '98345612', 'hernandez.bonilla.la@colegio.hn', 'Col. Las Hadas, Tegucigalpa', 'M'),
    ( 6, 'MAT', 2, '0501198545678', 'Sandra',    'Patricia',    'Sánchez',    'Zelaya',    '1985-02-14', '87129456', 'sanchez.zelaya.sp@colegio.hn',  'Col. Trejo, San Pedro Sula', 'F'),
    ( 7, 'MAT', 2, '0801197267234', 'Roberto',   'Carlos',      'Pérez',      'Maradiaga', '1972-08-22', '94567821', 'perez.maradiaga.rc@colegio.hn', 'Col. Lomas del Guijarro, Tegucigalpa', 'M'),
    ( 8, 'MAT', 4, '0801198889345', 'Karla',     'Mariela',     'Gómez',      'Cardona',   '1988-12-07', '93216547', 'gomez.cardona.km@colegio.hn',   'Col. Las Colinas, Tegucigalpa', 'F'),
    ( 9, 'MAT', 2, '0501198013456', 'Marvin',    'Josué',       'Flores',     'Reyes',     '1980-04-19', '86543219', 'flores.reyes.mj@colegio.hn',    'Barrio Río de Piedras, San Pedro Sula', 'M'),
    (10, 'MAT', 4, '0301198324567', 'Glenda',    'Yamileth',    'Cruz',       'López',     '1983-10-11', '98762341', 'cruz.lopez.gy@colegio.hn',      'Res. Los Almendros, Comayagua', 'F'),

    -- 11..20 LEN (Tit 6 Lengua y Lit.)
    (11, 'LEN', 6, '0801197035678', 'Óscar',     'Daniel',      'Mejía',      'García',    '1970-06-28', '95438762', 'mejia.garcia.od@colegio.hn',    'Col. Florencia Norte, Tegucigalpa', 'M'),
    (12, 'LEN', 6, '0501198646789', 'Iris',      'Carolina',    'Reyes',      'Hernández', '1986-01-15', '84327651', 'reyes.hernandez.ic@colegio.hn', 'Col. Bella Vista, San Pedro Sula', 'F'),
    (13, 'LEN', 6, '0801197757891', 'Wilmer',    'Alexis',      'Maradiaga',  'Flores',    '1977-11-23', '91234567', 'maradiaga.flores.wa@colegio.hn','Col. El Pedregal, Tegucigalpa', 'M'),
    (14, 'LEN', 6, '0301198168912', 'Rosa',      'Elena',       'Discua',     'Pérez',     '1981-03-08', '97651432', 'discua.perez.re@colegio.hn',    'Barrio El Centro, Comayagua', 'F'),
    (15, 'LEN', 6, '0801198479123', 'Edwin',     'Mauricio',    'Cálix',      'Sánchez',   '1984-07-29', '88765432', 'calix.sanchez.em@colegio.hn',   'Col. San Ángel, Tegucigalpa', 'M'),
    (16, 'LEN', 6, '0501197881234', 'Dilcia',    'Marisol',     'Bonilla',    'Rodríguez', '1978-09-17', '92348765', 'bonilla.rodriguez.dm@colegio.hn','Col. Universidad, San Pedro Sula', 'F'),
    (17, 'LEN', 6, '0801196992345', 'Jorge',     'Luis',        'Zelaya',     'Martínez',  '1969-12-05', '96783412', 'zelaya.martinez.jl@colegio.hn', 'Col. Palmira, Tegucigalpa', 'M'),
    (18, 'LEN', 6, '0301198710345', 'Sonia',     'Lorena',      'Cardona',    'Cruz',      '1987-04-22', '83456789', 'cardona.cruz.sl@colegio.hn',    'Res. Bosques de Suyapa, Comayagua', 'F'),
    (19, 'LEN', 6, '0801197321456', 'Norman',    'Rigoberto',   'García',     'López',     '1973-08-13', '94321876', 'garcia.lopez.nr@colegio.hn',    'Col. Cerro Grande, Tegucigalpa', 'M'),
    (20, 'LEN', 6, '0501198932567', 'Yolanda',   'Esperanza',   'Hernández',  'Mejía',     '1989-02-26', '98765431', 'hernandez.mejia.ye@colegio.hn', 'Barrio Suyapa, San Pedro Sula', 'F'),

    -- 21..28 CIE (Tit 3 Biologia / Tit 5 Quimica)
    (21, 'CIE', 3, '0801197643678', 'Héctor',    'Manuel',      'Martínez',   'Reyes',     '1976-10-08', '85672341', 'martinez.reyes.hm@colegio.hn',  'Col. Tepeyac, Tegucigalpa', 'M'),
    (22, 'CIE', 5, '0301198554789', 'Claudia',   'Beatriz',     'Rodríguez',  'Discua',    '1985-06-19', '95432187', 'rodriguez.discua.cb@colegio.hn','Col. San Miguel, Comayagua', 'F'),
    (23, 'CIE', 3, '0801197165891', 'Fredy',     'Orlando',     'López',      'Maradiaga', '1971-01-31', '92345678', 'lopez.maradiaga.fo@colegio.hn', 'Col. El Hogar, Tegucigalpa', 'M'),
    (24, 'CIE', 5, '0501198376912', 'Marlene',   'Suyapa',      'Sánchez',    'Cálix',     '1983-09-14', '87654321', 'sanchez.calix.ms@colegio.hn',   'Col. Jardines del Valle, San Pedro Sula', 'F'),
    (25, 'CIE', 3, '0801196787123', 'Ramón',     'Enrique',     'Pérez',      'Bonilla',   '1967-05-27', '94561234', 'perez.bonilla.re@colegio.hn',   'Col. Loarque, Tegucigalpa', 'M'),
    (26, 'CIE', 5, '0301198098234', 'Vilma',     'Argentina',   'Gómez',      'Zelaya',    '1980-11-10', '96782345', 'gomez.zelaya.va@colegio.hn',    'Res. Villa Real, Comayagua', 'F'),
    (27, 'CIE', 3, '0801198211456', 'Selvin',    'Adonai',      'Flores',     'Cardona',   '1982-03-02', '81234567', 'flores.cardona.sa@colegio.hn',  'Col. Altos de Toncontín, Tegucigalpa', 'M'),
    (28, 'CIE', 5, '0501197422567', 'Daysi',     'Concepción',  'Cruz',       'García',    '1974-07-21', '93456712', 'cruz.garcia.dc@colegio.hn',     'Barrio Cabañas, San Pedro Sula', 'F'),

    -- 29..35 ING (Tit 9 Lenguas Extranjeras)
    (29, 'ING', 9, '0801198833678', 'Walter',    'Geovanny',    'Mejía',      'Hernández', '1988-12-15', '97893214', 'mejia.hernandez.wg@colegio.hn', 'Col. La Granja, Tegucigalpa', 'M'),
    (30, 'ING', 9, '0301197944789', 'Reina',     'Isabel',      'Reyes',      'Martínez',  '1979-04-06', '85671234', 'reyes.martinez.ri@colegio.hn',  'Col. Centroamérica, Comayagua', 'F'),
    (31, 'ING', 9, '0801196555891', 'Mario',     'Alfonso',     'Maradiaga',  'Rodríguez', '1965-09-29', '91238765', 'maradiaga.rodriguez.ma@colegio.hn','Col. La Sosa, Tegucigalpa', 'M'),
    (32, 'ING', 9, '0501199166912', 'Lesbia',    'Mercedes',    'Discua',     'López',     '1991-01-12', '98761234', 'discua.lopez.lm@colegio.hn',    'Col. Las Mesetas, San Pedro Sula', 'F'),
    (33, 'ING', 9, '0801198677123', 'Iván',      'Alejandro',   'Cálix',      'Sánchez',   '1986-08-25', '84321987', 'calix.sanchez.ia@colegio.hn',   'Col. El Sauce, Tegucigalpa', 'M'),
    (34, 'ING', 9, '0301197688234', 'Migdalia',  'Esther',      'Bonilla',    'Pérez',     '1976-02-18', '92347651', 'bonilla.perez.me@colegio.hn',   'Res. Las Glorias, Comayagua', 'F'),
    (35, 'ING', 9, '0801198199345', 'Denis',     'Mauricio',    'Zelaya',     'Gómez',     '1981-06-04', '95672341', 'zelaya.gomez.dm@colegio.hn',    'Col. El Prado, Tegucigalpa', 'M'),

    -- 36..40 CON (Tit 10 Ninguno - admin/contab)
    (36, 'CON', 10,'0501198412456', 'Aracely',   'Beatriz',     'Cardona',    'Flores',    '1984-10-27', '86542187', 'cardona.flores.ab@colegio.hn',  'Col. Lempira, San Pedro Sula', 'F'),
    (37, 'CON', 10,'0801199013567', 'Cristian',  'Josué',       'García',     'Cruz',      '1990-03-15', '98764321', 'garcia.cruz.cj@colegio.hn',     'Col. Buenos Aires, Tegucigalpa', 'M'),
    (38, 'CON', 10,'0301197214678', 'Lourdes',   'Alicia',      'Hernández',  'Mejía',     '1972-11-08', '93451876', 'hernandez.mejia.la@colegio.hn', 'Barrio La Libertad, Comayagua', 'F'),
    (39, 'CON', 10,'0801197815789', 'Ronald',    'Estuardo',    'Martínez',   'Reyes',     '1978-05-21', '87653412', 'martinez.reyes.re@colegio.hn',  'Col. Modelo, Tegucigalpa', 'M'),
    (40, 'CON', 10,'0501198916891', 'Jessica',   'Vanessa',     'Rodríguez',  'Maradiaga', '1989-09-03', '91237654', 'rodriguez.maradiaga.jv@colegio.hn','Col. Sunseri, San Pedro Sula', 'F'),

    -- 41..44 SOC (Tit 7 Geografia / Tit 8 Filosofia)
    (41, 'SOC', 8, '0801198517912', 'Allan',     'Rolando',     'López',      'Discua',    '1985-01-26', '94568721', 'lopez.discua.ar@colegio.hn',    'Col. Las Lomas, Tegucigalpa', 'M'),
    (42, 'SOC', 7, '0301197718234', 'Norma',     'Suyapa',      'Sánchez',    'Cálix',     '1977-07-09', '82345671', 'sanchez.calix.ns@colegio.hn',   'Col. San José, Comayagua', 'F'),
    (43, 'SOC', 8, '0801196619345', 'Edgardo',   'Antonio',     'Pérez',      'Bonilla',   '1966-12-22', '96784231', 'perez.bonilla.ea@colegio.hn',   'Col. Villa Olímpica, Tegucigalpa', 'M'),
    (44, 'SOC', 7, '0501199220456', 'Mirna',     'Leticia',     'Gómez',      'Zelaya',    '1992-04-14', '95436712', 'gomez.zelaya.ml@colegio.hn',    'Col. Satélite, San Pedro Sula', 'F'),

    -- 45..48 TEC (Tit 1 Ing. Sistemas)
    (45, 'TEC', 1, '0801198021567', 'Bayron',    'Estuardo',    'Flores',     'Cardona',   '1980-08-30', '88763214', 'flores.cardona.be@colegio.hn',  'Col. 21 de Octubre, Tegucigalpa', 'M'),
    (46, 'TEC', 1, '0301198722678', 'Wendy',     'Carolina',    'Cruz',       'García',    '1987-02-11', '92348761', 'cruz.garcia.wc@colegio.hn',     'Res. El Carmen, Comayagua', 'F'),
    (47, 'TEC', 1, '0801197323789', 'Nelson',    'Geovanny',    'Mejía',      'Hernández', '1973-10-24', '97895432', 'mejia.hernandez.ng@colegio.hn', 'Col. La Joya, Tegucigalpa', 'M'),
    (48, 'TEC', 1, '0501198324891', 'Mayra',     'Elizabeth',   'Reyes',      'Martínez',  '1983-06-07', '83457621', 'reyes.martinez.me@colegio.hn',  'Col. Trejo, San Pedro Sula', 'F'),

    -- 49..50 ART (Tit 10 Ninguno - educacion fisica)
    (49, 'ART', 10,'0801197925912', 'Franklin',  'Omar',        'Maradiaga',  'Rodríguez', '1979-12-19', '91235678', 'maradiaga.rodriguez.fo@colegio.hn','Col. El Reparto, Tegucigalpa', 'M'),
    (50, 'ART', 10,'0301198626134', 'Suyapa',    'Margarita',   'Discua',     'López',     '1986-03-31', '98763215', 'discua.lopez.sm@colegio.hn',    'Col. Villas del Real, Comayagua', 'F'),

    -- 51..70 CON (refuerzo cuello de botella BTP)  Tit 10 Ninguno
    (51, 'CON', 10, '0801197812458', 'Carlos',    'Eduardo',     'Mendoza',     'Cruz',         '1978-03-15', '98521470', 'mendoza.ce@colegio.hn',       'Col. Kennedy, Tegucigalpa', 'M'),
    (52, 'CON', 10, '0501198312726', 'María',     'Fernanda',    'López',       'Rivera',       '1983-07-22', '97834521', 'lopez.mf@colegio.hn',         'Res. Las Hadas, San Pedro Sula', 'F'),
    (53, 'CON', 10, '0801197034527', 'José',      'Antonio',     'Martínez',    'Pineda',       '1970-11-08', '99124578', 'martinez.ja@colegio.hn',      'Col. Miraflores, Tegucigalpa', 'M'),
    (54, 'CON', 10, '0301198511240', 'Ana',       'Lucía',       'Reyes',       'Hernández',    '1985-05-30', '96587412', 'reyes.al@colegio.hn',         'Barrio El Centro, Comayagua', 'F'),
    (55, 'CON', 10, '0801198122195', 'Roberto',   'Carlos',      'Galeano',     'Sosa',         '1981-09-14', '98745632', 'galeano.rc@colegio.hn',       'Col. Las Colinas, Tegucigalpa', 'M'),
    (56, 'CON', 10, '0501197745684', 'Sandra',    'Patricia',    'Cáceres',     'Velásquez',    '1977-02-25', '97456389', 'caceres.sp@colegio.hn',       'Res. Trejo, San Pedro Sula', 'F'),
    (57, 'CON', 10, '0801197567240', 'Luis',      'Alberto',     'Rodríguez',   'Maradiaga',    '1975-12-03', '99563214', 'rodriguez.la@colegio.hn',     'Col. Florencia, Tegucigalpa', 'M'),
    (58, 'CON', 10, '0801198289351', 'Karla',     'Vanessa',     'Ortiz',       'Bonilla',      '1982-04-18', '96321478', 'ortiz.kv@colegio.hn',         'Col. La Sosa, Tegucigalpa', 'F'),
    (59, 'CON', 10, '0501197313462', 'Marvin',    'Josué',       'Aguilar',     'Ramírez',      '1973-08-09', '98123654', 'aguilar.mj@colegio.hn',       'Res. Jardines del Valle, San Pedro Sula', 'M'),
    (60, 'CON', 10, '0301198623573', 'Iris',      'Gabriela',    'Suazo',       'Andino',       '1986-10-27', '97852146', 'suazo.ig@colegio.hn',         'Barrio Suyapa, Comayagua', 'F'),
    (61, 'CON', 10, '0801197935684', 'Edwin',     'Daniel',      'Padilla',     'Castellanos',  '1979-06-11', '99784563', 'padilla.ed@colegio.hn',       'Col. El Pedregal, Tegucigalpa', 'M'),
    (62, 'CON', 10, '0501198947895', 'Ligia',     'Esperanza',   'Discua',      'Murillo',      '1989-01-20', '96147852', 'discua.le@colegio.hn',        'Res. Country, San Pedro Sula', 'F'),
    (63, 'CON', 10, '0801197858907', 'Fernando',  'Rafael',      'Zelaya',      'Banegas',      '1978-11-04', '98963214', 'zelaya.fr@colegio.hn',        'Col. Loarque, Tegucigalpa', 'M'),
    (64, 'CON', 10, '0301198470128', 'Glenda',    'Marisol',     'Posadas',     'Torres',       '1984-07-16', '97369852', 'posadas.gm@colegio.hn',       'Barrio La Libertad, Comayagua', 'F'),
    (65, 'CON', 10, '0801197781239', 'Óscar',     'René',        'Ávila',       'Salgado',      '1977-09-25', '99258741', 'avila.or@colegio.hn',         'Col. Cerro Grande, Tegucigalpa', 'M'),
    (66, 'CON', 10, '0501198293450', 'Mirna',     'Yolanda',     'Fúnez',       'Erazo',        '1982-12-08', '96741258', 'funez.my@colegio.hn',         'Res. Bellavista, San Pedro Sula', 'F'),
    (67, 'CON', 10, '0801197605671', 'Héctor',    'Manuel',      'Romero',      'Pavón',        '1976-03-22', '98456321', 'romero.hm@colegio.hn',        'Col. Los Robles, Tegucigalpa', 'M'),
    (68, 'CON', 10, '0301198718792', 'Wendy',     'Carolina',    'Laínez',      'Mejía',        '1987-05-13', '97123456', 'lainez.wc@colegio.hn',        'Barrio Cabañas, Comayagua', 'F'),
    (69, 'CON', 10, '0801198530913', 'Rolando',   'David',       'Espinoza',    'Flores',       '1985-08-29', '99852741', 'espinoza.rd@colegio.hn',      'Col. Tepeyac, Tegucigalpa', 'M'),
    (70, 'CON', 10, '0501197642034', 'Brenda',    'Lizeth',      'Cardona',     'Ulloa',        '1976-02-17', '96852147', 'cardona.bl@colegio.hn',       'Res. Universidad, San Pedro Sula', 'F'),

    -- 71..80 LEN (refuerzo Lengua y Literatura)  Tit 6
    (71, 'LEN', 6,  '0801197954155', 'Patricia',  'Eugenia',     'Valladares',  'Pineda',       '1979-04-26', '98214563', 'valladares.pe@colegio.hn',    'Col. Palmira, Tegucigalpa', 'F'),
    (72, 'LEN', 6,  '0501198466276', 'Juan',      'Carlos',      'Fonseca',     'Uclés',        '1984-06-10', '97456128', 'fonseca.jc@colegio.hn',       'Res. Sevilla, San Pedro Sula', 'M'),
    (73, 'LEN', 6,  '0801197378397', 'Rosa',      'María',       'Carranza',    'Gómez',        '1973-10-05', '99654128', 'carranza.rm@colegio.hn',      'Col. Las Brisas, Tegucigalpa', 'F'),
    (74, 'LEN', 6,  '0301198690518', 'Mario',     'Alejandro',   'Quintanilla', 'Henríquez',    '1986-12-19', '96541237', 'quintanilla.ma@colegio.hn',   'Barrio El Sol, Comayagua', 'M'),
    (75, 'LEN', 6,  '0801197802639', 'Yessenia',  'Marlene',     'Amador',      'Cálix',        '1978-08-07', '98741259', 'amador.ym@colegio.hn',        'Col. Centroamérica, Tegucigalpa', 'F'),
    (76, 'LEN', 6,  '0501198314750', 'Walter',    'Geovanny',    'Turcios',     'Munguía',      '1983-01-23', '97852369', 'turcios.wg@colegio.hn',       'Res. Lomas, San Pedro Sula', 'M'),
    (77, 'LEN', 6,  '0801197626871', 'Claudia',   'Iveth',       'Bardales',    'Castro',       '1976-09-12', '99362514', 'bardales.ci@colegio.hn',      'Col. La Granja, Tegucigalpa', 'F'),
    (78, 'LEN', 6,  '0301198538992', 'Erick',     'Mauricio',    'Núñez',       'Vásquez',      '1985-03-28', '96258417', 'nunez.em@colegio.hn',         'Barrio Torondón, Comayagua', 'M'),
    (79, 'LEN', 6,  '0801197751014', 'Doris',     'Aracely',     'Inestroza',   'Madrid',       '1977-11-15', '98147523', 'inestroza.da@colegio.hn',     'Col. 21 de Octubre, Tegucigalpa', 'F'),
    (80, 'LEN', 6,  '0501198463135', 'Allan',     'Roberto',     'Sevilla',     'Portillo',     '1984-04-02', '97963258', 'sevilla.ar@colegio.hn',       'Res. Florencia, San Pedro Sula', 'M'),

    -- 81..85 MAT (refuerzo)  Tit 2 Mat / Tit 4 Fisica
    (81, 'MAT', 2,  '0801197975256', 'Jorge',     'Luis',        'Castellanos', 'Aceituno',     '1979-07-21', '99741258', 'castellanos.jl@colegio.hn',   'Col. Hato de Enmedio, Tegucigalpa', 'M'),
    (82, 'MAT', 2,  '0501198287377', 'Suyapa',    'Beatriz',     'Recinos',     'Aguilera',     '1982-05-09', '96147369', 'recinos.sb@colegio.hn',       'Res. Honduras, San Pedro Sula', 'F'),
    (83, 'MAT', 4,  '0801197099498', 'Wilmer',    'Ariel',       'Borjas',      'Cabrera',      '1970-02-14', '98369741', 'borjas.wa@colegio.hn',        'Col. La Vega, Tegucigalpa', 'M'),
    (84, 'MAT', 2,  '0301198601619', 'Norma',     'Ondina',      'Maldonado',   'Escobar',      '1986-08-26', '97258963', 'maldonado.no@colegio.hn',     'Barrio San Sebastián, Comayagua', 'F'),
    (85, 'MAT', 4,  '0801198013730', 'Henry',     'Alexander',   'Perdomo',     'López',        '1980-10-03', '99852369', 'perdomo.ha@colegio.hn',       'Col. El Hogar, Tegucigalpa', 'M'),

    -- 86..90 CIE (refuerzo)  Tit 3 Biologia / Tit 5 Quimica
    (86, 'CIE', 3,  '0501197725851', 'Lourdes',   'Marina',      'Velásquez',   'Rodríguez',    '1977-06-18', '96741852', 'velasquez.lm@colegio.hn',     'Res. El Pedregal, San Pedro Sula', 'F'),
    (87, 'CIE', 5,  '0801198437972', 'César',     'Augusto',     'Pineda',      'Banegas',      '1984-09-30', '98963147', 'pineda.ca@colegio.hn',        'Col. Las Lomas, Tegucigalpa', 'M'),
    (88, 'CIE', 3,  '0301198350193', 'Heidy',     'Johana',      'Alemán',      'Cortés',       '1983-12-07', '97369258', 'aleman.hj@colegio.hn',        'Barrio La Estación, Comayagua', 'F'),
    (89, 'CIE', 5,  '0801197262314', 'Nelson',    'Geovany',     'Ordóñez',     'Tábora',       '1972-01-24', '99147258', 'ordonez.ng@colegio.hn',       'Col. Villa Olímpica, Tegucigalpa', 'M'),
    (90, 'CIE', 3,  '0501198874435', 'Yolanda',   'Esperanza',   'Andrade',     'Mejía',        '1988-04-11', '96852741', 'andrade.ye@colegio.hn',       'Res. Plaza, San Pedro Sula', 'F'),

    -- 91..95 ING (refuerzo)  Tit 9 Lenguas Extranjeras
    (91, 'ING', 9,  '0801197586556', 'Andrea',    'Michelle',    'Salinas',     'Romero',       '1975-11-29', '98147369', 'salinas.am@colegio.hn',       'Col. Loma Linda, Tegucigalpa', 'F'),
    (92, 'ING', 9,  '0301198198677', 'Gerardo',   'Vinicio',     'Matute',      'Sandoval',     '1981-02-06', '97852963', 'matute.gv@colegio.hn',        'Barrio Abajo, Comayagua', 'M'),
    (93, 'ING', 9,  '0801198410798', 'Lesly',     'Daniela',     'Chinchilla',  'García',       '1984-10-14', '99258147', 'chinchilla.ld@colegio.hn',    'Col. Toncontín, Tegucigalpa', 'F'),
    (94, 'ING', 9,  '0501197923819', 'Rubén',     'Darío',       'Membreño',    'Aguilar',      '1979-05-23', '96369852', 'membreno.rd@colegio.hn',      'Res. Las Mercedes, San Pedro Sula', 'M'),
    (95, 'ING', 9,  '0801197735930', 'Mayra',     'Alejandra',   'Cantarero',   'Zúniga',       '1977-08-01', '98741963', 'cantarero.ma@colegio.hn',     'Col. Buenos Aires, Tegucigalpa', 'F'),

    -- 96..98 SOC (refuerzo)  Tit 7 Geografia / Tit 8 Filosofia
    (96, 'SOC', 8,  '0301198247051', 'Francisco', 'Javier',      'Murillo',     'Espinal',      '1982-06-15', '97456852', 'murillo.fj@colegio.hn',       'Barrio San Juan, Comayagua', 'M'),
    (97, 'SOC', 7,  '0801197859172', 'Reina',     'Isabel',      'Hernández',   'Bueso',        '1978-12-22', '99852147', 'hernandez.ri@colegio.hn',     'Col. El Prado, Tegucigalpa', 'F'),
    (98, 'SOC', 8,  '0501198571293', 'Marco',     'Tulio',       'Erazo',       'Lagos',        '1985-07-08', '96147852', 'erazo.mt@colegio.hn',         'Res. Suyapa, San Pedro Sula', 'M'),

    -- 99..100 ART (refuerzo Ed. Fisica)  Tit 10
    (99, 'ART', 10, '0801197683414', 'Diana',     'Carolina',    'Paz',         'Rivera',       '1976-04-17', '98369852', 'paz.dc@colegio.hn',           'Col. La Reforma, Tegucigalpa', 'F'),
    (100,'ART', 10, '0301198495535', 'Bayron',    'Estuardo',    'Colindres',   'Fúnez',        '1984-01-30', '97258369', 'colindres.be@colegio.hn',     'Barrio El Carmen, Comayagua', 'M');

/* ----------------------------------------------------------------------------
   2. INSERT en tbPersonas (solo identidades nuevas)
   ---------------------------------------------------------------------------- */

INSERT INTO app.tbPersonas
    (Per_Imagen, Per_Identidad, Per_PrimerNombre, Per_SegundoNombre,
     Per_ApellidoPaterno, Per_ApellidoMaterno, Per_FechaNacimiento,
     Per_CorreoElectronico, Per_Telefono, Per_Direccion, Per_Sexo,
     Per_UsuarioRegistra, Per_FechaRegistra)
SELECT
    '/Content/img/profile/profile-13.jpeg',
    nd.Identidad, nd.PrimerNombre, nd.SegundoNombre,
    nd.ApellidoPaterno, nd.ApellidoMaterno, nd.FechaNacimiento,
    nd.Email, nd.Telefono, nd.Direccion, nd.Sexo,
    @UsuarioRegistra, GETDATE()
FROM #NuevosDocentes nd
WHERE NOT EXISTS (
    SELECT 1 FROM app.tbPersonas p WHERE p.Per_Identidad = nd.Identidad
);

PRINT CONCAT('Personas insertadas: ', @@ROWCOUNT);

/* ----------------------------------------------------------------------------
   3. INSERT en tbEmpleados (para las personas que aun no son empleados)
   Emp_Codigo correlativo a partir del max actual (formato 'DOCNNN').
   ---------------------------------------------------------------------------- */

DECLARE @SiguienteCodigo INT;
SELECT @SiguienteCodigo = ISNULL(MAX(TRY_CAST(SUBSTRING(Emp_Codigo, 4, 10) AS INT)), 0) + 1
FROM app.tbEmpleados
WHERE Emp_Codigo LIKE 'DOC%';

;WITH NuevosEmpleados AS (
    SELECT
        p.Per_Id,
        nd.Tit_Id,
        ROW_NUMBER() OVER (ORDER BY p.Per_Id) AS Orden
    FROM #NuevosDocentes nd
    JOIN app.tbPersonas p ON p.Per_Identidad = nd.Identidad
    WHERE NOT EXISTS (
        SELECT 1 FROM app.tbEmpleados e WHERE e.Per_Id = p.Per_Id
    )
)
INSERT INTO app.tbEmpleados (Emp_Codigo, Per_Id, Tit_Id, Car_Id, Dep_Id)
SELECT
    CONCAT('DOC', RIGHT('000' + CAST(@SiguienteCodigo + Orden - 1 AS VARCHAR(10)), 3)),
    Per_Id,
    Tit_Id,
    3,  -- Car_Id Docencia
    3   -- Dep_Id Area Docente
FROM NuevosEmpleados;

PRINT CONCAT('Empleados (docentes) insertados: ', @@ROWCOUNT);

/* ----------------------------------------------------------------------------
   4. Asignacion profesor-materia
   Construyo una tabla #AsignacionPlanificada (Emp_Id, Mat_Id) y luego inserto
   solo las combinaciones que no existan en tbEmpleados_tbMaterias.
   ---------------------------------------------------------------------------- */

IF OBJECT_ID('tempdb..#AsignacionPlanificada') IS NOT NULL DROP TABLE #AsignacionPlanificada;
CREATE TABLE #AsignacionPlanificada (Emp_Id INT, Mat_Id INT);

/* 4.a Docentes existentes (los 12 con Car_Id=3 ya en BD).
   Repartidos por area para que el generador tenga cobertura total.        */
INSERT INTO #AsignacionPlanificada (Emp_Id, Mat_Id) VALUES
    -- Emp 4  -> SOC
    (4, 1), (4, 5), (4, 7), (4, 11),
    -- Emp 8  -> SOC
    (8, 12), (8, 13), (8, 20), (8, 23),
    -- Emp 12 -> MAT
    (12, 2), (12, 15), (12, 16),
    -- Emp 16 -> MAT
    (16, 15), (16, 17), (16, 19),
    -- Emp 20 -> CIE
    (20, 4), (20, 6), (20, 18),
    -- Emp 24 -> CIE
    (24, 6), (24, 10), (24, 18),
    -- Emp 28 -> LEN
    (28, 3), (28, 22), (28, 24),
    -- Emp 32 -> LEN
    (32, 22), (32, 24), (32, 34),
    -- Emp 36 -> ING
    (36, 8), (36, 26), (36, 36),
    -- Emp 40 -> ING
    (40, 8), (40, 36), (40, 47),
    -- Emp 44 -> TEC
    (44, 9), (44, 25), (44, 27),
    -- Emp 48 -> CON
    (48, 14), (48, 32), (48, 33);

/* 4.b Docentes nuevos: distribucion circular con cobertura COMPLETA.
   Cada docente toma K materias contiguas de su area, desplazadas por
   un step que distribuye uniformemente entre los docentes. Garantiza
   que TODAS las materias del area sean cubiertas por al menos 1 docente
   (en areas con muchas materias y pocos docentes, ej CON).             */

;WITH MateriasPorArea AS (
    SELECT * FROM (VALUES
        ('MAT', 2),('MAT',15),('MAT',16),('MAT',17),('MAT',19),
        ('LEN', 3),('LEN',22),('LEN',24),('LEN',34),
        ('CIE', 4),('CIE', 6),('CIE',10),('CIE',18),
        ('ING', 8),('ING',26),('ING',36),('ING',47),
        ('CON',14),('CON',32),('CON',33),('CON',35),('CON',37),('CON',38),
        ('CON',39),('CON',41),('CON',42),('CON',43),('CON',44),('CON',45),('CON',46),
        ('SOC', 1),('SOC', 5),('SOC', 7),('SOC',11),('SOC',12),('SOC',13),
        ('SOC',20),('SOC',23),('SOC',30),('SOC',31),
        ('TEC', 9),('TEC',25),('TEC',27),('TEC',28),('TEC',29),
        ('ART',21)
    ) AS m(Area, Mat_Id)
),
/* Parametros por area: K = materias por docente, Stp = paso entre docentes  */
ParamsArea AS (
    SELECT * FROM (VALUES
        ('MAT', 5, 1),  -- 5 mat / 10 doc: todos toman las 5
        ('LEN', 4, 1),  -- 4 mat / 10 doc: todos toman las 4
        ('CIE', 4, 1),  -- 4 mat / 8 doc
        ('ING', 4, 1),  -- 4 mat / 7 doc
        ('CON', 6, 2),  -- 13 mat / 5 doc: cada uno toma 6 con paso 2
        ('SOC', 5, 2),  -- 10 mat / 4 doc: cada uno toma 5 con paso 2
        ('TEC', 5, 1),  -- 5 mat / 4 doc: todos toman las 5
        ('ART', 1, 1)   -- 1 mat / 2 doc
    ) AS p(Area, K, Stp)
),
RankMateriasArea AS (
    SELECT Area, Mat_Id,
           ROW_NUMBER() OVER (PARTITION BY Area ORDER BY Mat_Id) - 1 AS rn,  -- 0-based
           COUNT(*)     OVER (PARTITION BY Area)                AS total
    FROM MateriasPorArea
),
DocenteEmp AS (
    SELECT nd.N, nd.Area, e.Emp_Id,
           ROW_NUMBER() OVER (PARTITION BY nd.Area ORDER BY nd.N) - 1 AS rnDoc  -- 0-based
    FROM #NuevosDocentes nd
    JOIN app.tbPersonas p   ON p.Per_Identidad = nd.Identidad
    JOIN app.tbEmpleados e  ON e.Per_Id = p.Per_Id
),
OffJ AS (
    SELECT v.j FROM (VALUES (0),(1),(2),(3),(4),(5)) AS v(j)
)
INSERT INTO #AsignacionPlanificada (Emp_Id, Mat_Id)
SELECT DISTINCT d.Emp_Id, m.Mat_Id
FROM DocenteEmp d
JOIN ParamsArea pa         ON pa.Area = d.Area
JOIN RankMateriasArea m    ON m.Area  = d.Area
JOIN OffJ o             ON o.j < pa.K
WHERE ((d.rnDoc * pa.Stp + o.j) % m.total) = m.rn;

/* 4.c Materia 40 (OTR - Cultura de calidad) estaba huerfana. Se la asigno
   a todos los docentes SOC (la materia es transversal y de areas afines). */
INSERT INTO #AsignacionPlanificada (Emp_Id, Mat_Id)
SELECT e.Emp_Id, 40
FROM #NuevosDocentes nd
JOIN app.tbPersonas p  ON p.Per_Identidad = nd.Identidad
JOIN app.tbEmpleados e ON e.Per_Id = p.Per_Id
WHERE nd.Area = 'SOC';

/* 4.d Tambien doy mat 40 a 2 docentes SOC existentes para reforzar */
INSERT INTO #AsignacionPlanificada (Emp_Id, Mat_Id) VALUES (4, 40), (8, 40);

/* 4.d.bis CUELLO DE BOTELLA CON (Contabilidad/Admon/Finanzas):
   24 secciones BTP demandan ~28 periodos CON c/u = ~670 periodos.
   5 docentes CON puros solo aguantan ~125. Hacemos polivalentes a los
   docentes nuevos NO-CON: cada uno toma 2 materias CON adicionales
   rotativas. Realista para un colegio chico donde mat/cie/tec dan apoyo
   a clases de admon basicas.                                            */
INSERT INTO #AsignacionPlanificada (Emp_Id, Mat_Id)
SELECT e.Emp_Id, mc.Mat_Id
FROM #NuevosDocentes nd
JOIN app.tbPersonas p   ON p.Per_Identidad = nd.Identidad
JOIN app.tbEmpleados e  ON e.Per_Id = p.Per_Id
JOIN (
    VALUES (0,14),(1,32),(2,33),(3,35),(4,37),(5,38),(6,39),
           (7,41),(8,42),(9,43),(10,44),(11,45),(12,46)
) AS mc(pos, Mat_Id)
   ON mc.pos IN (
        ((nd.N * 2)     % 13),
        ((nd.N * 2 + 1) % 13)
      )
WHERE nd.Area <> 'CON';

/* 4.e INSERT en tbEmpleados_tbMaterias (evitando duplicados) */
INSERT INTO app.tbEmpleados_tbMaterias (Emp_Id, Mat_Id)
SELECT DISTINCT ap.Emp_Id, ap.Mat_Id
FROM #AsignacionPlanificada ap
WHERE NOT EXISTS (
    SELECT 1 FROM app.tbEmpleados_tbMaterias em
    WHERE em.Emp_Id = ap.Emp_Id AND em.Mat_Id = ap.Mat_Id
);

PRINT CONCAT('Asignaciones profesor-materia insertadas: ', @@ROWCOUNT);

/* ----------------------------------------------------------------------------
   5. Resumen final
   ---------------------------------------------------------------------------- */

SELECT
    (SELECT COUNT(*) FROM app.tbEmpleados WHERE Car_Id = 3)         AS TotalDocentes,
    (SELECT COUNT(*) FROM app.tbEmpleados_tbMaterias)               AS TotalAsignaciones,
    (SELECT COUNT(DISTINCT Mat_Id) FROM app.tbEmpleados_tbMaterias) AS MateriasCubiertas;

GO
