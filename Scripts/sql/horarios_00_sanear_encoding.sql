/* ============================================================================
   horarios_00_sanear_encoding.sql
   ----------------------------------------------------------------------------
   Repara nombres con MOJIBAKE (bytes UTF-8 almacenados como Latin-1 dentro
   de NVARCHAR). Ejemplo: "MartÃ­nez" -> "Martínez".

   Causa raiz: algun proceso de carga insertó los bytes UTF-8 caracter por
   caracter, partiendo cada acento en dos code-points (ej. 'í' U+00ED se
   convirtió en 'Ã' U+00C3 + soft-hyphen U+00AD). El JSON serializer del API
   los reenvia tal cual, por eso el visor los muestra rotos.

   IDEMPOTENTE: aplicar REPLACE no afecta nombres que ya estan bien.

   Tablas saneadas:
     - app.tbPersonas (nombres y apellidos)
     - app.tbCursos   (Cur_Nombre)
     - app.tbMaterias (Mat_Nombre)

   Ejecutar con sqlcmd:
     sqlcmd -S "(localdb)\MSSQLLocalDB" -d DB_GestionColegial -E -f 65001 \
            -i Scripts/sql/horarios_00_sanear_encoding.sql
   ============================================================================ */

USE DB_GestionColegial;
GO
SET NOCOUNT ON;
GO

/* Funcion replace anidado para los pares mojibake mas comunes en espanol.
   NCHAR codes:
     0xC3 = 'Ã'
     0xC2 = 'Â'
     0xAD = soft hyphen (lo que sale de 'í')
     0xA1 = '¡'/'á' (depende)
     0xA9 = '©'/'é'
     0xB3 = '³'/'ó'
     0xBA = 'º'/'ú'
     0xB1 = '±'/'ñ'
     0x91 = 'Ñ' (mayuscula)
     0x81 = 'Á'
     0x89 = 'É'
     0x8D = 'Í'
     0x93 = 'Ó'
     0x9A = 'Ú'

   La regla: 'Ã' + caracter_diacritico = letra_acentuada_correcta.        */

DECLARE @FilasPer INT, @FilasCur INT, @FilasMat INT;

/* ----------------------------------------------------------------------------
   1. tbPersonas — nombres y apellidos
   ---------------------------------------------------------------------------- */
UPDATE app.tbPersonas
SET
    Per_PrimerNombre = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        Per_PrimerNombre,
        NCHAR(0xC3)+NCHAR(0xAD), N'í'),
        NCHAR(0xC3)+NCHAR(0xA1), N'á'),
        NCHAR(0xC3)+NCHAR(0xA9), N'é'),
        NCHAR(0xC3)+NCHAR(0xB3), N'ó'),
        NCHAR(0xC3)+NCHAR(0xBA), N'ú'),
        NCHAR(0xC3)+NCHAR(0xB1), N'ñ'),
        NCHAR(0xC3)+NCHAR(0x91), N'Ñ'),
        NCHAR(0xC3)+NCHAR(0x81), N'Á'),
        NCHAR(0xC3)+NCHAR(0x89), N'É'),
        NCHAR(0xC3)+NCHAR(0x8D), N'Í'),
        NCHAR(0xC3)+NCHAR(0x93), N'Ó'),
        NCHAR(0xC3)+NCHAR(0x9A), N'Ú'),
        NCHAR(0xC2)+NCHAR(0xBF), N'¿'),
        NCHAR(0xC2)+NCHAR(0xA1), N'¡'),
    Per_SegundoNombre = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        Per_SegundoNombre,
        NCHAR(0xC3)+NCHAR(0xAD), N'í'),
        NCHAR(0xC3)+NCHAR(0xA1), N'á'),
        NCHAR(0xC3)+NCHAR(0xA9), N'é'),
        NCHAR(0xC3)+NCHAR(0xB3), N'ó'),
        NCHAR(0xC3)+NCHAR(0xBA), N'ú'),
        NCHAR(0xC3)+NCHAR(0xB1), N'ñ'),
        NCHAR(0xC3)+NCHAR(0x91), N'Ñ'),
        NCHAR(0xC3)+NCHAR(0x81), N'Á'),
        NCHAR(0xC3)+NCHAR(0x89), N'É'),
        NCHAR(0xC3)+NCHAR(0x8D), N'Í'),
        NCHAR(0xC3)+NCHAR(0x93), N'Ó'),
        NCHAR(0xC3)+NCHAR(0x9A), N'Ú'),
    Per_ApellidoPaterno = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        Per_ApellidoPaterno,
        NCHAR(0xC3)+NCHAR(0xAD), N'í'),
        NCHAR(0xC3)+NCHAR(0xA1), N'á'),
        NCHAR(0xC3)+NCHAR(0xA9), N'é'),
        NCHAR(0xC3)+NCHAR(0xB3), N'ó'),
        NCHAR(0xC3)+NCHAR(0xBA), N'ú'),
        NCHAR(0xC3)+NCHAR(0xB1), N'ñ'),
        NCHAR(0xC3)+NCHAR(0x91), N'Ñ'),
        NCHAR(0xC3)+NCHAR(0x81), N'Á'),
        NCHAR(0xC3)+NCHAR(0x89), N'É'),
        NCHAR(0xC3)+NCHAR(0x8D), N'Í'),
        NCHAR(0xC3)+NCHAR(0x93), N'Ó'),
        NCHAR(0xC3)+NCHAR(0x9A), N'Ú'),
    Per_ApellidoMaterno = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        Per_ApellidoMaterno,
        NCHAR(0xC3)+NCHAR(0xAD), N'í'),
        NCHAR(0xC3)+NCHAR(0xA1), N'á'),
        NCHAR(0xC3)+NCHAR(0xA9), N'é'),
        NCHAR(0xC3)+NCHAR(0xB3), N'ó'),
        NCHAR(0xC3)+NCHAR(0xBA), N'ú'),
        NCHAR(0xC3)+NCHAR(0xB1), N'ñ'),
        NCHAR(0xC3)+NCHAR(0x91), N'Ñ'),
        NCHAR(0xC3)+NCHAR(0x81), N'Á'),
        NCHAR(0xC3)+NCHAR(0x89), N'É'),
        NCHAR(0xC3)+NCHAR(0x8D), N'Í'),
        NCHAR(0xC3)+NCHAR(0x93), N'Ó'),
        NCHAR(0xC3)+NCHAR(0x9A), N'Ú')
WHERE
    Per_PrimerNombre   LIKE N'%' + NCHAR(0xC3) + N'%'
 OR Per_SegundoNombre  LIKE N'%' + NCHAR(0xC3) + N'%'
 OR Per_ApellidoPaterno LIKE N'%' + NCHAR(0xC3) + N'%'
 OR Per_ApellidoMaterno LIKE N'%' + NCHAR(0xC3) + N'%';

SET @FilasPer = @@ROWCOUNT;
PRINT CONCAT('tbPersonas filas saneadas: ', @FilasPer);

/* ----------------------------------------------------------------------------
   2. tbCursos — nombres de carreras BTP
   ---------------------------------------------------------------------------- */
UPDATE app.tbCursos
SET Cur_Nombre = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    Cur_Nombre,
    NCHAR(0xC3)+NCHAR(0xAD), N'í'),
    NCHAR(0xC3)+NCHAR(0xA1), N'á'),
    NCHAR(0xC3)+NCHAR(0xA9), N'é'),
    NCHAR(0xC3)+NCHAR(0xB3), N'ó'),
    NCHAR(0xC3)+NCHAR(0xBA), N'ú'),
    NCHAR(0xC3)+NCHAR(0xB1), N'ñ'),
    NCHAR(0xC3)+NCHAR(0x91), N'Ñ'),
    NCHAR(0xC3)+NCHAR(0x81), N'Á'),
    NCHAR(0xC3)+NCHAR(0x89), N'É'),
    NCHAR(0xC3)+NCHAR(0x8D), N'Í')
WHERE Cur_Nombre LIKE N'%' + NCHAR(0xC3) + N'%';
SET @FilasCur = @@ROWCOUNT;
PRINT CONCAT('tbCursos filas saneadas: ', @FilasCur);

/* ----------------------------------------------------------------------------
   3. tbMaterias — nombres de materias
   ---------------------------------------------------------------------------- */
UPDATE app.tbMaterias
SET Mat_Nombre = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    Mat_Nombre,
    NCHAR(0xC3)+NCHAR(0xAD), N'í'),
    NCHAR(0xC3)+NCHAR(0xA1), N'á'),
    NCHAR(0xC3)+NCHAR(0xA9), N'é'),
    NCHAR(0xC3)+NCHAR(0xB3), N'ó'),
    NCHAR(0xC3)+NCHAR(0xBA), N'ú'),
    NCHAR(0xC3)+NCHAR(0xB1), N'ñ'),
    NCHAR(0xC3)+NCHAR(0x91), N'Ñ'),
    NCHAR(0xC3)+NCHAR(0x81), N'Á'),
    NCHAR(0xC3)+NCHAR(0x89), N'É'),
    NCHAR(0xC3)+NCHAR(0x8D), N'Í')
WHERE Mat_Nombre LIKE N'%' + NCHAR(0xC3) + N'%';
SET @FilasMat = @@ROWCOUNT;
PRINT CONCAT('tbMaterias filas saneadas: ', @FilasMat);

PRINT '';
PRINT 'Resumen:';
PRINT CONCAT('  - Personas: ', @FilasPer);
PRINT CONCAT('  - Cursos:   ', @FilasCur);
PRINT CONCAT('  - Materias: ', @FilasMat);

GO
