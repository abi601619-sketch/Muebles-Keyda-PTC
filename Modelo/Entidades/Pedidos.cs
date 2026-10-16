using Modelo.Conexión_DB;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Modelo.Entidades
{
    public class DbPedidos
    {
        private int idPedido;
        private DateTime FechaDePedido;
        private DateTime FechaDeEntrega;
        private string Estado;
        private int idCotizacion;

        public DbPedidos(int idPedido, DateTime fechaDePedido, DateTime fechaDeEntrega, string estado, int idCotizacion)
        {
            this.idPedido = idPedido;
            FechaDePedido = fechaDePedido;
            FechaDeEntrega = fechaDeEntrega;
            Estado = estado;
            this.idCotizacion = idCotizacion;
        }

        public int IdPedido { get => idPedido; set => idPedido = value; }
        public DateTime FechaDePedido1 { get => FechaDePedido; set => FechaDePedido = value; }
        public DateTime FechaDeEntrega1 { get => FechaDeEntrega; set => FechaDeEntrega = value; }
        public string Estado1 { get => Estado; set => Estado = value; }
        public int IdCotizacion { get => idCotizacion; set => idCotizacion = value; }

        public static DataTable CargarRegistroPedidos()
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string comandoSQL = "SELECT * FROM VerPedido;";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(comandoSQL, conexion))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La consulta tardó demasiado tiempo.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 208:
                        MessageBox.Show("La vista VerPedido no existe en la base de datos.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Ocurrió un error al cargar los pedidos.\n" + ex.Message, "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return new DataTable();
            }
        }

        public static DataTable CargarPedidosRecientes()
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string comandoSQL = "SELECT * FROM Pedido WHERE FechaDePedido >= '2026-07-15';";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(comandoSQL, conexion))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La consulta tardó demasiado tiempo.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 208:
                        MessageBox.Show("La tabla Pedido no existe en la base de datos.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Ocurrió un error al cargar los pedidos recientes.\n" + ex.Message, "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return new DataTable();
            }
        }


        public static bool ConvertirCotizacionAPedido(int idCotizacion, DateTime fechaEntrega)
        {

            using (SqlConnection conexion = Conexion.Conectar())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {

                try
                {

                    // 0. Verificar que la cotización no se haya convertido anteriormente
                    string queryValidar = @"
        SELECT Estado
        FROM dbo.Cotizacion WITH (UPDLOCK, HOLDLOCK)
        WHERE IdCotizacion = @IdCotizacion;

        IF @@ROWCOUNT = 0
            THROW 50010, 'La cotización no existe.', 1;

        IF EXISTS (
            SELECT 1
            FROM dbo.Pedido
            WHERE IdCotizacion = @IdCotizacion
        )
            THROW 50011,
                'Esta cotización ya fue convertida en pedido.',
                1;
    ";

                    using (SqlCommand cmdValidar =
                        new SqlCommand(queryValidar, conexion, transaccion))
                    {
                        cmdValidar.Parameters.Add(
                            "@IdCotizacion", SqlDbType.Int).Value = idCotizacion;

                        cmdValidar.ExecuteNonQuery();
                    }

                    // 1. Crear el pedido
                    string queryPedido = @"
                INSERT INTO dbo.Pedido
                    (FechaDePedido, FechaDeEntrega, Estado, IdCotizacion)
                VALUES
                    (@FechaPedido, @FechaEntrega, 'En proceso', @IdCotizacion);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int nuevoIdPedido;

                    using (SqlCommand cmdPedido =
                        new SqlCommand(queryPedido, conexion, transaccion))
                    {
                        cmdPedido.Parameters.Add(
                            "@FechaPedido", SqlDbType.DateTime).Value =
                            DateTime.Now;

                        cmdPedido.Parameters.Add(
                            "@FechaEntrega", SqlDbType.DateTime).Value =
                            fechaEntrega;

                        cmdPedido.Parameters.Add(
                            "@IdCotizacion", SqlDbType.Int).Value =
                            idCotizacion;

                        nuevoIdPedido = Convert.ToInt32(cmdPedido.ExecuteScalar());
                    }

                    // 2. Crear los detalles del pedido
                    string queryDetalle = @"
                INSERT INTO dbo.DetallePedido
                    (IdPedido, Mueble, Cantidad, Medidas)
                SELECT
                    @IdPedido,
                    DescripcionMueble,
                    Cantidad,
                    CAST(Largo AS VARCHAR(20)) + 'x' +
                    CAST(Ancho AS VARCHAR(20)) + 'x' +
                    CAST(Alto AS VARCHAR(20))
                FROM dbo.Productos_Cotizacion
                WHERE IdCotizacion = @IdCotizacion;";

                    int detallesInsertados;

                    using (SqlCommand cmdDetalle =
                        new SqlCommand(queryDetalle, conexion, transaccion))
                    {
                        cmdDetalle.Parameters.Add(
                            "@IdPedido", SqlDbType.Int).Value =
                            nuevoIdPedido;

                        cmdDetalle.Parameters.Add(
                            "@IdCotizacion", SqlDbType.Int).Value =
                            idCotizacion;

                        detallesInsertados = cmdDetalle.ExecuteNonQuery();
                    }

                    // Verificar que se haya creado al menos un mueble
                    if (detallesInsertados <= 0)
                    {
                        throw new InvalidOperationException(
                            "La cotización no contiene muebles para crear el pedido.");
                    }

                    // 3. Crear una producción por cada detalle del pedido
                    string queryProduccion = @"
                INSERT INTO dbo.Produccion
                    (IdPedido, IdDetallePedido, Progreso)
                SELECT
                    d.IdPedido,
                    d.IdDetallePedido,
                    0
                FROM dbo.DetallePedido AS d
                WHERE d.IdPedido = @IdPedido
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.Produccion AS p
                      WHERE p.IdDetallePedido = d.IdDetallePedido
                  );";

                    using (SqlCommand cmdProduccion =
                        new SqlCommand(queryProduccion, conexion, transaccion))
                    {
                        cmdProduccion.Parameters.Add(
                            "@IdPedido", SqlDbType.Int).Value =
                            nuevoIdPedido;

                        int produccionesInsertadas =
                            cmdProduccion.ExecuteNonQuery();

                        if (produccionesInsertadas != detallesInsertados)
                        {
                            throw new InvalidOperationException(
                                "No se pudo crear una producción para cada mueble del pedido.");
                        }
                    }

                    // 4. Confirmar todas las operaciones
                    transaccion.Commit();

                    return true;
                }
                catch (SqlException ex)
                {
                    try
                    {
                        transaccion.Rollback();
                    }
                    catch
                    {
                    }

                    string mensaje;

                    switch (ex.Number)
                    {
                        case 53:
                            mensaje = "No se pudo establecer conexión con el servidor.";
                            break;

                        case 4060:
                            mensaje = "No se pudo acceder a la base de datos.";
                            break;

                        case -2:
                            mensaje = "La operación tardó demasiado tiempo.";
                            break;

                        case 208:
                            mensaje = "Una de las tablas utilizadas no existe.";
                            break;

                        case 2627:
                        case 2601:
                            mensaje = "El pedido o alguno de sus datos ya existe.";
                            break;

                        case 547:
                            mensaje = "No se puede convertir la cotización. Verifique los datos relacionados.";
                            break;

                        case 515:
                            mensaje = "Faltan datos obligatorios para crear el pedido.";
                            break;

                        case 245:
                            mensaje = "Uno de los datos tiene un formato incorrecto.";
                            break;

                        case 8115:
                            mensaje = "Uno de los valores ingresados es demasiado grande.";
                            break;

                        case 8152:
                            mensaje = "Uno de los datos es demasiado largo.";
                            break;
                        case 50010:
                            mensaje = "La cotización no existe.";
                            break;

                        case 50011:
                            mensaje = "Esta cotización ya fue convertida en pedido.";
                            break;

                        default:
                            mensaje = "Error SQL (" + ex.Number + "): " + ex.Message;
                            break;
                    }

                    MessageBox.Show(
                        mensaje,
                        "Error al convertir cotización",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaccion.Rollback();
                    }
                    catch
                    {
                    }

                    MessageBox.Show(
                        "No se pudo convertir la cotización.\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
            }
        }



        public static bool ActualizarPedidoFecha(int idPedido, DateTime fechaEntrega)
        {
            using (SqlConnection conexion = Conexion.Conectar())
            {
                string query = "UPDATE Pedido SET FechaDeEntrega = @FechaEntrega WHERE IdPedido = @IdPedido;";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@FechaEntrega", fechaEntrega);
                    cmd.Parameters.AddWithValue("@IdPedido", idPedido);

                    try
                    {
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            return true;
                        }

                        MessageBox.Show("No se encontró el pedido que desea actualizar.", "Pedido no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    catch (SqlException ex)
                    {
                        switch (ex.Number)
                        {
                            case 53:
                                MessageBox.Show("No se pudo establecer conexión con el servidor.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            case 4060:
                                MessageBox.Show("No se pudo acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            case -2:
                                MessageBox.Show("La actualización tardó demasiado tiempo.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            case 208:
                                MessageBox.Show("La tabla Pedido no existe en la base de datos.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            case 547:
                                MessageBox.Show("No se puede actualizar la fecha del pedido. Verifique los datos relacionados.", "ERR-SQL-007", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;

                            case 515:
                                MessageBox.Show("La fecha de entrega es obligatoria.", "ERR-SQL-008", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;

                            default:
                                MessageBox.Show("Ocurrió un error al actualizar la fecha.\n" + ex.Message, "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }

                        return false;
                    }
                }
            }
        }

        public static DataTable BuscarPedido(string termino)
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string comandoSQL = "SELECT * FROM VerPedido WHERE CAST(IdPedido AS VARCHAR) LIKE @buscar OR Cliente LIKE @buscar;";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(comandoSQL, conexion))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@buscar", "%" + (termino ?? "") + "%");

                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        return dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La búsqueda tardó demasiado tiempo.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 208:
                        MessageBox.Show("La vista VerPedido no existe en la base de datos.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Ocurrió un error al buscar los pedidos.\n" + ex.Message, "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return new DataTable();
            }
        }
    }
}