/* ============================================================================
   nomina_00_permiso.sql
   ----------------------------------------------------------------------------
   Registra el permiso "Nomina Docentes" en seguridad.tbPantallas y lo asigna
   al rol "contador" (Rol_Id = 1006). El rol "admin" (Rol_Id = 1) ya tiene
   bypass automatico en SessionManager.

   Idempotente: si la pantalla o la asignacion ya existen, no las duplica.
   ============================================================================ */

USE DB_GestionColegial;
GO
SET NOCOUNT ON;
GO

DECLARE @PanId INT;
DECLARE @PanDescripcion NVARCHAR(150) = N'Nomina Docentes';

-- 1. Insertar pantalla si no existe
SELECT @PanId = Pan_Id
FROM seguridad.tbPantallas
WHERE Pan_Descripcion = @PanDescripcion AND Pan_EsEliminado = 0;

IF @PanId IS NULL
BEGIN
    INSERT INTO seguridad.tbPantallas
        (Pan_Descripcion, Pan_Grupo, Pan_Estado, Pan_EsEliminado,
         Pan_UsuarioRegistra, Pan_FechaRegistra)
    VALUES (@PanDescripcion, N'Financiero', 1, 0, 1, GETDATE());

    SET @PanId = SCOPE_IDENTITY();
    PRINT CONCAT('Pantalla creada: Pan_Id = ', @PanId);
END
ELSE
    PRINT CONCAT('Pantalla ya existia: Pan_Id = ', @PanId);

-- 2. Asignar a rol contador (1006) si no esta
IF NOT EXISTS (
    SELECT 1 FROM seguridad.tbRolesPantallas
    WHERE Rol_Id = 1006 AND Pan_Id = @PanId
)
BEGIN
    INSERT INTO seguridad.tbRolesPantallas (Rol_Id, Pan_Id)
    VALUES (1006, @PanId);
    PRINT 'Asignada al rol contador (1006)';
END
ELSE
    PRINT 'Ya estaba asignada al rol contador';

-- 3. Asignar tambien al rol admin (1) por consistencia (aunque bypassa)
IF NOT EXISTS (
    SELECT 1 FROM seguridad.tbRolesPantallas
    WHERE Rol_Id = 1 AND Pan_Id = @PanId
)
BEGIN
    INSERT INTO seguridad.tbRolesPantallas (Rol_Id, Pan_Id)
    VALUES (1, @PanId);
    PRINT 'Asignada al rol admin (1)';
END

GO
