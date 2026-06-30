/* ============================================================================
   horarios_02_generar_horarios.sql
   ----------------------------------------------------------------------------
   Generador automatico de horarios academicos para todas las secciones del
   colegio respetando estas restricciones DURAS:
     1. Un profesor no puede estar en 2 lugares al mismo (Dia, HoraInicio).
     2. Un aula no puede tener 2 clases al mismo (Dia, HoraInicio).
     3. Una seccion no puede tener 2 clases al mismo (Dia, HoraInicio).
     4. Un profesor solo da materias para las que esta autorizado
        (tbEmpleados_tbMaterias).
     5. Una materia solo se imparte en cursos donde existe en
        tbCursos_tbMaterias.
     6. Se respeta la carga semanal por area academica.
     7. Modalidad y aula vienen de tbCursos_tbSecciones.

   Parametros principales (editables en el bloque DECLARE de abajo):
     @AnioAcademico = 2026
     @SemestreId    = 1
     @UsuarioId     = 1

   Esquema de modalidades — cada sección académica es (Cun, Cur, Sec, Mda)
   leyendo el Mda_Id real desde tbAlumnos. Resultado: ~126 secciones
   académicas (42 grupos × 3 modalidades).

     - Mda_Id 1 (Matutina):    07:00 → 12:00, 6 bloques, receso 09:00-09:40
     - Mda_Id 2 (Vespertina):  13:00 → 18:00, 8 bloques, receso 15:00-15:40
     - Mda_Id 3 (Nocturna):    18:00 → 21:00, 4 bloques, receso 19:20-20:00

   Aulas: usadas por turnos secuenciales (no conflictivos en tiempo).

   Idempotente: borra (soft delete) los horarios del Anio+Semestre antes de
   regenerar.

   Requisitos previos:
     - horarios_01_seed_docentes.sql ya ejecutado
     - tbEmpleados_tbMaterias poblada
     - tbCursos_tbSecciones con aulas default por seccion
   ============================================================================ */

USE DB_GestionColegial;
GO
SET NOCOUNT ON;
GO

DECLARE @AnioAcademico INT = 2026;
DECLARE @SemestreId    INT = 1;
DECLARE @UsuarioId     INT = 1;

/* ----------------------------------------------------------------------------
   0. LIMPIEZA (soft delete) del horario existente del periodo objetivo
   ---------------------------------------------------------------------------- */
UPDATE app.tbHorarios
   SET Hor_EsEliminado = 1,
       Hor_UsuarioModifica = @UsuarioId,
       Hor_FechaModifica = GETDATE()
 WHERE Hor_Año = @AnioAcademico
   AND Sem_Id  = @SemestreId
   AND Hor_EsEliminado = 0;

PRINT CONCAT('Horarios previos marcados como eliminados: ', @@ROWCOUNT);

/* ----------------------------------------------------------------------------
   0.b ASEGURAR 24 AULAS (originales 20 + K-1, K-2, L-1, L-2)
   Las BTP de Cun 5 y Cun 6 comparten jornada vespertina y necesitan 24
   aulas simultaneas.
   ---------------------------------------------------------------------------- */
INSERT INTO app.tbAulas
    (Aul_Descripcion, Aul_EsEliminado, Aul_UsuarioRegistra, Aul_FechaRegistra)
SELECT v.d, 0, @UsuarioId, GETDATE()
FROM (VALUES
    ('K-1'),('K-2'),('L-1'),('L-2'),
    ('M-1'),('M-2'),('M-3'),
    ('N-1'),('N-2'),('N-3'),
    ('O-1'),('O-2'),('O-3'),
    ('P-1'),('P-2'),('P-3'),
    ('Q-1'),('Q-2'),('Q-3'),
    ('R-1'),('R-2'),('R-3'),
    ('S-1'),('S-2'),('S-3')
) AS v(d)
WHERE NOT EXISTS (
    SELECT 1 FROM app.tbAulas a
    WHERE a.Aul_Descripcion = v.d COLLATE DATABASE_DEFAULT AND a.Aul_EsEliminado = 0
);

PRINT CONCAT('Aulas extra creadas: ', @@ROWCOUNT);

/* ----------------------------------------------------------------------------
   0.c ASEGURAR HORAS NOCTURNAS (Hor_Id 20-24: 06:40 → 09:00 PM)
   ---------------------------------------------------------------------------- */
INSERT INTO app.tbHoras
    (Hor_Hora, Hor_EsEliminado, Hor_UsuarioRegistra, Hor_FechaRegistra)
SELECT v.h, 0, @UsuarioId, GETDATE()
FROM (VALUES ('06:40'),('07:20'),('08:00'),('08:40'),('09:00')) AS v(h)
WHERE NOT EXISTS (
    SELECT 1 FROM app.tbHoras x
    WHERE RTRIM(REPLACE(x.Hor_Hora, CHAR(10), '')) = v.h COLLATE DATABASE_DEFAULT
);

PRINT CONCAT('Horas nocturnas creadas: ', @@ROWCOUNT);

/* ----------------------------------------------------------------------------
   1. DEFINIR BLOQUES HORARIOS POR MODALIDAD
   Cada bloque = (Mda_Id, BloqueIdx, HoraInicio_FK, HoraFinaliza_FK)
   Usamos los Hor_Id de app.tbHoras (1..19).
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('tempdb..#Bloques') IS NOT NULL DROP TABLE #Bloques;
CREATE TABLE #Bloques (
    Mda_Id     INT NOT NULL,
    BloqueIdx  INT NOT NULL,
    HoraIni    INT NOT NULL,
    HoraFin    INT NOT NULL,
    PRIMARY KEY (Mda_Id, BloqueIdx)
);

/* Resolvemos Hor_Id por descripcion para que el script sea robusto si
   tbHoras se recrea con IDs distintos. Las horas son siempre las mismas:
   07:00, 07:40, 08:20, 09:00, 09:40, 10:00, 10:40, 11:20, 12:00,
   01:00, 01:40, 02:20, 03:00, 03:20, 04:00, 04:40, 05:20, 06:00, 06:30,
   06:40, 07:20, 08:00, 08:40, 09:00.                                    */
DECLARE
    @h0700 INT, @h0740 INT, @h0820 INT, @h0900 INT, @h0940 INT,
    @h1000 INT, @h1040 INT, @h1120 INT, @h1200 INT,
    @h0100 INT, @h0140 INT, @h0220 INT, @h0300 INT, @h0320 INT,
    @h0400 INT, @h0440 INT, @h0520 INT, @h0600 INT, @h0630 INT,
    @h0640 INT, @h0720 INT, @h0800 INT, @h0840 INT, @h0900b INT;

SELECT @h0700 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='07:00';
SELECT @h0740 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='07:40';
SELECT @h0820 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='08:20';
SELECT @h0900 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='09:00';
SELECT @h0940 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='09:40';
SELECT @h1000 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='10:00';
SELECT @h1040 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='10:40';
SELECT @h1120 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='11:20';
SELECT @h1200 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='12:00';
SELECT @h0100 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='01:00';
SELECT @h0140 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='01:40';
SELECT @h0220 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='02:20';
SELECT @h0300 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='03:00';
SELECT @h0320 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='03:20';
SELECT @h0400 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='04:00';
SELECT @h0440 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='04:40';
SELECT @h0520 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='05:20';
SELECT @h0600 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='06:00';
SELECT @h0630 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='06:30';
SELECT @h0640 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='06:40';
SELECT @h0720 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='07:20';
SELECT @h0800 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='08:00';
SELECT @h0840 = MIN(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='08:40';
-- '09:00' nocturno comparte string con '09:00' AM (Hor_Id 4). Tomamos el segundo registro:
SELECT @h0900b = MAX(Hor_Id) FROM app.tbHoras WHERE RTRIM(REPLACE(REPLACE(Hor_Hora,CHAR(10),''),CHAR(13),''))='09:00';

-- Matutina 07:00 - 12:00, receso 09:00-09:40 (40 min) -> 6 bloques de 40min
INSERT INTO #Bloques (Mda_Id, BloqueIdx, HoraIni, HoraFin) VALUES
    (1, 1, @h0700, @h0740),  -- 07:00 -> 07:40
    (1, 2, @h0740, @h0820),  -- 07:40 -> 08:20
    (1, 3, @h0820, @h0900),  -- 08:20 -> 09:00
    -- RECESO 09:00 -> 09:40 (40 min)
    (1, 4, @h0940, @h1000),  -- 09:40 -> 10:00 (corto, recreo amplio compensa)
    (1, 5, @h1000, @h1040),  -- 10:00 -> 10:40
    (1, 6, @h1040, @h1120),  -- 10:40 -> 11:20
    (1, 7, @h1120, @h1200);  -- 11:20 -> 12:00

-- Vespertina 13:00 - 18:00, receso 15:00-15:40 (40 min) -> 8 bloques
INSERT INTO #Bloques (Mda_Id, BloqueIdx, HoraIni, HoraFin) VALUES
    (2, 1, @h0100, @h0140), -- 01:00 -> 01:40
    (2, 2, @h0140, @h0220), -- 01:40 -> 02:20
    (2, 3, @h0220, @h0300), -- 02:20 -> 03:00
    -- RECESO 03:00 -> 03:20 (20 min nominal, extendido visualmente a 40 min)
    (2, 4, @h0320, @h0400), -- 03:20 -> 04:00
    (2, 5, @h0400, @h0440), -- 04:00 -> 04:40
    (2, 6, @h0440, @h0520), -- 04:40 -> 05:20
    (2, 7, @h0520, @h0600); -- 05:20 -> 06:00

-- Nocturna 18:00 - 21:00, receso 19:20-20:00 (40 min) -> 4 bloques
INSERT INTO #Bloques (Mda_Id, BloqueIdx, HoraIni, HoraFin) VALUES
    (3, 1, @h0600, @h0640),  -- 06:00 -> 06:40
    (3, 2, @h0640, @h0720),  -- 06:40 -> 07:20
    -- RECESO 07:20 -> 08:00 (40 min)
    (3, 3, @h0800, @h0840),  -- 08:00 -> 08:40
    (3, 4, @h0840, @h0900b); -- 08:40 -> 09:00

/* ----------------------------------------------------------------------------
   2. CATALOGO DE SECCIONES ACADEMICAS (42 filas)
   Para cada (Cun, Cur, Sec) detectamos su modalidad y aula default.
   Si no hay fila en tbCursos_tbSecciones para esa Sec, repartimos
   round-robin entre aulas libres.
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('tempdb..#Secciones') IS NOT NULL DROP TABLE #Secciones;
CREATE TABLE #Secciones (
    SecAcadId   INT IDENTITY(1,1) PRIMARY KEY,
    Cun_Id      INT NOT NULL,
    Cur_Id      INT NOT NULL,
    Sec_Id      INT NOT NULL,
    Mda_Id      INT NOT NULL,
    Aul_Id      INT NOT NULL,
    SlotsSem    INT NOT NULL  -- bloques disponibles por semana
);

/* Una seccion academica = (Cun, Cur, Sec, Mda) — lee Mda_Id real de los
   alumnos. Resultado: ~126 secciones distintas (42 grupos x 3 modalidades).*/
;WITH SecAlumnos AS (
    SELECT DISTINCT a.Cun_Id, a.Cur_Id, a.Sec_Id, a.Mda_Id
    FROM app.tbAlumnos a
    WHERE a.Mda_Id IN (1, 2, 3)
),
AulasOrden AS (
    SELECT Aul_Id, ROW_NUMBER() OVER (ORDER BY Aul_Id) AS rn
    FROM app.tbAulas
    WHERE Aul_EsEliminado = 0
),
SecCount AS (
    SELECT s.Cun_Id, s.Cur_Id, s.Sec_Id, s.Mda_Id,
           -- round-robin de aulas por modalidad (las aulas se reusan entre turnos)
           ROW_NUMBER() OVER (
               PARTITION BY s.Mda_Id
               ORDER BY s.Cun_Id, s.Cur_Id, s.Sec_Id
           ) AS posEnTurno
    FROM SecAlumnos s
),
NAulas AS (SELECT COUNT(*) AS n FROM AulasOrden)
INSERT INTO #Secciones (Cun_Id, Cur_Id, Sec_Id, Mda_Id, Aul_Id, SlotsSem)
SELECT
    sc.Cun_Id, sc.Cur_Id, sc.Sec_Id, sc.Mda_Id,
    ao.Aul_Id AS Aul_Id,
    -- Matutina 7 bloq * 5 = 35, Vespertina 7 bloq * 5 = 35, Nocturna 4 bloq * 5 = 20
    CASE sc.Mda_Id WHEN 1 THEN 35 WHEN 2 THEN 35 ELSE 20 END AS SlotsSem
FROM SecCount sc
CROSS JOIN NAulas na
JOIN AulasOrden ao ON ao.rn = ((sc.posEnTurno - 1) % na.n) + 1
ORDER BY sc.Mda_Id, sc.Cun_Id, sc.Cur_Id, sc.Sec_Id;

-- (Antes habia un override desde tbCursos_tbSecciones, pero asignaba la misma
--  aula a Decimo + Undecimo + Duodecimo del mismo curso y modalidad, lo cual
--  generaba colisiones de aula. Round-robin da 1 aula unica por seccion.)

DECLARE @TotalSec INT = (SELECT COUNT(*) FROM #Secciones);
PRINT CONCAT('Secciones academicas a programar: ', @TotalSec);

/* ----------------------------------------------------------------------------
   3. AREA POR MATERIA + PESO (periodos sugeridos/semana)
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('tempdb..#MateriasArea') IS NOT NULL DROP TABLE #MateriasArea;
CREATE TABLE #MateriasArea (
    Mat_Id INT NOT NULL PRIMARY KEY,
    Area   CHAR(3) NOT NULL,
    Peso   INT NOT NULL  -- periodos ideales / semana
);

INSERT INTO #MateriasArea (Mat_Id, Area, Peso) VALUES
    ( 1,'SOC',3),( 2,'MAT',5),( 3,'LEN',5),( 4,'CIE',4),( 5,'SOC',3),
    ( 6,'CIE',4),( 7,'SOC',3),( 8,'ING',4),( 9,'TEC',3),(10,'CIE',4),
    (11,'SOC',3),(12,'SOC',3),(13,'SOC',3),(14,'CON',4),(15,'MAT',5),
    (16,'MAT',5),(17,'MAT',5),(18,'CIE',4),(19,'MAT',5),(20,'SOC',3),
    (21,'ART',2),(22,'LEN',5),(23,'SOC',3),(24,'LEN',5),(25,'TEC',3),
    (26,'ING',4),(27,'TEC',3),(28,'TEC',3),(29,'TEC',3),(30,'SOC',3),
    (31,'SOC',3),(32,'CON',4),(33,'CON',4),(34,'LEN',5),(35,'CON',4),
    (36,'ING',4),(37,'CON',4),(38,'CON',4),(39,'CON',4),(40,'OTR',2),
    (41,'CON',4),(42,'CON',4),(43,'CON',4),(44,'CON',4),(45,'CON',4),
    (46,'CON',4),(47,'ING',4);

/* ----------------------------------------------------------------------------
   4. DEMANDA: para cada Seccion x Materia(en su curso), cuantos periodos
   debe recibir. Ajustado para no exceder SlotsSem de la seccion.
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('tempdb..#Demanda') IS NOT NULL DROP TABLE #Demanda;
CREATE TABLE #Demanda (
    SecAcadId       INT NOT NULL,
    Mat_Id          INT NOT NULL,
    Periodos        INT NOT NULL,
    Pendientes      INT NOT NULL,
    IntentosFallidos INT NOT NULL DEFAULT 0, -- 0..3: si llega a 3 sin slot, se rinde
    PRIMARY KEY (SecAcadId, Mat_Id)
);

;WITH MaterialSeccion AS (
    SELECT
        s.SecAcadId,
        s.SlotsSem,
        cm.Mat_Id,
        ma.Peso AS PesoArea
    FROM #Secciones s
    JOIN app.tbCursos_tbMaterias cm ON cm.Cur_Id = s.Cur_Id
    JOIN #MateriasArea ma            ON ma.Mat_Id = cm.Mat_Id
),
Totales AS (
    SELECT SecAcadId, SUM(PesoArea) AS PesoTotal, MAX(SlotsSem) AS Slots
    FROM MaterialSeccion
    GROUP BY SecAcadId
),
Distribuido AS (
    SELECT
        m.SecAcadId, m.Mat_Id,
        -- Periodos proporcionales al peso, redondeados, min 1
        CAST(
          CASE
            WHEN t.PesoTotal = 0 THEN 1
            ELSE ROUND(1.0 * m.PesoArea / t.PesoTotal * t.Slots, 0)
          END AS INT
        ) AS PeriodosBruto,
        m.PesoArea, t.Slots, t.PesoTotal
    FROM MaterialSeccion m
    JOIN Totales t ON t.SecAcadId = m.SecAcadId
)
INSERT INTO #Demanda (SecAcadId, Mat_Id, Periodos, Pendientes)
SELECT
    SecAcadId, Mat_Id,
    CASE WHEN PeriodosBruto < 1 THEN 1 ELSE PeriodosBruto END,
    CASE WHEN PeriodosBruto < 1 THEN 1 ELSE PeriodosBruto END
FROM Distribuido;

-- Ajuste fino: por si el redondeo produjo sumas != SlotsSem, recortar la
-- demanda mas alta hasta no exceder. (No es estrictamente necesario, pero
-- evita marear al algoritmo).
WHILE EXISTS (
    SELECT 1 FROM (
      SELECT d.SecAcadId, SUM(d.Periodos) AS Sum
      FROM #Demanda d GROUP BY d.SecAcadId
    ) x
    JOIN #Secciones s ON s.SecAcadId = x.SecAcadId
    WHERE x.Sum > s.SlotsSem
)
BEGIN
    UPDATE d
       SET Periodos = Periodos - 1,
           Pendientes = Pendientes - 1
    FROM #Demanda d
    WHERE d.Mat_Id = (
        SELECT TOP 1 d2.Mat_Id
        FROM #Demanda d2
        JOIN (
            SELECT d3.SecAcadId, SUM(d3.Periodos) AS Sum
            FROM #Demanda d3 GROUP BY d3.SecAcadId
        ) x ON x.SecAcadId = d2.SecAcadId
        JOIN #Secciones s ON s.SecAcadId = d2.SecAcadId
        WHERE x.Sum > s.SlotsSem
        ORDER BY d2.Periodos DESC, d2.Mat_Id
    )
      AND d.SecAcadId = (
        SELECT TOP 1 d2.SecAcadId
        FROM #Demanda d2
        JOIN (
            SELECT d3.SecAcadId, SUM(d3.Periodos) AS Sum
            FROM #Demanda d3 GROUP BY d3.SecAcadId
        ) x ON x.SecAcadId = d2.SecAcadId
        JOIN #Secciones s ON s.SecAcadId = d2.SecAcadId
        WHERE x.Sum > s.SlotsSem
        ORDER BY d2.Periodos DESC, d2.Mat_Id
    );
END

DECLARE @TotalPeriodos INT = (SELECT SUM(Periodos) FROM #Demanda);
PRINT CONCAT('Periodos totales a asignar: ', @TotalPeriodos);

/* ----------------------------------------------------------------------------
   5. CATALOGO DE DOCENTES POR MATERIA
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('tempdb..#DocentesMateria') IS NOT NULL DROP TABLE #DocentesMateria;
CREATE TABLE #DocentesMateria (Mat_Id INT, Emp_Id INT, PRIMARY KEY (Mat_Id, Emp_Id));
INSERT INTO #DocentesMateria (Mat_Id, Emp_Id)
SELECT em.Mat_Id, em.Emp_Id
FROM app.tbEmpleados_tbMaterias em
JOIN app.tbEmpleados e ON e.Emp_Id = em.Emp_Id
WHERE e.Car_Id = 3;  -- Docencia

/* Aviso de materias sin docente */
IF EXISTS (
    SELECT 1
    FROM (SELECT DISTINCT Mat_Id FROM #Demanda) d
    WHERE NOT EXISTS (SELECT 1 FROM #DocentesMateria m WHERE m.Mat_Id = d.Mat_Id)
)
BEGIN
    PRINT 'ATENCION: hay materias en la demanda sin docente asignado:';
    SELECT DISTINCT d.Mat_Id, ma.Mat_Nombre
    FROM (SELECT DISTINCT Mat_Id FROM #Demanda) d
    JOIN app.tbMaterias ma ON ma.Mat_Id = d.Mat_Id
    WHERE NOT EXISTS (SELECT 1 FROM #DocentesMateria m WHERE m.Mat_Id = d.Mat_Id);
END

/* ----------------------------------------------------------------------------
   6. RESULTADO acumulado y ASIGNACION GREEDY
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('tempdb..#Resultado') IS NOT NULL DROP TABLE #Resultado;
CREATE TABLE #Resultado (
    SecAcadId   INT NOT NULL,
    Cun_Id      INT NOT NULL,
    Cur_Id      INT NOT NULL,
    Sec_Id      INT NOT NULL,
    Mat_Id      INT NOT NULL,
    Emp_Id      INT NOT NULL,
    Aul_Id      INT NOT NULL,
    Mda_Id      INT NOT NULL,
    Dia_Id      INT NOT NULL,
    HoraIni     INT NOT NULL,
    HoraFin     INT NOT NULL
);

-- Indices para acelerar las verificaciones de colision
CREATE INDEX IX_Res_Sec  ON #Resultado(SecAcadId, Dia_Id, HoraIni);
CREATE INDEX IX_Res_Aul  ON #Resultado(Aul_Id,    Dia_Id, HoraIni);
CREATE INDEX IX_Res_Emp  ON #Resultado(Emp_Id,    Dia_Id, HoraIni);

/* Tabla de no-asignables: cuando una demanda no encuentre slot/docente
   se marca aqui y se decrementa Pendientes para evitar bucle infinito.   */
IF OBJECT_ID('tempdb..#NoAsignables') IS NOT NULL DROP TABLE #NoAsignables;
CREATE TABLE #NoAsignables (
    SecAcadId INT, Mat_Id INT, Faltantes INT,
    PRIMARY KEY (SecAcadId, Mat_Id)
);

DECLARE @Asignados INT = 0;
DECLARE @NoAsignados INT = 0;
DECLARE @Iter INT = 0;
DECLARE @MaxIter INT = 5000;  -- guarda

DECLARE
    @cSecAcadId INT, @cMat_Id INT, @cEmp_Id INT,
    @cCun INT, @cCur INT, @cSec INT, @cAul INT, @cMda INT,
    @cDia INT, @cHIni INT, @cHFin INT;

SET @MaxIter = 12000;

WHILE @Iter < @MaxIter
   AND EXISTS (SELECT 1 FROM #Demanda WHERE Pendientes > 0 AND IntentosFallidos < 3)
BEGIN
    SET @Iter += 1;

    /* Heuristica MOST-CONSTRAINED-FIRST con reintento:
       - prioriza materias con MAS pendientes que aun no han fallado 3 veces
       - dentro de eso, las que tienen menos docentes disponibles               */
    SELECT TOP 1
        @cSecAcadId = d.SecAcadId,
        @cMat_Id    = d.Mat_Id,
        @cCun       = s.Cun_Id,
        @cCur       = s.Cur_Id,
        @cSec       = s.Sec_Id,
        @cAul       = s.Aul_Id,
        @cMda       = s.Mda_Id
    FROM #Demanda d
    JOIN #Secciones s ON s.SecAcadId = d.SecAcadId
    WHERE d.Pendientes > 0 AND d.IntentosFallidos < 3
    ORDER BY
        d.IntentosFallidos ASC,  -- primero los que aun no han fallado
        (SELECT COUNT(*) FROM #DocentesMateria m WHERE m.Mat_Id = d.Mat_Id) ASC,
        s.SlotsSem ASC,
        d.Pendientes DESC,
        d.SecAcadId, d.Mat_Id;

    /* Intentar colocar 1 periodo: buscar (Dia, Bloque, Docente) que cumpla.
       Restricciones:
         - Seccion libre en (Dia, HoraIni)
         - Aula libre en (Dia, HoraIni)
         - Docente apto y libre en (Dia, HoraIni)
         - No mas de 2 periodos de la misma materia en el mismo dia       */
    SELECT TOP 1
        @cDia  = dia.Dia_Id,
        @cHIni = b.HoraIni,
        @cHFin = b.HoraFin,
        @cEmp_Id = dm.Emp_Id
    FROM #Bloques b
    JOIN (VALUES (1),(2),(3),(4),(5)) AS dia(Dia_Id) ON 1=1     -- Lunes a Viernes
    JOIN #DocentesMateria dm ON dm.Mat_Id = @cMat_Id
    WHERE b.Mda_Id = @cMda
      AND NOT EXISTS (
          SELECT 1 FROM #Resultado r
          WHERE r.SecAcadId = @cSecAcadId AND r.Dia_Id = dia.Dia_Id AND r.HoraIni = b.HoraIni
      )
      AND NOT EXISTS (
          SELECT 1 FROM #Resultado r
          WHERE r.Aul_Id = @cAul AND r.Dia_Id = dia.Dia_Id AND r.HoraIni = b.HoraIni
      )
      AND NOT EXISTS (
          SELECT 1 FROM #Resultado r
          WHERE r.Emp_Id = dm.Emp_Id AND r.Dia_Id = dia.Dia_Id AND r.HoraIni = b.HoraIni
      )
      -- Carga laboral: maximo 9 periodos/dia/docente (sin horas extra)
      AND NOT EXISTS (
          SELECT 1 FROM #Resultado r
          WHERE r.Emp_Id = dm.Emp_Id AND r.Dia_Id = dia.Dia_Id
          GROUP BY r.Emp_Id, r.Dia_Id
          HAVING COUNT(*) >= 9
      )
      AND NOT EXISTS (
          SELECT 1 FROM #Resultado r
          WHERE r.SecAcadId = @cSecAcadId AND r.Mat_Id = @cMat_Id AND r.Dia_Id = dia.Dia_Id
          GROUP BY r.SecAcadId, r.Mat_Id, r.Dia_Id
          HAVING COUNT(*) >= 2
      )
    ORDER BY
        -- distribuir lo mas posible a lo ancho de la semana
        (SELECT COUNT(*) FROM #Resultado r WHERE r.SecAcadId = @cSecAcadId AND r.Dia_Id = dia.Dia_Id) ASC,
        -- balancear carga del docente
        (SELECT COUNT(*) FROM #Resultado r WHERE r.Emp_Id = dm.Emp_Id) ASC,
        dia.Dia_Id, b.BloqueIdx, dm.Emp_Id;

    IF @cDia IS NULL
    BEGIN
        /* No se encontro slot en este intento. Incrementamos contador de
           fallos y dejamos los pendientes intactos: en la siguiente iteracion
           se procesara otra (sec, mat) y volveremos a esta cuando el estado
           del tablero haya cambiado. A los 3 fallos consecutivos se la deja
           definitivamente fuera (timeout para evitar loop infinito).        */
        UPDATE #Demanda SET IntentosFallidos = IntentosFallidos + 1
         WHERE SecAcadId = @cSecAcadId AND Mat_Id = @cMat_Id;

        -- Si llego a 3 fallos, registrar los faltantes en NoAsignables
        IF (SELECT IntentosFallidos FROM #Demanda
            WHERE SecAcadId = @cSecAcadId AND Mat_Id = @cMat_Id) >= 3
        BEGIN
            DECLARE @FaltX INT = (SELECT Pendientes FROM #Demanda
                                  WHERE SecAcadId = @cSecAcadId AND Mat_Id = @cMat_Id);
            MERGE #NoAsignables AS t
            USING (SELECT @cSecAcadId AS SecAcadId, @cMat_Id AS Mat_Id, @FaltX AS Faltantes) AS s
               ON t.SecAcadId = s.SecAcadId AND t.Mat_Id = s.Mat_Id
            WHEN MATCHED THEN UPDATE SET Faltantes = s.Faltantes
            WHEN NOT MATCHED THEN INSERT (SecAcadId, Mat_Id, Faltantes) VALUES (s.SecAcadId, s.Mat_Id, s.Faltantes);
            SET @NoAsignados += @FaltX;
        END
    END
    ELSE
    BEGIN
        INSERT INTO #Resultado
            (SecAcadId, Cun_Id, Cur_Id, Sec_Id, Mat_Id, Emp_Id, Aul_Id, Mda_Id, Dia_Id, HoraIni, HoraFin)
        VALUES
            (@cSecAcadId, @cCun, @cCur, @cSec, @cMat_Id, @cEmp_Id, @cAul, @cMda, @cDia, @cHIni, @cHFin);

        -- Exito: decrementar pendientes y resetear contador de fallos
        -- (estado del tablero cambio, podria ser asignable de nuevo)
        UPDATE #Demanda
           SET Pendientes = Pendientes - 1,
               IntentosFallidos = 0
         WHERE SecAcadId = @cSecAcadId AND Mat_Id = @cMat_Id;

        SET @Asignados += 1;
    END

    -- reset variables para siguiente iteracion
    SET @cDia = NULL;
    SET @cEmp_Id = NULL;
END

PRINT CONCAT('Iteraciones: ', @Iter);
PRINT CONCAT('Periodos asignados: ', @Asignados);
PRINT CONCAT('Periodos NO asignados (sin slot/docente): ', @NoAsignados);

/* ----------------------------------------------------------------------------
   7. INSERT EN tbHorarios (transactional)
   ---------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO app.tbHorarios
        (Cur_Id, Cun_Id, Mat_Id, Emp_Id, Sec_Id, Aul_Id, Dia_Id,
         Hor_HoraInicio, Hor_HoraFinaliza, Sem_Id, Mda_Id, Hor_Año,
         Hor_EsEliminado, Hor_UsuarioRegistra, Hor_FechaRegistra)
    SELECT
        r.Cur_Id, r.Cun_Id, r.Mat_Id, r.Emp_Id, r.Sec_Id, r.Aul_Id, r.Dia_Id,
        r.HoraIni, r.HoraFin, @SemestreId, r.Mda_Id, @AnioAcademico,
        0, @UsuarioId, GETDATE()
    FROM #Resultado r;

    PRINT CONCAT('Filas insertadas en tbHorarios: ', @@ROWCOUNT);
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    PRINT 'ERROR al insertar en tbHorarios:';
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;

/* ----------------------------------------------------------------------------
   8. RESUMEN
   ---------------------------------------------------------------------------- */
SELECT
    COUNT(*)                            AS HorariosCreados,
    COUNT(DISTINCT Emp_Id)              AS DocentesUsados,
    COUNT(DISTINCT Aul_Id)              AS AulasUsadas,
    COUNT(DISTINCT Mat_Id)              AS MateriasProgramadas
FROM app.tbHorarios
WHERE Hor_Año = @AnioAcademico AND Sem_Id = @SemestreId AND Hor_EsEliminado = 0;

IF EXISTS (SELECT 1 FROM #NoAsignables)
BEGIN
    PRINT '---- Periodos NO asignados (revisar manualmente) ----';
    SELECT na.SecAcadId, s.Cun_Id, s.Cur_Id, s.Sec_Id, na.Mat_Id, m.Mat_Nombre, na.Faltantes
    FROM #NoAsignables na
    JOIN #Secciones s ON s.SecAcadId = na.SecAcadId
    JOIN app.tbMaterias m ON m.Mat_Id = na.Mat_Id
    ORDER BY s.Cun_Id, s.Cur_Id, s.Sec_Id, na.Mat_Id;
END

GO
