-- Ejecutar sobre MueblesKeyda (en SSMS o con sqlcmd -f 65001), DESPUES de 20261016_cancelacion_pedidos_y_usuarios.sql.
-- Reejecutable. Solo agrega o reemplaza estructura; los unicos datos que toca son producciones antiguas sin mueble (ver 2 y 3).
-- Pasa la produccion a "una por mueble": columna Produccion.IdDetallePedido, vista VerProduccion,
-- sp_EliminarDetallePedido y el trigger que finaliza las cotizaciones de un cliente desactivado.

-- 1. Produccion.IdDetallePedido
IF COL_LENGTH('dbo.Produccion', 'IdDetallePedido') IS NULL
    ALTER TABLE dbo.Produccion ADD IdDetallePedido INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys
               WHERE name = 'FK_Produccion_DetallePedido' AND parent_object_id = OBJECT_ID('dbo.Produccion'))
    ALTER TABLE dbo.Produccion WITH NOCHECK ADD CONSTRAINT FK_Produccion_DetallePedido
        FOREIGN KEY (IdDetallePedido) REFERENCES dbo.DetallePedido(IdDetallePedido);
GO

-- 2. Las producciones antiguas (una por pedido, sin mueble) se asignan al primer mueble de su pedido
UPDATE pr
SET pr.IdDetallePedido = d.PrimerDetalle
FROM dbo.Produccion AS pr
INNER JOIN (
    SELECT IdPedido, MIN(IdDetallePedido) AS PrimerDetalle
    FROM dbo.DetallePedido
    GROUP BY IdPedido
) AS d
    ON d.IdPedido = pr.IdPedido
WHERE pr.IdDetallePedido IS NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.Produccion AS x WHERE x.IdDetallePedido = d.PrimerDetalle);
GO

-- 3. Los pedidos En proceso reciben una produccion (0 %) para cada mueble que aun no la tenga
INSERT INTO dbo.Produccion (IdPedido, IdDetallePedido, Progreso)
SELECT d.IdPedido, d.IdDetallePedido, 0
FROM dbo.DetallePedido AS d
INNER JOIN dbo.Pedido AS p
    ON p.IdPedido = d.IdPedido
WHERE p.Estado = 'En proceso'
  AND NOT EXISTS (SELECT 1 FROM dbo.Produccion AS x WHERE x.IdDetallePedido = d.IdDetallePedido);
GO

-- 4. Vista, procedimiento y trigger
CREATE OR ALTER VIEW dbo.VerProduccion AS
SELECT
    pro.IdProduccion,
    p.IdPedido,
    dp.IdDetallePedido,

    CASE
        WHEN tc.TipoCliente = 'Persona Natural'
        THEN CONCAT(c.Identificador1, ' ', c.Identificador2)
        ELSE c.Identificador1
    END AS Cliente,

    dp.Mueble AS Producto,
    dp.Medidas,
    dp.Cantidad,

    p.FechaDePedido AS [Fecha de Inicio],
    p.FechaDeEntrega AS [Fecha de Entrega],

    pro.Progreso,

    CASE
        WHEN pro.Progreso = 0 THEN 'Pendiente'
        WHEN pro.Progreso BETWEEN 1 AND 99 THEN 'En producción'
        WHEN pro.Progreso = 100 THEN 'Finalizado'
    END AS Estado

FROM dbo.Produccion AS pro

INNER JOIN dbo.Pedido AS p
    ON pro.IdPedido = p.IdPedido

INNER JOIN dbo.DetallePedido AS dp
    ON pro.IdDetallePedido = dp.IdDetallePedido
    AND pro.IdPedido = dp.IdPedido

INNER JOIN dbo.Cotizacion AS co
    ON p.IdCotizacion = co.IdCotizacion

INNER JOIN dbo.Cliente AS c
    ON co.IdCliente = c.IdCliente

INNER JOIN dbo.TipoCliente AS tc
    ON c.IdTipoCliente = tc.IdTipoCliente;
GO

CREATE OR ALTER PROCEDURE dbo.sp_EliminarDetallePedido
    @IdPedido INT,
    @IdDetallePedido INT,
    @ConfirmarCancelacion BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Estado VARCHAR(20);
        DECLARE @Cantidad INT;
        DECLARE @Cancelado BIT = 0;
        DECLARE @IdProduccion INT;

        -- Verificar pedido y bloquear modificaciones simultáneas
        SELECT @Estado = Estado
        FROM dbo.Pedido WITH (UPDLOCK, HOLDLOCK)
        WHERE IdPedido = @IdPedido;

        IF @Estado IS NULL
            THROW 50001, 'El pedido no existe.', 1;

        IF @Estado IN ('Finalizado', 'Cancelado')
            THROW 50004,
                'No se pueden modificar pedidos finalizados o cancelados.',
                1;

        -- Verificar que el detalle pertenezca al pedido
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.DetallePedido WITH (UPDLOCK, HOLDLOCK)
            WHERE IdPedido = @IdPedido
              AND IdDetallePedido = @IdDetallePedido
        )
            THROW 50003, 'El detalle no existe.', 1;

        -- Identificar la producción del mueble específico
        SELECT @IdProduccion = IdProduccion
        FROM dbo.Produccion WITH (UPDLOCK, HOLDLOCK)
        WHERE IdDetallePedido = @IdDetallePedido
          AND IdPedido = @IdPedido;

        -- Contar los productos del pedido
        SELECT @Cantidad = COUNT(*)
        FROM dbo.DetallePedido WITH (UPDLOCK, HOLDLOCK)
        WHERE IdPedido = @IdPedido;

        -- Si es el último producto, exigir confirmación
        IF @Cantidad = 1 AND @ConfirmarCancelacion = 0
            THROW 50002,
                'Este es el último producto. Confirme la cancelación.',
                1;

        -- Eliminar los materiales usados en esta producción
        IF @IdProduccion IS NOT NULL
        BEGIN
            DELETE FROM dbo.MaterialUtilizado
            WHERE IdProduccion = @IdProduccion;

            -- Eliminar solo la producción del mueble seleccionado
            DELETE FROM dbo.Produccion
            WHERE IdProduccion = @IdProduccion;
        END;

        -- Eliminar el detalle del mueble
        DELETE FROM dbo.DetallePedido
        WHERE IdPedido = @IdPedido
          AND IdDetallePedido = @IdDetallePedido;

        -- Si ya no quedan productos, cancelar el pedido
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.DetallePedido
            WHERE IdPedido = @IdPedido
        )
        BEGIN
            UPDATE dbo.Pedido
            SET Estado = 'Cancelado'
            WHERE IdPedido = @IdPedido;

            SET @Cancelado = 1;
        END;

        COMMIT TRANSACTION;

        SELECT @Cancelado AS Cancelado;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO

----------------TRIGGER PARA FINALIZAR LAS COTIZACIONES DE UN CLIENTE QUE SE DESACTIVA------------
-- Al desactivar un cliente (Cliente.Estado de 'Activo' a 'Inactivo'), sus cotizaciones
-- Pendientes y Aprobadas pasan a 'Finalizada'.

CREATE OR ALTER TRIGGER dbo.TR_Cliente_Desactivado_FinalizarCotizaciones
ON dbo.Cliente
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE co
    SET co.Estado = 'Finalizada'
    FROM dbo.Cotizacion AS co
    INNER JOIN inserted AS i
        ON co.IdCliente = i.IdCliente
    INNER JOIN deleted AS d
        ON d.IdCliente = i.IdCliente
    WHERE d.Estado = 'Activo'
      AND i.Estado = 'Inactivo'
      AND co.Estado IN ('Pendiente', 'Aprobada');
END;
GO
