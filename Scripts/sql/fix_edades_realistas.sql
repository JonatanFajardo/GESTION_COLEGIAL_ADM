/*
================================================================================
 Script: fix_edades_realistas.sql
 Fecha:  2026-06-25
 Autor:  jona (Usu_Id = 1)
 BD:     DB_GestionColegial  (LocalDB → (localdb)\MSSQLLocalDB)
================================================================================
 Propósito
 ---------
 La data sembrada de tbPersonas tenía fechas de nacimiento aleatorias que
 generaban edades irreales en los módulos de Cumpleaños y dashboards:
   - Empleados con 95 o 7 años.
   - Alumnos con edades de 5 a 95 en todos los grados.

 Este script corrige las fechas de nacimiento manteniendo el día/mes original
 (para no romper el módulo EmpleadosCumpleanos / AlumnosCumpleanos) y ajusta
 solo el AÑO para que la edad caiga en un rango realista.

 Reglas
 ------
 EMPLEADOS  → edad objetivo entre 25 y 60 años (rango laboral realista).
              Fórmula determinista por Per_Id:
                edad = 25 + (Per_Id * 7) % 36

 ALUMNOS    → edad canónica por grado (Cun_Orden) con ~25% de rezagados (+1).
                Séptimo   (Cun_Orden 1) → 12-13
                Octavo    (Cun_Orden 2) → 13-14
                Noveno    (Cun_Orden 3) → 14-15
                Décimo    (Cun_Orden 4) → 15-16
                Undécimo  (Cun_Orden 5) → 16-17
                Duodécimo (Cun_Orden 6) → 17-18
              Fórmula:
                edad = 11 + Cun_Orden + (CASE WHEN Alu_Id % 4 = 0 THEN 1 ELSE 0 END)

 Notas
 -----
 - Solo actualiza personas FUERA del rango aceptable.
 - Conserva MES y DÍA de Per_FechaNacimiento → cumpleaños no se mueven.
 - Asume que no hay nacimientos en 29-feb (validar antes de re-ejecutar).
 - Registra auditoría: Per_FechaModifica = GETDATE(), Per_UsuarioModifica = 1.
 - Idempotente: ejecutar dos veces es seguro (la segunda no toca nada).
================================================================================
*/

USE DB_GestionColegial;
GO

-- ---------------------------------------------------------------------------
-- Salvaguarda: si hay nacimientos en 29-feb, abortar (DATEFROMPARTS fallaría
-- al construir 29-feb en un año no bisiesto).
-- ---------------------------------------------------------------------------
IF EXISTS (
    SELECT 1
    FROM app.tbPersonas p
    WHERE MONTH(p.Per_FechaNacimiento) = 2
      AND DAY(p.Per_FechaNacimiento)   = 29
      AND (
          EXISTS (SELECT 1 FROM app.tbEmpleados e WHERE e.Per_Id = p.Per_Id)
          OR EXISTS (SELECT 1 FROM app.tbAlumnos   a WHERE a.Per_Id = p.Per_Id)
      )
)
BEGIN
    RAISERROR('Existen personas con cumpleaños 29-feb. Ajustar la fórmula antes de continuar.', 16, 1);
    RETURN;
END;

-- ---------------------------------------------------------------------------
-- 1) EMPLEADOS  → edad realista 25–60
-- ---------------------------------------------------------------------------
PRINT '--- Empleados antes ---';
SELECT
    COUNT(*)                                                AS Total,
    MIN(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadMin,
    MAX(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadMax,
    AVG(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadProm
FROM app.tbEmpleados e
INNER JOIN app.tbPersonas p ON e.Per_Id = p.Per_Id;

BEGIN TRAN;

UPDATE p
SET Per_FechaNacimiento = DATEFROMPARTS(
        YEAR(GETDATE()) - (25 + (p.Per_Id * 7) % 36),
        MONTH(p.Per_FechaNacimiento),
        DAY(p.Per_FechaNacimiento)
    ),
    Per_FechaModifica   = GETDATE(),
    Per_UsuarioModifica = 1
FROM app.tbPersonas p
INNER JOIN app.tbEmpleados e ON e.Per_Id = p.Per_Id
WHERE DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()) > 65
   OR DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()) < 22;

PRINT CONCAT('Empleados corregidos: ', @@ROWCOUNT);

COMMIT;

PRINT '--- Empleados después ---';
SELECT
    COUNT(*)                                                AS Total,
    MIN(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadMin,
    MAX(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadMax,
    AVG(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadProm
FROM app.tbEmpleados e
INNER JOIN app.tbPersonas p ON e.Per_Id = p.Per_Id;

-- ---------------------------------------------------------------------------
-- 2) ALUMNOS  → edad por grado (Cun_Orden) con ~25% de rezagados
-- ---------------------------------------------------------------------------
PRINT '--- Alumnos antes ---';
SELECT
    cn.Cun_Orden,
    cn.Cun_Descripcion,
    COUNT(*)                                                AS Total,
    MIN(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadMin,
    MAX(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadMax,
    AVG(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))   AS EdadProm
FROM app.tbAlumnos a
INNER JOIN app.tbPersonas p       ON a.Per_Id = p.Per_Id
INNER JOIN app.tbCursosNiveles cn ON a.Cun_Id = cn.Cun_Id
GROUP BY cn.Cun_Orden, cn.Cun_Descripcion
ORDER BY cn.Cun_Orden;

BEGIN TRAN;

UPDATE p
SET Per_FechaNacimiento = DATEFROMPARTS(
        YEAR(GETDATE()) - (11 + cn.Cun_Orden + CASE WHEN a.Alu_Id % 4 = 0 THEN 1 ELSE 0 END),
        MONTH(p.Per_FechaNacimiento),
        DAY(p.Per_FechaNacimiento)
    ),
    Per_FechaModifica   = GETDATE(),
    Per_UsuarioModifica = 1
FROM app.tbPersonas p
INNER JOIN app.tbAlumnos       a  ON a.Per_Id = p.Per_Id
INNER JOIN app.tbCursosNiveles cn ON a.Cun_Id = cn.Cun_Id
WHERE DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE())
      NOT BETWEEN (11 + cn.Cun_Orden) AND (12 + cn.Cun_Orden);

PRINT CONCAT('Alumnos corregidos: ', @@ROWCOUNT);

COMMIT;

PRINT '--- Alumnos después ---';
SELECT
    cn.Cun_Orden,
    cn.Cun_Descripcion,
    COUNT(*)                                                                  AS Total,
    MIN(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))                     AS EdadMin,
    MAX(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()))                     AS EdadMax,
    AVG(CAST(DATEDIFF(YEAR, p.Per_FechaNacimiento, GETDATE()) AS DECIMAL(5,2))) AS EdadProm,
    SUM(CASE WHEN a.Alu_Id % 4 = 0 THEN 1 ELSE 0 END)                         AS Rezagados
FROM app.tbAlumnos a
INNER JOIN app.tbPersonas p       ON a.Per_Id = p.Per_Id
INNER JOIN app.tbCursosNiveles cn ON a.Cun_Id = cn.Cun_Id
GROUP BY cn.Cun_Orden, cn.Cun_Descripcion
ORDER BY cn.Cun_Orden;
GO
