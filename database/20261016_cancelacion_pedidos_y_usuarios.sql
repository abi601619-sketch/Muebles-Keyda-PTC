-- Ejecutar sobre MueblesKeyda (en SSMS o con sqlcmd -f 65001). Reejecutable y no borra datos.
-- Agrega: Venta.IdCotizacion, el estado 'Cancelada' en Cotizacion, los procedimientos para cancelar
-- pedidos y los triggers de ventas y de desactivacion de usuarios.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- 1. Venta.IdCotizacion relaciona cada venta con su cotizacion
IF COL_LENGTH('dbo.Venta', 'IdCotizacion') IS NULL
    ALTER TABLE dbo.Venta ADD IdCotizacion INT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys
               WHERE name = 'FK_Venta_Cotizacion' AND parent_object_id = OBJECT_ID('dbo.Venta'))
    ALTER TABLE dbo.Venta ADD CONSTRAINT FK_Venta_Cotizacion
        FOREIGN KEY (IdCotizacion) REFERENCES dbo.Cotizacion(IdCotizacion);

-- 2. Cotizacion.Estado acepta 'Cancelada'
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

-- 3. Procedimientos y triggers
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

        SELECT @Estado = Estado
        FROM dbo.Pedido WITH (UPDLOCK, HOLDLOCK)
        WHERE IdPedido = @IdPedido;

        IF @Estado IS NULL
        BEGIN
            THROW 50001, 'El pedido no existe.', 1;
        END;

        IF @Estado IN ('Finalizado', 'Cancelado')
        BEGIN
            THROW 50004,
                'No se pueden modificar pedidos finalizados o cancelados.',
                1;
        END;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.DetallePedido WITH (UPDLOCK, HOLDLOCK)
            WHERE IdPedido = @IdPedido
              AND IdDetallePedido = @IdDetallePedido
        )
        BEGIN
            THROW 50003, 'El detalle no existe.', 1;
        END;

        SELECT @Cantidad = COUNT(*)
        FROM dbo.DetallePedido WITH (UPDLOCK, HOLDLOCK)
        WHERE IdPedido = @IdPedido;

        IF @Cantidad = 1 AND @ConfirmarCancelacion = 0
        BEGIN
            THROW 50002,
                'Este es el ultimo producto. Confirme la cancelacion.',
                1;
        END;

        DELETE FROM dbo.DetallePedido
        WHERE IdPedido = @IdPedido
          AND IdDetallePedido = @IdDetallePedido;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.DetallePedido
            WHERE IdPedido = @IdPedido
        )
        BEGIN
            DELETE FROM dbo.MaterialUtilizado
            WHERE IdProduccion IN (
                SELECT IdProduccion
                FROM dbo.Produccion
                WHERE IdPedido = @IdPedido
            );

            DELETE FROM dbo.Produccion
            WHERE IdPedido = @IdPedido;

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

--Procedimiento almacenado para eliminar los registros de un pedido cancelado
CREATE OR ALTER PROCEDURE sp_CancelarPedidoSinProductos
    @IdPedido INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @IdCotizacion INT;
        DECLARE @IdVenta INT;

        -- Verificar que exista el pedido
        SELECT @IdCotizacion = IdCotizacion
        FROM Pedido
        WHERE IdPedido = @IdPedido;

        IF @IdCotizacion IS NULL
            THROW 50001, 'El pedido no existe.', 1;

        -- Solo cancelar cuando ya no quedan productos
        IF EXISTS
        (
            SELECT 1
            FROM DetallePedido
            WHERE IdPedido = @IdPedido
        )
            THROW 50002,
                'El pedido todavía contiene productos.',
                1;

        -- No modificar pedidos ya cancelados o finalizados
        IF EXISTS
        (
            SELECT 1
            FROM Pedido
            WHERE IdPedido = @IdPedido
              AND Estado IN ('Cancelado', 'Finalizado')
        )
            THROW 50003,
                'El pedido ya está cancelado o finalizado. Requiere revisión.',
                1;

        -- Identificar la venta asociada a la cotización
        SELECT @IdVenta = IdVenta
        FROM Venta
        WHERE IdCotizacion = @IdCotizacion;

        -- No borrar una venta que tenga factura emitida
        IF @IdVenta IS NOT NULL
           AND EXISTS
           (
               SELECT 1
               FROM Factura
               WHERE IdVenta = @IdVenta
           )
            THROW 50004,
                'La venta tiene una factura asociada y requiere anulación formal.',
                1;

        -- Eliminar materiales registrados en la producción
        DELETE mu
        FROM MaterialUtilizado mu
        INNER JOIN Produccion pr
            ON pr.IdProduccion = mu.IdProduccion
        WHERE pr.IdPedido = @IdPedido;

        -- Eliminar producción asociada al pedido
        DELETE FROM Produccion
        WHERE IdPedido = @IdPedido;

        -- Eliminar detalles y venta asociada, si existe
        IF @IdVenta IS NOT NULL
        BEGIN
            DELETE FROM DetalleVenta
            WHERE IdVenta = @IdVenta;

            DELETE FROM Venta
            WHERE IdVenta = @IdVenta;
        END;

        -- Conservar el pedido en el historial
        UPDATE Pedido
        SET Estado = 'Cancelado'
        WHERE IdPedido = @IdPedido;

        COMMIT TRANSACTION;

        SELECT
            'El pedido fue cancelado correctamente.' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;

GO

----------------TRIGGER PARA CONTROLAR QUE UN PEDIDO FINALIZADO PASE A SER REGISTRO DE VENTAS------------

CREATE OR ALTER TRIGGER TR_Pedido_Finalizado_Venta
ON Pedido
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Venta
    (
        FechaVenta,
        IdCliente,
        SubTotal,
        IdCotizacion
    )
    SELECT
        GETDATE(),
        c.IdCliente,
        c.Total,
        i.IdCotizacion
    FROM inserted i
    INNER JOIN deleted d
        ON i.IdPedido = d.IdPedido
    INNER JOIN Cotizacion c
        ON i.IdCotizacion = c.IdCotizacion
    WHERE i.Estado = 'Finalizado'
      AND d.Estado <> 'Finalizado'
      AND NOT EXISTS
      (
          SELECT 1
          FROM Venta v
          WHERE v.IdCotizacion = i.IdCotizacion
      );

    INSERT INTO DetalleVenta
    (
        IdVenta,
        ProductoVendido,
        Cantidad,
        PrecioUnitario
    )
    SELECT
        v.IdVenta,
        pc.DescripcionMueble,
        pc.Cantidad,
        pc.PrecioUnitario
    FROM inserted i
    INNER JOIN deleted d
        ON i.IdPedido = d.IdPedido
    INNER JOIN Productos_Cotizacion pc
        ON pc.IdCotizacion = i.IdCotizacion
    INNER JOIN Venta v
        ON v.IdCotizacion = i.IdCotizacion
    WHERE i.Estado = 'Finalizado'
      AND d.Estado <> 'Finalizado'
      AND NOT EXISTS
      (
          SELECT 1
          FROM DetalleVenta dv
          WHERE dv.IdVenta = v.IdVenta
      );
END;
GO

----------------TRIGGER PARA CANCELAR LAS COTIZACIONES DE UN USUARIO QUE SE DESACTIVA------------
-- Al desactivar un usuario (Usuario.Estado de 1 a 0), sus cotizaciones Pendientes y Aprobadas
-- pasan a 'Cancelada'. Las Rechazadas y las Finalizadas conservan su estado. No se elimina ni se
-- modifica ningun otro registro: pedidos, produccion, ventas y facturas quedan como historial.

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
