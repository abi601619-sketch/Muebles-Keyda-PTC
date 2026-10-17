-- Ejecutar sobre MueblesKeyda (en SSMS o con sqlcmd -f 65001). Reejecutable; no borra datos.
--
-- Repara el error:  "Conversion failed when converting the varchar value 'Activo' to data type bit"
-- al actualizar, activar o desactivar usuarios.
-- Causa: Usuario.Estado es BIT (1 = activo, 0 = inactivo). Un trigger sobre Usuario que compare el
-- estado con los textos 'Activo' / 'Inactivo' (eso es valido en Cliente, cuyo Estado es texto, pero no en
-- Usuario) falla en CADA UPDATE de la tabla Usuario.
--
-- Este script:
--   1. Elimina los triggers de Usuario que comparan el estado con 'Activo' / 'Inactivo'.
--   2. Se asegura de que Cotizacion.Estado acepte 'Cancelada'.
--   3. Deja el trigger correcto: al desactivar un usuario (1 -> 0), sus cotizaciones Pendientes y
--      Aprobadas pasan a 'Cancelada'; no se elimina nada.

-- 1. Triggers de Usuario que comparan el estado con texto
DECLARE @Sql nvarchar(max) = N'';

SELECT @Sql = @Sql
       + N'DROP TRIGGER ' + QUOTENAME(OBJECT_SCHEMA_NAME(t.object_id)) + N'.' + QUOTENAME(t.name) + N';' + CHAR(10)
       + N'PRINT ''Trigger eliminado: ' + REPLACE(t.name, '''', '''''') + N''';' + CHAR(10)
FROM sys.triggers AS t
WHERE t.parent_id = OBJECT_ID('dbo.Usuario')
  AND t.name <> 'TR_Usuario_Desactivado_CancelarCotizaciones'
  AND (OBJECT_DEFINITION(t.object_id) LIKE '%''Activo''%'
    OR OBJECT_DEFINITION(t.object_id) LIKE '%''Inactivo''%');

IF @Sql = N''
    PRINT 'No habia triggers de Usuario con texto Activo/Inactivo.';
ELSE
    EXEC sp_executesql @Sql;
GO

-- 2. Cotizacion.Estado acepta 'Cancelada'
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Restriccion sysname;
DECLARE @Sql nvarchar(max);
DECLARE restricciones CURSOR LOCAL FAST_FORWARD FOR
    SELECT name FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.Cotizacion')
      AND definition LIKE '%Estado%';
OPEN restricciones;
FETCH NEXT FROM restricciones INTO @Restriccion;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Sql = N'ALTER TABLE dbo.Cotizacion DROP CONSTRAINT ' + QUOTENAME(@Restriccion);
    EXEC sp_executesql @Sql;
    FETCH NEXT FROM restricciones INTO @Restriccion;
END;
CLOSE restricciones;
DEALLOCATE restricciones;
ALTER TABLE dbo.Cotizacion WITH CHECK ADD CONSTRAINT CK_Cotizacion_Estado
    CHECK (Estado IN ('Aprobada', 'Pendiente', 'Rechazada', 'Finalizada', 'Cancelada'));

COMMIT TRANSACTION;
GO

-- 3. Trigger correcto (Estado es BIT: 1 / 0)
CREATE OR ALTER TRIGGER TR_Usuario_Desactivado_CancelarCotizaciones
ON Usuario
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Estado)
        RETURN;

    UPDATE co
    SET co.Estado = 'Cancelada'
    FROM Cotizacion co
    INNER JOIN inserted i
        ON co.IdUsuario = i.IdUsuario
    INNER JOIN deleted d
        ON d.IdUsuario = i.IdUsuario
    WHERE d.Estado = 1
      AND i.Estado = 0
      AND co.Estado IN ('Pendiente', 'Aprobada');
END;
GO
