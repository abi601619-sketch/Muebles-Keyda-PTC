-- Ejecutar sobre MueblesKeyda. Reejecutable; no toca ningun dato, solo crea/actualiza vistas.
-- Agrega VerCotizaciones2 (reporte de cotizaciones) y agrega IdCliente a las vistas de busqueda de clientes.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
EXEC(N'CREATE OR ALTER VIEW dbo.VerCotizaciones2 AS
    SELECT
        co.IdCotizacion AS IdCotizacion,
        co.Fecha AS Fecha,
        CASE
            WHEN tc.TipoCliente = ''Persona Natural''
            THEN CONCAT(c.Identificador1, '' '', c.Identificador2)
            ELSE c.Identificador1
        END AS Cliente,
        tc.TipoCliente AS TipoCliente,
        co.Estado AS Estado,
        co.Total AS Total
    FROM dbo.Cotizacion co
    INNER JOIN dbo.Cliente c
        ON co.IdCliente = c.IdCliente
    INNER JOIN dbo.TipoCliente tc
        ON c.IdTipoCliente = tc.IdTipoCliente;');
EXEC(N'CREATE OR ALTER VIEW dbo.BuscarClientesIndividuales AS
    SELECT
        c.IdCliente,
        c.Identificador1 AS Nombre,
        c.Identificador2 AS Apellidos,
        c.Documento AS DUI,
        c.Telefono,
        c.Correo,
        c.Direccion,
        c.Estado
    FROM dbo.Cliente c
    INNER JOIN dbo.TipoCliente tc
        ON c.IdTipoCliente = tc.IdTipoCliente
    WHERE tc.TipoCliente = ''Persona Natural'';');
EXEC(N'CREATE OR ALTER VIEW dbo.BuscarClientesCorporativos AS
    SELECT
        c.IdCliente,
        c.Identificador1 AS Empresa,
        c.Identificador2 AS Encargado,
        c.Documento AS NIT,
        c.Telefono,
        c.Correo,
        c.Direccion,
        c.Estado
    FROM dbo.Cliente c
    INNER JOIN dbo.TipoCliente tc
        ON c.IdTipoCliente = tc.IdTipoCliente
    WHERE tc.TipoCliente = ''Empresa'';');
COMMIT TRANSACTION;
