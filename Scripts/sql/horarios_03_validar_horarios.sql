/* ============================================================================
   horarios_03_validar_horarios.sql
   ----------------------------------------------------------------------------
   Valida que el horario generado cumpla las restricciones duras y reporta
   estadisticas utiles para revisar la calidad de la asignacion.

   Edita @AnioAcademico y @SemestreId si necesitas validar otro periodo.
   ============================================================================ */

USE DB_GestionColegial;
GO
SET NOCOUNT ON;
GO

DECLARE @AnioAcademico INT = 2026;
DECLARE @SemestreId    INT = 1;

PRINT '==============================================================';
PRINT CONCAT(' VALIDACION DE HORARIOS  Anio=', @AnioAcademico, '  Semestre=', @SemestreId);
PRINT '==============================================================';

/* ============================================================================
   1. CONFLICTOS DE PROFESOR (mismo Emp_Id en el mismo Dia + HoraInicio)
   ============================================================================ */
PRINT '';
PRINT '----- 1. CONFLICTOS DE PROFESOR (deberia ser 0 filas) -----';
SELECT
    h.Emp_Id,
    CONCAT(p.Per_PrimerNombre, ' ', p.Per_ApellidoPaterno) AS Profesor,
    h.Dia_Id,
    d.Dia_Descripcion,
    h.Hor_HoraInicio,
    COUNT(*) AS Choques
FROM app.tbHorarios h
JOIN app.tbEmpleados e ON e.Emp_Id = h.Emp_Id
JOIN app.tbPersonas p  ON p.Per_Id = e.Per_Id
JOIN app.tbDias d      ON d.Dia_Id = h.Dia_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
GROUP BY h.Emp_Id, p.Per_PrimerNombre, p.Per_ApellidoPaterno, h.Dia_Id, d.Dia_Descripcion, h.Hor_HoraInicio
HAVING COUNT(*) > 1
ORDER BY Choques DESC, h.Emp_Id;

/* ============================================================================
   2. CONFLICTOS DE AULA
   ============================================================================ */
PRINT '';
PRINT '----- 2. CONFLICTOS DE AULA (deberia ser 0 filas) -----';
SELECT
    h.Aul_Id,
    a.Aul_Descripcion,
    h.Dia_Id,
    d.Dia_Descripcion,
    h.Hor_HoraInicio,
    COUNT(*) AS Choques
FROM app.tbHorarios h
JOIN app.tbAulas a ON a.Aul_Id = h.Aul_Id
JOIN app.tbDias  d ON d.Dia_Id = h.Dia_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
GROUP BY h.Aul_Id, a.Aul_Descripcion, h.Dia_Id, d.Dia_Descripcion, h.Hor_HoraInicio
HAVING COUNT(*) > 1
ORDER BY Choques DESC, h.Aul_Id;

/* ============================================================================
   3.b SOBRECARGA DE DOCENTE (mas de 9 periodos/dia)
   ============================================================================ */
PRINT '';
PRINT '----- 3.b SOBRECARGA DE DOCENTE (>9 periodos/dia) -----';
SELECT
    h.Emp_Id,
    CONCAT(p.Per_PrimerNombre, ' ', p.Per_ApellidoPaterno) AS Profesor,
    h.Dia_Id,
    d.Dia_Descripcion,
    COUNT(*) AS Periodos
FROM app.tbHorarios h
JOIN app.tbEmpleados e ON e.Emp_Id = h.Emp_Id
JOIN app.tbPersonas p  ON p.Per_Id = e.Per_Id
JOIN app.tbDias d      ON d.Dia_Id = h.Dia_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
GROUP BY h.Emp_Id, p.Per_PrimerNombre, p.Per_ApellidoPaterno, h.Dia_Id, d.Dia_Descripcion
HAVING COUNT(*) > 9
ORDER BY Periodos DESC;

/* ============================================================================
   3. CONFLICTOS DE SECCION (misma Cur+Sec con 2 clases al mismo tiempo)
   ============================================================================ */
PRINT '';
PRINT '----- 3. CONFLICTOS DE SECCION (deberia ser 0 filas) -----';
SELECT
    h.Cun_Id,
    h.Cur_Id,
    c.Cur_Nombre,
    h.Sec_Id,
    s.Sec_Descripcion,
    h.Dia_Id,
    h.Hor_HoraInicio,
    COUNT(*) AS Choques
FROM app.tbHorarios h
JOIN app.tbCursos c    ON c.Cur_Id = h.Cur_Id
JOIN app.tbSecciones s ON s.Sec_Id = h.Sec_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
GROUP BY h.Cun_Id, h.Cur_Id, c.Cur_Nombre, h.Sec_Id, s.Sec_Descripcion, h.Dia_Id, h.Hor_HoraInicio
HAVING COUNT(*) > 1
ORDER BY Choques DESC;

/* ============================================================================
   4. PROFESOR ASIGNADO A MATERIA QUE NO PUEDE ENSEÑAR
   ============================================================================ */
PRINT '';
PRINT '----- 4. PROFESORES IMPARTIENDO MATERIA NO AUTORIZADA (deberia ser 0) -----';
SELECT DISTINCT h.Emp_Id, h.Mat_Id, m.Mat_Nombre,
       CONCAT(p.Per_PrimerNombre, ' ', p.Per_ApellidoPaterno) AS Profesor
FROM app.tbHorarios h
JOIN app.tbMaterias m  ON m.Mat_Id = h.Mat_Id
JOIN app.tbEmpleados e ON e.Emp_Id = h.Emp_Id
JOIN app.tbPersonas p  ON p.Per_Id = e.Per_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
  AND NOT EXISTS (
      SELECT 1 FROM app.tbEmpleados_tbMaterias em
      WHERE em.Emp_Id = h.Emp_Id AND em.Mat_Id = h.Mat_Id
  );

/* ============================================================================
   5. MATERIA IMPARTIDA EN CURSO DONDE NO PERTENECE
   ============================================================================ */
PRINT '';
PRINT '----- 5. MATERIA-CURSO NO AUTORIZADO (deberia ser 0) -----';
SELECT DISTINCT h.Cur_Id, h.Mat_Id, m.Mat_Nombre, c.Cur_Nombre
FROM app.tbHorarios h
JOIN app.tbMaterias m ON m.Mat_Id = h.Mat_Id
JOIN app.tbCursos c   ON c.Cur_Id = h.Cur_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
  AND NOT EXISTS (
      SELECT 1 FROM app.tbCursos_tbMaterias cm
      WHERE cm.Cur_Id = h.Cur_Id AND cm.Mat_Id = h.Mat_Id
  );

/* ============================================================================
   6. COBERTURA POR SECCION
   ============================================================================ */
PRINT '';
PRINT '----- 6. PERIODOS POR SECCION (resumen) -----';
SELECT
    h.Cun_Id,
    h.Cur_Id,
    c.Cur_Nombre,
    h.Sec_Id,
    s.Sec_Descripcion,
    COUNT(*) AS PeriodosSemanales,
    COUNT(DISTINCT h.Mat_Id) AS Materias
FROM app.tbHorarios h
JOIN app.tbCursos c    ON c.Cur_Id = h.Cur_Id
JOIN app.tbSecciones s ON s.Sec_Id = h.Sec_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
GROUP BY h.Cun_Id, h.Cur_Id, c.Cur_Nombre, h.Sec_Id, s.Sec_Descripcion
ORDER BY h.Cun_Id, h.Cur_Id, h.Sec_Id;

/* ============================================================================
   7. CARGA HORARIA POR DOCENTE
   ============================================================================ */
PRINT '';
PRINT '----- 7. CARGA HORARIA POR DOCENTE -----';
SELECT
    h.Emp_Id,
    CONCAT(p.Per_PrimerNombre, ' ', p.Per_ApellidoPaterno) AS Profesor,
    COUNT(*) AS PeriodosSem,
    COUNT(DISTINCT h.Mat_Id) AS MateriasDistintas,
    COUNT(DISTINCT CONCAT(h.Cur_Id,'-',h.Sec_Id)) AS SeccionesAtendidas
FROM app.tbHorarios h
JOIN app.tbEmpleados e ON e.Emp_Id = h.Emp_Id
JOIN app.tbPersonas p  ON p.Per_Id = e.Per_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
GROUP BY h.Emp_Id, p.Per_PrimerNombre, p.Per_ApellidoPaterno
ORDER BY PeriodosSem DESC;

/* ============================================================================
   8. USO DE AULAS
   ============================================================================ */
PRINT '';
PRINT '----- 8. USO DE AULAS -----';
SELECT
    h.Aul_Id,
    a.Aul_Descripcion,
    h.Mda_Id,
    COUNT(*) AS PeriodosOcupados,
    COUNT(DISTINCT CONCAT(h.Cur_Id,'-',h.Sec_Id)) AS SeccionesQueLaUsan
FROM app.tbHorarios h
JOIN app.tbAulas a ON a.Aul_Id = h.Aul_Id
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0
GROUP BY h.Aul_Id, a.Aul_Descripcion, h.Mda_Id
ORDER BY h.Aul_Id, h.Mda_Id;

/* ============================================================================
   9. RESUMEN GENERAL
   ============================================================================ */
PRINT '';
PRINT '----- 9. RESUMEN GENERAL -----';
SELECT
    COUNT(*)                                                       AS TotalHorarios,
    COUNT(DISTINCT CONCAT(h.Cun_Id,'-',h.Cur_Id,'-',h.Sec_Id))     AS SeccionesCubiertas,
    COUNT(DISTINCT h.Emp_Id)                                       AS DocentesAsignados,
    COUNT(DISTINCT h.Mat_Id)                                       AS MateriasProgramadas,
    COUNT(DISTINCT h.Aul_Id)                                       AS AulasUsadas,
    COUNT(DISTINCT h.Dia_Id)                                       AS DiasUsados
FROM app.tbHorarios h
WHERE h.Hor_Año = @AnioAcademico AND h.Sem_Id = @SemestreId AND h.Hor_EsEliminado = 0;

GO
